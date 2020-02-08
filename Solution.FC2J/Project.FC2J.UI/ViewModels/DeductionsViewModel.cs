using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using Caliburn.Micro;
using Project.FC2J.Models;
using Project.FC2J.Models.Customer;
using Project.FC2J.Models.User;
using Project.FC2J.UI.EventModels;
using Project.FC2J.UI.Helpers;
using Project.FC2J.UI.Helpers.AppSetting;
using Project.FC2J.UI.Models;
using Project.FC2J.UI.Providers;

namespace Project.FC2J.UI.ViewModels
{
    public class DeductionsViewModel : Screen
    {

        #region Private fields

        private readonly ICustomerEndpoint _customerEndpoint;
        private readonly IApiAppSetting _apiAppSetting;
        private readonly IMapper _mapper;
        private readonly ILoggedInUser _loggedInUser;
        private readonly IDeductionEndpoint _deductionEndpoint;
        private readonly IEventAggregator _events;

        private List<Deduction> allDeductions = new List<Deduction>();

        #endregion

        #region Constructors And Functions

        public DeductionsViewModel(IDeductionEndpoint deductionEndpoint,
            ICustomerEndpoint customerEndpoint,
            IApiAppSetting apiAppSetting,
            ILoggedInUser loggedInUser,
            IMapper mapper, 
            IEventAggregator events)
        {
            _customerEndpoint = customerEndpoint;
            _apiAppSetting = apiAppSetting;
            _loggedInUser = loggedInUser;
            _deductionEndpoint = deductionEndpoint;
            _mapper = mapper;
            _events = events;
        }

        private async Task LoadPartners()
        {
            var partnerList = await _customerEndpoint.GetList();
            var partners = _mapper.Map<List<CustomerDisplayModel>>(partnerList);
            Partners = new BindingList<CustomerDisplayModel>(partners);
            IsSpinnerVisible = !_apiAppSetting.SleepInSeconds.AndGridWillBeBack();
            OnFocusControl = true;
        }

        private async Task LoadDeductions()
        {
            _deductionEndpoint.SetParameters(0,SelectedPartner.Id);
            allDeductions = await _deductionEndpoint.GetList();
            var deductions = _mapper.Map<List<DeductionDisplayModel>>(allDeductions);
            Deductions = new BindingList<DeductionDisplayModel>(deductions);
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            IsSpinnerVisible = true;
            OnFocusControl = false;
            await LoadPartners();
        }

        
        public void FilterLists(string value)
        {
            var deductions = new List<DeductionDisplayModel>();

            SearchInput = value;

            if (string.IsNullOrWhiteSpace(SearchInput))
            {
                if (Deductions.Count != allDeductions.Count)
                    deductions = _mapper.Map<List<DeductionDisplayModel>>(allDeductions);
                else
                {
                    return;
                }
            }
            else
            {
                deductions = _mapper.Map<List<DeductionDisplayModel>>(allDeductions.Where(c => c.Particular.ToLower().Contains(SearchInput.ToLower())));
            }

            Deductions = new BindingList<DeductionDisplayModel>(deductions);
        }

        private void OnClear()
        {
            Particular = string.Empty;
            Amount = 0;
            SelectedDeduction = null;
        }
      
        #endregion

        #region CanExecutes
        public bool CanClear => string.IsNullOrEmpty(Particular) == false || Amount > 0;

        public bool CanDelete => SelectedDeduction != null && string.IsNullOrEmpty(SelectedDeduction.PONo) ;

        public bool CanSave => string.IsNullOrEmpty(Particular)==false && Amount > 0 && string.IsNullOrEmpty(SelectedDeduction?.PONo);

        public bool CanShow => ControlsEnabled;

        #endregion

        #region Controls Enabled

        public bool ParticularEnabled => ControlsEnabled;

        public bool AmountEnabled => ControlsEnabled;

        private bool _controlsEnabled;

        private bool ControlsEnabled
        {
            get
            {
                return _controlsEnabled;
            }
            set
            {
                _controlsEnabled = value;
                NotifyOfPropertyChange(() => ControlsEnabled);
                NotifyOfPropertyChange(() => AmountEnabled);
                NotifyOfPropertyChange(() => ParticularEnabled);
                NotifyOfPropertyChange(() => CanShow);
                NotifyOfPropertyChange(() => CanDelete);
            }
        }

        #endregion

        #region Buttons


        public void Close()
        {
            TryClose();
        }

        public void Clear()
        {
            if (MessageBox.Show("Are you sure?", "Clear Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                OnClear();
            }
        }

        public async Task Delete()
        {
            if (MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _deductionEndpoint.Remove(Convert.ToInt64(SelectedDeduction.Id), SelectedDeduction.CustomerId);
                Deductions.Remove(SelectedDeduction);
                NotifyOfPropertyChange(() => Deductions);
                OnClear();
                NotifyOfPropertyChange(() => CanDelete);
            }
        }

        public async Task Save()
        {
            if (MessageBox.Show("Are you sure?", "Save Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

                if (isShowClicked == false)
                {
                    await Show();
                }

                var deduction = new Deduction
                {
                    CustomerId = SelectedPartner.Id,
                    Particular = Particular,
                    Amount = Amount
                };



                if (SelectedDeduction == null)
                {
                    try
                    {
                        var result = await _deductionEndpoint.Save(deduction);
                        deduction.Id = result.Id;
                        var newDeduction = _mapper.Map<DeductionDisplayModel>(deduction);
                        Deductions.Add(newDeduction);
                        allDeductions.Add(deduction);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    deduction.Id = SelectedDeduction.Id;
                    await _deductionEndpoint.Update(deduction);
                    SelectedDeduction.Particular = Particular;
                    SelectedDeduction.Amount = Amount;

                    var existingItem = allDeductions.FirstOrDefault(x => x.Id == deduction.Id);

                    if (existingItem != null)
                    {
                        existingItem.Particular = Particular;
                        existingItem.Amount = Amount;
                    }

                }


                OnClear();

            }
        }

        private bool isShowClicked = false;
        public async Task Show()
        {
            await LoadDeductions();
            isShowClicked = true;
        }

        #endregion

        #region Properties
        private string _particular;

        public string Particular
        {
            get { return _particular; }
            set
            {
                _particular = value;
                NotifyOfPropertyChange(() => Particular);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        private decimal _amount;

        public decimal Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                NotifyOfPropertyChange(() => Amount);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanClear);
            }
        }


        private string _searchInput;
        public string SearchInput
        {
            get { return _searchInput; }
            set
            {
                _searchInput = value;
                NotifyOfPropertyChange(() => SearchInput);
            }
        }
        private BindingList<DeductionDisplayModel> _deductions;
        public BindingList<DeductionDisplayModel> Deductions
        {
            get { return _deductions; }
            set
            {
                _deductions = value;
                NotifyOfPropertyChange(() => Deductions);
            }
        }

        private DeductionDisplayModel _selectedDeduction;
        public DeductionDisplayModel SelectedDeduction
        {
            get { return _selectedDeduction; }
            set
            {
                _selectedDeduction = value;
                if (_selectedDeduction != null)
                {
                    Particular = _selectedDeduction.Particular;
                    Amount = _selectedDeduction.Amount;
                }
                NotifyOfPropertyChange(() => SelectedDeduction);
                NotifyOfPropertyChange(() => CanDelete);
                NotifyOfPropertyChange(() => CanSave);
            }
        }


        private CustomerDisplayModel _selectedPartner;
        public CustomerDisplayModel SelectedPartner
        {
            get { return _selectedPartner; }
            set
            {
                _selectedPartner = value;
                NotifyOfPropertyChange(() => SelectedPartner);
                ControlsEnabled = SelectedPartner != null;
                Deductions = new BindingList<DeductionDisplayModel>();
            }
        }

        private BindingList<CustomerDisplayModel> _partners;
        public BindingList<CustomerDisplayModel> Partners
        {
            get { return _partners; }
            set
            {
                _partners = value;
                NotifyOfPropertyChange(() => Partners);
                NotifyOfPropertyChange(() => PartnersProvider);
                NotifyOfPropertyChange(() => SelectedPartner);
            }
        }

        public PartnerSuggestionProvider PartnersProvider
        {
            get
            {
                var partnersProvider = new PartnerSuggestionProvider { Partners = Partners };
                return partnersProvider;
            }
        }

        private bool _isSpinnerVisible = false;
        public bool IsSpinnerVisible    
        {
            get { return _isSpinnerVisible; }
            set
            {
                _isSpinnerVisible = value;
                NotifyOfPropertyChange(() => IsSpinnerVisible);
            }
        }

        private bool _onFocusControl;

        public bool OnFocusControl
        {
            get { return _onFocusControl; }
            set
            {
                _onFocusControl = value;
                NotifyOfPropertyChange(() => OnFocusControl);
            }
        }


        #endregion

    }
}
