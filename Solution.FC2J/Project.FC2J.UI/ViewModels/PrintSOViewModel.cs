using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AutoMapper;
using Caliburn.Micro;
using Project.FC2J.Models;
using Project.FC2J.Models.Sale;
using Project.FC2J.Models.User;
using Project.FC2J.UI.Helpers;
using Project.FC2J.UI.Models;
using Project.FC2J.UI.Providers;

namespace Project.FC2J.UI.ViewModels
{
    public class PrintSOViewModel : Screen
    {

        private static Grid _toPrint;
        private readonly IMapper _mapper;
        private readonly ILoggedInUser _loggedInUser;
        private readonly ISaleEndpoint _saleEndpoint;
        private readonly IEventAggregator _events;
        private IDeductionEndpoint _deductionEndpoint;

        public PrintSOViewModel(ISaleEndpoint saleEndpoint,  ILoggedInUser loggedInUser, IMapper mapper, IEventAggregator events, IDeductionEndpoint deductionEndpoint)
        {
            _saleEndpoint = saleEndpoint;
            _loggedInUser = loggedInUser;
            _mapper = mapper;
            _events = events;
            _deductionEndpoint = deductionEndpoint;
        }

       
        public void Print(object source)
        {
            
            EnumVisual((Visual)this.GetView());
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                try
                {
                    printDialog.PrintVisual(_toPrint, "Sales Order");
                }
                catch (Exception e)
                {
                    MessageBox.Show($"{e.Message}","Printing Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
        }

        public static void EnumVisual(Visual myVisual)
        {
            _toPrint = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                // Retrieve child visual at specified index value.
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myVisual, i);

                // Do processing of the child visual object.
                var thisGrid = childVisual as Grid;

                if (thisGrid != null)
                {
                    if (thisGrid.Name.Equals("ToPrint"))
                    {
                        _toPrint = thisGrid;
                        break;
                    }
                }
                if (_toPrint == null)
                    EnumVisual(childVisual);
                else
                    break;
            }
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadSales();
        }

        private async Task LoadSales()
        {
            var salesList = await _saleEndpoint.GetSales(_loggedInUser.User.UserName.ToLower());
            var sales = _mapper.Map<List<SalesDisplayModel>>(salesList);
            Sales = new BindingList<SalesDisplayModel>(sales);
        }

        public SaleSuggestionProvider PONoProvider
        {
            get
            {
                var partnersProvider = new SaleSuggestionProvider { Sales = Sales };
                return partnersProvider;
            }
        }

        private BindingList<SalesDisplayModel> _sales;
        public BindingList<SalesDisplayModel> Sales
        {
            get { return _sales; }
            set
            {
                _sales = value;
                NotifyOfPropertyChange(() => Sales);
                NotifyOfPropertyChange(() => PONoProvider);
            }
        }

        private SalesDisplayModel _selectedPONo;
        public SalesDisplayModel SelectedPONo
        {
            get
            {
                return _selectedPONo;
            }
            set
            {
                if (_selectedPONo != value)
                {
                    _count = 0;
                    _selectedPONo = value;
                    NotifyOfPropertyChange(() => SelectedPONo);

                    NotifyOfPropertyChange(() => DeliveryDate);
                    NotifyOfPropertyChange(() => DueDate);
                    NotifyOfPropertyChange(() => Customer);
                    NotifyOfPropertyChange(() => Address);
                    NotifyOfPropertyChange(() => PaymentTerms);
                    NotifyOfPropertyChange(() => Trade);
                    NotifyOfPropertyChange(() => SONo);
                    NotifyOfPropertyChange(() => PromoDiscount);
                    NotifyOfPropertyChange(() => PickupDiscount);
                    NotifyOfPropertyChange(() => CashDiscount);
                    NotifyOfPropertyChange(() => Outright);
                    NotifyOfPropertyChange(() => TotalSales);
                    NotifyOfPropertyChange(() => VAT12);

                    _events.Publish("SelectedPartner Changed", action => {
                        Task.Factory.StartNew(OnGetSaleDetails());
                    });
                }

            }
        }

        public string PromoDiscount => SelectedPONo?.PromoDiscount.ToString("C").Substring(1);
        public string PickupDiscount => SelectedPONo?.PickUpDiscount.ToString("C").Substring(1);
        public string CashDiscount => SelectedPONo?.CashDiscount.ToString("C").Substring(1);
        public string Outright => SelectedPONo?.Outright.ToString("C").Substring(1);

        public string VAT12 => SelectedPONo?.TotalProductTaxPrice.ToString("C").Substring(1);
        public string VATExemptSales => CalculateVATExemptSales().ToString("C").Substring(1);
        public string VATSales => CalculateVATSales().ToString("C").Substring(1);


        private decimal CalculateVATSales()
        {
            if (Products == null) return 0;
            decimal vatSales = 0;
            foreach (var item in Products)
            {
                if(item.IsTaxable)
                    vatSales += item.SubTotalProductSalePrice - item.SubTotalProductTaxPrice;
            }
            return vatSales;
        }
        private decimal CalculateVATExemptSales()
        {
            if (Products == null) return 0;
            decimal vatExemptSales = 0;
            foreach (var item in Products)
            {
                if (item.IsTaxable == false)
                    vatExemptSales += item.SubTotalProductSalePrice ;
            }
            return vatExemptSales;

        }

        private decimal CalculateTotalSales()
        {
            var totalSales = SelectedPONo?.TotalProductSalePrice - SelectedPONo?.Outright - SelectedPONo?.CashDiscount - SelectedPONo?.PickUpDiscount - SelectedPONo?.PromoDiscount;
            return Convert.ToDecimal(totalSales);
        }

        public string TotalSales => CalculateTotalSales().ToString("C").Substring(1);

        public string TotalAmountDue => SelectedPONo?.TotalPrice.ToString("C").Substring(1);
        public string DeliveryDate => SelectedPONo?.DeliveryDate.ToString("MMM-dd-yyyy");
        public string DueDate => SelectedPONo?.DueDate.ToString("MMM-dd-yyyy");
        public string Customer => SelectedPONo?.CustomerName;
        public string Address => SelectedPONo?.CustomerAddress2;
        public string PaymentTerms => SelectedPONo?.SelectedPaymentType;
        public string Trade => string.Empty;
        public string SONo => $"{SelectedPONo?.SONo} {SelectedPONo?.InvoiceNo}";

        public string TotalQuantity => $"Total {CalculateTotalQty().ToString("C").Substring(1)}";

        private decimal CalculateTotalQty()
        {
            if (Products != null)
            {
                return (decimal)Products.Sum(item => item.OrderQuantity);
            }
            return 0;
        }


        private int _count = 0;
        private System.Action OnGetSaleDetails()
        {
            return async () =>
            {
                if (SelectedPONo != null)
                {
                    _allSaleDetails = await _saleEndpoint.GetSaleDetails(SelectedPONo.Id, SelectedPONo.CustomerId);
                    var sales = _mapper.Map<List<SaleDetailDisplayModel>>(_allSaleDetails);
                    Products = new BindingList<SaleDetailDisplayModel>(sales);

                    NotifyOfPropertyChange(() => Products);
                    NotifyOfPropertyChange(() => TotalQuantity);
                    NotifyOfPropertyChange(() => VATExemptSales);
                    NotifyOfPropertyChange(() => VATSales);
                    NotifyOfPropertyChange(() => TotalAmountDue);

                    var allDeductions = await _deductionEndpoint.GetDeductions(SelectedPONo.PONo, SelectedPONo.CustomerId);
                    var deductions = _mapper.Map<List<DeductionDisplayModel>>(allDeductions);
                    Deductions = new BindingList<DeductionDisplayModel>(deductions);

                    NotifyOfPropertyChange(() => Deductions);
                    _count += 1;
                    Console.WriteLine($"call {_count}");
                }
            };
        }

        private List<SaleDetail> _allSaleDetails = new List<SaleDetail>();
        private BindingList<SaleDetailDisplayModel> _products;
        public BindingList<SaleDetailDisplayModel>  Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
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

        public void Close()
        {
            TryClose();
        }


    }
}
