using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Project.FC2J.Models.Dtos;
using Project.FC2J.Models.Enums;
using Project.FC2J.UI.Helpers;

namespace Project.FC2J.UI.UserControls
{
    /// <summary>
    /// Interaction logic for SalesListFilterWindow.xaml
    /// </summary>
    public partial class SalesListFilterWindow : Window
    {
        private readonly IKeyValueEndpoint _keyValueEndpoint;
        public PeriodType Period { get; private set; }
        public int PeriodNumber { get; private set; }


        public SalesListFilterWindow(IKeyValueEndpoint keyValueEndpoint, int periodNumber, PeriodType periodType = PeriodType.Week)
        {
            _keyValueEndpoint = keyValueEndpoint;
            PeriodNumber = periodNumber;
            Period = periodType;
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Number.Text = PeriodNumber.ToString();
            PeriodTypeControl.Text = Period.ToString();
        }


        private void PeriodTypeControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = PeriodTypeControl.SelectedItem;
            if (selected == null) return;
            var period = selected.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", "");
            Period = period == "Week" ? PeriodType.Week : PeriodType.Month;
            CanSave();
            OnWarningCheck();
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

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

        private void Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            int quantity;
            int.TryParse(Number.Text, out quantity);

            PeriodNumber = quantity;
            CanSave();
            OnWarningCheck();
        }

        private void OnWarningCheck()
        {
            var visible = (PeriodNumber >= 4 && Period == PeriodType.Week)
                          || (PeriodNumber >= 1 && Period == PeriodType.Month);

            Warning.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }
        private void CanSave()
        {
            if (Save == null) return;
            var output = PeriodNumber > 0;
            Save.IsEnabled = output;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var values = new List<KeyValueDto>
            {
                new KeyValueDto
                {
                    Key = "PeriodNumber",
                    Value = PeriodNumber.ToString()
                },new KeyValueDto
                {
                    Key = "Period",
                    Value = Period.ToString()
                },
            };
            await _keyValueEndpoint.Save(values);

            DialogResult = true;
            Close();
        }
    }
}
