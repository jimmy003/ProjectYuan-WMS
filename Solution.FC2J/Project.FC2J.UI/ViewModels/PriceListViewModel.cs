using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using AutoMapper;
using Caliburn.Micro;
using Microsoft.Reporting.WinForms;
using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Product;
using Project.FC2J.Models.Purchase;
using Project.FC2J.UI.Helpers;
using Project.FC2J.UI.Helpers.AppSetting;
using Project.FC2J.UI.Helpers.Products;
using Project.FC2J.UI.Models;
using Project.FC2J.UI.UserControls;

namespace Project.FC2J.UI.ViewModels
{
    public class PriceListViewModel : Screen, IHandle<Product>
    {
        private readonly IPriceListEndpoint _priceListEndpoint;
        private readonly IProductEndpoint _productEndpoint;
        private readonly ICustomerEndpoint _customerEndpoint;
        private readonly IMapper _mapper;
        private IEventAggregator _events;

        public PriceListViewModel(IProductEndpoint productEndpoint, ICustomerEndpoint customerEndpoint, IPriceListEndpoint priceListEndpoint, IMapper mapper, IApiAppSetting appSetting, IEventAggregator events)
        {
            _priceListEndpoint = priceListEndpoint;
            _productEndpoint = productEndpoint;
            _customerEndpoint = customerEndpoint;
            _mapper = mapper;
            _events = events;
            _events.Subscribe(this);
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            IsGridVisible = false;
            EditIsCalled = false;
            NotifyOfPropertyChange(() => CanUpdate);
            await LoadPriceLists();
        }

        private List<PriceList> allPriceLists = new List<PriceList>();
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
            var priceLists = new List<PricelistDisplayModel>();

            SearchInput = value;

            if (string.IsNullOrWhiteSpace(SearchInput))
            {
                if (Pricelists.Count != allPriceLists.Count)
                    priceLists = _mapper.Map<List<PricelistDisplayModel>>(allPriceLists);
                else
                {
                    return;
                }
            }
            else
            {
                priceLists = _mapper.Map<List<PricelistDisplayModel>>(allPriceLists.Where(c => c.Name.ToLower().Contains(SearchInput.ToLower())));
            }

            Pricelists = new BindingList<PricelistDisplayModel>(priceLists);
        }
        private List<Product> allProductRecords = new List<Product>();

        public void FilterProductLists(string value)
        {
            List<PricelistProducts> products;

            if (string.IsNullOrWhiteSpace(value))
            {
                if (Products.Count != allProductRecords.Count)
                    products = _mapper.Map<List<PricelistProducts>>(allProductRecords);
                else
                {
                    return;
                }
            }
            else
            {
                products = _mapper.Map<List<PricelistProducts>>(allProductRecords.Where(c => c.Name.ToLower().Contains(value.ToLower())));
            }
            Products = new ObservableCollection<PricelistProducts>(products);
            
        }

        //

        private async Task LoadPriceLists()
        {
            try
            {
                allPriceLists = await _priceListEndpoint.GetList();
                var pricelistsMap = _mapper.Map<List<PricelistDisplayModel>>(allPriceLists);
                Pricelists = new BindingList<PricelistDisplayModel>(pricelistsMap);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            _addIsCalled = false;
            NotifyOfPropertyChange(() => PricelistNameEnabled);
            //NotifyOfPropertyChange(() => DiscountPolicy1Enabled);
            //NotifyOfPropertyChange(() => DiscountPolicy2Enabled);
            NotifyOfPropertyChange(() => PricelistName);
            IsGridVisible = true;

        }

        
        public async Task RowSelect()
        {

            var value = new PricelistProducts
            {
                Id = SelectedProduct.Id,
                Name = SelectedProduct.Name,
                SalePrice_CORON = SelectedProduct.SalePrice_CORON,
                SalePrice_LUBANG = SelectedProduct.SalePrice_LUBANG,
                SalePrice_SANILDEFONSO = SelectedProduct.SalePrice_SANILDEFONSO,

                DeductionFixPrice = SelectedProduct.DeductionFixPrice,
                DeductionOutright = SelectedProduct.DeductionOutright,
                Discount = SelectedProduct.Discount,
                DeductionPromoDiscount = SelectedProduct.DeductionPromoDiscount
            };

            //ProductName.Text = _payload.Name;
            //SalePriceCoron.Text = _payload.SalePrice_CORON.ToString("N2");
            //SalePriceLubang.Text = _payload.SalePrice_LUBANG.ToString("N2");
            //SalePriceSanIldefonso.Text = _payload.SalePrice_SANILDEFONSO.ToString("N2");
            //FixPrice.Text = _payload.DeductionFixPrice.ToString("N2");
            //Outright.Text = _payload.DeductionOutright.ToString("N2");
            //Discount.Text = _payload.Discount.ToString("N2");
            //Promo.Text = _payload.DeductionPromoDiscount.ToString("N2");
            var templateId = Convert.ToInt64(SelectedPricelist?.Id);
            var pricelistDetails = new PricelistDetails(templateId, value, _priceListEndpoint, _events);
            pricelistDetails.ShowDialog();
        }

        private async Task LoadProducts(long id=0)
        {
            try
            {
                allProductRecords = await _productEndpoint.GetList(id);
                var productsMap = _mapper.Map<List<PricelistProducts>>(allProductRecords);
                Products = new ObservableCollection<PricelistProducts>(productsMap);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private async Task LoadCustomers()
        {
            try
            {
                var getCustomers= await _customerEndpoint.GetList();
                var customersMap = _mapper.Map<List<CustomerDisplayModel>>(getCustomers);
                SourceCustomers = new BindingList<CustomerDisplayModel>(customersMap);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        private PricelistDisplayModel _selectedPricelist;
        public PricelistDisplayModel SelectedPricelist
        {
            get { return _selectedPricelist; }
            set
            {
                _selectedPricelist = value;
                NotifyOfPropertyChange(() => SelectedPricelist);
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

        public async Task AddCustomer()
        {
            TargetCustomers.Add(SelectedSourceCustomer);

            var priceListCustomer = new PriceListCustomer { PriceListId = SelectedPricelist.Id };
            priceListCustomer.CustomerIds.Add(SelectedSourceCustomer.Id);
            var output = await _priceListEndpoint.SavePriceListCustomers(priceListCustomer);
            SelectedPricelist.Subscribed += 1;

            //update PriceLists

            var pricelist = Pricelists.FirstOrDefault(item => item.Id == output.PriceListId);
            if (pricelist != null)
            {
                pricelist.Subscribed -= 1;
            }


            SourceCustomers.Remove(SelectedSourceCustomer);
            SelectedSourceCustomer = null;
            NotifyOfPropertyChange(() => SubscribedCustomers);
            NotifyOfPropertyChange(() => SourcedCustomers);

            //if (TargetCustomers != null)
            //{
            //    var priceListCustomer = new PriceListCustomer { PriceListId = value.Id };
            //    foreach (var customer in TargetCustomers)
            //    {
            //        priceListCustomer.CustomerIds.Add(customer.Id);
            //    }

            //    await _priceListEndpoint.SavePriceListCustomers(priceListCustomer);
            //}

        }
        public bool CanRemoveCustomer => SelectedTargetCustomer != null;

        public async Task RemoveCustomer()
        {
            SourceCustomers.Add(SelectedTargetCustomer);

            await _priceListEndpoint.RemovePriceListCustomer(SelectedTargetCustomer.Id);
            SelectedPricelist.Subscribed -= 1;

            TargetCustomers.Remove(SelectedTargetCustomer);

            NotifyOfPropertyChange(() => SubscribedCustomers);
            NotifyOfPropertyChange(() => SourcedCustomers);
        }

        public string SubscribedCustomers => TargetCustomers == null ? "This Partners subscribed to this pricelist" : $"{TargetCustomers.Count} Partner" + (TargetCustomers.Count > 1 ? "s" : "") + " subscribed to this pricelist";
        public string SourcedCustomers => SourceCustomers == null
            ? "Select from Partners below"
            : $"Select from {SourceCustomers.Count} Partner" + (SourceCustomers.Count > 1 ? "s" : "") + " below";

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

        
        private PricelistProducts _selectedProduct;
        public PricelistProducts SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
            }
        }


        private ObservableCollection<PricelistProducts> _products;
        public ObservableCollection<PricelistProducts> Products
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
       

        //Controls Enabled
        public bool PricelistNameEnabled => AddIsCalled || EditIsCalled;

        private string _copyAs;
        public string CopyAs
        {
            get { return _copyAs; }
            set
            {
                if(_copyAs != value)
                {
                    _copyAs = value;
                    NotifyOfPropertyChange(() => CopyAs);
                    NotifyOfPropertyChange(() => CanSubmit);
                }
            }
        }


        private string _pricelistName;
        public string PricelistName
        {
            get { return _pricelistName; }
            set
            {
                if(_pricelistName != value)
                {
                    _pricelistName = value;
                    NotifyOfPropertyChange(() => PricelistName);
                    NotifyOfPropertyChange(() => CanSave);
                    NotifyOfPropertyChange(() => CanUpdate);
                }
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
        
        //public bool AddIsCalled => _addIsCalled;
        private bool _addIsCalled;
        public bool AddIsCalled
        {
            get { return _addIsCalled; }
            set
            {
                _addIsCalled = value;
                NotifyOfPropertyChange(() => AddIsCalled);
            }
        }

        private bool _editIsCalled;
        public bool EditIsCalled
        {
            get { return _editIsCalled; }
            set
            {
                _editIsCalled = value;
                NotifyOfPropertyChange(() => EditIsCalled);
            }
        }

        
        public async Task Add()
        {

            PricelistName = string.Empty;
            AddIsCalled = true;
            NotifyOfPropertyChange(() => PricelistNameEnabled);
            
            await LoadProducts();
            await LoadCustomers();

            TargetCustomers = new BindingList<CustomerDisplayModel>();
            NotifyOfPropertyChange(() => SubscribedCustomers);
            OnButtonSave(false);
        }

        public bool CanSave => !string.IsNullOrEmpty(PricelistName);

        public bool CanUpdate
        {
            get
            {
                var output = string.IsNullOrEmpty(PricelistName) == false && EditIsCalled && PricelistName != SelectedPricelist.Name;
                return output;
            }
        }

        public async Task Update()
        {

            var itemExist = allPriceLists.FirstOrDefault(c => c.Name.ToLower().Equals(PricelistName.ToLower()) && c.Id != SelectedPricelist.Id );
            if (itemExist != null)
            {
                MessageBox.Show("Pricelist name already exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (MessageBox.Show($"Are you sure?{Environment.NewLine}Update Pricelist from [{SelectedPricelist.Name}] to [{PricelistName}]", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var value = new PriceList
                    {
                        Name = PricelistName,
                        Id = SelectedPricelist.Id
                    };

                    await _priceListEndpoint.UpdatePricelistName(value);
                    SelectedPricelist.Name = PricelistName;
                    MessageBox.Show("Successfully Updated", "Confirmed", MessageBoxButton.OK);


                }
            }

        }

        private string _buttonSaveLabel = "SAVE";    
        public string ButtonSaveLabel
        {
            get { return _buttonSaveLabel; }
            set
            {
                _buttonSaveLabel = value; 
                NotifyOfPropertyChange(() => ButtonSaveLabel);
            }
        }

        private bool _isButtonSaveVisible;
        public bool IsButtonSaveVisible
        {
            get { return _isButtonSaveVisible; }
            set
            {
                _isButtonSaveVisible = value;
                NotifyOfPropertyChange(() => IsButtonSaveVisible);
            }
        }

        private bool _isCopyAs;
        public bool IsCopyAs
        {
            get { return _isCopyAs; }
            set
            {
                _isCopyAs = value;
                NotifyOfPropertyChange(() => IsCopyAs);
            }
        }

        public bool SaveEnabled => !IsCopyAs;
        

        public bool CanCancel => true;
        public bool CanSubmit => string.IsNullOrEmpty(CopyAs) == false;

        public void Cancel()
        {
            IsCopyAs = false;
            NotifyOfPropertyChange(() => SaveEnabled);
        }

        public async Task Submit()
        {
            if (MessageBox.Show("Are you sure?", $"Copy Pricelist [{PricelistName}] as[{CopyAs}] Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var value = new PriceList
                {
                    Name = CopyAs,
                    SourceId = SelectedPricelist.Id
                };

                value = await _priceListEndpoint.Save(value);
                if (value.Id.Equals(-1))
                {
                    MessageBox.Show("Pricelist name already exist", "Error", MessageBoxButton.OK ,MessageBoxImage.Error );
                }
                else
                {
                    //add to list 
                    value.Name = CopyAs;
                    allPriceLists.Add(value);
                    Pricelists.Add(_mapper.Map<PricelistDisplayModel>(value));
                    SelectedPricelist = _mapper.Map<PricelistDisplayModel>(value);
                    NotifyOfPropertyChange(() => Pricelists);
                    await OnRowSelect();
                    
                    //clear 

                    CopyAs = string.Empty;
                    Cancel();
                    
                    MessageBox.Show("Successfully copied", "Confirmed", MessageBoxButton.OK);

                }
                
            }

        }

        public async Task Save()
        {
            if (ButtonSaveLabel.Equals("SAVE"))
            {
                if (MessageBox.Show("Are you sure?", "Save PriceList Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var value = new PriceList
                    {
                        Name = PricelistName,
                        DiscountPolicy = true,
                        Subscribed = TargetCustomers.Count,
                        IsForSalesOrder = true
                    };

                    value = await _priceListEndpoint.Save(value);
                    if (value.Id.Equals(-1))
                    {
                        MessageBox.Show("Pricelist name already exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        AddIsCalled = false;

                        ////reset all after saved
                        Products = new ObservableCollection<PricelistProducts>();
                        SourceCustomers = new BindingList<CustomerDisplayModel>();
                        TargetCustomers = new BindingList<CustomerDisplayModel>();
                        await LoadPriceLists();

                        MessageBox.Show("Successfully saved?", $"PriceList ({PricelistName})", MessageBoxButton.OK);

                        PricelistName = string.Empty;
                        NotifyOfPropertyChange(() => CanSave);
                        NotifyOfPropertyChange(() => PricelistNameEnabled);
                    }

                }
            }
            else
            {
                IsCopyAs = true;
                NotifyOfPropertyChange(() => SaveEnabled);
            }

        }

        public void Close()
        {
            TryClose();
        }

        public async Task OnRowSelect()
        {
            await LoadProducts(SelectedPricelist.Id);
            await LoadCustomers();
            await LoadTargetCustomers(SelectedPricelist.Id);

            PricelistName = SelectedPricelist.Name;
            EditIsCalled = true;
            DiscountPolicy1 = !SelectedPricelist.DiscountPolicy;
            if (!DiscountPolicy1)
                DiscountPolicy2 = true;

            OnButtonSave(true);
            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => PricelistNameEnabled);
        }

        private void OnButtonSave(bool b)
        {
            IsButtonSaveVisible = true;
            ButtonSaveLabel = b ? "COPY AS" : "SAVE";
        }

        private List<TargetCustomer> _targetCustomersId = new List<TargetCustomer>();
        

        private async Task LoadTargetCustomers(long id = 0)
        {
            try
            {
                _targetCustomersId = await _priceListEndpoint.GetTargetCustomers(id);
                TargetCustomers = new BindingList<CustomerDisplayModel>();

                if (_targetCustomersId.Count > 0)
                {
                    
                    foreach (var customerId in _targetCustomersId)
                    {
                        var targetCustomer = SourceCustomers.FirstOrDefault(c => c.Id == customerId.CustomerId);
                        if (targetCustomer == null) continue;
                        TargetCustomers.Add(targetCustomer);
                        SourceCustomers.Remove(targetCustomer);
                    }

                    NotifyOfPropertyChange(() => TargetCustomers);
                    NotifyOfPropertyChange(() => SourcedCustomers);

                    NotifyOfPropertyChange(() => SourceCustomers);
                    NotifyOfPropertyChange(() => SubscribedCustomers);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        public void Handle(Product message)
        {
            SelectedProduct.SalePrice_CORON = message.SalePrice_CORON;
            SelectedProduct.SalePrice_LUBANG = message.SalePrice_LUBANG;
            SelectedProduct.SalePrice_SANILDEFONSO = message.SalePrice_SANILDEFONSO;

            SelectedProduct.DeductionFixPrice = message.DeductionFixPrice;
            SelectedProduct.DeductionOutright = message.DeductionOutright;
            SelectedProduct.Discount = message.Discount;
            SelectedProduct.DeductionPromoDiscount = message.DeductionPromoDiscount;
        }
    }
}
