using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Project.FC2J.Models.Product;
using Project.FC2J.UI.Helpers;
using Project.FC2J.UI.Models;

namespace Project.FC2J.UI.UserControls
{
    /// <summary>
    /// Interaction logic for PricelistDetails.xaml
    /// </summary>
    public partial class PricelistDetails : Window
    {
        private IPriceListEndpoint _priceListEndpoint;
        private PricelistProducts _payload;
        private long _pricelistTemplateId;

        public PricelistDetails(long pricelistTemplateId, PricelistProducts payload, IPriceListEndpoint priceListEndpoint, IEventAggregator events)
        {
            InitializeComponent();
            _priceListEndpoint = priceListEndpoint;
            _payload = payload;
            _pricelistTemplateId = pricelistTemplateId;
            _events = events;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            ProductName.Text = _payload.Name;
            SalePriceCoron.Text = _payload.SalePrice_CORON.ToString("N2");
            SalePriceLubang.Text = _payload.SalePrice_LUBANG.ToString("N2");
            SalePriceSanIldefonso.Text = _payload.SalePrice_SANILDEFONSO.ToString("N2");
            FixPrice.Text = _payload.DeductionFixPrice.ToString("N2");
            Outright.Text = _payload.DeductionOutright.ToString("N2");
            Discount.Text = _payload.Discount.ToString("N2");
            Promo.Text = _payload.DeductionPromoDiscount.ToString("N2");
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private IEventAggregator _events;

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        // Use the PreviewTextInputHandler to respond to key presses 
        private void PreviewTextInputHandler(Object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text)) e.CancelCommand();
            }
            else e.CancelCommand();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            decimal fixprice;
            double discount;
            decimal outright;
            decimal promo;

            decimal coron;
            decimal lubang;
            decimal sanildefonso;

            decimal.TryParse(FixPrice.Text, out fixprice);
            double.TryParse(Discount.Text, out discount);
            decimal.TryParse(Outright.Text, out outright);
            decimal.TryParse(Promo.Text, out promo);

            decimal.TryParse(SalePriceCoron.Text, out coron);
            decimal.TryParse(SalePriceLubang.Text, out lubang);
            decimal.TryParse(SalePriceSanIldefonso.Text, out sanildefonso);

            var product = new Product
            {
                Id = _payload.Id,
                DeductionFixPrice = fixprice,
                DeductionOutright = outright,
                Discount = discount,
                DeductionPromoDiscount = promo,
                SalePrice_CORON = coron,
                SalePrice_LUBANG = lubang,
                SalePrice_SANILDEFONSO = sanildefonso
            };

            _priceListEndpoint.UpdatePricelistTemplateDetails(_pricelistTemplateId, product);
            _events.PublishOnUIThread(product);
            MessageBox.Show("Product's Pricelist is successfully updated", "System Information", MessageBoxButton.OK);

            Close();
        }

        private void Quantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanSave();
        }

        private void CanSave()
        {
            if (Save == null) return;
            bool output = true;
            Save.IsEnabled = output;
        }
    }
}
