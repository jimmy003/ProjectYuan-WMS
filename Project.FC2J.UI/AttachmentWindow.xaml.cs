using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows;
using Project.FC2J.Models.Purchase;
using Project.FC2J.UI.Models;

namespace Project.FC2J.UI
{
    /// <summary>
    /// Interaction logic for AttachmentWindow.xaml
    /// </summary>
    public partial class AttachmentWindow : System.Windows.Window
    {

        public AttachmentWindow()
        {
            InitializeComponent();
            InvoiceDate.DisplayDate = DateTime.Now;
            InvoiceDate.Text = InvoiceDate.DisplayDate.ToShortDateString();
            Amount.Text = string.Empty;
            InvoiceNo.Text = string.Empty;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            foreach (var item in Items)
            {
                if (string.IsNullOrEmpty(item.InvoiceNo) == true)
                    SourceItems.Items.Add(new MyData {Id = item.Product.Id,
                                            Description = item.Description,
                                            Quantity = item.CartQuantity,
                                            Amount = item.SubTotal
                    });
            }

            SourceItems.Items.Refresh();
        }

        public long OrderHeaderId { get; set; }
        public string UserName { get; set; }
        public POPayment POPayment { get; set; }

        public BindingList<CartItemDisplayModel> Items { get; set; }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            POPayment = new POPayment
            {
                UserName = UserName,
                OrderHeaderId = OrderHeaderId,
                Amount = Convert.ToDecimal(Amount.Text),
                InvoiceDate = Convert.ToDateTime(InvoiceDate.SelectedDate),
                InvoiceNo = InvoiceNo.Text.Trim(),
                items = _myData
            };
            DialogResult = true;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var selected = SourceItems.SelectedItem ;
            if (selected != null)
            {
                TargetItems.Items.Add((MyData)selected);
                SourceItems.Items.Remove((MyData)selected);
                var amount = (decimal)0;
                decimal.TryParse(Amount.Text, out amount);
                Amount.Text = (amount + ((MyData)selected).Amount).ToString();

                _myData.Add((MyData) selected);
            }

            btnDialogOk.IsEnabled = TargetItems.Items.Count > 0 && string.IsNullOrWhiteSpace(InvoiceNo.Text) == false;
        }

        private List<MyData> _myData = new List<MyData>();

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var selected = TargetItems.SelectedItem;
            if (selected != null)
            {
                SourceItems.Items.Add(selected);
                TargetItems.Items.Remove(selected);
                var amount = (decimal)0;
                decimal.TryParse(Amount.Text, out amount);
                Amount.Text = (amount - ((MyData)selected).Amount).ToString();

                _myData.Remove((MyData)selected);
            }

            btnDialogOk.IsEnabled = TargetItems.Items.Count > 0 && string.IsNullOrWhiteSpace(InvoiceNo.Text) == false;
        }

        private void InvoiceNo_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            btnDialogOk.IsEnabled = TargetItems.Items.Count > 0 && string.IsNullOrWhiteSpace(InvoiceNo.Text) == false;
        }
    }
}
