using AutoMapper;
using Caliburn.Micro;
using Project.FC2J.Models.Product;
using Project.FC2J.UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Project.FC2J.UI.Helpers.AppSetting;
using Project.FC2J.UI.Helpers.Products;

namespace Project.FC2J.UI.ViewModels
{
    public class ProductViewModel : Screen, IHandle<Product>
    {
        private readonly IProductEndpoint _productEndpoint;
        private readonly IApiAppSetting _appSetting;
        private readonly IMapper _mapper;
        private string _productName;
        private string _sFAReferenceNo;
        private IEventAggregator _events;

        private decimal _costPrice = 0;
        private string _productId;

        public ProductViewModel(IProductEndpoint productEndpoint, IMapper mapper, IApiAppSetting appSetting, IEventAggregator events)
        {
            _productEndpoint = productEndpoint;
            _mapper = mapper;
            _events = events;
            _events.Subscribe(this);
            _appSetting = appSetting;
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

            SearchInput = value;

            if (string.IsNullOrWhiteSpace(SearchInput))
            {
                if (Products.Count != allRecords.Count)
                    products = _mapper.Map<List<ProductDisplayModel>>(allRecords);
                else
                {
                    return;
                }
            }
            else
            {
                products = _mapper.Map<List<ProductDisplayModel>>(allRecords.Where(c => c.Name.ToLower().Contains(SearchInput.ToLower())));
            }
            Products = new ObservableCollection<ProductDisplayModel>(products);
            NotifyOfPropertyChange(() => Count);
        }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
            await LoadInternalCategories();
            await LoadCategories();
        }


        #region Products

        private List<Product> allRecords = new List<Product>();
        private async Task LoadProducts()
        {
            allRecords = await _productEndpoint.GetList();
            var products = _mapper.Map<List<ProductDisplayModel>>(allRecords);
            Products = new ObservableCollection<ProductDisplayModel>(products);
            IsGridVisible = true;
            NotifyOfPropertyChange(() => Count);
        }

        private string ProductsCount()
        {
            if(Products != null)
                return Products.Count > 1 ? $"[{Products.Count} products]" : $"[{Products.Count} product]";
            return string.Empty;
        }

        private ObservableCollection<ProductDisplayModel> _products;
        public ObservableCollection<ProductDisplayModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
                NotifyOfPropertyChange(() => CollectionView);
            }
        }

        private CollectionView _collectionView = null;
        public CollectionView CollectionView
        {
            get
            {
                _collectionView = (CollectionView)CollectionViewSource.GetDefaultView(Products);
                _collectionView?.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                return _collectionView;
            }
        }

        private ProductDisplayModel _selectedProduct;
        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                if (_selectedProduct != value)
                {
                    _selectedProduct = value;

                    ProductId = _selectedProduct?.Id.ToString();
                    ProductName = _selectedProduct?.Name;
                    SFAReferenceNo = _selectedProduct?.SFAReferenceNo;
                    UnitOfMeasure = _selectedProduct?.UnitOfMeasure;
                    SFAUnitOfMeasure = _selectedProduct?.SFAUnitOfMeasure;
                    SalePriceCORON = Convert.ToDecimal(_selectedProduct?.SalePrice_CORON);
                    SalePriceLUBANG = Convert.ToDecimal(_selectedProduct?.SalePrice_LUBANG);
                    SalePriceSANILDEFONSO = Convert.ToDecimal(_selectedProduct?.SalePrice_SANILDEFONSO);
                    CostPrice = Convert.ToDecimal(_selectedProduct?.CostPrice);
                    IsTaxable = Convert.ToBoolean(_selectedProduct?.IsTaxable);
                    IsFeeds = Convert.ToBoolean(_selectedProduct?.IsFeeds);
                    IsConvertibleToBag = Convert.ToBoolean(_selectedProduct?.IsConvertibleToBag);
                    KiloPerUnit = Convert.ToDecimal(_selectedProduct?.KiloPerUnit);
                    Description = _selectedProduct != null ? 
                            $"Coron={_selectedProduct?.Stock_CORON}; Lubang={_selectedProduct?.Stock_LUBANG}; San Ildefonso={_selectedProduct?.Stock_SANILDEFONSO}" 
                            : string.Empty;

                    SelectedCategory = Categories.FirstOrDefault(item => item.Category == _selectedProduct?.Category);
                    SelectedInternalCategory = InternalCategories.FirstOrDefault(item => item.InternalCategory == _selectedProduct?.InternalCategory);

                    IsEditable = Convert.ToBoolean(SelectedProduct?.IsEditable);

                    NotifyOfPropertyChange(() => SelectedProduct);
                    NotifyOfPropertyChange(() => Description);
                    NotifyOfPropertyChange(() => CanDelete);

                }

            }
        }

        #endregion

        #region Internal Category

        public string Count => ProductsCount();
        private BindingList<ProductInternalCategory> _internalCategories;
        public BindingList<ProductInternalCategory> InternalCategories
        {
            get { return _internalCategories; }
            set
            {
                if (value != _internalCategories)
                {
                    _internalCategories = value;
                    NotifyOfPropertyChange(() => InternalCategories);
                }
            }
        }
        private async Task LoadInternalCategories()
        {
            var records = await _productEndpoint.GetInternalCategories();
            InternalCategories = new BindingList<ProductInternalCategory>(records.ToList());
        }
        private ProductInternalCategory _selectedInternalCategory;
        public ProductInternalCategory SelectedInternalCategory
        {
            get { return _selectedInternalCategory; }
            set
            {
                if (value != _selectedInternalCategory)
                {
                    _selectedInternalCategory = value;
                    NotifyOfPropertyChange(() => SelectedInternalCategory);
                    NotifyOfPropertyChange(() => CanSave);
                    NotifyOfPropertyChange(() => CanClear);
                }
            }
        }


        #endregion

        #region SFA Category

        private BindingList<ProductSFACategory> _categories;
        public BindingList<ProductSFACategory> Categories
        {
            get { return _categories; }
            set
            {
                if (value != _categories)
                {
                    _categories = value;
                    NotifyOfPropertyChange(() => Categories);
                }
            }
        }
        private async Task LoadCategories()
        {
            var records = await _productEndpoint.GetCategories();
            Categories = new BindingList<ProductSFACategory>(records.ToList());
        }
        private ProductSFACategory _selectedCategory;
        public ProductSFACategory SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                if (value != _selectedCategory)
                {
                    _selectedCategory = value;
                    NotifyOfPropertyChange(() => SelectedCategory);
                    NotifyOfPropertyChange(() => CanSave);
                    NotifyOfPropertyChange(() => CanClear);
                }
            }
        }
        #endregion

        private bool _isEditable = true;
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value; 
                NotifyOfPropertyChange( () => IsEditable);

            }
        }



        private bool _isTaxable;
        public bool IsTaxable
        {
            get { return _isTaxable; }
            set
            {
                _isTaxable = value;
                NotifyOfPropertyChange(() => IsTaxable);
            }
        }

        private bool _isFeeds;
        public bool IsFeeds
        {
            get { return _isFeeds; }
            set
            {
                _isFeeds = value;
                NotifyOfPropertyChange(() => IsFeeds);
            }
        }

        private bool _isConvertibleToBag;
        public bool IsConvertibleToBag
        {
            get { return _isConvertibleToBag; }
            set
            {
                if(_isConvertibleToBag != value)
                {
                    _isConvertibleToBag = value;
                    KiloPerUnit = 0;
                    NotifyOfPropertyChange(() => IsConvertibleToBag);
                    NotifyOfPropertyChange(() => KiloPerUnit);
                    NotifyOfPropertyChange(() => CanSave);
                    NotifyOfPropertyChange(() => CanClear);
                }
            }
        }
        private decimal _kiloPerUnit = 0;
        public decimal KiloPerUnit
        {
            get { return _kiloPerUnit; }
            set
            {
                _kiloPerUnit = value;
                NotifyOfPropertyChange(() => KiloPerUnit);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        public string ProductId
        {
            get { return _productId; }
            set
            {
                _productId = value;
                NotifyOfPropertyChange(() => ProductId);
            }
        }
        public decimal CostPrice
        {
            get { return _costPrice; }
            set
            {
                _costPrice = value;
                NotifyOfPropertyChange(() => CostPrice);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        private decimal _salePriceCORON = 0;
        public decimal SalePriceCORON
        {
            get { return _salePriceCORON; }
            set
            {
                _salePriceCORON = value;
                NotifyOfPropertyChange(() => SalePriceCORON);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        private decimal _salePriceLUBANG = 0;
        public decimal SalePriceLUBANG
        {
            get { return _salePriceLUBANG; }
            set
            {
                _salePriceLUBANG = value;
                NotifyOfPropertyChange(() => SalePriceLUBANG);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        private decimal _salePriceSANILDEFONSO = 0;
        public decimal SalePriceSANILDEFONSO
        {
            get { return _salePriceSANILDEFONSO; }
            set
            {
                _salePriceSANILDEFONSO = value;
                NotifyOfPropertyChange(() => SalePriceSANILDEFONSO);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        private string _unitOfMeasure;
        public string UnitOfMeasure
        {
            get { return _unitOfMeasure; }
            set
            {
                _unitOfMeasure = value;
                NotifyOfPropertyChange(() => UnitOfMeasure);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanClear);
            }
        }
        private string _sfaUnitOfMeasure;
        public string SFAUnitOfMeasure
        {
            get { return _sfaUnitOfMeasure; }
            set
            {
                if(_sfaUnitOfMeasure != value)
                {
                    _sfaUnitOfMeasure = value;
                    NotifyOfPropertyChange(() => SFAUnitOfMeasure);
                    NotifyOfPropertyChange(() => CanSave);
                    NotifyOfPropertyChange(() => CanClear);
                }
            }
        }

        public string SFAReferenceNo
        {
            get { return _sFAReferenceNo; }
            set
            {
                _sFAReferenceNo = value;
                NotifyOfPropertyChange(() => SFAReferenceNo);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanClear);
            }
        }
        public string ProductName
        {
            get { return _productName; }
            set
            {
                _productName = value;
                NotifyOfPropertyChange(() => ProductName);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => Description);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value; 
                NotifyOfPropertyChange(() => Description);
            }
        }



        public bool CanSave
        {
            get
            {
                var output = string.IsNullOrWhiteSpace(ProductName) == false
                              && SelectedCategory != null
                              && SelectedInternalCategory != null
                              && string.IsNullOrWhiteSpace(SFAReferenceNo) == false
                              && string.IsNullOrWhiteSpace(UnitOfMeasure) == false
                              && string.IsNullOrWhiteSpace(SFAUnitOfMeasure) == false
                              && SalePriceCORON > 0
                              && SalePriceLUBANG > 0
                              && SalePriceSANILDEFONSO > 0;
                              // || (IsConvertibleToBag && KiloPerUnit > 0);
                              if (IsConvertibleToBag && KiloPerUnit <= 0)
                                  output = false;
                return output;
            }
        }

        public bool CanDelete
        {
            get
            {
                bool output = false;

                if (string.IsNullOrWhiteSpace(ProductId) == false && IsEditable)
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
                await _productEndpoint.Remove(Convert.ToInt64(ProductId));
                var recordToDelete = allRecords.FirstOrDefault(product => product.Id == Convert.ToInt64(ProductId));
                if (recordToDelete != null)
                    allRecords.Remove(recordToDelete);
                Products.Remove(SelectedProduct);
                NotifyOfPropertyChange(() => Count);
                OnClear();
            }
        }

        public async Task Save()
        {
            if (MessageBox.Show("Are you sure?", "Save Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var product = new Product
                {
                    SFAReferenceNo = SFAReferenceNo,
                    Name = ProductName,
                    Description = $"[{SFAReferenceNo}] {ProductName}",
                    Category = SelectedCategory?.Category,
                    InternalCategory = SelectedInternalCategory?.InternalCategory,
                    UnitOfMeasure = UnitOfMeasure,
                    SFAUnitOfMeasure = SFAUnitOfMeasure,
                    SalePrice_CORON = SalePriceCORON,
                    SalePrice_LUBANG = SalePriceLUBANG,
                    SalePrice_SANILDEFONSO = SalePriceSANILDEFONSO,
                    CostPrice = CostPrice,
                    IsTaxable = IsTaxable,
                    IsFeeds = IsFeeds,
                    IsConvertibleToBag = IsConvertibleToBag,
                    KiloPerUnit = KiloPerUnit,
                    IsEditable = true
                };

                if (SelectedProduct == null)
                {
                    try
                    {
                        var result = await _productEndpoint.Save(product);
                        allRecords.Add(result);
                        var newProduct = _mapper.Map<ProductDisplayModel>(result);
                        Products.Add(newProduct);
                        NotifyOfPropertyChange(() => Count);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    
                }
                else
                {
                    product.Id = SelectedProduct.Id;
                    await _productEndpoint.Update(product);
                    SelectedProduct.SFAReferenceNo = SFAReferenceNo;
                    SelectedProduct.Name = ProductName;
                    SelectedProduct.Description = Description;
                    SelectedProduct.Category = SelectedCategory?.Category;
                    SelectedProduct.InternalCategory = SelectedInternalCategory?.InternalCategory;
                    SelectedProduct.UnitOfMeasure = UnitOfMeasure;
                    SelectedProduct.SFAUnitOfMeasure = SFAUnitOfMeasure;
                    SelectedProduct.SalePrice_CORON = SalePriceCORON;
                    SelectedProduct.SalePrice_LUBANG = SalePriceLUBANG;
                    SelectedProduct.SalePrice_SANILDEFONSO = SalePriceSANILDEFONSO;
                    SelectedProduct.CostPrice = CostPrice;
                    SelectedProduct.IsTaxable = IsTaxable;
                    SelectedProduct.IsFeeds = IsFeeds;
                    SelectedProduct.IsConvertibleToBag = IsConvertibleToBag;
                    SelectedProduct.KiloPerUnit = KiloPerUnit;
                }

                MessageBox.Show("Product is Successfully Saved.", "Save Confirmed!", MessageBoxButton.OK);
                OnClear();
            }
        }


        public bool CanClear
        {
            get
            {
                var output = string.IsNullOrWhiteSpace(ProductName) == false
                              || SelectedCategory != null
                              || SelectedInternalCategory != null
                              || string.IsNullOrWhiteSpace(SFAReferenceNo) == false
                              || string.IsNullOrWhiteSpace(UnitOfMeasure) == false
                              || string.IsNullOrWhiteSpace(SFAUnitOfMeasure) == false
                              || SalePriceCORON > 0
                              || SalePriceLUBANG > 0
                              || SalePriceSANILDEFONSO > 0
                              || (IsConvertibleToBag && KiloPerUnit > 0);

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
            SelectedProduct = null;
            ProductName = string.Empty;
            SFAReferenceNo = string.Empty;
            Description = string.Empty;
            UnitOfMeasure = string.Empty;
            SFAUnitOfMeasure = string.Empty;
            SalePriceCORON = 0;
            SalePriceLUBANG = 0;
            SalePriceSANILDEFONSO = 0;
            CostPrice = 0;
            ProductId = string.Empty;
            IsTaxable = false;
            SelectedCategory = null;
            SelectedInternalCategory = null;
            NotifyOfPropertyChange(() => CanDelete);
            IsEditable = true;
            IsFeeds = false;
            IsConvertibleToBag = false;
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

        private bool _isGridVisible = false;

        public bool IsGridVisible
        {
            get { return _isGridVisible; }
            set
            {
                _isGridVisible = value;
                NotifyOfPropertyChange(() => IsGridVisible);
                NotifyOfPropertyChange(() => IsLoadingVisible);
            }
        }

        public bool IsLoadingVisible => !_isGridVisible;

        public void Close()
        {
            TryClose();
        }

        public void Handle(Product message)
        {
            var product = Products.FirstOrDefault(item => item.Id == message.Id);
            if (product != null)
            {
                product.Stock_CORON = message.Stock_CORON;
                product.Stock_LUBANG = message.Stock_LUBANG;
                product.Stock_SANILDEFONSO = message.Stock_SANILDEFONSO;
            }

            var product2 = allRecords.FirstOrDefault(item => item.Id == message.Id);
            if (product2 == null) return;
            product2.Stock_CORON = message.Stock_CORON;
            product2.Stock_LUBANG = message.Stock_LUBANG;
            product2.Stock_SANILDEFONSO = message.Stock_SANILDEFONSO;

        }
    }
}
