using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using Caliburn.Micro;
using Project.FC2J.Models.Enums;
using Project.FC2J.Models.Sale;
using Project.FC2J.Models.User;
using Project.FC2J.UI.Helpers;
using Project.FC2J.UI.Models;

namespace Project.FC2J.UI.ViewModels
{
    public class CollectionViewModel : Screen
    {
        private readonly IMapper _mapper;
        private readonly ISaleEndpoint _saleEndpoint;
        private readonly IEventAggregator _events;
        private readonly ILoggedInUser _loggedInUser;

        public CollectionViewModel(ISaleEndpoint saleEndpoint, IMapper mapper, IEventAggregator events, ILoggedInUser loggedInUser)
        {
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
            _events = events;
            _loggedInUser = loggedInUser;
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

        public bool IsAdmin => _loggedInUser.User.UserRole.RoleName.ToLower().Equals("admin");

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadCollections();
            await LoadCollected();
            await LoadBadAccount();
            OnReset();
            IsLoadingVisible = false;
            NotifyOfPropertyChange(() => IsAdmin);
        }

        private List<OrderHeader> _allSalesForCollections;
        private List<OrderHeader> _allSalesCollected;
        private List<OrderHeader> _allSalesBadAccount;

        private async Task LoadCollections()
        {
            //

            _allSalesForCollections = await _saleEndpoint.GetCollection(_loggedInUser.User.UserName.ToLower());
            var sales = _mapper.Map<List<SalesDisplayModel>>(_allSalesForCollections);
            SalesCollections = new BindingList<SalesDisplayModel>(sales);
        }

        private async Task LoadCollected()
        {
            _allSalesCollected = await _saleEndpoint.GetCollected(_loggedInUser.User.UserName.ToLower(),1);
            var sales = _mapper.Map<List<SalesDisplayModel>>(_allSalesCollected);
            SalesCollected = new BindingList<SalesDisplayModel>(sales);
        }

        private async Task LoadBadAccount()
        {
            _allSalesBadAccount = await _saleEndpoint.GetCollected(_loggedInUser.User.UserName.ToLower(), 0);
            var sales = _mapper.Map<List<SalesDisplayModel>>(_allSalesBadAccount);
            SalesBadAccount = new BindingList<SalesDisplayModel>(sales);
        }

        private void OnReset()
        {
            ResetPaymentOption();
            ResetModeOption();
            ClearCashParameters();
            ClearCheckParameters();
            SelectedSalesCollection = null;
            IsBadAccount = false;
        }

        private void ResetPaymentOption()
        {
            IsCash = true;
            IsCheck = false;
        }

        private void ResetModeOption()
        {
            IsFull = true;
            IsPartial = false;
        }

        private void ClearCashParameters()
        {
            CashAmount = 0;
            CashAmountRemarks = string.Empty;
        }

        private void ClearCheckParameters()
        {
            CheckDate = DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            Bank = string.Empty;
            CheckNo = string.Empty;
            CheckAmount = 0;
        }

        public void Close()
        {
            TryClose();
        }
        public void Clear()
        {
            OnReset();
        }

        public async Task Refresh()
        {
            IsLoadingVisible = true;
            await LoadCollections();
            await LoadCollected();
            await LoadBadAccount();
            OnReset();
            IsLoadingVisible = false;
        }

        public bool CanSubmit
        {
            get
            {
                if (IsCash)
                {
                    return CashAmount > 0 && CashAmount <= Remaining;
                }
                return CheckAmount > 0 && !string.IsNullOrWhiteSpace(Bank) && !string.IsNullOrWhiteSpace(CheckNo) && CheckAmount <= Remaining;
            }
        } 

        public async Task Submit()
        {
            if (MessageBox.Show("Are you sure?", "Submit Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var salePayment = new SalePayment
                {
                    Id = Convert.ToInt64(SelectedSalesCollection?.Id),
                    CustomerId = Convert.ToInt64(SelectedSalesCollection?.CustomerId),
                    UserName = _loggedInUser.User.UserName.ToLower(),
                    OrderStatusId = IsFull ? (long) OrderStatusEnum.PAID : (long) OrderStatusEnum.PARTIAL,
                    PONo = SelectedSalesCollection?.PONo,
                    PaidAmount = IsCash ? Convert.ToDecimal(CashAmount) : Convert.ToDecimal(CheckAmount),
                    IsCash = IsCash,
                    CashRemarks = IsCash ? CashAmountRemarks : string.Empty,
                    CheckBank = IsCheck ? Bank : string.Empty,
                    CheckNumber = IsCheck ? CheckNo : string.Empty,
                    CheckDate = Convert.ToDateTime(CheckDate, CultureInfo.InvariantCulture)
                };
                if (IsBadAccount)
                    salePayment.OrderStatusId = (long) OrderStatusEnum.BADACCOUNTS;

                await _saleEndpoint.PayInvoice(salePayment);
                MessageBox.Show("Payment is processed", "System Confirmation", MessageBoxButton.OK);
                await Refresh();
            }
        }

        public bool CanRetrievePaid => SelectedSalesCollected != null;
        public async Task RetrievePaid()
        {
            if (MessageBox.Show("Are you sure?", "Retrieve Paid Invoice Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var salePayment = new SalePayment
                {
                    Id = Convert.ToInt64(SelectedSalesCollected?.Id),
                    CustomerId = Convert.ToInt64(SelectedSalesCollected?.CustomerId),
                    UserName = _loggedInUser.User.UserName.ToLower()
                };

                IsLoadingVisible = true;
                await _saleEndpoint.RetrievePaidBadSale(salePayment);
                await LoadCollections();
                await LoadCollected();
                //await LoadBadAccount();
                OnReset();
                IsLoadingVisible = false;
            }

        }

        public bool CanRetrieveBadAccount => SelectedSalesBadAccount != null;
        public async Task RetrieveBadAccount()
        {
            if (MessageBox.Show("Are you sure?", "Retrieve Bad Account Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var salePayment = new SalePayment
                {
                    Id = Convert.ToInt64(SelectedSalesBadAccount?.Id),
                    CustomerId = Convert.ToInt64(SelectedSalesBadAccount?.CustomerId),
                    UserName = _loggedInUser.User.UserName.ToLower()
                };

                IsLoadingVisible = true;
                await _saleEndpoint.RetrievePaidBadSale(salePayment);
                await LoadCollections();
                //await LoadCollected();
                await LoadBadAccount();
                OnReset();
                IsLoadingVisible = false;
            }

        }

        public void FilterLists(string value)
        {
            List<SalesDisplayModel> _salesCollection;

            if (string.IsNullOrWhiteSpace(value))
            {
                if (SalesCollections.Count != _allSalesForCollections.Count)
                    _salesCollection = _mapper.Map<List<SalesDisplayModel>>(_allSalesForCollections);
                else
                {
                    return;
                }
            }
            else
            {
                _salesCollection = _mapper.Map<List<SalesDisplayModel>>(_allSalesForCollections.Where(c => c.CustomerName.ToLower().Contains(value.ToLower())));
                if(_salesCollection == null || _salesCollection.Count == 0 )
                    _salesCollection = _mapper.Map<List<SalesDisplayModel>>(_allSalesForCollections.Where(c => c.PONo.ToLower().Contains(value.ToLower())));
            }

            SalesCollections = new BindingList<SalesDisplayModel>(_salesCollection);
        }

        #region Properties 
        private BindingList<SalesDisplayModel> _salesCollections;
        public BindingList<SalesDisplayModel> SalesCollections
        {
            get { return _salesCollections; }
            set
            {
                _salesCollections = value;
                NotifyOfPropertyChange(() => SalesCollections);
                NotifyOfPropertyChange(() => Count);
            }
        }
        public string Count => _allSalesForCollections?.Count.ToString();

        private SalesDisplayModel _selectedSalesCollected;
        public SalesDisplayModel SelectedSalesCollected
        {
            get { return _selectedSalesCollected; }
            set
            {
                _selectedSalesCollected = value;
                NotifyOfPropertyChange(() => SelectedSalesCollected);
                NotifyOfPropertyChange(() => CanRetrievePaid);
            }
        }
        private BindingList<SalesDisplayModel> _salesCollected;
        public BindingList<SalesDisplayModel> SalesCollected
        {
            get { return _salesCollected; }
            set
            {
                _salesCollected = value;
                NotifyOfPropertyChange(() => SalesCollected);
            }
        }

        private SalesDisplayModel _selectedSalesBadAccount;
        public SalesDisplayModel SelectedSalesBadAccount
        {
            get { return _selectedSalesBadAccount; }
            set
            {
                _selectedSalesBadAccount = value;
                NotifyOfPropertyChange(() => SelectedSalesBadAccount);
                NotifyOfPropertyChange(() => CanRetrieveBadAccount);
            }
        }
        private BindingList<SalesDisplayModel> _salesBadAccount;
        public BindingList<SalesDisplayModel> SalesBadAccount
        {
            get { return _salesBadAccount; }
            set
            {
                _salesBadAccount = value;
                NotifyOfPropertyChange(() => SalesBadAccount);
            }
        }
        #region Selected Invoice Details

        private SalesDisplayModel _selectedSalesCollection;
        public SalesDisplayModel SelectedSalesCollection
        {
            get { return _selectedSalesCollection; }
            set
            {
                _selectedSalesCollection = value;
                NotifyOfPropertyChange(() => SelectedSalesCollection);
                NotifyOfPropertyChange(() => DeliveryDate);
                NotifyOfPropertyChange(() => CustomerName);
                NotifyOfPropertyChange(() => PONo);
                NotifyOfPropertyChange(() => TotalPrice);
                NotifyOfPropertyChange(() => DueDate);
                NotifyOfPropertyChange(() => PaidAmount);
                NotifyOfPropertyChange(() => Remaining);
                NotifyOfPropertyChange(() => IsOverlayVisible);
                NotifyOfPropertyChange(() => IsOverdue);
                NotifyOfPropertyChange(() => IsNeardue);
                NotifyOfPropertyChange(() => NdaysToDue);

                NotifyOfPropertyChange(() => IsFull);
                SetCashValue();
            }
        }

        public string DeliveryDate => SelectedSalesCollection?.DeliveryDate.ToString("MMM-dd-yyyy");
        public string CustomerName => SelectedSalesCollection?.CustomerName;
        public string PONo => SelectedSalesCollection?.PONo;
        public decimal TotalPrice => Convert.ToDecimal(SelectedSalesCollection?.TotalPrice);
        public string DueDate => SelectedSalesCollection?.DueDate.ToString("MMM-dd-yyyy");
        public decimal PaidAmount => Convert.ToDecimal(SelectedSalesCollection?.PaidAmount);
        public decimal Remaining => Convert.ToDecimal(SelectedSalesCollection?.TotalPrice) - Convert.ToDecimal(SelectedSalesCollection?.PaidAmount);

        public bool IsOverlayVisible => SelectedSalesCollection == null;
        public bool IsOverdue => Convert.ToBoolean(SelectedSalesCollection?.IsOverdue);
        public bool IsNeardue => Convert.ToBoolean(SelectedSalesCollection?.IsNeardue);
        public string NdaysToDue => $"{SelectedSalesCollection?.NearDueDays} before Due Date" ;
        

        #endregion



        private bool _isCash;
        public bool IsCash
        {
            get { return _isCash; }
            set
            {
                _isCash = value;
                NotifyOfPropertyChange(() => IsCash);
                SetCashValue();
            }
        }
        private bool _isCheck;
        public bool IsCheck
        {
            get { return _isCheck; }
            set
            {
                _isCheck = value;
                NotifyOfPropertyChange(() => IsCheck);
                SetCashValue();
            }
        }
        private bool _isFull;
        public bool IsFull
        {
            get { return _isFull; }
            set
            {
                _isFull = value;
                NotifyOfPropertyChange(() => IsFull);
                SetCashValue();
            }
        }

        private void SetCashValue()
        {
            CashAmount = 0;
            CheckAmount = 0;

            if (IsFull)
            {
                if (IsCash)
                {
                    CashAmount = TotalPrice - Convert.ToDecimal(SelectedSalesCollection?.PaidAmount);
                    NotifyOfPropertyChange(() => CashAmount);
                }
                else
                {
                    CheckAmount = TotalPrice - Convert.ToDecimal(SelectedSalesCollection?.PaidAmount);
                    NotifyOfPropertyChange(() => CheckAmount);
                    NotifyOfPropertyChange(() => CanSubmit);
                }
            }
        }
        private bool _isPartial;
        public bool IsPartial
        {
            get { return _isPartial; }
            set
            {
                _isPartial = value;
                NotifyOfPropertyChange(() => IsPartial);
            }
        }
        private decimal _cashAmount;
        public decimal CashAmount
        {
            get
            {
                return _cashAmount;
            }
            set
            {
                _cashAmount = value;
                NotifyOfPropertyChange(() => CashAmount);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }
        private string _cashAmountRemarks;
        public string CashAmountRemarks
        {
            get
            {
                return _cashAmountRemarks;
            }
            set
            {
                _cashAmountRemarks = value;
                NotifyOfPropertyChange(() => CashAmountRemarks);
            }
        }
        private string _checkDate;
        public string CheckDate
        {
            get { return _checkDate; }
            set
            {
                _checkDate = value;
                NotifyOfPropertyChange(() => CheckDate);
            }
        }
        private string _bank;
        public string Bank
        {
            get { return _bank; }
            set
            {
                _bank = value;
                NotifyOfPropertyChange(() => Bank);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }
        private string _checkNo;
        public string CheckNo
        {
            get { return _checkNo; }
            set
            {
                _checkNo = value;
                NotifyOfPropertyChange(() => CheckNo);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }
        private decimal _checkAmount;
        public decimal CheckAmount
        {
            get { return _checkAmount; }
            set
            {
                _checkAmount = value;
                NotifyOfPropertyChange(() => CheckAmount);
            }
        }
        private bool _isBadAccount;

        public bool IsBadAccount
        {
            get { return _isBadAccount; }
            set
            {
                _isBadAccount = value;
                NotifyOfPropertyChange(() => IsBadAccount);
            }
        }

        #endregion


    }
}
