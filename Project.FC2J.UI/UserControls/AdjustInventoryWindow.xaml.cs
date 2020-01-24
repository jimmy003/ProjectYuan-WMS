using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Caliburn.Micro;
using Project.FC2J.Models.Product;
using Project.FC2J.UI.Helpers.Products;
using Xceed.Wpf.Toolkit.Panels;

namespace Project.FC2J.UI.UserControls
{
    /// <summary>
    /// Interaction logic for AdjustInventoryWindow.xaml
    /// </summary>
    public partial class AdjustInventoryWindow : Window
    {
        private IProductEndpoint _productEndpoint;
        private string _supplier = "";
        private IEventAggregator _events;
        private string _requestBy;

        public AdjustInventoryWindow(IProductEndpoint productEndpoint, IEventAggregator events, string requestBy)
        {
            InitializeComponent();
            _productEndpoint = productEndpoint;
            _events = events;
            _requestBy = requestBy;
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            var products = await _productEndpoint.GetList(0);
            Products.ItemsSource = products;
        }

        private Product _selectedProduct;
        private void Products_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = Products.SelectedItem;
            if (selected != null)
            {
                _selectedProduct = (Product) selected;
                OnSetStockQuantity();
                SFAReferenceNo.Text = _selectedProduct.SFAReferenceNo;
                CanSave();
            }

        }

        private void OnSetStockQuantity()
        {
            if(_selectedProduct == null) return;
            switch (_supplier)
            {
                case "Coron":
                    StockQuantity.Text = _selectedProduct.Stock_CORON.ToString();
                    break;
                case "Lubang":
                    StockQuantity.Text = _selectedProduct.Stock_LUBANG.ToString();
                    break;
                case "San Ildefonso":
                    StockQuantity.Text = _selectedProduct.Stock_SANILDEFONSO.ToString();
                    break;
            }

        }


        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text

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

        private void CanSave()
        {
            if(Save == null) return;

            bool output = _selectedProduct !=null && 
                          Quantity.Text.Length > 0 && 
                          string.IsNullOrWhiteSpace(_supplier) == false;

            if (IsAction.IsChecked == false) //signifies minus the current stock
            {
                float quantity;
                float stock;

                float.TryParse(Quantity.Text, out quantity);
                float.TryParse(StockQuantity.Text, out stock);
                output = !(stock - quantity <= 0);
            }

            Save.IsEnabled = output;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if(_selectedProduct != null)
            {
                var quantity = (float)0;
                float.TryParse(Quantity.Text, out quantity);

                await _productEndpoint.UpdateProductInventory(new InventoryAdjustment
                {
                    ProductId = _selectedProduct.Id,
                    ProductName = _selectedProduct.Name,
                    Action =  !Convert.ToBoolean(IsAction.IsChecked),
                    OriginalQuantity = (float)_selectedProduct.StockQuantity,
                    Quantity = quantity,
                    Remarks = Remarks.Text.Trim(),
                    Supplier = _supplier,
                    RequestBy = _requestBy
                });

                _events.PublishOnUIThread(GetUpdatedProduct());

                MessageBox.Show("Inventory is successfully updated", "System Information", MessageBoxButton.OK);
                await OnClear();

            }
        }

        private Product GetUpdatedProduct()
        {
            float quantity;
            float stock;

            float.TryParse(Quantity.Text, out quantity);
            float.TryParse(StockQuantity.Text, out stock);

            var product = new Product
            {
                Id = _selectedProduct.Id,
                Stock_CORON = _selectedProduct.Stock_CORON,
                Stock_LUBANG = _selectedProduct.Stock_LUBANG,
                Stock_SANILDEFONSO = _selectedProduct.Stock_SANILDEFONSO
            };

            float newValue ;
            if (IsAction.IsChecked == false) //signifies minus the current stock
            {
                newValue = stock - quantity;
            }
            else
            {
                newValue = stock + quantity;
            }

            switch (_supplier)
            {
                case "Coron":
                    product.Stock_CORON = newValue;
                    break;
                case "Lubang":
                    product.Stock_LUBANG = newValue;
                    break;
                case "San Ildefonso":
                    product.Stock_SANILDEFONSO = newValue;
                    break;
            }

            return product;

        }

        private async Task OnClear()
        {
            StockQuantity.Text = string.Empty;
            Quantity.Text = string.Empty;
            Remarks.Text = string.Empty;
            await LoadProducts();
        }

        private void Quantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanSave();
        }


        private void Supplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = Supplier.SelectedItem;
            if (selected != null)
            {
                _supplier = selected.ToString().Replace("System.Windows.Controls.ComboBoxItem: ","");
                OnSetStockQuantity();
                CanSave();
            }

        }

        private void IsAction_Checked(object sender, RoutedEventArgs e)
        {
            CanSave();
        }
    }
}
