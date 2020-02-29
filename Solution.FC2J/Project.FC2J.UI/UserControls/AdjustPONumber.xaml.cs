using System;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Project.FC2J.UI.Helpers;

namespace Project.FC2J.UI.UserControls
{
    /// <summary>
    /// Interaction logic for AdjustPONumber.xaml
    /// </summary>
    public partial class AdjustPONumber : Window
    {

        private readonly string _poNo;
        private readonly string _customerId;
        private readonly string _salesId;
        private readonly ISaleEndpoint _saleEndpoint;

        public AdjustPONumber(string poNo, string customerId, string salesId, ISaleEndpoint saleEndpoint)
        {
            InitializeComponent();
            _poNo = poNo;
            _customerId = customerId;
            _salesId = salesId;
            _saleEndpoint = saleEndpoint;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            OldPONo.Text = $"Old PO Number: {_poNo}" ;
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private readonly string poNo;

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }


        // Use the PreviewTextInputHandler to respond to key presses 
        private void PreviewTextInputHandler(Object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);

        }

        private  void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text)) e.CancelCommand();
            }
            else e.CancelCommand();
        }
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var _newPoNO = PONo.Text.Trim();
            await _saleEndpoint.UpdatePONumber(_customerId, _poNo, _newPoNO, Convert.ToInt64(_salesId));

            MessageBox.Show("PONo is successfully updated", "System Information", MessageBoxButton.OK);
            NewPONo = _newPoNO;
            Close();
        }

        public string NewPONo { get; set; }

        private void PONo_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CanSave();
        }

        private void CanSave()
        {
            if (Save == null) return;

            bool output = PONo.Text.Length > 0 &&
                          PONo.Text != _poNo;
            Save.IsEnabled = output;
        }
    }
}
