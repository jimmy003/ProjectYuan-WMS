using AutoMapper;
using Caliburn.Micro;
using Project.FC2J.Models.Customer;
using Project.FC2J.UI.Helpers;
using Project.FC2J.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Project.FC2J.UI.Helpers.AppSetting;

namespace Project.FC2J.UI.ViewModels
{
    public class CustomerViewModel : Screen
    {
        private BindingList<CustomerDisplayModel> _partners;
        private readonly IMapper _mapper;
        private readonly ICustomerEndpoint _customerEndpoint;
        private readonly IPriceListEndpoint _priceListEndpoint;
        private readonly IApiAppSetting _appSetting;


        public CustomerViewModel(ICustomerEndpoint customerEndpoint, IMapper mapper, IApiAppSetting appSetting, IPriceListEndpoint priceListEndpoint)
        {
            _customerEndpoint = customerEndpoint;
            _mapper = mapper;
            _appSetting = appSetting;
            _priceListEndpoint = priceListEndpoint;
            IsLoadingVisible = true;
        }
        private bool _isLoadingVisible;

        public bool IsLoadingVisible
        {
            get { return _isLoadingVisible; }
            set
            {
                _isLoadingVisible = value;
                NotifyOfPropertyChange(() => IsLoadingVisible);
            }
        }
        public BindingList<CustomerDisplayModel> Partners
        {
            get { return _partners; }
            set
            {
                _partners = value;
                NotifyOfPropertyChange(() => Partners);
            }
        }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadPartners();
            IsLoadingVisible = false;
        }

        private Farm _selectedFarm;
        public Farm SelectedFarm
        {
            get { return _selectedFarm; }
            set
            {
                if(_selectedFarm != value)
                {
                    FarmId = value != null ? value.Id : 0;
                    _selectedFarm = value;
                    NotifyOfPropertyChange(() => SelectedFarm);
                }
            }
        }


        private BindingList<Farm> _farms;
        public BindingList<Farm> Farms
        {
            get { return _farms; }
            set
            {
                _farms = value;
                NotifyOfPropertyChange(() => Farms);
            }
        }

        private BindingList<Payment> _payments;
        public BindingList<Payment> Payments
        {
            get { return _payments; }
            set 
            { 
                _payments = value;
                NotifyOfPropertyChange(() => Payments);
            }
        }

        private BindingList<PriceList> _priceLists;
        public BindingList<PriceList> PriceLists
        {
            get { return _priceLists; }
            set
            {
                _priceLists = value;
                NotifyOfPropertyChange(() => PriceLists);
            }
        }

        private PriceList _selectedPriceList;
        public PriceList SelectedPriceList
        {
            get { return _selectedPriceList; }
            set
            {
                _selectedPriceList = value;
                NotifyOfPropertyChange(() => SelectedPriceList);
                NotifyOfPropertyChange(() => CanClear);
                NotifyOfPropertyChange(() => CanSave);
            }
        }


        private async Task LoadPriceLists()
        {
            try
            {
                var priceLists = await _priceListEndpoint.GetList();
                PriceLists = new BindingList<PriceList>(priceLists);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private async Task LoadPaymentTypes()
        {
            try
            {
                var paymentList = await _customerEndpoint.GetPayments();
                Payments = new BindingList<Payment>(paymentList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private List<Customer> allPartners = new List<Customer>();

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


        public void FilterCustomers(string message)
        {
            var partners = new List<CustomerDisplayModel>();

            SearchInput = message;

            if (string.IsNullOrWhiteSpace(SearchInput))
            {
                if (Partners.Count != allPartners.Count)
                    partners = _mapper.Map<List<CustomerDisplayModel>>(allPartners);
                else
                {
                    return;
                }
            }
            else
            {
                partners = _mapper.Map<List<CustomerDisplayModel>>(allPartners.Where(c => c.Name.ToLower().Contains(SearchInput.ToLower())));
            }
            
            Partners = new BindingList<CustomerDisplayModel>(partners);
        }
        private async Task LoadPartners()
        {
            allPartners = await _customerEndpoint.GetList();
            var partners = _mapper.Map<List<CustomerDisplayModel>>(allPartners);
            Partners = new BindingList<CustomerDisplayModel>(partners);
            await LoadPaymentTypes();
            await LoadPriceLists();
            await LoadFarms();
        }

        private async Task LoadFarms()
        {
            var farms = await _customerEndpoint.GetFarms();
            Farms = new BindingList<Farm>(farms.ToList());
        }

        private CustomerDisplayModel _selectedPartner;
        public CustomerDisplayModel SelectedPartner
        {
            get { return _selectedPartner; }
            set
            {
                _selectedPartner = value;
                if (_selectedPartner != null)
                {
                    Id = _selectedPartner.Id.ToString();
                    PartnerName = _selectedPartner.Name;
                    Brgy = _selectedPartner.Address1;
                    City = _selectedPartner.Address2;
                    MobileNo = _selectedPartner.MobileNo;
                    TIN = _selectedPartner.TIN;
                    ReferenceNo = _selectedPartner.ReferenceNo;

                    
                    if(_selectedPartner.FarmId > 0 )
                    {
                        var existingFarm = Farms.FirstOrDefault(x => x.Id == _selectedPartner.FarmId);
                        SelectedFarm = existingFarm;
                        IsFarm = true;
                    }
                    else
                    {
                        IsFarm = false;
                        SelectedFarm = null;
                    }
                    var existingPriceList = PriceLists.FirstOrDefault(x => x.Id == _selectedPartner.PriceListId);
                    SelectedPriceList = existingPriceList;
                    var existingPaymentType = Payments.FirstOrDefault(x => x.Id == _selectedPartner.PaymentTypeId);
                    SelectedPaymentType = existingPaymentType;
                    
                }

                NotifyOfPropertyChange(() => SelectedPaymentType);
                NotifyOfPropertyChange(() => SelectedPriceList);
                NotifyOfPropertyChange(() => SelectedPartner);
            }
        }

        private string _referenceNo;
        public string ReferenceNo
        {
            get { return _referenceNo; }
            set
            {
                _referenceNo = value;
                NotifyOfPropertyChange(() => ReferenceNo);
                NotifyOfPropertyChange(() => CanClear);
            }
        }
        private Payment _selectedPaymentType;
        public Payment SelectedPaymentType
        {
            get { return _selectedPaymentType; }
            set
            {
                _selectedPaymentType = value;
                NotifyOfPropertyChange(() => SelectedPaymentType);
                NotifyOfPropertyChange(() => CanClear);
                NotifyOfPropertyChange(() => CanSave);
            }
        }
       

        private string _tin;
        public string TIN
        {
            get { return _tin; }
            set
            {
                _tin = value;
                NotifyOfPropertyChange(() => TIN);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        private string _mobileNo;
        public string MobileNo
        {
            get { return _mobileNo; }
            set
            {
                _mobileNo = value;
                NotifyOfPropertyChange(() => MobileNo);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                NotifyOfPropertyChange(() => City);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        private string _brgy;
        public string Brgy
        {
            get { return _brgy; }
            set
            {
                _brgy = value;
                NotifyOfPropertyChange(() => Brgy);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        private string _partnerName;
        public string PartnerName
        {
            get { return _partnerName; }
            set
            {
                _partnerName = value;
                NotifyOfPropertyChange(() => PartnerName);
                NotifyOfPropertyChange(() => CanClear);
                NotifyOfPropertyChange(() => CanSave);
            }
        }
        private long _farmId;
        public long FarmId
        {
            get { return _farmId; }
            set
            {
                _farmId = value;
                NotifyOfPropertyChange(() => FarmId);
            }
        }
        private bool _isFarm;
        public bool IsFarm
        {
            get { return _isFarm; }
            set
            {
                _isFarm = value;
                NotifyOfPropertyChange(() => IsFarm);
                if (_isFarm == false)
                    SelectedFarm = null;
            }
        }
        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyOfPropertyChange(() => Id);
                NotifyOfPropertyChange(() => CanClear);
                NotifyOfPropertyChange(() => CanDelete);
            }
        }

        public bool CanClear
        {
            get
            {
                var output = string.IsNullOrWhiteSpace(Id) == false
                              || string.IsNullOrWhiteSpace(PartnerName) == false
                              || string.IsNullOrWhiteSpace(Brgy) == false
                              || string.IsNullOrWhiteSpace(City) == false
                              || string.IsNullOrWhiteSpace(MobileNo) == false
                              || string.IsNullOrWhiteSpace(TIN) == false
                              || string.IsNullOrWhiteSpace(ReferenceNo) == false
                              || SelectedPaymentType != null
                              || SelectedPriceList != null;

                return output;
            }
        }
        public void Clear()
        {
            if (MessageBox.Show("Are you sure?", "Clear Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                OnClear();
            }
        }

        private void OnClear()
        {
            Id = string.Empty;
            PartnerName = string.Empty;
            Brgy = string.Empty;
            City = string.Empty;
            MobileNo = string.Empty;
            TIN = string.Empty;
            ReferenceNo = string.Empty;
            SelectedPaymentType = null;
            SelectedPriceList = null;
            SelectedPartner = null;
            IsFarm = false;
            NotifyOfPropertyChange(() => CanDelete);
        }

        public bool CanDelete
        {
            get
            {
                bool output = false;

                if (string.IsNullOrWhiteSpace(Id) == false)
                {
                    output = true;
                }
                return output;
            }
        }

        public async Task Delete()
        {
            if (MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _customerEndpoint.Remove(Convert.ToInt64(Id));
                Partners.Remove(SelectedPartner);
                OnClear();
            }
        }

        public bool CanSave
        {
            get
            {
                var output = string.IsNullOrWhiteSpace(PartnerName) == false
                              && SelectedPaymentType != null
                              && SelectedPriceList != null;

                return output;
            }
        }

        public async Task Save()
        {
            if (MessageBox.Show("Are you sure?", "Save Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var customer = new Customer
                {
                    Name = PartnerName,
                    Address1 = Brgy,
                    Address2 = City,
                    MobileNo = MobileNo,
                    TIN = TIN,
                    ReferenceNo = ReferenceNo,
                    FarmId = FarmId,
                    PaymentType = SelectedPaymentType.PaymentType,
                    PaymentTypeId = SelectedPaymentType.Id,
                    PriceList = SelectedPriceList.Name,
                    PriceListId = SelectedPriceList.Id
                };

                if (SelectedPartner == null)
                {
                    var result = await _customerEndpoint.Save(customer);
                    var newPartner = _mapper.Map<CustomerDisplayModel>(result);
                    allPartners.Add(result);
                    Partners.Add(newPartner);
                }
                else
                {
                    customer.Id = SelectedPartner.Id;
                    await _customerEndpoint.Update(customer);
                    SelectedPartner.Name = PartnerName;
                    SelectedPartner.Address1 = Brgy;
                    SelectedPartner.Address2 = City;
                    SelectedPartner.MobileNo = MobileNo;
                    SelectedPartner.TIN = TIN;
                    SelectedPartner.ReferenceNo = ReferenceNo;
                    SelectedPartner.FarmId = FarmId;
                    SelectedPartner.PaymentType = SelectedPaymentType.PaymentType;
                    SelectedPartner.PaymentTypeId = SelectedPaymentType.Id;
                    SelectedPartner.PriceListId = SelectedPriceList.Id;
                    SelectedPartner.PriceList = SelectedPriceList.Name;
                }
                OnClear();
            }
        }

        public void Close()
        {
            TryClose();
        }

       
    }
}
