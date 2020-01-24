namespace Project.FC2J.UI.Helpers
{
    public interface IApiAppSetting
    {
        string AuthLogin { get; }
        string AuthRegister { get; }
        string Customer { get; }
        string CustomerShipTo { get; }
        string CustomerPayment { get; }
        string CustomerProduct { get; }
        string CustomerPricelist { get; }
        string Product { get; }
        decimal TaxRate { get; }
        string Sale { get; }
        string User { get; }
        string Role { get; }
        string Startup { get; }
        int SleepInSeconds { get; }

        string AuthHash { get; }
        string GetUser
        {
            get;
        }

        string SaleSONo { get; }

        string InvoiceNo { get;  }

        string Branch { get; }

        string DefaultPassword { get; }
        string Pricelist { get; }
        string PricelistCustomer { get; }

        string TargetCustomer { get; }
        string PricePerKilo { get;  }

        string Deduction { get; }
    }
}