using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using Caliburn.Micro;
using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Product;
using Project.FC2J.UI.Helpers;
using Project.FC2J.UI.Helpers.AppSetting;
using Project.FC2J.UI.Helpers.Products;
using Project.FC2J.UI.Models;

namespace Project.FC2J.UI.ViewModels
{
    public class PriceListPOViewModel : Screen
    {
        private readonly IPriceListEndpoint _priceListEndpoint;
        private readonly IProductEndpoint _productEndpoint;
        private readonly IMapper _mapper;
        private readonly IEventAggregator _events;

        public PriceListPOViewModel(IProductEndpoint productEndpoint, IPriceListEndpoint priceListEndpoint, IMapper mapper, IEventAggregator events)
        {
            _priceListEndpoint = priceListEndpoint;
            _productEndpoint = productEndpoint;
            _mapper = mapper;
            _events = events;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadPriceLists();
            OnReset();
        }

        private bool _isHidePriceList;
        public bool IsHidePriceList
        {
            get { return _isHidePriceList; }
            set
            {
                _isHidePriceList = value; 
                NotifyOfPropertyChange(() => IsHidePriceList);
            }
        }
        private bool _isAdd;
        public bool IsAdd
        {
            get { return _isAdd; }
            set
            {
                _isAdd = value;
                NotifyOfPropertyChange(() => IsAdd);
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
        public void FilterLists(string value)
        {
            List<ProductDisplayModel> products;

            if (string.IsNullOrWhiteSpace(value))
            {
                if (Products.Count != _allProducts.Count)
                    products = _mapper.Map<List<ProductDisplayModel>>(_allProducts);
                else
                {
                    return;
                }
            }
            else
            {
                products = _mapper.Map<List<ProductDisplayModel>>(_allProducts.Where(c => c.Name.ToLower().Contains(value.ToLower())));
            }

            Products = new BindingList<ProductDisplayModel>(products);
        }

        private async Task LoadPriceLists()
        {
            try
            {
                var allPriceLists = await _priceListEndpoint.GetList(0);
                var pricelistsMap = _mapper.Map<List<PricelistDisplayModel>>(allPriceLists);
                PriceLists = new BindingList<PricelistDisplayModel>(pricelistsMap);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        private void OnReset()
        {
            Products = new BindingList<ProductDisplayModel>();
            SelectedProduct = null;
            SelectedPriceList = null;
            IsHidePriceList = true;
            IsAdd = false;
            SearchInput = string.Empty;
        }

        public bool CanSavePriceList => string.IsNullOrWhiteSpace(PriceListName) == false &&
                                        string.IsNullOrWhiteSpace(Email) == false;
        public async Task SavePriceList()
        {
            if (MessageBox.Show("Are you sure?", "Save PriceList Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

                var priceList = new PriceList
                {
                    IsForSalesOrder = false,
                    Name = PriceListName.Trim(),
                    Email = Email,
                    Subscribed = 1
                };

                if (IsAdd)
                {
                    var itemExist = PriceLists.FirstOrDefault(x => x.Name == PriceListName.Trim());

                    if (itemExist != null)
                    {
                        MessageBox.Show("Price List Name already exists.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    var result = await _priceListEndpoint.Save(priceList);
                    MessageBox.Show("Price List successfully saved.", "Confirmation", MessageBoxButton.OK);
                    await LoadPriceLists();
                    OnReset();
                }
                else
                {

                    var itemExist = PriceLists.FirstOrDefault(x => x.Name == PriceListName.Trim() && x.Id != SelectedPriceList.Id);
                    if (itemExist != null)
                    {
                        MessageBox.Show("Price List Name already exists.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    priceList.Id = SelectedPriceList.Id;
                    await _priceListEndpoint.UpdatePOPricelist(priceList);
                    MessageBox.Show("Price List successfully updated.", "Confirmation", MessageBoxButton.OK);
                }

            }
        }
        private PricelistDisplayModel _selectedPricelist;
        public PricelistDisplayModel SelectedPriceList
        {
            get { return _selectedPricelist; }
            set
            {
                _selectedPricelist = value;
                NotifyOfPropertyChange(() => SelectedPriceList);
                NotifyOfPropertyChange(() => IsEditPriceList);
                NotifyOfPropertyChange(() => IsNotEditPriceList);
                PriceListName = SelectedPriceList?.Name;
                Email = SelectedPriceList?.Email;
                SelectedProduct = null;
                _events.Publish("SelectedPriceList Changed", action => {
                    Task.Factory.StartNew(OnLoadProducts());
                });
                IsAdd = SelectedPriceList == null;
                IsHidePriceList = SelectedPriceList == null;
            }
        }

        public bool IsEditPriceList => SelectedPriceList == null;
        public bool IsNotEditPriceList => !IsEditPriceList;
        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
                NotifyOfPropertyChange(() => CanSavePriceList);
            }
        }

        private string _priceListName;
        public string PriceListName
        {
            get { return _priceListName; }
            set
            {
                _priceListName = value;
                NotifyOfPropertyChange(() => PriceListName);
                NotifyOfPropertyChange(() => CanSavePriceList);
            }
        }


        private System.Action OnLoadProducts()
        {
            return async () =>
            {
                if (SelectedPriceList != null)
                    await LoadProducts();
                else
                {
                    Products = new BindingList<ProductDisplayModel>();
                }
            };
        }
        private async Task LoadProducts()
        {
            _allProducts = await _priceListEndpoint.GetPriceList(SelectedPriceList.Id);
            var products = _mapper.Map<List<ProductDisplayModel>>(_allProducts);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        private List<Product> _allProducts = new List<Product>();
        private BindingList<ProductDisplayModel> _products;
        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
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
                NotifyOfPropertyChange(() => SFAReferenceNo);
                NotifyOfPropertyChange(() => ProductName);
                NotifyOfPropertyChange(() => Category);
                NotifyOfPropertyChange(() => UnitOfMeasure);
                NotifyOfPropertyChange(() => CanSave);

                SalePrice = Convert.ToDecimal(SelectedProduct?.SalePrice);
                UnitDiscount = Convert.ToDecimal(SelectedProduct?.UnitDiscount);
            }
        }

        public string SFAReferenceNo => SelectedProduct?.SFAReferenceNo;
        public string ProductName => SelectedProduct?.Name;
        public string Category => SelectedProduct?.Category;
        public string UnitOfMeasure => SelectedProduct?.UnitOfMeasure;
        private decimal _salePrice;
        public decimal SalePrice
        {
            get { return _salePrice; }
            set
            {
                _salePrice = value;
                NotifyOfPropertyChange(() => SalePrice);
            }
        }
        private decimal _unitDiscount;
        public decimal UnitDiscount
        {
            get { return _unitDiscount; }
            set
            {
                _unitDiscount = value;
                NotifyOfPropertyChange(() => UnitDiscount);
            }
        }


        public void Close()
        {
            TryClose();
        }

        public bool CanSave => SelectedProduct != null;
        public async Task Save()
        {
            if (MessageBox.Show("Are you sure?", "Save Product Pricing Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var productPrice = new ProductPrice
                {
                    Id = Convert.ToInt64(SelectedProduct?.Id),
                    PriceListId = Convert.ToInt64(SelectedPriceList?.Id),
                    SalePrice = SalePrice,
                    UnitDiscount= UnitDiscount
                };

                await _productEndpoint.UpdateProductPrice(productPrice);

                MessageBox.Show("Product Price is successfully saved.", "Confirmation", MessageBoxButton.OK);
                if (SelectedProduct != null)
                {
                    SelectedProduct.SalePrice = SalePrice;
                    SelectedProduct.UnitDiscount = UnitDiscount;
                }

                SelectedProduct = null;
            }
        }

        public void Add()
        {
            OnReset();
            IsHidePriceList = false;
            IsAdd = true;
        }

    }
}
