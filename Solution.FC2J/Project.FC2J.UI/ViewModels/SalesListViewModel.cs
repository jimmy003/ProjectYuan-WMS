using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Caliburn.Micro;
using Project.FC2J.UI.Helpers;
using AutoMapper;
using Project.FC2J.Models.Sale;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Order;
using Project.FC2J.Models.User;
using Project.FC2J.UI.Helpers.AppSetting;
using Project.FC2J.UI.Helpers.Products;
using Project.FC2J.UI.Models;
using Project.FC2J.UI.Providers;
using Project.FC2J.UI.UserControls;
using MessageBox = System.Windows.MessageBox;
using Screen = Caliburn.Micro.Screen;
using TextBox = System.Windows.Controls.TextBox;

namespace Project.FC2J.UI.ViewModels
{
    public class SalesListViewModel : Screen
    {

        private bool _isSalesCommitted = false;

        //Sales Order 
        private readonly IProductEndpoint _productEndpoint;
        private BindingList<ProductDisplayModel> _products;
        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();
        private CartItemDisplayModel _selectedCartItem;
        private BindingList<CustomerDisplayModel> _partners;
        private readonly ICustomerEndpoint _customerEndpoint;
        private readonly IApiAppSetting _apiAppSetting;
        private readonly ISaleEndpoint _saleEndpoint;
        private readonly IPriceListEndpoint _priceListEndpoint;
        private readonly ISaleData _saleData;
        private readonly IEventAggregator _events;
        private readonly IMapper _mapper;
        private readonly IProfileData _profileData;
        private readonly ILoggedInUser _loggedInUser;

        private string _orderNo = "";
        private string _poNo = "";
        private string _soNo = "";

        private string _invoiceNo = string.Empty;
        private string _invoiceDate = string.Empty;

        private string _customerId = "";
        private string _orderDate;
        private int _orderStatusId = 1;
        private string _selectedPriceList = "";


        //Sales List


        public SalesListViewModel(IProductEndpoint productEndpoint,
            ICustomerEndpoint customerEndpoint,
            ISaleEndpoint saleEndpoint,
            IProfileData profileData,
            IApiAppSetting apiAppSetting,
            ILoggedInUser loggedInUser,
            IEventAggregator events,
            ISaleData saleData,
            IPriceListEndpoint priceListEndpoint,
            IMapper mapper, IApiAppSetting appSetting,
            IDeductionEndpoint deductionEndpoint)
        {
            _productEndpoint = productEndpoint;
            _customerEndpoint = customerEndpoint;
            _saleEndpoint = saleEndpoint;
            _priceListEndpoint = priceListEndpoint;
            _profileData = profileData;
            _apiAppSetting = apiAppSetting;
            _loggedInUser = loggedInUser;
            _saleData = saleData;
            _events = events;
            _mapper = mapper;
            _deductionEndpoint = deductionEndpoint;
            NotifyOfPropertyChange(() => IsListVisible);
        }


        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            //Sales List
            IsGridVisible = false;
            IsLoadingVisible = true;
            IsOverrideHidden = false;
            await LoadSales();
        }


        #region Sales List

        private async Task LoadSales()
        {
            var salesList = await _saleEndpoint.GetSales(_loggedInUser.User.UserName.ToLower());
            var sales = _mapper.Map<List<SalesDisplayModel>>(salesList);
            Sales = new ObservableCollection<SalesDisplayModel>(sales);
            await Show("0");
            IsGridVisible = true;
            IsLoadingVisible = false;
        }
        
        private bool _isGridVisible = false;
        public bool IsGridVisible
        {
            get { return _isGridVisible; }
            set
            {
                _isGridVisible = value;
                NotifyOfPropertyChange(() => IsGridVisible);
            }
        }

        private bool _isLoadingVisible = false;
        public bool IsLoadingVisible
        {
            get { return _isLoadingVisible; }
            set
            {
                _isLoadingVisible = value;
                NotifyOfPropertyChange(() => IsLoadingVisible);
            }
        }
        //SelectedSale
        private SalesDisplayModel _selectedSale;
        public SalesDisplayModel SelectedSale
        {
            get { return _selectedSale; }
            set
            {
                _selectedSale = value;
                NotifyOfPropertyChange(() => SelectedSale);
            }
        }

        private ObservableCollection<SalesDisplayModel> _sales;
        public ObservableCollection<SalesDisplayModel> Sales
        {
            get { return _sales; }
            set
            {
                _sales = value;
                NotifyOfPropertyChange(() => Sales);
                NotifyOfPropertyChange(() => CollectionView);
            }
        }
        private CollectionView _collectionView = null;
        public CollectionView CollectionView
        {
            get
            {
                _collectionView = (CollectionView)CollectionViewSource.GetDefaultView(Sales);
                _collectionView?.SortDescriptions.Add(new SortDescription("DeliveryDate", ListSortDirection.Descending));
                return _collectionView;
            }
        }
        public bool IsListVisible => _isListVisible;

        public bool IsSalesOrderVisible => !_isListVisible;

        public void Close()
        {
            TryClose();
        }

        public async Task CreateNewSO()
        {
            await ResetSalesViewModel();
            await OnShowSalesDetail(null);
            Show("1");
        }

        public async Task OnShowSalesDetail(SalesDisplayModel value, string mode)
        {
            value = SelectedSale;

            if (value.OrderStatusId <= 3)
            {
                await Show("1");
                await OnShowSalesDetail(value);
            }
            else
            {
                MessageBox.Show($"Record is {value.OrderStatus} already", "System Information", MessageBoxButton.OK);
            }
        }

        

        private List<Deduction> _usedDeductions = new List<Deduction>();

        public async Task OnShowSalesDetail(SalesDisplayModel value)
        {

            _saleData.Value = _mapper.Map<SaleHeader>(value);
            if (value != null)
            {
                _saleData.Value.SaleDetails = await _saleEndpoint.GetSaleDetails(_saleData.Value.Id, _saleData.Value.CustomerId);
                _usedDeductions = await _deductionEndpoint.GetDeductions(_saleData.Value.PONo, _saleData.Value.CustomerId);
            }

            //Sales Order
            await LoadPartners();
            await LoadSOInvoiceNo();
            await LoadPaymentTypes();

            Branch = _apiAppSetting.Branch;

            if (_saleData.Value == null)
            {
                //new sales order
                SalesId = null;
                UserName = _profileData.UserName;
                DeliveryDate = DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            }
            else
            {
                //submitted or validated  

                SelectedPartner = Partners.FirstOrDefault(x => x.Id == _saleData.Value.CustomerId);
                SelectedPaymentType = Payments.FirstOrDefault(x => x.Id == _saleData.Value.SelectedPaymentTypeId);

                DeliveryDate = _saleData.Value.DeliveryDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                DueDate = _saleData.Value.DueDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                InvoiceDate = _saleData.Value.OrderDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

                SONo = _saleData.Value.SONo;
                InvoiceNo = _saleData.Value.InvoiceNo;
                UserName = _saleData.Value.UserName;
                PONo = _saleData.Value.PONo;
                SalesId = _saleData.Value.Id.ToString();

                
                //SelectedSupplier = "System.Windows.Controls.ComboBoxItem: San Ildefonso" ;
                //Supplier = _saleData.Value.SaleDetails[0].Supplier;

                OrderStatusId = Convert.ToInt32(_saleData.Value.OrderStatusId);
                SelectedSupplier = "System.Windows.Controls.ComboBoxItem: " + _saleData.Value.SaleDetails[0].Supplier;
                IsSupplier = false;
                IsSupplierDisplay = true;
                SupplierDisplay = $"Supplier: {_saleData.Value.SaleDetails[0].Supplier}";
                OtherDeduction = "P" + _saleData.Value.LessPrice.ToString("C").Substring(1);

                IsPickup = _saleData.Value.PickUpDiscount > 0;

                await LoadProducts();

                SetCart();

                //should show list of used deductions
                await SetDeductions();

                //CashDiscount = CalculateDeductionCashDiscount();
                var cashDiscount = CalculateDeductionCashDiscount();
                if (cashDiscount != _saleData.Value.CashDiscount)
                {
                    CashDiscount = _saleData.Value.CashDiscount;
                }
                else
                {
                    CashDiscount = cashDiscount;
                }


            }


        }

        public int SelectedButton => _isListVisible ? 0 : 1;

        private bool _isListVisible = true;
        public async Task Show(string selected)
        {

            if (selected == "0")
            {
                _isListVisible = true;
            }
            else
            {
                _isListVisible = false;
                await ResetSalesViewModel();
                await OnShowSalesDetail(null);
            }

            NotifyOfPropertyChange(() => IsListVisible);
            NotifyOfPropertyChange(() => IsSalesOrderVisible);
            NotifyOfPropertyChange(() => SelectedButton);

            if (_isSalesCommitted)
            {
                _isSalesCommitted = false;
                await LoadSales();
            }
        }

        private bool _isPriceListGridVisible = false;
        public bool IsSalesOrderGridVisible => !_isPriceListGridVisible;
        public bool IsPriceListGridVisible => _isPriceListGridVisible;
        public bool IsPriceListBorderVisible => _isPriceListGridVisible;
       
        #endregion



        #region Sales Order
        private void SetCart()
        {

            Cart = new BindingList<CartItemDisplayModel>();
            _saleData.Value.SaleDetails.ForEach(item =>
            {
                var product = Products.FirstOrDefault(p => p.Id == item.ProductId);

                Cart.Add(new CartItemDisplayModel
                {
                    Product = product,
                    CartQuantity = item.OrderQuantity,
                    Supplier = item.Supplier
                });

            });

            CashDiscount = CalculateDeductionCashDiscount();

            NotifyOfPropertyChange(() => PickupDiscount);
            NotifyOfPropertyChange(() => Deductions);

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => TotalQuantity);
            NotifyOfPropertyChange(() => TotalQuantityUOMComputed);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => StockQuantity);
            NotifyOfPropertyChange(() => CanValidateSale);
        }

        public bool IsSalesId => !string.IsNullOrEmpty(SalesId);
        public bool IsFromSalesList => _saleData.IsFromSalesList;
        public string Role => _loggedInUser.User.UserRole.RoleName;

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

        private async Task LoadSOInvoiceNo()
        {
            SONo = await _saleEndpoint.GetSONo();
            var invoice = await _saleEndpoint.GetInvoiceNo();
            InvoiceNo = invoice.InvoiceNo;


            InvoiceDate = DateTime.ParseExact(invoice.InvoiceDate, "MM/dd/yyyy", CultureInfo.InvariantCulture)
                .ToString("MM/dd/yyyy", CultureInfo.InvariantCulture); //Convert.ToDateTime(invoice.InvoiceDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            //DateTime.ParseExact(invoice.InvoiceDate, "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString();

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
                ErrorMessage = $"LoadPaymentTypes: {ex.Message}";
            }
        }

        private async Task LoadProducts()
        {
            var productList = await _customerEndpoint.GetCustomerPriceList(SelectedPartner.Id, Convert.ToInt32(_supplierEnum));
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        private async Task LoadPartners()
        {
            var partnerList = await _customerEndpoint.GetList();
            var partners = _mapper.Map<List<CustomerDisplayModel>>(partnerList);
            Partners = new BindingList<CustomerDisplayModel>(partners);
        }


        private string _salesId;

        public string SalesId
        {
            get { return _salesId; }
            set
            {
                _salesId = value;
                NotifyOfPropertyChange(() => IsCancelVisible);
                NotifyOfPropertyChange(() => OrderStatus);
                NotifyOfPropertyChange(() => SalesId);
                NotifyOfPropertyChange(() => IsSalesId);
                NotifyOfPropertyChange(() => CanValidateSale);
                NotifyOfPropertyChange(() => PONoEnabled);
                NotifyOfPropertyChange(() => PartnersControlEnabled);
                NotifyOfPropertyChange(() => SupplierEnabled);
                NotifyOfPropertyChange(() => DeliveryDateEnabled);
                NotifyOfPropertyChange(() => ProductsControlEnabled);
                NotifyOfPropertyChange(() => PaymentsEnabled);
                NotifyOfPropertyChange(() => ItemQuantityEnabled);
            }
        }

        private string _branch;
        public string Branch
        {
            get { return _branch; }
            set
            {
                _branch = value;
                NotifyOfPropertyChange(() => Branch);
            }
        }
        public string SONo
        {
            get { return _soNo; }
            set
            {
                _soNo = value;
                NotifyOfPropertyChange(() => SONo);
            }
        }
        public string InvoiceNo
        {
            get { return _invoiceNo; }
            set
            {
                _invoiceNo = value;
                NotifyOfPropertyChange(() => InvoiceNo);
            }
        }


        public string InvoiceDate
        {
            get { return _invoiceDate; }
            set
            {
                _invoiceDate = value;
                NotifyOfPropertyChange(() => InvoiceDate);
            }
        }



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
        public ProductSuggestionProvider ProductsProvider
        {
            get
            {
                var productsProvider = new ProductSuggestionProvider { Products = Products };
                return productsProvider;
            }
        }


        private SupplierEnum? _supplierEnum;
        private string _selectedSupplier;
        public string SelectedSupplier
        {
            get { return _selectedSupplier; }
            set
            {
                if(_selectedSupplier != value)
                {
                    _selectedSupplier = value;
                    switch (_selectedSupplier)
                    {
                        case "System.Windows.Controls.ComboBoxItem: Lubang":
                            _supplierEnum = SupplierEnum.LUBANG;
                            break;
                        case "System.Windows.Controls.ComboBoxItem: Coron":
                            _supplierEnum = SupplierEnum.CORON;
                            break;
                        case "System.Windows.Controls.ComboBoxItem: San Ildefonso":
                            _supplierEnum = SupplierEnum.SANILDEFONSO;
                            break;
                    }

                    if (SalesId == null)
                    {
                        _events.Publish("SelectedPartner Changed",action => { Task.Factory.StartNew(OnLoadProducts()); });
                    }
                    NotifyOfPropertyChange(() => SelectedSupplier);
                }
            }
        }

        private bool _isSupplierDisplay;
        public bool IsSupplierDisplay
        {
            get { return _isSupplierDisplay; }
            set
            {
                _isSupplierDisplay = value;
                NotifyOfPropertyChange(() => IsSupplierDisplay);
            }
        }

        private bool _isSupplier;
        public bool IsSupplier
        {
            get { return _isSupplier; }
            set
            {
                _isSupplier = value;
                NotifyOfPropertyChange(() => IsSupplier);
            }
        }

        private string _supplierDisplay;
        public string SupplierDisplay 
        {
            get { return _supplierDisplay; }
            set
            {
                _supplierDisplay = value;
                NotifyOfPropertyChange(() => SupplierDisplay);
            }
        }


        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
                NotifyOfPropertyChange(() => ProductsProvider);
            }
        }

        public long PriceListId => SelectedPartner?.PriceListId ?? 0;
        public string PriceList => SelectedPartner?.PriceList ?? "";

        private CustomerDisplayModel _selectedPartner;
        public CustomerDisplayModel SelectedPartner
        {
            get
            {
                return _selectedPartner;
            }
            set
            {
                _selectedPartner = value;

                NotifyOfPropertyChange(() => SelectedPartner);
                SelectedPaymentType = null;
                if (SelectedPartner != null)
                {
                    var existingPaymentType = Payments.FirstOrDefault(x => x.Id == _selectedPartner.PaymentTypeId);
                    SelectedPaymentType = existingPaymentType;
                }
                NotifyOfPropertyChange(() => CanCheckOut);
                NotifyOfPropertyChange(() => CanValidateSale);

                NotifyOfPropertyChange(() => Brgy);
                NotifyOfPropertyChange(() => City);
                NotifyOfPropertyChange(() => PriceListId);
                NotifyOfPropertyChange(() => PriceList);

                _events.Publish("SelectedPartner Changed", action =>
                {
                    Task.Factory.StartNew(OnLoadProducts());
                });

                NotifyOfPropertyChange(() => ProductsControlEnabled);
                NotifyOfPropertyChange(() => ItemQuantityEnabled);
                NotifyOfPropertyChange(() => CanShowPriceList);
                NotifyOfPropertyChange(() => CanShowDeduction);



            }
        }

        private System.Action OnLoadProducts()
        {
            return async () =>
            {
                if (SelectedPartner != null && _supplierEnum != null)
                    await LoadProducts();
                else
                {
                    Products = new BindingList<ProductDisplayModel>();
                }
            };
        }


        public string Brgy => SelectedPartner?.Address1 ?? "";
        public string City => SelectedPartner?.Address2 ?? "";

        private Payment _selectedPaymentType;
        public Payment SelectedPaymentType
        {
            get
            {
                return _selectedPaymentType;
            }
            set
            {
                _selectedPaymentType = value;
                NotifyOfPropertyChange(() => SelectedPaymentType);
                NotifyOfPropertyChange(() => CanCheckOut);
                CalculateDeliveryDate();
            }
        }

        private void CalculateDeliveryDate()
        {
            DeliveryDate = DateTime.Now.ToString("MM/dd/yyyy",
                System.Globalization.CultureInfo.InvariantCulture);

            if (_selectedPaymentType != null)
            {
                try
                {
                    if (!_selectedPaymentType.PaymentType.ToLower().Equals("immediate payment"))
                    {
                        var d = Convert.ToInt32(_selectedPaymentType.PaymentType.ToLower().Replace(" days", ""));
                        DeliveryDate = DateTime.Now.AddDays(d).ToString("MM/dd/yyyy",
                            System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ErrorMessage = $"SelectedPaymentType: {ex.Message}";
                }
            }
            //DueDate = DeliveryDate;

        }

        private ProductDisplayModel _selectedProduct;

        public ProductDisplayModel SelectedProduct
        {
            get
            {
                return _selectedProduct;
            }
            set
            {
                if(_selectedProduct != value)
                {
                    _selectedProduct = value;
                    NotifyOfPropertyChange(() => SelectedProduct);
                    NotifyOfPropertyChange(() => ProductsControlEnabled);
                    NotifyOfPropertyChange(() => Description);
                    NotifyOfPropertyChange(() => Id);
                    NotifyOfPropertyChange(() => SalePrice);
                    NotifyOfPropertyChange(() => StockQuantity);
                    NotifyOfPropertyChange(() => CanAddToCart);
                }
            }
        }


        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }



        public string Description => SelectedProduct == null ? "" : SelectedProduct.Description;

        public string Id
        {
            get
            {
                if (SelectedProduct == null)
                {
                    return "";
                }
                return SelectedProduct.Id.ToString();
            }
        }

        public decimal SalePrice
        {
            get
            {
                if (SelectedProduct == null)
                {
                    return 0;
                }
                return SelectedProduct.DeductionFixPrice > 0 ? SelectedProduct.DeductionFixPrice : SelectedProduct.SalePrice;
            }
        }

        public double StockQuantity
        {
            get
            {
                if (SelectedProduct == null)
                {
                    return 0;
                }
                return SelectedProduct.StockQuantity;
            }
        }

        public BindingList<CartItemDisplayModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private float _itemQuantity = 1;
        public float ItemQuantity
        {
            get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public string CustomerId
        {
            get { return _customerId; }
            set
            {
                _customerId = value;
                NotifyOfPropertyChange(() => CustomerId);
            }
        }

        public string OrderNo
        {
            get { return _orderNo; }
            set
            {
                _orderNo = value;
                NotifyOfPropertyChange(() => OrderNo);
            }
        }

        public string PONo
        {
            get { return _poNo; }
            set
            {
                _poNo = value;
                NotifyOfPropertyChange(() => PONo);
                NotifyOfPropertyChange(() => CanCheckOut);
                NotifyOfPropertyChange(() => CanValidateSale);
            }
        }


        public string OrderDate
        {
            get { return _orderDate; }
            set
            {
                _orderDate = value;
                NotifyOfPropertyChange(() => OrderDate);
            }
        }

        private string _deliveryDate;
        public string DeliveryDate
        {
            get { return _deliveryDate; }
            set
            {
                if(_deliveryDate != value)
                {
                    _deliveryDate = value;
                    NotifyOfPropertyChange(() => DeliveryDate);
                    NotifyOfPropertyChange(() => CanValidateSale);
                    try
                    {
                        DueDate = Convert.ToDateTime(_deliveryDate).ToShortDateString();
                    }
                    catch (Exception e)
                    {
                        DueDate = string.Empty;
                    }
                }
            }
        }

        private string _dueDate;
        public string DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
                NotifyOfPropertyChange(() => DueDate);
            }
        }


        public string OrderStatus
        {
            get
            {
                if (string.IsNullOrEmpty(SalesId))
                {
                    return "New";
                }
                else
                {

                    return ((OrderStatusEnum) OrderStatusId).ToString();

                    //switch ((OrderStatusEnum)OrderStatusId)
                    //{
                    //    case OrderStatusEnum.SUBMITTED:
                    //    {
                    //        break;
                    //    }
                    //    case OrderStatusEnum.VALIDATED:
                    //    {
                    //        break;
                    //    }
                    //    case OrderStatusEnum.CANCELLED:
                    //    {
                    //        break;
                    //    }
                    //    default:
                    //    {
                    //        break;
                    //    }
                    //}

                    //if (OrderStatusId == 1)
                    //{
                    //    return "Submitted";
                    //}
                    //else if (OrderStatusId == 2)
                    //{
                    //    return "Validated";
                    //}
                    //else if (OrderStatusId == 3)
                    //{
                    //    return "Cancelled";
                    //}
                    //return "New";
                }

            }
        }

        public bool IsCancelVisible => string.IsNullOrEmpty(SalesId) == false 
                                       && (OrderStatusId == 1 || OrderStatusId == 2)
                                       && Role.ToLower().Equals("admin");

        public int OrderStatusId
        {
            get { return _orderStatusId; }
            set
            {
                _orderStatusId = value;
                NotifyOfPropertyChange(() => IsCancelVisible);
                NotifyOfPropertyChange(() => OrderStatusId);
                NotifyOfPropertyChange(() => CanCheckOut);
                NotifyOfPropertyChange(() => CanValidateSale);
                NotifyOfPropertyChange(() => CanAddToCart);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
                NotifyOfPropertyChange(() => PaymentsEnabled);
                NotifyOfPropertyChange(() => PartnersControlEnabled);
                NotifyOfPropertyChange(() => ProductsControlEnabled);
                NotifyOfPropertyChange(() => DeliveryDateEnabled);
                NotifyOfPropertyChange(() => OrderStatus);
                NotifyOfPropertyChange(() => ItemQuantityEnabled);

            }
        }

        public string SelectedPriceList
        {
            get { return _selectedPriceList; }
            set
            {
                _selectedPriceList = value;
                NotifyOfPropertyChange(() => SelectedPriceList);
            }
        }

       
        public string SubTotal => "P" + CalculateSubTotal().ToString("C").Substring(1);
        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;

            foreach (var item in Cart)
            {
                if (item.Product.IsTaxable)
                {
                    subTotal += Math.Round(item.SubTotal / (1 + _apiAppSetting.TaxRate),2);
                }
                else
                {
                    subTotal += item.SubTotal;
                }
            }

            return subTotal;
        }

        public string Tax => "P" + CalculateTax().ToString("C").Substring(1);

        public string Deductions => "P" + CalculateDeduction.ToString("C").Substring(1);

        public string TotalDeductionOutright => "P" + CalculateDeductionOutright().ToString("C").Substring(1);

        private decimal CalculateDeductionOutright()
        {

            decimal output = 0;
            foreach (var item in Cart)
            {
                output += item.FixPrice > 0 ? 0 : item.Product.DeductionOutright * (decimal)item.CartQuantity;
            }

            return output;
            //var output = Cart.Sum(item => item.FixPrice > 0 ? 0 :  item.Product.DeductionOutright * (decimal) item.CartQuantity);
            //return output;
        }

        private decimal CalculateDeductionCashDiscount()
        {
            return Cart.Sum(item => item.CashDiscount * (decimal)item.CartQuantity);
        }

        private bool _isPickup;
        public bool IsPickup
        {
            get { return _isPickup; }
            set
            {
                _isPickup = value;
                NotifyOfPropertyChange(() => IsPickup);
                NotifyOfPropertyChange(() => PickupDiscount);
                NotifyOfPropertyChange(() => Total);
            }
        }

        public string PickupDiscount => "P" + CalculatePickupDiscount().ToString("C").Substring(1);

        private decimal CalculatePickupDiscount()
        {
            var _pricePerKilo = (decimal)0;
            decimal.TryParse(_apiAppSetting.PricePerKilo, out _pricePerKilo);
            if (IsPickup)
                return (decimal)TotalQuantityUOMComputed * _pricePerKilo;
            return 0;
        }


        public string TotalDeductionPromoDiscount => "P" + CalculateDeductionPromoDiscount().ToString("C").Substring(1);

        private decimal CalculateDeductionPromoDiscount()
        {
            decimal output = 0;
            foreach (var item in Cart)
            {
                output += item.FixPrice > 0 ? 0 : item.Product.DeductionPromoDiscount * (decimal)item.CartQuantity;
            }
            
            return output;

        }

        public decimal CalculateDeduction =>
            CalculateDeductionOutright() + CashDiscount +
            CalculateDeductionPromoDiscount();

        private decimal CalculateTax()
        {

            decimal tax = 0;
            foreach (var item in Cart)
            {
                if (item.Product.IsTaxable)
                {
                    var vatSales =  Math.Round(item.SubTotal / (1 + _apiAppSetting.TaxRate), 2);
                    tax += item.SubTotal - vatSales;
                }
            }

            return tax;
        }

        public string Total
        {
            get
            {
                decimal otherDeductionDecimal;
                // Parse a floating-point value with a thousands separator. 
                Decimal.TryParse(OtherDeduction?.Replace("P",""), out otherDeductionDecimal);
                
                var total = CalculateSubTotal() + CalculateTax() - CalculateDeduction - otherDeductionDecimal - CalculatePickupDiscount();

                if (total < 0)
                {
                    return "-P" + total.ToString("C").Substring(2);
                }
                else
                {
                    return "P" + total.ToString("C").Substring(1);
                }
                
            }
        }

        public decimal TotalQuantityUOMComputed
        {
            get
            {
                var total = CalculateTotalQuantity();
                return total;
            }
        }

        private decimal CalculateTotalQuantity()
        {
            decimal qty = 0;
            if (Cart != null)
            {
                if (Cart.Count > 0)
                {
                    foreach (var product in Cart)
                    {
                        var unit = product.Product.KiloPerUnit * (decimal) product.CartQuantity / 50;
                        qty += unit;
                    }
                }
            }
            return qty;
        }

        public decimal TotalQuantity => (decimal) Cart.Sum(item => item.CartQuantity);
        


        //public string TotalQuantity => $"Total {CalculateTotalQty().ToString("C").Substring(1)}";

        //private decimal CalculateTotalQty()
        //{
        //    //25KG
        //    //Unit(s)
        //    decimal qty = 0;
        //    if (Products != null)
        //    {
        //        if (Products.Count > 0)
        //        {
        //            foreach (var product in Products)
        //            {
        //                if (product.ProductUnitOfMeasure.ToLower().Equals("unit(s)"))
        //                {
        //                    qty += (decimal)product.OrderQuantity;
        //                }
        //                else
        //                {
        //                    var unit = Convert.ToDecimal(product.ProductUnitOfMeasure.ToLower().Replace("kg", "")) / 50;
        //                    qty += unit;
        //                }
        //            }
        //        }
        //    }
        //    return qty;
        //}

        public bool CanAddToCart
        {
            get
            {
                var output = ItemQuantity > 0 && SelectedProduct?.StockQuantity >= ItemQuantity && OrderStatusId != 3;
                // Make sure something is selected
                // Make sure there is an item quantity
                return output;

            }
        }


        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }

        public void AddToCart()
        {

            var existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct &&
                                                        x.Supplier == SelectedSupplier.Replace("System.Windows.Controls.ComboBoxItem: ","")    );
            if (existingItem != null)
            {
                existingItem.CartQuantity += ItemQuantity;
            }
            else
            {
                var item = new CartItemDisplayModel
                {
                    Product = SelectedProduct,
                    CartQuantity = ItemQuantity,
                    Supplier = SelectedSupplier.Replace("System.Windows.Controls.ComboBoxItem: ", "")
                };
                Cart.Add(item);
            }

            SelectedProduct.StockQuantity -= ItemQuantity;
            ItemQuantity = 1;

            CashDiscount = CalculateDeductionCashDiscount();

            NotifyOfPropertyChange(() => Deductions);
            NotifyOfPropertyChange(() => PickupDiscount);
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            
            NotifyOfPropertyChange(() => TotalQuantity);
            NotifyOfPropertyChange(() => TotalQuantityUOMComputed);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => StockQuantity);
            NotifyOfPropertyChange(() => CanValidateSale);

        }

        public bool CanRemoveFromCart
        {
            get
            {
                var output = SelectedCartItem != null && SelectedCartItem?.CartQuantity > 0 && OrderStatusId != 3;
                // Make sure something is selected
                return output;
            }
        }

        public void OnEditDetail()
        {
            if (SelectedCartItem == null) return;
            var cartItem = new CartItemWindow(SelectedCartItem.Description, SelectedCartItem.CartQuantity);
            var dialogResult = cartItem.ShowDialog();
            
            if (Convert.ToBoolean(dialogResult))
            {
                var value = cartItem.QuantityUpdated;

                if (value > 0)
                {
                    SelectedCartItem.Product.StockQuantity = value;
                    var existingItem = Cart.FirstOrDefault(x => x.Product.Id == SelectedCartItem.Product.Id);
                    if (existingItem != null)
                    {
                        existingItem.CartQuantity = value;
                    }

                    OnUpdateCartItem();
                }
            }
        }

        public void RemoveFromCart()
        {
            if (MessageBox.Show("Are you sure?", "Remove Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SelectedCartItem.Product.StockQuantity += SelectedCartItem.CartQuantity;
                Cart.Remove(SelectedCartItem);
                OnUpdateCartItem();
            }
        }

        private void OnUpdateCartItem()
        {
            NotifyOfPropertyChange(() => PickupDiscount);
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => TotalQuantity);
            NotifyOfPropertyChange(() => TotalQuantityUOMComputed);
            NotifyOfPropertyChange(() => Deductions);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);
            NotifyOfPropertyChange(() => StockQuantity);
            NotifyOfPropertyChange(() => CanValidateSale);
        }

        public bool CanCheckOut
        {
            get
            {
                var output = Cart.Count > 0
                              && SelectedPartner != null
                              && string.IsNullOrWhiteSpace(PONo) == false
                              && SelectedPaymentType != null
                              && OrderStatusId == 1
                              && PriceListId > 0;
                // Make sure there is something in the cart
                return output;

            }
        }

        
        public async Task CheckOut()
        {
            if (MessageBox.Show("Are you sure?", "Submit Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {

                    OrderStatusId = 1;
                    decimal lessPriceValue;
                    Decimal.TryParse(OtherDeduction?.Replace("P",""), out lessPriceValue);
                    long _id = 0 ;

                    if(_saleData.Value!=null)
                        _id = _saleData.Value.Id;

                    _saleData.Value = new SaleHeader
                    {
                        Id = _id,
                        SONo = SONo,
                        PONo = PONo,
                        InvoiceNo = InvoiceNo,
                        OrderDate = Convert.ToDateTime(InvoiceDate, CultureInfo.InvariantCulture),
                        DeliveryDate = Convert.ToDateTime(DeliveryDate, CultureInfo.InvariantCulture),
                        DueDate = Convert.ToDateTime(DueDate, CultureInfo.InvariantCulture),

                        OrderStatusId = Convert.ToInt64(OrderStatusId),
                        TotalOrderQuantity = (float)TotalQuantity,
                        TotalOrderQuantityUOMComputed = (float)TotalQuantityUOMComputed,

                        TotalProductTaxPrice = Convert.ToDecimal(Tax.Substring(1)),
                        TotalDeductionPrice = Convert.ToDecimal(Deductions.Substring(1)),

                        PickUpDiscount = CalculatePickupDiscount(),

                        Outright = CalculateDeductionOutright(),
                        CashDiscount = CashDiscount,
                        PromoDiscount = CalculateDeductionPromoDiscount(),
                        LessPrice = lessPriceValue,

                        TotalPrice = Convert.ToDecimal(Total.Substring(1)),

                        TotalProductSalePrice = CalculateSubTotal(),

                        CustomerId = Convert.ToInt64(SelectedPartner.Id),
                        SelectedPaymentTypeId = SelectedPaymentType.Id,
                        PriceListId = SelectedPartner.PriceListId,
                        UserName = UserName
                    };

                    _saleData.Value.Total =
                        _saleData.Value.TotalProductSalePrice + _saleData.Value.TotalProductTaxPrice;


                    var lineNo = 0;
                    foreach (var item in Cart)
                    {
                        lineNo++;

                        if (_saleData.Value.IsVatable == false)
                        {
                            _saleData.Value.IsVatable = item.Product.IsTaxable;
                        }

                        var unitPrice = item.Product.DeductionFixPrice > 0 ? item.Product.DeductionFixPrice : item.Product.SalePrice;
                        var taxRate = item.Product.IsTaxable ? _apiAppSetting.TaxRate : 0;

                        item.Product.DeductionCashDiscount = unitPrice * (decimal)item.Product.Discount;

                        var subTotalProductSalePrice = unitPrice * (decimal)item.CartQuantity;

                        var subTotalProductTaxPrice = subTotalProductSalePrice - Math.Round(subTotalProductSalePrice /
                                                                             (1 + taxRate), 2) ;

                        _saleData.Value.SaleDetails.Add(new SaleDetail
                        {
                            LineNo = lineNo,
                            ProductId = item.Product.Id,
                            OrderQuantity = item.CartQuantity,
                            TaxRate = taxRate,
                            Discount = item.Product.Discount,
                            DeductionCashDiscount = item.Product.DeductionCashDiscount,
                            DeductionFixPrice = item.Product.DeductionFixPrice,
                            DeductionOutright = item.Product.DeductionOutright,
                            DeductionPromoDiscount = item.Product.DeductionPromoDiscount,
                            SubTotalProductSalePrice = subTotalProductSalePrice,
                            SubTotalProductTaxPrice = subTotalProductTaxPrice,
                            Supplier = item.Supplier,
                            SupplierId = GetSupplierId(item.Supplier)
                        });
                    }

                    _saleData.Value.Deductions = SelectedDeductionList;

                    var result = await _saleEndpoint.PostSale(_saleData.Value);
                    SalesId = result.Id.ToString();
                    _saleData.Value.Id = result.Id;

                    MessageBox.Show("Sales is submitted!", "System Confirmation", MessageBoxButton.OK);

                    //clean up
                    _isSalesCommitted = true;
                    SelectedProduct = null;

                }
                catch (Exception ex)
                {
                    ErrorMessage = $"CheckOut: {ex.Message}";
                }
            }

        }

        private int GetSupplierId(string itemSupplier)
        {
            switch (itemSupplier)
            {
                case "Coron":
                    return 0;
                case "Lubang":
                    return 1;
            }
            return 2;
        }

        private decimal _cashDiscount;
        public decimal CashDiscount
        {
            get { return _cashDiscount; }
            set
            {
                if(_cashDiscount != value)
                {
                    _cashDiscount = value;
                    NotifyOfPropertyChange(() => CashDiscount);
                    NotifyOfPropertyChange(() => Deductions);
                    NotifyOfPropertyChange(() => Total);
                }
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        public bool CanValidateSale
        {
            get
            {
                var output = string.IsNullOrWhiteSpace(SalesId) == false
                              && (OrderStatusId == 1 );
                return output;
            }
        }

        public async Task ValidateSale()
        {
            if (MessageBox.Show("Are you sure?", "Submit Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (OrderStatusId == 2)
                {

                    decimal lessPriceValue;
                    Decimal.TryParse(OtherDeduction?.Replace("P", ""), out lessPriceValue);

                    _saleData.Value.DeliveryDate = Convert.ToDateTime(DeliveryDate, CultureInfo.InvariantCulture);
                    _saleData.Value.TotalOrderQuantity = (float)TotalQuantity;
                    _saleData.Value.TotalOrderQuantityUOMComputed = (float)TotalQuantityUOMComputed;
                    _saleData.Value.TotalProductSalePrice = Convert.ToDecimal(Total.Substring(1));
                    _saleData.Value.TotalProductTaxPrice = Convert.ToDecimal(Tax.Substring(1));

                    _saleData.Value.PickUpDiscount = CalculatePickupDiscount();

                    _saleData.Value.Outright = CalculateDeductionOutright();
                    _saleData.Value.CashDiscount = CashDiscount;
                    _saleData.Value.PromoDiscount = CalculateDeductionPromoDiscount();


                    _saleData.Value.LessPrice = lessPriceValue;
                    _saleData.Value.TotalPrice = Convert.ToDecimal(Total.Substring(1));
                    _saleData.Value.Total = CalculateSubTotal();

                    _saleData.Value.SelectedPaymentTypeId = SelectedPaymentType.Id;

                    var lineNo = 0;
                    _saleData.Value.SaleDetails = new List<SaleDetail>();
                    
                    foreach (var item in Cart)
                    {
                        lineNo++;
                        if (_saleData.Value.IsVatable == false)
                        {
                            _saleData.Value.IsVatable = item.Product.IsTaxable;
                        }

                        var unitPrice = item.Product.DeductionFixPrice > 0 ? item.Product.DeductionFixPrice : item.Product.SalePrice;
                        var taxRate = item.Product.IsTaxable ? _apiAppSetting.TaxRate : 0;

                        item.Product.DeductionCashDiscount = unitPrice * (decimal)item.Product.Discount;

                        var subTotalProductSalePrice = unitPrice * (decimal)item.CartQuantity;

                        var subTotalProductTaxPrice = subTotalProductSalePrice - Math.Round(subTotalProductSalePrice /
                                                                                            (1 + taxRate), 2);

                        _saleData.Value.SaleDetails.Add(new SaleDetail
                        {
                            LineNo = lineNo,
                            ProductId = item.Product.Id,
                            OrderQuantity = item.CartQuantity,
                            TaxRate = taxRate,
                            Discount = item.Product.Discount,
                            DeductionCashDiscount = item.Product.DeductionCashDiscount,
                            DeductionFixPrice = item.Product.DeductionFixPrice,
                            DeductionOutright = item.Product.DeductionOutright,
                            DeductionPromoDiscount = item.Product.DeductionPromoDiscount,
                            SubTotalProductSalePrice = subTotalProductSalePrice,
                            SubTotalProductTaxPrice = subTotalProductTaxPrice
                        });
                    }

                }

                _saleData.Value.PriceListId = PriceListId;

                await ProcessValidateSales();
                //clean up
                _isSalesCommitted = true;
                SelectedProduct = null;

                //if (!Role.ToLower().Equals("admin"))
                //{
                //    IsOverrideHidden = true;
                //    IsGridVisible = false;
                //}
                //else
                //{
                   
                //}
            }
        }

        private async Task ProcessValidateSales()
        {
            _saleData.Value.Revalidate = true;
            _saleData.Value.OrderStatusId = 2;
            if (IsPickup)
                _saleData.Value.OrderStatusId = (long)OrderStatusEnum.DELIVERED;
            _saleData.Value.OverrideUser = UserName;
            var id = await _saleEndpoint.PostSale(_saleData.Value);

            MessageBox.Show("Sales is validated!", "System Confirmation", MessageBoxButton.OK);
            OrderStatusId = (int)_saleData.Value.OrderStatusId;

        }


        public bool CanNewSO => true;

        public async Task NewSO()
        {
            if (MessageBox.Show("Are you sure you want to create New Sales Order?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await ResetSalesViewModel();
                await OnShowSalesDetail(null);
            }
        }

        public async Task CancelSO()
        {
            if (MessageBox.Show("Are you sure you want to Cancel this Sales Order?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _saleData.Value.OrderStatusId = 3;
                _saleData.Value.OverrideUser = _loggedInUser.User.UserName;

                await _saleEndpoint.PostSale(_saleData.Value);

                MessageBox.Show("Sales Order is cancelled!", "System Confirmation", MessageBoxButton.YesNo);

                OrderStatusId = (int)_saleData.Value.OrderStatusId;
                _isSalesCommitted = true;
                Show("0");

            }

        }
        public bool PartnersControlEnabled => string.IsNullOrEmpty(SalesId);

        public bool SupplierEnabled => string.IsNullOrEmpty(SalesId);

        public bool PaymentsEnabled => IsCancelVisible || string.IsNullOrEmpty(SalesId);

        public bool DeliveryDateEnabled => IsCancelVisible || string.IsNullOrEmpty(SalesId);

        public bool ProductsControlEnabled
        {
            get
            {
                if ((IsCancelVisible || string.IsNullOrEmpty(SalesId)) == false)
                {
                    return false;
                }
                else
                {
                    return SelectedPartner != null;
                }

            }
        }

        public bool ItemQuantityEnabled
        {
            get
            {
                if ((IsCancelVisible || string.IsNullOrEmpty(SalesId)) == false)
                {
                    return false;
                }
                else
                {
                    return SelectedPartner != null;
                }

            }
        }



        public bool PONoEnabled => string.IsNullOrEmpty(SalesId);

        private async Task ResetSalesViewModel()
        {
            Cart = new BindingList<CartItemDisplayModel>();
            DeductionsList = new BindingList<DeductionDisplayModel>();
            _usedDeductions = new List<Deduction>();
            IsDeductionControlsVisible = true; 

            SelectedPartner = null;
            SelectedProduct = null;
            SelectedPaymentType = null;
            _saleData.Value = null;
            OtherDeduction = "0.00";
            SelectedSupplier = null;

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => TotalQuantity);
            NotifyOfPropertyChange(() => CanCheckOut);

            await LoadSOInvoiceNo();
            NotifyOfPropertyChange(() => SONo);
            NotifyOfPropertyChange(() => InvoiceNo);
            NotifyOfPropertyChange(() => InvoiceDate);

            DeliveryDate = string.Empty;
            DueDate = string.Empty;
            PONo = string.Empty;
            SalesId = string.Empty;

            OrderStatusId = 1;
            IsSupplier = true;
            IsSupplierDisplay = false;

        }


        public void GotFocusMethod(object source)
        {
            var based = source as TextBox;
            based.SelectAll();
        }

        public void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs eve)
        {
            TextBox tb = (sender as TextBox);

            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    eve.Handled = true;

                    tb.Focus();
                }
            }
        }

        private IHandle<string> _handleImplementation;

        public string PriceListLabel => $"Price List - [{PriceListId}] {PriceList}";

        public async Task ShowPriceList()
        {
            _isPriceListGridVisible = !_isPriceListGridVisible;
            NotifyOfPropertyChange(() => IsSalesOrderGridVisible);
            NotifyOfPropertyChange(() => IsPriceListGridVisible);
            NotifyOfPropertyChange(() => IsPriceListBorderVisible);
            if (_isPriceListGridVisible)
            {
                IsLoadingPriceListVisible = true;
                NotifyOfPropertyChange(() => PriceListLabel);
                await OnShowPriceListDetail();
            }
        }

        public bool CanShowPriceList
        {
            get
            {

                if (SelectedPartner == null)
                {
                    return false;
                }
                else
                {
                    return !string.IsNullOrEmpty(PriceList);
                }

            }
        }

        #endregion

        #region PriceList

        private bool _isLoadingPriceListVisible;

        public bool IsLoadingPriceListVisible
        {
            get { return _isLoadingPriceListVisible; }
            set
            {
                _isLoadingPriceListVisible = value;
                NotifyOfPropertyChange(() => IsLoadingPriceListVisible);
                NotifyOfPropertyChange(() => IsContainerPriceListVisible);
            }
        }

        public bool IsContainerPriceListVisible => !IsLoadingPriceListVisible;



        public async Task OnShowPriceListDetail()
        {
            await LoadPricelistProducts();
            await LoadPriceListCustomers();
            await LoadTargetCustomers();

            PricelistName = PriceList;
            var discountPolicy =  await GetPricelistDiscountPolicy();
            DiscountPolicy1 = !discountPolicy;
            if (!DiscountPolicy1)
                DiscountPolicy2 = true;
            NotifyOfPropertyChange(() => CanSavePriceList);
        }

        private async Task<bool> GetPricelistDiscountPolicy()
        {
            var value = false;
            try
            {
                PriceList getPriceListRecord = await _priceListEndpoint.GetRecord(PriceListId);
                value = getPriceListRecord.DiscountPolicy;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return value;
        }
        private async Task LoadPricelistProducts()
        {
            try
            {
                var getProducts = await _productEndpoint.GetList(PriceListId);
                var productsMap = _mapper.Map<List<PricelistProducts>>(getProducts);
                PricelistProducts = new BindingList<PricelistProducts>(productsMap);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            IsLoadingPriceListVisible = false;
        }
        private async Task LoadPriceListCustomers()
        {
            try
            {
                var getCustomers = await _customerEndpoint.GetList();
                var customersMap = _mapper.Map<List<CustomerDisplayModel>>(getCustomers);
                SourceCustomers = new BindingList<CustomerDisplayModel>(customersMap);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private List<PricelistDisplayModel> _allPricelists = new List<PricelistDisplayModel>();
        private List<ProductDisplayModel> _allProducts = new List<ProductDisplayModel>();

        private BindingList<PricelistDisplayModel> _pricelists;
        public BindingList<PricelistDisplayModel> Pricelists
        {
            get { return _pricelists; }
            set
            {
                _pricelists = value;
                NotifyOfPropertyChange(() => Pricelists);
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }


        private CustomerDisplayModel _selectedSourceCustomer;
        public CustomerDisplayModel SelectedSourceCustomer
        {
            get { return _selectedSourceCustomer; }
            set
            {
                _selectedSourceCustomer = value;
                NotifyOfPropertyChange(() => SelectedSourceCustomer);
                NotifyOfPropertyChange(() => CanAddCustomer);
            }
        }

        public bool CanAddCustomer => SelectedSourceCustomer != null;

        public void AddCustomer()
        {
            TargetCustomers.Add(SelectedSourceCustomer);
            SourceCustomers.Remove(SelectedSourceCustomer);
            SelectedSourceCustomer = null;
            NotifyOfPropertyChange(() => SubscribedCustomers);
            NotifyOfPropertyChange(() => SourcedCustomers);
        }
        public bool CanRemoveCustomer => SelectedTargetCustomer != null;

        public void RemoveCustomer()
        {
            SourceCustomers.Add(SelectedTargetCustomer);
            TargetCustomers.Remove(SelectedTargetCustomer);
            SelectedTargetCustomer = null;
            NotifyOfPropertyChange(() => SubscribedCustomers);
            NotifyOfPropertyChange(() => SourcedCustomers);
        }

        private CustomerDisplayModel _selectedTargetCustomer;
        public CustomerDisplayModel SelectedTargetCustomer
        {
            get { return _selectedTargetCustomer; }
            set
            {
                _selectedTargetCustomer = value;
                NotifyOfPropertyChange(() => SelectedTargetCustomer);
                NotifyOfPropertyChange(() => CanRemoveCustomer);

            }
        }


        //======Products
        private BindingList<PricelistProducts> _pricelistProducts;
        public BindingList<PricelistProducts> PricelistProducts
        {
            get { return _pricelistProducts; }
            set
            {
                _pricelistProducts = value;
                NotifyOfPropertyChange(() => PricelistProducts);
            }
        }

        private ProductDisplayModel _selectedPricelistProduct;
        public ProductDisplayModel SelectedPricelistProduct
        {
            get { return _selectedPricelistProduct; }
            set
            {
                _selectedPricelistProduct = value;
                NotifyOfPropertyChange(() => SelectedPricelistProduct);
                if (SelectedPricelistProduct == null)
                    return;
                DeductionFixPrice = SelectedPricelistProduct.DeductionFixPrice;
                DeductionOutright = SelectedPricelistProduct.DeductionOutright;
                Discount = SelectedPricelistProduct.Discount;
                DeductionPromoDiscount = SelectedPricelistProduct.DeductionPromoDiscount;
            }
        }

        private decimal _deductionFixPrice;
        public decimal DeductionFixPrice
        {
            get { return _deductionFixPrice; }
            set
            {
                _deductionFixPrice = value;
                NotifyOfPropertyChange(() => DeductionFixPrice);
            }
        }

        private decimal _deductionOutright;
        public decimal DeductionOutright
        {
            get { return _deductionOutright; }
            set
            {
                _deductionOutright = value;
                NotifyOfPropertyChange(() => DeductionOutright);
            }
        }

        private double _discount;
        public double Discount
        {
            get { return _discount; }
            set
            {
                _discount = value;
                NotifyOfPropertyChange(() => Discount);
            }
        }
        private decimal _deductionPromoDiscount;
        public decimal DeductionPromoDiscount
        {
            get { return _deductionPromoDiscount; }
            set
            {
                _deductionPromoDiscount = value;
                NotifyOfPropertyChange(() => DeductionPromoDiscount);
            }
        }

        private string _pricelistName;
        public string PricelistName
        {
            get { return _pricelistName; }
            set
            {
                _pricelistName = value;
                NotifyOfPropertyChange(() => PricelistName);
                NotifyOfPropertyChange(() => CanSavePriceList);
            }
        }

        private bool _discountPolicy1 = true;
        public bool DiscountPolicy1
        {
            get { return _discountPolicy1; }
            set
            {
                _discountPolicy1 = value;
                NotifyOfPropertyChange(() => DiscountPolicy1);
            }
        }

        private bool _discountPolicy2;
        public bool DiscountPolicy2
        {
            get { return _discountPolicy2; }
            set
            {
                _discountPolicy2 = value;
                NotifyOfPropertyChange(() => DiscountPolicy2);
            }
        }

        private BindingList<CustomerDisplayModel> _sourceCustomers = new BindingList<CustomerDisplayModel>();
        public BindingList<CustomerDisplayModel> SourceCustomers
        {
            get { return _sourceCustomers; }
            set
            {
                _sourceCustomers = value;
                NotifyOfPropertyChange(() => SourceCustomers);
                NotifyOfPropertyChange(() => SourcedCustomers);
            }
        }

        public string SourcedCustomers => SourceCustomers == null 
            ? "Select from Partners below" 
            : $"Select from {SourceCustomers.Count} Partner"+ (SourceCustomers.Count > 1 ? "s" : "") + " below";

        private BindingList<CustomerDisplayModel> _targetCustomers = new BindingList<CustomerDisplayModel>();
        public BindingList<CustomerDisplayModel> TargetCustomers
        {
            get { return _targetCustomers; }
            set
            {
                _targetCustomers = value;
                NotifyOfPropertyChange(() => TargetCustomers);
                NotifyOfPropertyChange(() => SubscribedCustomers);
            }
        }
        public string SubscribedCustomers => TargetCustomers == null ? "This Partners subscribed to this pricelist" : $"{TargetCustomers.Count} Partner" + (TargetCustomers.Count > 1 ? "s" : "") + " subscribed to this pricelist";
        public bool CanSavePriceList => !string.IsNullOrEmpty(PricelistName);

        public async Task SavePriceList()
        {

            if (MessageBox.Show("Are you sure?", "Save PriceList Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var value = new PriceList
                {
                    Name = PricelistName,
                    DiscountPolicy = !DiscountPolicy1,
                    Subscribed = TargetCustomers.Count
                };

                //edited
                try
                {
                    value.Id = PriceListId;
                    await _priceListEndpoint.Update(value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                if (TargetCustomers != null)
                {
                    var priceListCustomer = new PriceListCustomer { PriceListId = value.Id };
                    foreach (var customer in TargetCustomers)
                    {
                        priceListCustomer.CustomerIds.Add(customer.Id);
                    }

                    await _priceListEndpoint.SavePriceListCustomers(priceListCustomer);
                }

                //logic to save the deductions here
                try
                {
                    var productDeductionsMap = _mapper.Map<List<ProductDeduction>>(PricelistProducts);
                    var productDeductions = new List<ProductDeduction>(productDeductionsMap);
                    var priceListProduct = new PriceListProduct
                    {
                        PriceListId = value.Id,
                        ProductDeductions = productDeductions
                    };

                    await _customerEndpoint.SaveCustomerPricelist(priceListProduct);

                    //reset all after saved
                    PricelistProducts = new BindingList<PricelistProducts>();
                    SourceCustomers = new BindingList<CustomerDisplayModel>();
                    TargetCustomers = new BindingList<CustomerDisplayModel>();

                    PricelistName = string.Empty;


                    await LoadProducts();
                    NotifyOfPropertyChange(() => Products);

                    NotifyOfPropertyChange(() => CanSavePriceList);

                    MessageBox.Show("Successfully saved?", $"PriceList ({PriceList})", MessageBoxButton.OK);

                    await ShowPriceList();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    
                }

            }


        }

        

        private List<TargetCustomer> _targetCustomersId = new List<TargetCustomer>();
        private async Task LoadTargetCustomers()
        {
            try
            {
                _targetCustomersId = await _priceListEndpoint.GetTargetCustomers(PriceListId);
                if (_targetCustomersId.Count > 0)
                {
                    TargetCustomers = new BindingList<CustomerDisplayModel>();

                    foreach (var customerId in _targetCustomersId)
                    {
                        var targetCustomer = SourceCustomers.FirstOrDefault(c => c.Id == customerId.CustomerId);
                        if (targetCustomer == null) continue;
                        TargetCustomers.Add(targetCustomer);
                        SourceCustomers.Remove(targetCustomer);
                    }

                    NotifyOfPropertyChange(() => SubscribedCustomers);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        #endregion

        #region Override 

        private string _userNameOverride;
        private string _passwordOverride;
        private string _errorMessageOverride;
        public string ErrorMessageOverride
        {
            get { return _errorMessageOverride; }
            set
            {
                _errorMessageOverride = value;
                NotifyOfPropertyChange(() => ErrorMessageOverride);
                NotifyOfPropertyChange(() => IsErrorOverrideVisible);
            }
        }
        public bool IsErrorOverrideVisible
        {
            get
            {
                var output = ErrorMessageOverride?.Length > 0;
                return output;
            }
        }
        public string UserNameOverride
        {
            get { return _userNameOverride; }
            set
            {
                _userNameOverride = value;
                NotifyOfPropertyChange(() => UserNameOverride);
                NotifyOfPropertyChange(() => CanSubmitOverride);
            }
        }

        public string UserPasswordOverride
        {
            get { return _passwordOverride; }
            set
            {
                _passwordOverride = value;
                NotifyOfPropertyChange(() => UserPasswordOverride);
                NotifyOfPropertyChange(() => CanSubmitOverride);
            }
        }
        public bool CanSubmitOverride
        {
            get
            {
                var output = UserNameOverride?.Length > 0 && UserPasswordOverride?.Length > 0;
                return output;
            }
        }

        public bool CanCancel => true;

        public void CancelOverride()
        {
            IsOverrideHidden = false;
            IsGridVisible = true;
        }

        public async Task SubmitOverride()
        {
            if (MessageBox.Show("Are you sure?", "Override Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

                try
                {
                    ErrorMessage = "";

                    await _saleData.Authenticate(UserNameOverride, UserPasswordOverride);

                    _saleData.Value.Revalidate = _saleData.Value.OrderStatusId == 2;
                    _saleData.Value.OrderStatusId = 2;
                    _saleData.Value.OverrideUser = UserNameOverride;

                    await _saleEndpoint.PostSale(_saleData.Value);

                    MessageBox.Show("Sales is validated!", "System Confirmation", MessageBoxButton.YesNo);

                    //clean up
                    _isSalesCommitted = true;
                    SelectedProduct = null;
                    OrderStatusId = (int)_saleData.Value.OrderStatusId;
                    CancelOverride();

                }
                catch (Exception ex)
                {
                    ErrorMessageOverride = ex.Message;
                }

            }


        }
        private bool _isOverrideHidden = true;

        public bool IsOverrideHidden
        {
            get { return _isOverrideHidden; }
            set
            {
                _isOverrideHidden = value; 
                NotifyOfPropertyChange(() => IsOverrideHidden);
            }
        }

        #endregion

        #region Deductions List

        private BindingList<DeductionDisplayModel> _deductionsList;
        public BindingList<DeductionDisplayModel> DeductionsList
        {
            get { return _deductionsList; }
            set
            {
                _deductionsList = value;
                NotifyOfPropertyChange(() => DeductionsList);
            }
        }
       
        public void CheckBoxClicked()
        {
            CalculateSelectedDeduction();
        }

        private void CalculateSelectedDeduction(bool isFromSelectAll = false)
        {
            var isCheckAll = false;
            var current = string.Empty;
            TotalDeductionAmount = 0;
            SelectedDeductionList = new List<long>();

            foreach (var a in DeductionsList)
            {

                if (isFromSelectAll)
                {
                    a.IsChecked = SelectAll;
                }

                if (a.IsChecked)
                {
                    TotalDeductionAmount += a.Amount;
                    SelectedDeductionList.Add(a.Id);
                }

                if (string.IsNullOrEmpty(current))
                {
                    current = a.IsChecked.ToString();
                    isCheckAll = a.IsChecked;
                }
                else
                {
                    isCheckAll = isCheckAll && a.IsChecked;
                }

            }

            OtherDeduction = "P" + TotalDeductionAmount.ToString("C").Substring(1);

            _selectAll = isCheckAll;
            NotifyOfPropertyChange(() => SelectAll);
        }

        private bool _selectAll;

        public bool SelectAll
        {
            get { return _selectAll; }
            set
            {
                _selectAll = value; 
                NotifyOfPropertyChange(() => SelectAll);
                _events.Publish("SelectAll Changed", action => {
                    Task.Factory.StartNew(OnSelectAllEvent());
                });

            }
        }

        private System.Action OnSelectAllEvent()
        {
            return async () => await Task.Run(() => CalculateSelectedDeduction(true));
        }

        private List<long> SelectedDeductionList = new List<long>();

        private decimal _totalDeductionAmount;
        public decimal TotalDeductionAmount
        {
            get { return _totalDeductionAmount; }
            set
            {
                _totalDeductionAmount = value; 
                NotifyOfPropertyChange(() => TotalDeductionAmount);
            }
        }


        private string _otherDeduction;
        public string OtherDeduction 
        {
            get
            {
              
                return _otherDeduction;
            }
            set
            {
                _otherDeduction = value;
                NotifyOfPropertyChange(() => OtherDeduction);
                NotifyOfPropertyChange(() => Total);
            }
        }

        private IDeductionEndpoint _deductionEndpoint;

        private List<Deduction> _allDeductions = new List<Deduction>();

        private async Task LoadDeductions()
        {
            _deductionEndpoint.SetParameters(0, SelectedPartner.Id);
            _allDeductions = await _deductionEndpoint.GetList();
            var deductions = _mapper.Map<List<DeductionDisplayModel>>(_allDeductions);
            DeductionsList = new BindingList<DeductionDisplayModel>(deductions);
        }

        public bool CanShowDeduction => SelectedPartner != null && (OrderStatusId == 1 || OrderStatusId == 0);
        public async Task ShowDeduction()
        {
            await LoadDeductions();
        }

        private bool _isDeductionControlsVisible = true;
        public bool IsDeductionControlsVisible
        {
            get { return _isDeductionControlsVisible; }
            set
            {
                _isDeductionControlsVisible = value; 
                NotifyOfPropertyChange(() => IsDeductionControlsVisible);
            }
        }

        private async Task SetDeductions()
        {
            IsDeductionControlsVisible = false;
            if (OrderStatusId == 2)
            {
                var deductions = _mapper.Map<List<DeductionDisplayModel>>(_usedDeductions);
                DeductionsList = new BindingList<DeductionDisplayModel>(deductions);

                foreach (var deductionDisplayModel in DeductionsList)
                {
                    deductionDisplayModel.IsChecked = true;
                }

            }
            else if (OrderStatusId != 3)
            {
                IsDeductionControlsVisible = true;
                await LoadDeductions();
                _usedDeductions.ForEach(item =>
                {
                    var _newDeduction = new Deduction
                    {
                        Id = item.Id,
                        Amount = item.Amount,
                        Particular = item.Particular,
                        PONo = item.PONo,
                        UsedAmount = item.UsedAmount,
                    };
                    _allDeductions.Add(_newDeduction);
                    var deduction = _mapper.Map<DeductionDisplayModel>(_newDeduction);
                    deduction.IsChecked = true;
                    DeductionsList.Add(deduction);
                });

                


            }

        }

        #endregion

    }
}
