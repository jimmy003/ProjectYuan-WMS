﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using AutoMapper;
using Caliburn.Micro;
using Project.FC2J.Models.Enums;
using Project.FC2J.Models.Sale;
using Project.FC2J.Models.User;
using Project.FC2J.UI.Helpers;
using Project.FC2J.UI.Models;
using Project.FC2J.UI.Providers;

namespace Project.FC2J.UI.ViewModels
{
    public class ReceiverViewModel : Screen
    {

        private SaleHeader _saleHeader = new SaleHeader();
        private Invoice _selectedPONo;

        private readonly IMapper _mapper;
        private readonly ISaleEndpoint _saleEndpoint;
        private readonly IEventAggregator _events;
        private readonly ILoggedInUser _loggedInUser;
        private IEnumerable<Invoice> _invoices;

        public ReceiverViewModel(ISaleEndpoint saleEndpoint, ILoggedInUser loggedInUser, IMapper mapper, IEventAggregator events)
        {
            _saleEndpoint = saleEndpoint;
            _loggedInUser = loggedInUser;
            _mapper = mapper;
            _events = events;
        }


        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            IsLoadingVisible = true;
            await LoadInvoices();
            IsLoadingVisible = false;
        }

        private bool _isLoadingVisible; 
        public bool IsLoadingVisible
        {
            get => _isLoadingVisible;
            set
            {
                _isLoadingVisible = value;
                NotifyOfPropertyChange(() => IsLoadingVisible);
            }
        }

        private async Task LoadInvoices()
        {
            _invoices = await _saleEndpoint.GetInvoices();

            var allRecords = await _saleEndpoint.GetReceiverSalesOrders();
            ReceiverSalesOrders = new ObservableCollection<ReceiverSalesOrder>(allRecords);

            NotifyOfPropertyChange(() => PoNoProvider);
            NotifyOfPropertyChange(() => InvoiceCount);
        }

        public async Task Refresh()
        {
            await LoadInvoices();
        }

        public InvoiceSuggestionProvider PoNoProvider
        {
            get
            {
                var invoicesProvider = new InvoiceSuggestionProvider(_invoices);
                return invoicesProvider;
            }
        }

        private ObservableCollection<ReceiverSalesOrder> _receiverSalesOrders;
        public ObservableCollection<ReceiverSalesOrder> ReceiverSalesOrders
        {
            get => _receiverSalesOrders;
            set
            {
                _receiverSalesOrders = value;
                NotifyOfPropertyChange(() => ReceiverSalesOrders);
                NotifyOfPropertyChange(() => CollectionView);
            }
        }

        private CollectionView _collectionView = null;
        public CollectionView CollectionView
        {
            get
            {
                _collectionView = (CollectionView)CollectionViewSource.GetDefaultView(ReceiverSalesOrders);
                _collectionView?.SortDescriptions.Add(new SortDescription("Partner", ListSortDirection.Ascending));
                return _collectionView;
            }
        }

        private ReceiverSalesOrder _selectedSalesOrder;
        public ReceiverSalesOrder SelectedSalesOrder
        {
            get => _selectedSalesOrder;
            set
            {
                _selectedSalesOrder = value;
                NotifyOfPropertyChange(() => SelectedSalesOrder);
            }
        }

        public Invoice SelectedPoNo
        {
            get
            {
                return _selectedPONo;
            }
            set
            {
                _selectedPONo = value;

                if (SelectedPoNo != null)
                {
                    _events.Publish("SelectedPONo Changed", action =>
                    {
                        Task.Factory.StartNew(OnLoadSelectedInvoiceDetails());
                    });
                }

                NotifyOfPropertyChange(() => SelectedPoNo);
                NotifyOfPropertyChange(() => CanReceive);
                NotifyOfPropertyChange(() => IsOverlayVisible);

            }
        }
        public string TotalAmountDue => _saleHeader?.TotalPrice.ToString("C").Substring(1);

        public bool IsOverlayVisible
        {
            get
            {
                if (SelectedPoNo == null)
                {
                    OnClearAfterReceive();
                    return true;
                }
                return false;
            }
        }

        public string DeliveryDate
        {
            get
            {
                if (_saleHeader != null)

                {
                    if (_saleHeader.DeliveryDate.ToString("MMM-dd-yyyy").Equals("Jan-01-0001"))
                        return string.Empty;
                    return _saleHeader.DeliveryDate.ToString("MMM-dd-yyyy");
                }
                return string.Empty;
            }
        }

        public string DueDate => _saleHeader?.DueDate.ToString("MMM-dd-yyyy");
        public string Customer => _saleHeader?.CustomerName;
        public string Address => _saleHeader?.CustomerAddress2;
        public string PaymentTerms => _saleHeader?.SelectedPaymentType;
        public string SONo => $"{_saleHeader?.SONo} {_saleHeader?.InvoiceNo}";
        public string TotalOrderQuantity => _saleHeader?.TotalOrderQuantity.ToString("C").Substring(1);
        public string TotalOrderQuantityUOMComputed => $"Total {_saleHeader?.TotalOrderQuantityUOMComputed.ToString("C").Substring(1)}";
        public string VAT12 => _saleHeader?.TotalProductTaxPrice.ToString("C").Substring(1);
        public string PromoDiscount => _saleHeader?.PromoDiscount.ToString("C").Substring(1);
        public string PickupDiscount => _saleHeader?.PickUpDiscount.ToString("C").Substring(1);
        public string CashDiscount => _saleHeader?.CashDiscount.ToString("C").Substring(1);
        public string Outright => _saleHeader?.Outright.ToString("C").Substring(1);
        public string VATExemptSales => _saleHeader?.Total.ToString("C").Substring(1);

        private List<SaleDetail> _orderedProducts = new List<SaleDetail>();

        private System.Action OnLoadSelectedInvoiceDetails()
        {
            return async () =>
            {
                if (SelectedPoNo == null) return;
                _saleHeader = await _saleEndpoint.GetSaleHeader(SelectedPoNo.CustomerId, SelectedPoNo.Id);
                if (_saleHeader != null)
                {
                    //initial Order Status, that is 4=Delivered                     
                    _saleHeader.OrderStatusId = (long)OrderStatusEnum.DELIVERED;
                }

                //store the ordered items
                _orderedProducts = await _saleEndpoint.GetSaleDetails(SelectedPoNo.Id, SelectedPoNo.CustomerId);

                OnLoadOrderedProducts();
                OnClearReturnProducts();

                NotifyOfPropertyChange(() => DeliveryDate);
                NotifyOfPropertyChange(() => Customer);
                NotifyOfPropertyChange(() => DueDate);
                NotifyOfPropertyChange(() => Address);
                NotifyOfPropertyChange(() => PaymentTerms);
                NotifyOfPropertyChange(() => SONo);
                NotifyOfPropertyChange(() => TotalAmountDue);
                NotifyOfPropertyChange(() => VATExemptSales);
                NotifyOfPropertyChange(() => PromoDiscount);
                NotifyOfPropertyChange(() => PickupDiscount);
                NotifyOfPropertyChange(() => CashDiscount);
                NotifyOfPropertyChange(() => Outright);
                NotifyOfPropertyChange(() => VAT12);
                NotifyOfPropertyChange(() => TotalOrderQuantity);
                NotifyOfPropertyChange(() => TotalOrderQuantityUOMComputed);

            };
        }

        private void OnClearReturnProducts()
        {
            ReturnProducts = new BindingList<SaleDetailDisplayModel>();
        }

        private void OnLoadOrderedProducts()
        {
            var sales = _mapper.Map<List<SaleDetailDisplayModel>>(_orderedProducts);
            Products = new BindingList<SaleDetailDisplayModel>(sales);
            NotifyOfPropertyChange(() => Products);
        }

        private BindingList<SaleDetailDisplayModel> _products;
        public BindingList<SaleDetailDisplayModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private SaleDetailDisplayModel _selectedProduct;
        public SaleDetailDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanReturn);
            }
        }

        private BindingList<SaleDetailDisplayModel> _returnProducts;
        public BindingList<SaleDetailDisplayModel> ReturnProducts
        {
            get { return _returnProducts; }
            set
            {
                _returnProducts = value;
                NotifyOfPropertyChange(() => ReturnProducts);
            }
        }

        private SaleDetailDisplayModel _selectedReturnProduct;
        public SaleDetailDisplayModel SelectedReturnProduct
        {
            get { return _selectedReturnProduct; }
            set
            {
                _selectedReturnProduct = value;
                NotifyOfPropertyChange(() => SelectedReturnProduct);
                NotifyOfPropertyChange(() => CanRemove);
            }
        }

        public bool CanReceive => SelectedPoNo != null;

        public async Task Receive()
        {
            var receiveInvoice = new ReceiveInvoice
            {
                Invoice = _selectedPONo,
                SaleHeader = _saleHeader
            };
            try
            {
                receiveInvoice.Returns = _mapper.Map<List<SaleDetail>>(ReturnProducts);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            //process based on the adjusted parameters
            UpdatedOrderStatusId();
            RecomputeSaleHeader(); 

            await _saleEndpoint.ReceiveInvoice(receiveInvoice);

            MessageBox.Show("Invoice is received!", "System Confirmation", MessageBoxButton.OK);

            SelectedPoNo = null;
            OnClearAfterReceive();

            await LoadInvoices();

        }

        public string InvoiceCount => _invoices == null ? "0" : _invoices.Count().ToString();

        private void RecomputeSaleHeader()
        {

            _saleHeader.UserName = _loggedInUser.User.UserName.ToLower();

            if (Products.Count == ReturnProducts.Count)
            {
                _saleHeader.TotalOrderQuantity = 0;
                _saleHeader.TotalOrderQuantityUOMComputed = 0;

                _saleHeader.TotalProductSalePrice = 0;
                _saleHeader.Total = 0;
                _saleHeader.TotalProductTaxPrice = 0;

                _saleHeader.TotalDeductionPrice = 0;
                _saleHeader.Outright = 0;
                _saleHeader.CashDiscount = 0;
                _saleHeader.PromoDiscount = 0;
                
                _saleHeader.LessPrice = 0;
                _saleHeader.TotalPrice = 0;
                
                //


            }
            else if (ReturnProducts.Count > 0)
            {
                _saleHeader.TotalOrderQuantity = Products.Sum(x => x.OrderQuantity);
                _saleHeader.TotalOrderQuantityUOMComputed = (float) CalculateTotalQuantityUomComputed();

                _saleHeader.TotalProductSalePrice = CalculateSubTotal();
                _saleHeader.Total = _saleHeader.TotalProductSalePrice;
                _saleHeader.TotalProductTaxPrice = CalculateTaxPrice();

                _saleHeader.Outright = CalculateDeductionOutright();
                _saleHeader.CashDiscount = CalculateDeductionCashDiscount();
                _saleHeader.PromoDiscount = CalculateDeductionPromoDiscount();
                _saleHeader.TotalDeductionPrice = _saleHeader.Outright +
                                                  _saleHeader.CashDiscount +
                                                  _saleHeader.PromoDiscount;

                //as is 
                //_saleHeader.LessPrice = x;
                _saleHeader.TotalPrice = _saleHeader.TotalProductSalePrice + _saleHeader.TotalProductTaxPrice
                                        - _saleHeader.TotalDeductionPrice - _saleHeader.LessPrice;
            }

        }

        private decimal CalculateTaxPrice()
        {
            return Products.Sum(x => Convert.ToDecimal(x.Price) * (decimal)x.OrderQuantity * x.TaxRate);
        }
        private decimal CalculateSubTotal()
        {
            return Products.Sum(item => Convert.ToDecimal(item.Price) * (decimal)item.OrderQuantity);
        }
        private decimal CalculateDeductionOutright()
        {
            return Products.Sum(item => item.DeductionOutright);
        }

        private decimal CalculateDeductionCashDiscount()
        {
            return Products.Sum(item => item.DeductionCashDiscount);
        }

        private decimal CalculateDeductionPromoDiscount()
        {
            return Products.Sum(item => (item.DeductionPromoDiscount));
        }

        private decimal CalculateTotalQuantityUomComputed()
        {
            decimal qty = 0;
            if (Products != null)
            {
                if (Products.Count > 0)
                {
                    foreach (var product in Products)
                    {
                        if (product.ProductUnitOfMeasure.ToLower().Equals("unit(s)"))
                        {
                            //qty += (decimal)product.CartQuantity;
                            //do nothing 
                        }
                        else
                        {
                            var unit = (Convert.ToDecimal(product.ProductUnitOfMeasure.ToLower().Replace("kg", "")) / 50) * (decimal)product.OrderQuantity;
                            qty += unit;
                        }
                    }
                }
            }
            return qty;
        }

        private void UpdatedOrderStatusId()
        {
            if (Products.Count == ReturnProducts.Count)
                _saleHeader.OrderStatusId = (long) OrderStatusEnum.RETURNEDALL;
            else if (ReturnProducts.Count > 0)
                _saleHeader.OrderStatusId = (long)OrderStatusEnum.DELIVERED_WITH_RETURNS;
            else
                _saleHeader.OrderStatusId = (long)OrderStatusEnum.DELIVERED;
        }

        private void OnClearAfterReceive()
        {
            
            WithReturns = false;
            _saleHeader = null;
            Products = new BindingList<SaleDetailDisplayModel>();
            ReturnProducts = new BindingList<SaleDetailDisplayModel>();
            NotifyOfPropertyChange(() => DeliveryDate);
            NotifyOfPropertyChange(() => Customer);
        }

        public bool CanReturn => SelectedProduct != null;

        public void Return()
        {

            var dialog = new ReturnWindow
            {
                Description = $"{SelectedProduct.ProductName} [Qty]: {SelectedProduct.OrderQuantity}",
                MaxQuantity = SelectedProduct.OrderQuantity
            };

            if (dialog.ShowDialog() == false) return;

            try
            {
                if (Math.Abs(dialog.EnteredQuantity - SelectedProduct.OrderQuantity) < TOLERANCE)
                {
                    ReturnProducts.Add(SelectedProduct);
                    Products.Remove(SelectedProduct);
                    SelectedProduct = null;
                }
                else
                {

                    var product = new SaleDetailDisplayModel
                    {
                        Id = SelectedProduct.Id,
                        ProductId = SelectedProduct.ProductId,
                        SupplierId = SelectedProduct.SupplierId,
                        Supplier = SelectedProduct.Supplier,
                        OrderQuantity = dialog.EnteredQuantity,
                        ProductUnitOfMeasure = SelectedProduct.ProductUnitOfMeasure,
                        ProductName = SelectedProduct.ProductName,
                        DeductionFixPrice = SelectedProduct.DeductionFixPrice,
                        ProductSalePrice = SelectedProduct.ProductSalePrice,
                        SubTotalProductSalePrice = SelectedProduct.SubTotalProductSalePrice
                    };
                    product.SubTotalProductSalePrice = (decimal)product.OrderQuantity * Convert.ToDecimal(product.Price);
                    ReturnProducts.Add(product);

                    SelectedProduct.OrderQuantity -= dialog.EnteredQuantity;
                    SelectedProduct.SubTotalProductSalePrice = (decimal)SelectedProduct.OrderQuantity * Convert.ToDecimal(SelectedProduct.Price);
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }


        }

        public bool CanRemove => SelectedReturnProduct != null;

        public void Remove()
        {
            Products.Add(SelectedReturnProduct);
            ReturnProducts.Remove(SelectedReturnProduct);
            SelectedReturnProduct = null;
        }

        public void Close()
        {
            TryClose();
        }

        public void ClickCheckBox()
        {
            WithReturns = !WithReturns;
        }

        private bool _withReturns;

        public bool WithReturns
        {
            get { return _withReturns; }
            set
            {
                _withReturns = value; 
                NotifyOfPropertyChange(() => WithReturns);
                if (_selectedPONo != null)
                    _selectedPONo.WithReturns = _withReturns;

                if (_withReturns == false)
                {
                    //reset all from original 
                    OnLoadOrderedProducts();
                    OnClearReturnProducts();
                }
            }
        }

        public float TOLERANCE { get; private set; } = (float) 0.001;
    }
}
