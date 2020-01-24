using AutoMapper;
using Caliburn.Micro;
using Project.FC2J.Models;
using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Product;
using Project.FC2J.UI.Helpers;
using Project.FC2J.UI.Models;
using Project.FC2J.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Reporting.WinForms;
using Project.FC2J.Models.Purchase;
using Project.FC2J.Models.Sale;
using Project.FC2J.Models.User;
using Project.FC2J.UI.AttachedProperties;
using Project.FC2J.UI.Helpers.AppSetting;
using Project.FC2J.UI.Helpers.Excel;
using Project.FC2J.UI.Helpers.Products;
using Project.FC2J.UI.Helpers.Purchase;
using Project.FC2J.UI.Helpers.Reports;

namespace Project.FC2J.UI
{
    public class Bootstrapper : BootstrapperBase
    {

        private SimpleContainer _container = new SimpleContainer();
        public Bootstrapper()
        {
            Initialize();

            ConventionManager.AddElementConvention<PasswordBox>(
                SelectTextOnFocus.ActiveProperty,
                "Text",
                "ActivePropertyChanged");

            ConventionManager.AddElementConvention<PasswordBox>(
            PasswordBoxHelper.BoundPasswordProperty,
            "Password",
            "PasswordChanged");

            ConventionManager.AddElementConvention<FrameworkElement>(
                     UIElement.IsEnabledProperty,
                     "IsEnabled",
                     "IsEnabledChanged");

            var baseBindProperties = ViewModelBinder.BindProperties;
            
            ViewModelBinder.BindProperties =
                (frameWorkElements, viewModels) =>
                {
                    foreach (var frameworkElement in frameWorkElements)
                    {
                        var propertyName = frameworkElement.Name + "Enabled";
                        var property = viewModels
                             .GetPropertyCaseInsensitive(propertyName);
                        if (property != null)
                        {
                            var convention = ConventionManager
                                .GetElementConvention(typeof(FrameworkElement));
                            ConventionManager.SetBindingWithoutBindingOverwrite(
                                viewModels,
                                propertyName,
                                property,
                                frameworkElement,
                                convention,
                                convention.GetBindableProperty(frameworkElement));
                        }
                    }
                    return baseBindProperties(frameWorkElements, viewModels);
                };
        }

        private IMapper ConfigureAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PriceList, PricelistDisplayModel>();
                cfg.CreateMap<Product, ProductDisplayModel>();
                cfg.CreateMap<Customer, CustomerDisplayModel>();
                cfg.CreateMap<CartItem, CartItemDisplayModel>();
                cfg.CreateMap<User, UserDisplayModel>();
                cfg.CreateMap<OrderHeader, SalesDisplayModel>();
                cfg.CreateMap<SalesDisplayModel, SaleHeader>()
                    .ForMember(c => c.SaleDetails, option => option.Ignore());
                cfg.CreateMap<Product, PricelistProducts>();
                cfg.CreateMap<PricelistProducts, ProductDeduction>();
                cfg.CreateMap<Deduction, DeductionDisplayModel>();
                cfg.CreateMap<SaleDetail, SaleDetailDisplayModel>();
                cfg.CreateMap<SaleDetailDisplayModel, SaleDetail>();
                cfg.CreateMap<Invoice, InvoiceDisplayModel>();
                cfg.CreateMap<POPayment, POPaymentItemDisplayModel>();

            });

            var output = config.CreateMapper();

            return output;
        }

        protected override void Configure()
        {

            Parser.CreateTrigger = delegate (DependencyObject target, string triggerText)
            {
                System.Windows.Interactivity.EventTrigger eventTrigger;
                if (triggerText == null)
                {
                    ElementConvention elementConvention = ConventionManager.GetElementConvention(target.GetType());
                    return elementConvention.CreateTrigger();
                }
                string eventName = triggerText.Replace("[", String.Empty).Replace("]", String.Empty);
                if (eventName.StartsWith("Delayed", StringComparison.OrdinalIgnoreCase))
                {
                    eventName = eventName.Replace("DelayedEvent", String.Empty).Trim();
                    eventTrigger = new DelayedEventTrigger();
                }
                else
                {
                    eventName = eventName.Replace("Event", String.Empty).Trim();
                    eventTrigger = new System.Windows.Interactivity.EventTrigger();
                }

                eventTrigger.EventName = eventName;
                return eventTrigger;
            };

            ConventionManager.ApplyUpdateSourceTrigger = (bindableProperty, element, binding, propertyInfo) =>
            {
                if (element is TextBox)
                {
                    return;
                }
                
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            };

            _container.Instance(ConfigureAutoMapper());

            _container.Instance(_container)
                .PerRequest<IProductEndpoint, ProductEndpoint>()
                .PerRequest<ISaleEndpoint, SaleEndpoint>()
                .PerRequest<ICustomerEndpoint, CustomerEndpoint>()
                .PerRequest<IProfileData, ProfileData>()
                .PerRequest<IPriceListEndpoint, PriceListEndpoint>()
                .PerRequest<IUserEndpoint, UserEndpoint>()
                .PerRequest<IDeductionEndpoint, DeductionEndpoint>()
                .PerRequest<IPurchaseEndpoint, PurchaseEndpoint>();

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<IApiAppSetting, ApiAppSetting>()
                .Singleton<ILoggedInUser, LoggedInUser>()
                .Singleton<ISaleData, SaleData>()
                .Singleton<IReportEndpoint, ReportEndpoint>()
                .Singleton<IExcelHelper, ExcelHelper>()
                .Singleton<IAPIHelper, APIHelper>();

            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));

        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }
        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
