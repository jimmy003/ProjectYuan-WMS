using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
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
        private readonly List<MyData> _myData = new List<MyData>();
        public long OrderHeaderId { get; set; }
        public string UserName { get; set; }
        public POPayment POPayment { get; set; }

        public BindingList<CartItemDisplayModel> Items { get; set; }

        public AttachmentWindow()
        {
            InitializeComponent();
            InvoiceDate.DisplayDate = DateTime.Now;
            InvoiceDate.Text = InvoiceDate.DisplayDate.ToShortDateString();
            Amount.Text = string.Empty;
            InvoiceNo.Text = string.Empty;
            Qty.Text = string.Empty;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            foreach (var item in Items)
            {
                if (string.IsNullOrEmpty(item.InvoiceNo) == true && item.CartQuantity > 0)
                    SourceItems.Items.Add(new MyData {Id = item.Product.Id,
                                            Description = item.Description,
                                            Quantity = item.CartQuantity,
                                            Amount = item.SubTotal
                    });
            }

            SourceItems.Items.Refresh();
        }

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

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (_selected == null) return;
            var commitQty = Convert.ToSingle(Qty.Text);
            if(Math.Abs(commitQty) < 0.0001) return;

            var value = new MyData
            {
                Quantity = commitQty,
                Id = ((MyData)_selected).Id,
                Description = ((MyData)_selected).Description,
                Amount = ((MyData)_selected).Amount
            };

            UpdateSource(value);
            
            UpdateTarget(value);

            UpdateData(value);

            var unitPrice = value.Amount / (decimal)value.Quantity;
            decimal.TryParse(Amount.Text, out var amount);
            var computedAmout = unitPrice * (decimal)commitQty;
            Amount.Text = (amount + computedAmout).ToString("N4");
            Qty.Text = "0";

            btnDialogOk.IsEnabled = TargetItems.Items.Count > 0 && string.IsNullOrWhiteSpace(InvoiceNo.Text) == false;
        }

        private void UpdateSource(MyData value, bool isDeduct = true )
        {
            var found = false;
            if (isDeduct == false)
            {
                foreach (var item in SourceItems.Items)
                {
                    if (((MyData)item).Id != value.Id) continue;
                    ((MyData)item).Quantity = ((MyData)item).Quantity + value.Quantity;
                    found = true;
                    break;
                }

                if (found == false)
                    SourceItems.Items.Add(value);
            }
            else
            {
                if (Math.Abs(((MyData)_selected).Quantity - value.Quantity) < 0.01)
                {
                    SourceItems.Items.Remove((MyData)_selected);
                }
                else
                {

                    foreach (var item in SourceItems.Items)
                    {
                        if (((MyData)item).Id != value.Id) continue;
                        ((MyData)item).Quantity = ((MyData)item).Quantity - value.Quantity;
                        break;
                    }
                }
            }
            SourceItems.Items.Refresh();
        }

        private void UpdateTarget(MyData value)
        {
            var found = false;
            foreach (var item in TargetItems.Items)
            {
                if (((MyData) item).Id != value.Id) continue;
                ((MyData) item).Quantity = ((MyData) item).Quantity + value.Quantity;
                found = true;
                break;
            }

            if (found == false)
                TargetItems.Items.Add(value);
            TargetItems.Items.Refresh();
        }

        private void UpdateData(MyData value, bool isDeduct = true)
        {
            var found = false;

            if (isDeduct == false)
            {
                foreach (var item in _myData)
                {
                    if (item.Id != value.Id) continue;
                    _myData.Remove(item);
                    break;
                }
            }
            else
            {
                foreach (var item in _myData)
                {
                    if (item.Id != value.Id) continue;
                    item.Quantity += value.Quantity;
                    found = true;
                    break;
                }
                if (found == false)
                {
                    _myData.Add(new MyData
                    {
                        Quantity = value.Quantity,
                        Id = value.Id,
                        Description = value.Description
                    });
                }
            }
        }


        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var selected = TargetItems.SelectedItem;
            if (selected != null)
            {
                var value = new MyData
                {
                    Quantity = ((MyData)selected).Quantity,
                    Id = ((MyData)selected).Id,
                    Description = ((MyData)selected).Description,
                    Amount = ((MyData)selected).Amount
                };

                UpdateSource(value, false);

                TargetItems.Items.Remove(selected);

                decimal.TryParse(Amount.Text, out var amount);
                Amount.Text = (amount - value.Amount).ToString("N4");

                UpdateData(value, false);
            }

            btnDialogOk.IsEnabled = TargetItems.Items.Count > 0 && string.IsNullOrWhiteSpace(InvoiceNo.Text) == false;
        }

        private void InvoiceNo_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            btnDialogOk.IsEnabled = TargetItems.Items.Count > 0 && string.IsNullOrWhiteSpace(InvoiceNo.Text) == false;
        }

        private object _selected;
        private void SourceItems_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                Qty.Text = "0";
            }
            catch
            {
                // ignored
            }

            _selected = SourceItems.SelectedItem;
            if (_selected != null)
            {
                Qty.Text = ((MyData)_selected).Quantity.ToString();
            }
        }

        private void Qty_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (Add == null) return;
            var isEnabled = Qty.Text.Length > 0;
            if (isEnabled == false) return;
            float.TryParse(Qty.Text, out var quantity);
            float.TryParse(((MyData)_selected).Quantity.ToString(), out var allowed);
            isEnabled = (allowed - quantity < 0) == false;
            
            Add.IsEnabled = isEnabled;
        }
    }
}
