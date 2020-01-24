using Caliburn.Micro;
using Project.FC2J.Models;
using Project.FC2J.UI.EventModels;
using Project.FC2J.UI.Helpers;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using Project.FC2J.Models.User;
using Project.FC2J.UI.Helpers.AppSetting;
using Project.FC2J.UI.Helpers.Excel;
using Project.FC2J.UI.Helpers.Products;
using Project.FC2J.UI.Helpers.Reports;
using Project.FC2J.UI.Reports;
using Project.FC2J.UI.UserControls;

namespace Project.FC2J.UI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>, IHandle<ViewModelActions>
    {
        private readonly IEventAggregator _events;
        private readonly ILoggedInUser _user;
        private readonly IAPIHelper _apiHelper;
        private readonly IApiAppSetting _apiAppSetting;
        private readonly ILoggedInUser _loggedInUser;
        private readonly ISaleData _saleData;
        private IReportEndpoint _reportEndpoint;
        private IExcelHelper _excelHelper;
        private IProductEndpoint _productEndpoint;

        public ShellViewModel(IEventAggregator events, ILoggedInUser user, IAPIHelper apiHelper,
            IApiAppSetting apiAppSetting, ILoggedInUser loggedInUser, ISaleData saleData,
            IReportEndpoint reportEndpoint, IExcelHelper excelHelper, IProductEndpoint productEndpoint)
        {
            _events = events;
            _user = user;
            _saleData = saleData;
            _excelHelper = excelHelper;
            _loggedInUser = loggedInUser;
            _apiHelper = apiHelper;
            _apiAppSetting = apiAppSetting;
            _reportEndpoint = reportEndpoint;
            _events.Subscribe(this);
            _productEndpoint = productEndpoint;
            ActivateItem(IoC.Get<LoginViewModel>());

            IsProfileVisible = false;
            NotifyOfPropertyChange(() => IsLogoutVisible);

        }

        public sealed override void ActivateItem(object item)
        {
            base.ActivateItem(item);
        }

        public void ExitApplication()
        {
            if (MessageBox.Show("Are you sure?", "Exit Application Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TryClose();
            }
        }

        public void LogOut()
        {
            _user.ResetUserModel();
            _apiHelper.LogOffUser();
            ActivateItem(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLogoutVisible);

            ResetModules();
            NotifyOfPropertyChange(() => IsVisible);

            NotifyOfPropertyChange(() => IsContentManagementVisible);
            NotifyOfPropertyChange(() => IsSalesVisible);
            NotifyOfPropertyChange(() => IsCollectionsVisible);
            NotifyOfPropertyChange(() => IsAccountVisible);
            NotifyOfPropertyChange(() => IsPurchasesVisible);
            NotifyOfPropertyChange(() => IsReportsVisible);

            IsProfileVisible = false;
        }
      
        public void Handle(ViewModelActions message)
        {

            switch (message)
            {

                case ViewModelActions.CUSTOMER:
                    ActivateItem(IoC.Get<CustomerViewModel>());
                    break;

                case ViewModelActions.PRODUCT:
                    ActivateItem(IoC.Get<ProductViewModel>());
                    break;

                case ViewModelActions.USER:
                    ActivateItem(IoC.Get<UserViewModel>());
                    break;


                case ViewModelActions.SALESLIST:
                    ActivateItem(IoC.Get<SalesListViewModel>());
                    break;

                case ViewModelActions.PRICELIST:
                    ActivateItem(IoC.Get<PriceListViewModel>());
                    break;

                case ViewModelActions.DEDUCTIONS:
                    ActivateItem(IoC.Get<DeductionsViewModel>());
                    break;

                case ViewModelActions.PRINTSO:
                    ActivateItem(IoC.Get<PrintSOViewModel>());
                    break;


                case ViewModelActions.RECEIVER:
                    ActivateItem(IoC.Get<ReceiverViewModel>());
                    break;
                case ViewModelActions.MONITORING:
                    ActivateItem(IoC.Get<CollectionViewModel>());
                    break;

                case ViewModelActions.PURCHASEORDER:
                    ActivateItem(IoC.Get<PurchaseOrderViewModel>());
                    break;
                case ViewModelActions.PRICELIST_PO:
                    ActivateItem(IoC.Get<PriceListPOViewModel>());
                    break;

                case ViewModelActions.SALESREPORT:
                    OnSalesReport();
                    break;

                case ViewModelActions.PROFILE:
                    ActivateItem(IoC.Get<ProfileViewModel>());
                    break;

                case ViewModelActions.REPORTS_INVENTORY:
                    var inputDialog = new ReportViewerScreen(_reportEndpoint, _excelHelper);
                    inputDialog.ShowDialog();
                    break;

                case ViewModelActions.ADJUSTINVENTORY:
                    var adjustInventoryWindow = new AdjustInventoryWindow(_productEndpoint, _events, _user.User?.UserName);
                    adjustInventoryWindow.ShowDialog();
                    break;

                case ViewModelActions.ADJUSTINVENTORYAPPROVAL:
                    ActivateItem(IoC.Get<AdminViewModel>());
                    break;


            }
        }

        private void OnSalesReport()
        {
            var inputDialog = new SalesReport(_reportEndpoint, _excelHelper, _apiAppSetting.ReportTIN);
            inputDialog.ShowDialog();
        }


        #region Properties

        private bool _isProfileVisible;
        public bool IsProfileVisible
        {
            get { return _isProfileVisible; }
            set
            {
                _isProfileVisible = value;
                NotifyOfPropertyChange(() => IsProfileVisible);
            }
        }
        public bool IsLogoutVisible => string.IsNullOrWhiteSpace(_user.Token) == false;

        public bool IsFirstLogon => _loggedInUser.User?.PasswordX == _apiAppSetting.DefaultPassword;

        #endregion

        private bool GetHeaderMenu(string header)
        {
            var result = false;
            if (IsVisible != null && IsVisible.Count > 0)
            {
                if (header == "COLLECTIONS")
                {
                    result = IsVisible[ViewModelActions.MONITORING.ToString()] || IsVisible[ViewModelActions.RECEIVER.ToString()];
                }
                if (header == "SALES")
                {
                    result = IsVisible[ViewModelActions.SALESLIST.ToString()] || IsVisible[ViewModelActions.PRICELIST.ToString()] || IsVisible[ViewModelActions.DEDUCTIONS.ToString()] || IsVisible[ViewModelActions.PRINTSO.ToString()];
                }
                if (header == "CONTENTMANAGEMENT")
                {
                    result = IsVisible[ViewModelActions.CUSTOMER.ToString()] || IsVisible[ViewModelActions.PRODUCT.ToString()] || IsVisible[ViewModelActions.USER.ToString()] || IsVisible[ViewModelActions.ADJUSTINVENTORY.ToString()] || IsVisible[ViewModelActions.ADJUSTINVENTORYAPPROVAL.ToString()];
                }

                if (header == "PURCHASES")
                {
                    result = IsVisible[ViewModelActions.PURCHASEORDER.ToString()] || IsVisible[ViewModelActions.PRICELIST_PO.ToString()];
                }

                if (header == "REPORTS")
                {
                    result = IsVisible[ViewModelActions.REPORTS_INVENTORY.ToString()] ;
                }


            }

            return result;
        }
        public IDictionary<string, bool> IsVisible => _modules;

        private Dictionary<string, bool> _modules;

        private void LoadUserModules()
        {
            _modules = _user.User?.UserModules.ToDictionary(module => module.ModuleName, module => module.Access);
        }

        private void ResetModules()
        {
            var modules = new Dictionary<string, bool>();
            foreach (var item in _modules)
            {
                modules.Add(item.Key, false);
            }
            _modules = modules;
            IsProfileVisible = false;
        }

        private void SetModules()
        {
            LoadUserModules();
            IsProfileVisible = true;
        }
        public bool IsContentManagementVisible => GetHeaderMenu("CONTENTMANAGEMENT");
        public bool IsSalesVisible => GetHeaderMenu("SALES");
        public bool IsCollectionsVisible => GetHeaderMenu("COLLECTIONS");
        public bool IsPurchasesVisible => GetHeaderMenu("PURCHASES");
        public bool IsReportsVisible => GetHeaderMenu("REPORTS");
        public bool IsAccountVisible => IsProfileVisible || IsLogoutVisible;

        public void Handle(LogOnEvent message)
        {

            DeactivateItem(ActiveItem,true);
            IsProfileVisible = true;

            //to change password
            if (IsFirstLogon) 
            {
                IsProfileVisible = false;
                ActivateItem(IoC.Get<ProfileViewModel>());
            }
            else 
            {
                IsProfileVisible = true;
                LoadUserModules();
                NotifyOfPropertyChange(() => IsVisible);
            }

            NotifyOfPropertyChange(() => IsContentManagementVisible);
            NotifyOfPropertyChange(() => IsSalesVisible);
            NotifyOfPropertyChange(() => IsCollectionsVisible);
            NotifyOfPropertyChange(() => IsAccountVisible);
            NotifyOfPropertyChange(() => IsPurchasesVisible); 
            NotifyOfPropertyChange(() => IsReportsVisible); 
            NotifyOfPropertyChange(() => IsLogoutVisible);

        }


        protected override void ChangeActiveItem(object newItem, bool closePrevious)
        {
            base.ChangeActiveItem(newItem, closePrevious);

            if(IsFirstLogon)
            {
                return;
            }

            SetModules();

            if (ActiveItem == null)
            {
                NotifyOfPropertyChange(() => IsVisible);
                NotifyOfPropertyChange(() => IsLogoutVisible);
                return;
            }

            var t = ActiveItem.GetType();

            switch (t.Name)
            {
                case "CustomerViewModel":
                    _modules[ViewModelActions.CUSTOMER.ToString()] = false;
                    break;

                case "ProductViewModel":
                    _modules[ViewModelActions.PRODUCT.ToString()] = false;
                    break;

                case "UserViewModel":
                    _modules[ViewModelActions.USER.ToString()] = false;
                    break;

                
                case "SalesListViewModel":
                    _modules[ViewModelActions.SALESLIST.ToString()] = false;
                    break;

                case "PriceListViewModel":
                    _modules[ViewModelActions.PRICELIST.ToString()] = false;
                    break;

                case "DeductionsViewModel":
                    _modules[ViewModelActions.DEDUCTIONS.ToString()] = false;
                    break;

                case "PrintSOViewModel":
                    _modules[ViewModelActions.PRINTSO.ToString()] = false;
                    break;

                case "AdminViewModel":
                    _modules[ViewModelActions.ADJUSTINVENTORYAPPROVAL.ToString()] = false;
                    break;


                case "ProfileViewModel":
                    IsProfileVisible = false;
                    break;
                default:
                    break;
            }

            NotifyOfPropertyChange(() => IsVisible);
            NotifyOfPropertyChange(() => IsLogoutVisible);

        }



        
    }
}
