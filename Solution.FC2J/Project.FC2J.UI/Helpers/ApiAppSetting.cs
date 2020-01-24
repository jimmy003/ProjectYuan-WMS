using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.UI.Helpers
{
    public class ApiAppSetting : IApiAppSetting
    {

        private readonly string _authLogin;
        private readonly string _authRegister;
        private readonly string _customer;
        private readonly string _customerShipto;
        private readonly string _customerPayment;
        private readonly string _customerProduct;
        private readonly string _customerPricelist;
        private readonly string _product;
        private decimal _taxRate;
        private readonly string _sale;
        private readonly string _user;
        private readonly string _role;
        private readonly int _sleepInSeconds;
        private readonly string _startup;
        private readonly string _authHash;
        private readonly string _getuser;
        private readonly string _saleSONo;
        private readonly string _invoiceNo;
        private readonly string _branch;
        private readonly string _defaultPassword;
        private readonly string _pricelist;
        private readonly string _priceListCustomer;
        private readonly string _targetCustomer;
        private readonly string _pricePerKilo;
        private readonly string _deduction;

        public ApiAppSetting()
        {
            _authLogin = ConfigurationManager.AppSettings["authLogin"];
            _authRegister = ConfigurationManager.AppSettings["authRegister"];
            _customer = ConfigurationManager.AppSettings["customer"];

            _customerShipto = ConfigurationManager.AppSettings["customerShipTo"];
            _customerPayment = ConfigurationManager.AppSettings["customerPayment"];
            _customerProduct = ConfigurationManager.AppSettings["customerProduct"];
            _customerPricelist = ConfigurationManager.AppSettings["customerPricelist"];

            _product = ConfigurationManager.AppSettings["product"];
            _sale = ConfigurationManager.AppSettings["sale"];
            _user = ConfigurationManager.AppSettings["user"];
            _role = ConfigurationManager.AppSettings["role"];
            _startup = ConfigurationManager.AppSettings["startup"];
            _sleepInSeconds = Convert.ToInt32( ConfigurationManager.AppSettings["sleepInSeconds"]);
            _authHash = ConfigurationManager.AppSettings["authHash"];
            _getuser = ConfigurationManager.AppSettings["getuser"];
            _saleSONo = ConfigurationManager.AppSettings["saleSONo"];
            _invoiceNo = ConfigurationManager.AppSettings["invoiceNo"];
            _branch = ConfigurationManager.AppSettings["branch"];
            _defaultPassword = ConfigurationManager.AppSettings["defaultPassword"];
            _pricelist = ConfigurationManager.AppSettings["pricelist"];
            _priceListCustomer = ConfigurationManager.AppSettings["pricelistCustomer"];
            _targetCustomer = ConfigurationManager.AppSettings["targetCustomer"];
            _pricePerKilo = ConfigurationManager.AppSettings["pricePerKilo"];
            _deduction = ConfigurationManager.AppSettings["deduction"];
        }

        public string Startup
        {
            get
            {
                return _startup;
            }
        }

        public string AuthLogin
        {
            get
            {
                return _authLogin;
            }
        }
        public string AuthRegister => _authRegister;

        public string Customer => _customer;
        public string Product => _product;

        public decimal TaxRate
        {
            get
            {
                string rateText = ConfigurationManager.AppSettings["taxRate"];

                bool isValidTaxRate = Decimal.TryParse(rateText, out _taxRate);
                if(isValidTaxRate == false)
                {
                    throw new ConfigurationErrorsException("The tax rate is not set up properly");
                }

                return _taxRate;
            }
        }

        public string Sale => _sale;

        public string CustomerShipTo => _customerShipto;

        public string CustomerPayment => _customerPayment;

        public string CustomerProduct => _customerProduct;

        public string CustomerPricelist => _customerPricelist;

        public string User => _user;

        public int SleepInSeconds => _sleepInSeconds;

        public string Role => _role;

        public string AuthHash => _authHash;

        public string GetUser => _getuser;

        public string SaleSONo => _saleSONo;

        public string InvoiceNo => _invoiceNo;
        public string Branch => _branch;
        public string DefaultPassword => _defaultPassword;
        public string Pricelist => _pricelist;
        public string PricelistCustomer => _priceListCustomer;
        public string TargetCustomer => _targetCustomer;

        public string PricePerKilo => _pricePerKilo;
        public string Deduction => _deduction;
    }
}
