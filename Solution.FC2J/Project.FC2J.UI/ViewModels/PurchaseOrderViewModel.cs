using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using Caliburn.Micro;
using Project.FC2J.Models.Purchase;
using Project.FC2J.Models.User;
using Project.FC2J.UI.Helpers;
using Project.FC2J.UI.Helpers.AppSetting;
using Project.FC2J.UI.Helpers.Purchase;
using Project.FC2J.UI.Models;
using Project.FC2J.UI.Providers;
using Project.FC2J.UI.UserControls;

namespace Project.FC2J.UI.ViewModels
{
    public class PurchaseOrderViewModel : Screen
    {

        private readonly IMapper _mapper;
        private readonly IPurchaseEndpoint _purchaseEndpoint;
        private readonly IEventAggregator _events;
        private readonly ILoggedInUser _loggedInUser;
        private IPriceListEndpoint _priceListEndpoint;
        private readonly IApiAppSetting _apiAppSetting;

        public PurchaseOrderViewModel(IPurchaseEndpoint purchaseEndpoint, IMapper mapper, IEventAggregator events, ILoggedInUser loggedInUser, IPriceListEndpoint priceListEndpoint, IApiAppSetting apiAppSetting)
        {
            _purchaseEndpoint = purchaseEndpoint;
            _mapper = mapper;
            _events = events;
            _loggedInUser = loggedInUser;
            _priceListEndpoint = priceListEndpoint;
            _apiAppSetting = apiAppSetting;
            IsLoadingVisible = true;
            IsDeliverMode = false;
            IsShowNormalGrid = true;
            IsDeliveredAll = false;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadPriceLists();
            OnReset();
            IsLoadingVisible = false;
        }

        private async Task LoadPriceLists()
        {
            var allPriceLists = await _priceListEndpoint.GetList(0);
            var pricelists = _mapper.Map<List<PricelistDisplayModel>>(allPriceLists);
            PriceLists = new BindingList<PricelistDisplayModel>(pricelists);
        }
        private async Task LoadProducts()
        {
            var productList = await _priceListEndpoint.GetPriceList(SelectedPriceList.Id);
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        private System.Action OnLoadProducts()
        {
            return async () =>
            {
                if (IsDeliverMode)
                    return;
                if (SelectedPriceList != null)
                    await LoadProducts();
                else
                {
                    Products = new BindingList<ProductDisplayModel>();
                }
            };
        }

        private void OnReset()
        {
            DeliveryDate = DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            PurchaseDate = DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            PONo = string.Empty;
            Cart = new BindingList<CartItemDisplayModel>();
            CartDeliver = new BindingList<CartItemDisplayModel>();
            SelectedProduct = new ProductDisplayModel();
            PaymentInvoices = new BindingList<POPaymentItemDisplayModel>();
            SelectedPaymentInvoice = new POPaymentItemDisplayModel();
            NotifyOfPropertyChange(() => TotalQuantity);
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => TaxPrice);
            NotifyOfPropertyChange(() => Total);
            IsDeliverMode = false;
            IsShowNormalGrid = true;
            IsAcknowledged = false;
            IsSubmitted = false;
            RetrieveLabel = "RETRIEVE";
            IsDeliveredAll = false;
            Id = 0;
        }

        public void Close()
        {
            TryClose();
        }

        public bool CanSubmit => Cart?.Count > 0 && string.IsNullOrWhiteSpace(PONo) == false && IsSubmitted == false;

        private PurchaseOrder _po = new PurchaseOrder();

        public async Task Submit()
        {
            if (MessageBox.Show("Are you sure?", "Submit Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SetPoPayload();

                var result =  await _purchaseEndpoint.Save(_po);

                MessageBox.Show("Purchase Order successfully saved.", "Confirmation", MessageBoxButton.OK);
                IsSubmitted = true;
                Id = result.PoHeader.Id;
            }
        }

        private void SetPoPayload()
        {
            _po.PoHeader = new PoHeader
            {
                PONo = PONo,
                PurchaseDate = Convert.ToDateTime(PurchaseDate, CultureInfo.InvariantCulture),
                DeliveryDate = Convert.ToDateTime(DeliveryDate, CultureInfo.InvariantCulture),
                PickUpDiscount = 0,
                Outright = 0,
                CashDiscount = 0,
                PromoDiscount = 0,
                OtherDeduction = 0,
                TotalQuantity =(double)TotalQuantity,
                TotalQuantityUOMComputed = 0,
                SubTotal = SubTotal,
                TaxPrice = TaxPrice,
                Total = Convert.ToDecimal(Total.Replace("P","")),
                UserName = _loggedInUser.User.UserName.ToLower(),
                PriceListId = Convert.ToInt64(SelectedPriceList?.Id),
                SupplierName = SelectedPriceList?.Name,
                SupplierEmail = SelectedPriceList?.Email,
            };

            var count = 0;
            var poDetails = new List<PoDetail>();
            foreach (var cartItemDisplayModel in Cart)
            {
                count += 1;
                var detail = new PoDetail
                {
                    LineNo = count,
                    ProductId = cartItemDisplayModel.Product.Id,
                    Quantity = cartItemDisplayModel.CartQuantity,
                    Name = cartItemDisplayModel.Product.Name,
                    Category = cartItemDisplayModel.Product.Category,
                    SalePrice = cartItemDisplayModel.Product.CostPrice,
                    UnitDiscount = cartItemDisplayModel.Product.UnitDiscount,
                    UnitOfMeasure = cartItemDisplayModel.Product.UnitOfMeasure,
                    SFAUnitOfMeasure = cartItemDisplayModel.Product.SFAUnitOfMeasure,
                    SFAReferenceNo = cartItemDisplayModel.Product.SFAReferenceNo,
                    SubTotal = cartItemDisplayModel.SubTotalCostPrice,
                    IsTaxable = cartItemDisplayModel.Product.IsTaxable,
                    NetWeight = cartItemDisplayModel.NetWeight,
                    TaxPrice = cartItemDisplayModel.PoTax,
                    TaxType = cartItemDisplayModel.TaxType
                };
                detail.TaxRate = detail.IsTaxable ? _apiAppSetting.PoTaxRate : 0;
                detail.TaxPrice = detail.IsTaxable ? cartItemDisplayModel.PoTax : 0;
                poDetails.Add(detail);

                if (_po.PoHeader.IsVatable == false)
                    _po.PoHeader.IsVatable = detail.IsTaxable;

            }

            _po.PoDetails = poDetails;
        }

        public bool CanAcknowledge => Id > 0 && IsAcknowledged == false;
        public async Task Acknowledge()
        {
            if (MessageBox.Show("Are you sure?", "Acknowledge Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _po = new PurchaseOrder()
                {
                    PoHeader = new PoHeader
                    {
                        Id = Id,
                        UserName = _loggedInUser.User.UserName.ToLower()
                    },
                    PoDetails = new List<PoDetail>()
                };

                await _purchaseEndpoint.Update(_po);

                MessageBox.Show("Purchase Order successfully acknowledged.", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                OnReset();
            }
        }

        private string _retrieveLabel = "RETRIEVE";
        public string RetrieveLabel
        {
            get { return _retrieveLabel; }
            set
            {
                _retrieveLabel  = value; 
                NotifyOfPropertyChange( () => RetrieveLabel);
            }
        }

        public bool CanRetrieve => string.IsNullOrWhiteSpace(PONo) == false;
        public async Task Retrieve()
        {
            if (RetrieveLabel.Equals("CLEAR"))
            {
                OnReset();
                PurchaseOrder = null;
                return;
            }

            await ReloadPODetails();
        }

        private async Task ReloadPODetails()
        {
            var pono = PONo;

            try
            {
                PurchaseOrder = await _purchaseEndpoint.GetPurchaseOrder(PONo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (PurchaseOrder == null)
            {
                MessageBox.Show($"Entered PO No ({PONo}) is not found.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            OnReset();
            PONo = pono;

            RetrieveLabel = "CLEAR";
            var pricelistItem = PriceLists.FirstOrDefault(x => x.Id == PurchaseOrder.PoHeader.PriceListId);
            SelectedPriceList = pricelistItem;

            var details = new BindingList<CartItemDisplayModel>();
            foreach (var detail in PurchaseOrder.PoDetails)
            {
                if (PurchaseOrder.PoHeader.POStatusId == 4 && detail.IsDelivered) continue;

                detail.IsDelivered = true;
                var item = new CartItemDisplayModel
                {
                    Product = new ProductDisplayModel
                    {
                        Id = detail.ProductId,
                        Name = detail.Name,
                        Category = detail.Category,
                        UnitOfMeasure = detail.UnitOfMeasure,
                        SalePrice = detail.SalePrice,
                        UnitDiscount = detail.UnitDiscount,
                        SFAUnitOfMeasure = detail.SFAUnitOfMeasure,
                        SFAReferenceNo = detail.SFAReferenceNo,
                        IsTaxable = detail.IsTaxable
                    },
                    InvoiceNo = detail.InvoiceNo,
                    CartQuantity = (float) detail.Quantity,
                    PoTaxRate = detail.IsTaxable ? detail.TaxRate : 0,
                    IsDelivered = detail.IsDelivered
                };
                details.Add(item);
            }

            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            Id = PurchaseOrder.PoHeader.Id;
            PurchaseDate = PurchaseOrder.PoHeader.PurchaseDate.ToString("d", culture);
            DeliveryDate = PurchaseOrder.PoHeader.DeliveryDate.ToString("d", culture);

            IsSubmitted = true;
            if (PurchaseOrder.PoHeader.POStatusId == 1)
            {
                Cart = details;
                IsShowNormalGrid = true;
                IsDeliverMode = false;
                IsAcknowledged = false;
            }
            else
            {
                IsDeliveredAll = PurchaseOrder.PoHeader.POStatusId == 3;
                IsAcknowledged = true;
                if (IsDeliveredAll)
                {
                    Cart = details;
                    IsShowNormalGrid = true;
                    IsDeliverMode = false;
                    await LoadPaymentInvoices();
                }
                else
                {
                    CartDeliver = details;
                    IsShowNormalGrid = false;
                    IsDeliverMode = true;
                }
            }

            NotifyOfPropertyChange(() => TotalQuantity);
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => TaxPrice);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanDeliver);
        }

        private async Task LoadPaymentInvoices()
        {
            var list = await _purchaseEndpoint.GetPayments(Id);
            var mapList = _mapper.Map<List<POPaymentItemDisplayModel>>(list);
            PaymentInvoices = new BindingList<POPaymentItemDisplayModel>(mapList);
        }

        public void CheckBoxClicked(CartItemDisplayModel item, int check)
        {
            if (PurchaseOrder == null) return;

            item.IsDelivered = check == 1;
                               
            foreach (var cartItemDisplayModel in CartDeliver)
            {
                if (cartItemDisplayModel.IsDelivered == false)
                {
                    PurchaseOrder.IsWithReturns = true;
                    break;
                }
            }

            NotifyOfPropertyChange(() => TotalQuantity);
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => TaxPrice);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanDeliver);

        }

        public bool CanDeliver => TotalQuantity > 0;
        public async Task Deliver()
        {
            if (MessageBox.Show("Are you sure?", "Mark As Deliver Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var isUpdate = PurchaseOrder.PoHeader.POStatusId == 4;
                if (isUpdate)
                {
                    PurchaseOrder.IsWithReturns = true;
                }

                var detailsList = new List<PoDetail>();
                foreach (var cart in CartDeliver)
                {
                    detailsList.Add(new PoDetail
                    {
                        Id = cart.Product.Id,
                        IsDelivered = cart.IsDelivered,
                        ProductId = cart.Product.Id,
                        Quantity = cart.CartQuantity
                    });
                }
                PurchaseOrder.PoDetails = detailsList;

                PurchaseOrder.PoHeader.TotalQuantity = isUpdate ? PurchaseOrder.PoHeader.TotalQuantity + (double)TotalQuantity : (double) TotalQuantity;
                PurchaseOrder.PoHeader.SubTotal = isUpdate ? PurchaseOrder.PoHeader.SubTotal + SubTotal :  SubTotal;
                PurchaseOrder.PoHeader.TaxPrice = isUpdate ? PurchaseOrder.PoHeader.TaxPrice + TaxPrice : TaxPrice;
                PurchaseOrder.PoHeader.Total = isUpdate ? PurchaseOrder.PoHeader.Total + Convert.ToDecimal(Total.Replace("P", "")) :  Convert.ToDecimal(Total.Replace("P", ""));

                await _purchaseEndpoint.Delivered(PurchaseOrder);
                MessageBox.Show("Purchase Order is successfully Delivered.", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                OnReset();
                SelectedPriceList = null;
            }
        }

        public async Task ListPurchases()
        {
            var inputDialog = new PoListView
            {
                CollectionData = await _purchaseEndpoint.GetPurchasesOrder(_loggedInUser.User.UserName.ToLower())
            };

            if (inputDialog.ShowDialog() != true) return;
            PONo = inputDialog.PONo;
            await ReloadPODetails();
        }

        public async Task Attach()
        {
            var inputDialog = new AttachmentWindow
            {
                OrderHeaderId = Id,
                UserName = _loggedInUser.User.UserName.ToLower(),
                Items = Cart
            };

            if (inputDialog.ShowDialog() != true) return;
            try
            {
                var poPayment = await _purchaseEndpoint.InsertPayment(inputDialog.POPayment);
                MessageBox.Show("PO Invoice successfully saved.", "Confirmation", MessageBoxButton.OK);
                await LoadPaymentInvoices();
                foreach (var item in inputDialog.POPayment.items)
                {
                    var cart = Cart.FirstOrDefault(i => i.Product.Id == item.Id);
                    if(cart!= null)
                        cart.InvoiceNo = inputDialog.POPayment.InvoiceNo;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public bool CanDeletePaymentInvoice => SelectedPaymentInvoice?.Id > 0;

        public async Task DeletePaymentInvoice()
        {
            if (MessageBox.Show("Are you sure?", $"Delete Payment Invoice ({SelectedPaymentInvoice.InvoiceNo}) Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _purchaseEndpoint.DeletePayment(SelectedPaymentInvoice.Id, _loggedInUser.User.UserName.ToLower());


                MessageBox.Show("Payment Invoice successfully deleted.", "Confirmation", MessageBoxButton.OK);
                await LoadPaymentInvoices();

                foreach (var cartItemDisplayModel in Cart)
                {
                    if (cartItemDisplayModel.InvoiceNo == SelectedPaymentInvoice.InvoiceNo)
                        cartItemDisplayModel.InvoiceNo = string.Empty;
                }

            }
        }



        private string _attachmentMessage;

        public string AttachmentMessage
        {
            get { return _attachmentMessage; }
            set
            {
                _attachmentMessage = value; 
                NotifyOfPropertyChange( () => AttachmentMessage);
            }
        }


        private bool _isDeliveredAll;
        public bool IsDeliveredAll
        {
            get { return _isDeliveredAll; }
            set
            {
                _isDeliveredAll = value; 
                NotifyOfPropertyChange(() => IsDeliveredAll);
                NotifyOfPropertyChange(() => IsShowNormalGrid);
            }
        }


        private PurchaseOrder _purchaseOrder;

        public PurchaseOrder PurchaseOrder
        {
            get { return _purchaseOrder; }
            set
            {
                _purchaseOrder = value; 
                NotifyOfPropertyChange(() => PurchaseOrder);
            }
        }

        private bool _isDeliver ;
        public bool IsDeliverMode
        {
            get { return _isDeliver; }
            set
            {
                _isDeliver = value; 
                NotifyOfPropertyChange(() => IsDeliverMode);
            }
        }
        //IsSubmitted
        private bool _isShowNormalGrid;
        public bool IsShowNormalGrid
        {
            get { return _isShowNormalGrid; }
            set
            {
                _isShowNormalGrid = value;
                NotifyOfPropertyChange(() => IsShowNormalGrid);
            }
        }
        private bool _isSubmitted;
        public bool IsSubmitted
        {
            get { return _isSubmitted; }
            set
            {
                _isSubmitted = value;
                NotifyOfPropertyChange(() => IsSubmitted);
                NotifyOfPropertyChange(() => CanRemove);
            }
        }
        private bool _isAcknowledged;
        public bool IsAcknowledged
        {
            get { return _isAcknowledged; }
            set
            {
                _isAcknowledged = value;
                NotifyOfPropertyChange(() => IsAcknowledged);
                NotifyOfPropertyChange(() => CanRemove);
                NotifyOfPropertyChange(() => CanAcknowledge);
            }
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

        private BindingList<PricelistDisplayModel> _pricelists;
        public BindingList<PricelistDisplayModel> PriceLists
        {
            get { return _pricelists; }
            set
            {
                _pricelists = value;
                NotifyOfPropertyChange(() => PriceLists);
            }
        }

        private PricelistDisplayModel _selectedPriceList;
        public PricelistDisplayModel SelectedPriceList
        {
            get { return _selectedPriceList; }
            set
            {
                _selectedPriceList = value;
                NotifyOfPropertyChange(() => SelectedPriceList);
                SelectedProduct = null;
                _events.Publish("SelectedPriceList Changed", action => {
                    Task.Factory.StartNew(OnLoadProducts());
                });

            }
        }
        private BindingList<ProductDisplayModel> _products;
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

        private ProductDisplayModel _selectedProduct;
        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => SalePrice);
                NotifyOfPropertyChange(() => UnitDiscount);
                NotifyOfPropertyChange(() => SFAReferenceNo);
                NotifyOfPropertyChange(() => CanAdd);
            }
        }

        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();
        public BindingList<CartItemDisplayModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private BindingList<POPaymentItemDisplayModel> _paymentInvoices = new BindingList<POPaymentItemDisplayModel>();
        public BindingList<POPaymentItemDisplayModel> PaymentInvoices
        {
            get { return _paymentInvoices; }
            set
            {
                _paymentInvoices = value;
                NotifyOfPropertyChange(() => PaymentInvoices);
            }
        }
        private POPaymentItemDisplayModel _selectedPaymentInvoice;
        public POPaymentItemDisplayModel SelectedPaymentInvoice
        {
            get { return _selectedPaymentInvoice; }
            set
            {
                _selectedPaymentInvoice = value;
                NotifyOfPropertyChange(() => SelectedPaymentInvoice);
                NotifyOfPropertyChange(() => CanDeletePaymentInvoice);
            }
        }

        private BindingList<CartItemDisplayModel> _cartDeliver = new BindingList<CartItemDisplayModel>();
        public BindingList<CartItemDisplayModel> CartDeliver
        {
            get { return _cartDeliver; }
            set
            {
                _cartDeliver = value;
                NotifyOfPropertyChange(() => CartDeliver);
            }
        }

        private CartItemDisplayModel _selectedCartItem;
        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemove);
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

        public decimal SalePrice => Convert.ToDecimal(SelectedProduct?.SalePrice);
        public decimal UnitDiscount => Convert.ToDecimal(SelectedProduct?.UnitDiscount);
        public string SFAReferenceNo => SelectedProduct?.SFAReferenceNo;

        private double _quantity=1;
        public double Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                NotifyOfPropertyChange(() => Quantity);
                NotifyOfPropertyChange(() => CanAdd);
            }
        }

        public bool CanAdd => SelectedProduct != null && Quantity > 0;
        public void Add()
        {
            var existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

            if (existingItem != null)
            {
                existingItem.CartQuantity += (float) Quantity;
            }
            else
            {
                var item = new CartItemDisplayModel
                {
                    Product = SelectedProduct,
                    CartQuantity = (float)Quantity,
                    PoTaxRate = SelectedProduct.IsTaxable ? _apiAppSetting.PoTaxRate : 0
                };
                Cart.Add(item);
            }

            
            Quantity = 1;
            NotifyOfPropertyChange(() => TotalQuantity);
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => TaxPrice);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanDeliver);
        }

        public bool CanRemove => SelectedCartItem != null && IsDeliverMode == false 
                                                          && IsAcknowledged == false
                                                          && IsSubmitted == false; 

        public void Remove()
        {
            if (MessageBox.Show("Are you sure?", "Remove Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Cart.Remove(SelectedCartItem);
                OnUpdateCartItem();
            }
        }

        private void OnUpdateCartItem()
        {
            NotifyOfPropertyChange(() => TotalQuantity);
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => TaxPrice);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanDeliver);
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

        public decimal TotalQuantity
        {
            get
            {
                if (IsDeliverMode == false)
                    return (decimal)Cart.Sum(item => item.CartQuantity);
                return (decimal) CartDeliver
                    .Where(item => item.IsDelivered)
                    .Sum(item => item.CartQuantity);
            }
        }
        public decimal SubTotal => CalculateSubTotal();
        public decimal TaxPrice => CalculateTaxPrice();

        private decimal CalculateSubTotal()
        {
            if (IsDeliverMode == false)
                return Cart.Sum(item => item.SubTotalCostPrice);
            return CartDeliver
                .Where(item => item.IsDelivered)
                .Sum(item => item.SubTotalCostPrice);

        }
        private decimal CalculateTaxPrice()
        {
            if (IsDeliverMode == false)
                return Cart.Sum(item => item.PoTax);
            return CartDeliver
                .Where(item => item.IsDelivered)
                .Sum(item => item.PoTax);
        }

        public string Total => "P" + CalculateTotal().ToString("C").Substring(1);

        private decimal CalculateTotal()
        {
            return CalculateSubTotal() + CalculateTaxPrice();
        }
        private string _deliveryDate;
        public string DeliveryDate
        {
            get { return _deliveryDate; }
            set
            {
                _deliveryDate = value;
                NotifyOfPropertyChange(() => DeliveryDate);
            }
        }
        private string _purchaseDate;
        public string PurchaseDate
        {
            get { return _purchaseDate; }
            set
            {
                _purchaseDate = value;
                NotifyOfPropertyChange(() => PurchaseDate);
            }
        }
        private string _poNo;
        public string PONo
        {
            get { return _poNo; }
            set
            {
                _poNo = value;
                NotifyOfPropertyChange(() => PONo);
                NotifyOfPropertyChange(() => CanSubmit);
                NotifyOfPropertyChange(() => CanRetrieve);
            }
        }

        private long _id;
        /// <summary>
        /// This refers to the created Id for this Purchase Order.
        /// If this has value, then the Acknowledge button is enabled
        /// </summary>
        public long Id
        {
            get { return _id; }
            set
            {
                _id = value; 
                NotifyOfPropertyChange(() => Id);
                NotifyOfPropertyChange(() => CanSubmit);
                NotifyOfPropertyChange(() => CanAcknowledge);
            }
        }



    }
}
