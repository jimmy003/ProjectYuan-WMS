using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Project.FC2J.Models.Purchase;

namespace Project.FC2J.UI.UserControls
{
    /// <summary>
    /// Interaction logic for PoListView.xaml
    /// </summary>
    public partial class PoListView : Window
    {
        public PoListView()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Purchases.ItemsSource = CollectionData;
        }

        public List<PoHeader> CollectionData { get; set; }
        public string PONo { get; private set; }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            var selected = Purchases.SelectedItem;
            if (selected != null)
            {
                PONo = ((PoHeader) selected).PONo;
                DialogResult = true;
            }
        }

        private void Purchases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Select.IsEnabled = false;
            var selected = Purchases.SelectedItem;
            if (selected != null)
            {
                PONo = ((PoHeader)selected).PONo;
                Select.IsEnabled = true;
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selected = Purchases.SelectedItem;
            if (selected != null)
            {
                PONo = ((PoHeader)selected).PONo;
                DialogResult = true;
            }
        }
    }
}
