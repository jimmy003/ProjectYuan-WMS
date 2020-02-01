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

namespace Project.FC2J.UI.UserControls
{
    /// <summary>
    /// Interaction logic for CartItemWindow.xaml
    /// </summary>
    public partial class CartItemWindow : Window
    {
        private float _quantity;
        private string _item;

        

        public CartItemWindow(string item, float quantity)
        {
            InitializeComponent();
            _item = item;
            _quantity = quantity;
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            OriginalValue.Text = $"{_item} : {_quantity}";
            ProductDescription.Text = _item;
            Quantity.Text = _quantity.ToString("N2");
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

        private void Quantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanSave();
        }

        private void CanSave()
        {
            if (Save == null) return;
            _quantity = 0;
            var output = Quantity.Text.Length > 0 ;
            float quantity;

            float.TryParse(Quantity.Text, out quantity);
            output = quantity > 0;
            _quantity = quantity;

            Save.IsEnabled = output;
        }

        public float QuantityUpdated => _quantity;

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }


    }
}
