using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using AutoMapper;
using Project.FC2J.Models.Product;
using Project.FC2J.Models.User;
using Project.FC2J.UI.Helpers.Products;
using Project.FC2J.UI.Models;
using Screen = Caliburn.Micro.Screen;

namespace Project.FC2J.UI.ViewModels
{
    public class AdminViewModel : Screen
    {
        private readonly IProductEndpoint _productEndpoint;
        private bool _isAdmin;
        private readonly ILoggedInUser _user;

        public AdminViewModel(IProductEndpoint productEndpoint, ILoggedInUser user)
        {
            IsLoadingVisible = true;
            _productEndpoint = productEndpoint;
            _user = user;
            _isAdmin = _user.User.UserName.ToLower().Equals("admin");
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
            List<InventoryAdjustment> inventories;

            SearchInput = value;

            if (string.IsNullOrWhiteSpace(SearchInput))
            {
                if (Inventories.Count != allRecords.Count)
                    inventories = allRecords;
                else
                {
                    return;
                }
            }
            else
            {
                inventories = allRecords.Where(c => c.ProductName.ToLower().Contains(SearchInput.ToLower())).ToList();
            }
            Inventories = new ObservableCollection<InventoryAdjustment>(inventories);
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
        public void Close()
        {
            TryClose();
        }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadInventories();
        }

        private List<InventoryAdjustment> allRecords = new List<InventoryAdjustment>();
        private async Task LoadInventories()
        {
            var list = await _productEndpoint.GetForApprovalInventoryAdjustment();
            allRecords = list.ToList();
            Inventories = new ObservableCollection<InventoryAdjustment>(allRecords);
            IsLoadingVisible = false;
        }

        private ObservableCollection<InventoryAdjustment> _inventories;
        public ObservableCollection<InventoryAdjustment> Inventories
        {
            get { return _inventories; }
            set
            {
                _inventories = value;
                NotifyOfPropertyChange(() => Inventories);
                NotifyOfPropertyChange(() => CollectionView);
            }
        }
        private CollectionView _collectionView = null;
        public CollectionView CollectionView
        {
            get
            {
                _collectionView = (CollectionView)CollectionViewSource.GetDefaultView(Inventories);
                _collectionView?.SortDescriptions.Add(new SortDescription("RequestBy", ListSortDirection.Ascending));
                return _collectionView;
            }
        }

        public string RequestBy => SelectedInventory?.RequestBy;
        public string Supplier => SelectedInventory?.Supplier;
        public string ProductName => SelectedInventory?.ProductName;
        public string Quantity => SelectedInventory?.Quantity.ToString("N2");

        public string Action
        {
            get
            {
                if (SelectedInventory == null)
                {
                    return string.Empty;
                }

                return SelectedInventory.Action ? "Decrement Stocks" : "Increment Stocks";
            }
        }

        private InventoryAdjustment _selectedInventory;
        public InventoryAdjustment SelectedInventory
        {
            get { return _selectedInventory; }
            set
            {
                if (_selectedInventory != value)
                {
                    _selectedInventory = value;

                    NotifyOfPropertyChange(() => SelectedInventory);
                    NotifyOfPropertyChange(() => RequestBy);
                    NotifyOfPropertyChange(() => Supplier);
                    NotifyOfPropertyChange(() => ProductName);
                    NotifyOfPropertyChange(() => Quantity);
                    NotifyOfPropertyChange(() => Action);

                    NotifyOfPropertyChange(() => CanClear);
                    NotifyOfPropertyChange(() => CanApprove);
                    NotifyOfPropertyChange(() => CanDisapprove);

                }

            }
        }

        public bool CanClear => SelectedInventory != null;

        public bool CanApprove => SelectedInventory != null && _isAdmin;

        public bool CanDisapprove => SelectedInventory != null && _isAdmin;

        public async Task Clear()
        {
            SelectedInventory = null;
        }

        public async Task Approve()
        {
            await ProcessInventoryAdjustment("Approve", true);
        }

        
        public async Task Disapprove()
        {
            await ProcessInventoryAdjustment("Disapprove", false);
        }

        private async Task ProcessInventoryAdjustment(string actionTodo, bool action)
        {
            if (MessageBox.Show("Are you sure?", $"{actionTodo} Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var inventory = new InventoryAdjustment
                {
                    Id = SelectedInventory.Id,
                    Action = SelectedInventory.Action,
                    Supplier = SelectedInventory.Supplier,
                    Quantity = SelectedInventory.Quantity,
                    ProductId = SelectedInventory.ProductId,
                    IsApproved = action,
                    ApprovedBy = _user.User.UserName
                };

                await _productEndpoint.ApproveInventoryAdjustment(inventory);

                MessageBox.Show($"Record is Successfully {actionTodo}d.", $"{actionTodo}d Confirmed!", MessageBoxButton.OK);
                SelectedInventory = null;
                await LoadInventories();
            }
        }

    }
}
