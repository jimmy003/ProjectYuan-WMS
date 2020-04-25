using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Project.FC2J.UI
{
    /// <summary>
    /// Interaction logic for ReturnWindow.xaml
    /// </summary>
    public partial class ReturnWindow : Window
    {
        public float MaxQuantity { get; set; }
        public float EnteredQuantity { get; set; }
        public string Description { get; set; }

        public ReturnWindow()
        {
            InitializeComponent();
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }


        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Qty.Text = MaxQuantity.ToString("###0");
            LabelToReturn.Text = Description;
        }

        private void Qty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BtnDialogOk == null) return;
            var isEnabled = Qty.Text.Length > 0;
            if (isEnabled == false) return;
            float.TryParse(Qty.Text, out var quantity);
            if (quantity.Equals(0))
                isEnabled = false;
            else
                isEnabled = (MaxQuantity - quantity < 0) == false;
            BtnDialogOk.IsEnabled = isEnabled;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            float.TryParse(Qty.Text, out var quantity);
            EnteredQuantity = quantity;
            DialogResult = true;
        }

        private void PreviewTextInputHandler(object sender, TextCompositionEventArgs e)
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
    }
}
