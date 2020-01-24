using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Project.FC2J.UI.Helpers
{
    public static class SelectTextOnFocus 
    {

        public static readonly DependencyProperty ActiveProperty = DependencyProperty.RegisterAttached(
            "Active",
            typeof(bool),
            typeof(SelectTextOnFocus),
            new PropertyMetadata(false, ActivePropertyChanged));

        private static void ActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                TextBox textBox = d as TextBox;
                if ((e.NewValue as bool?).GetValueOrDefault(false))
                {
                    textBox.GotKeyboardFocus += OnKeyboardFocusSelectText;
                    textBox.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
                }
                else
                {
                    textBox.GotKeyboardFocus -= OnKeyboardFocusSelectText;
                    textBox.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                }
            }
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dependencyObject = GetParentFromVisualTree(e.OriginalSource);

            if (dependencyObject == null)
            {
                return;
            }

            var textBox = (TextBox)dependencyObject;
            if (!textBox.IsKeyboardFocusWithin)
            {
                textBox.Focus();
                e.Handled = true;
            }
        }

        private static DependencyObject GetParentFromVisualTree(object source)
        {
            DependencyObject parent = source as UIElement;
            while (parent != null && !(parent is TextBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent;
        }

        private static void OnKeyboardFocusSelectText(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }

        [AttachedPropertyBrowsableForChildrenAttribute(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetActive(DependencyObject @object)
        {
            return (bool)@object.GetValue(ActiveProperty);
        }

        public static void SetActive(DependencyObject @object, bool value)
        {
            @object.SetValue(ActiveProperty, value);
        }
        //public static bool GetEnable(FrameworkElement frameworkElement)
        //{
        //    return (bool)frameworkElement.GetValue(EnableProperty);
        //}

        //public static void SetEnable(FrameworkElement frameworkElement, bool value)
        //{
        //    frameworkElement.SetValue(EnableProperty, value);
        //}

        //public static readonly DependencyProperty EnableProperty =
        //    DependencyProperty.RegisterAttached("Enable",
        //        typeof(bool), typeof(SelectAllFocusBehavior),
        //        new FrameworkPropertyMetadata(false, OnEnableChanged));

        //private static void OnEnableChanged
        //    (DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var frameworkElement = d as FrameworkElement;
        //    if (frameworkElement == null) return;

        //    if (e.NewValue is bool == false) return;

        //    if ((bool)e.NewValue)
        //    {
        //        frameworkElement.GotFocus += SelectAll;
        //        frameworkElement.PreviewMouseDown += IgnoreMouseButton;
        //    }
        //    else
        //    {
        //        frameworkElement.GotFocus -= SelectAll;
        //        frameworkElement.PreviewMouseDown -= IgnoreMouseButton;
        //    }
        //}

        //private static void SelectAll(object sender, RoutedEventArgs e)
        //{
        //    var frameworkElement = e.OriginalSource as FrameworkElement;
        //    if (frameworkElement is TextBox)
        //        ((TextBoxBase)frameworkElement).SelectAll();
        //    else if (frameworkElement is PasswordBox)
        //        ((PasswordBox)frameworkElement).SelectAll();
        //}

        //private static void IgnoreMouseButton
        //    (object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    var frameworkElement = sender as FrameworkElement;
        //    if (frameworkElement == null || frameworkElement.IsKeyboardFocusWithin) return;
        //    e.Handled = true;
        //    frameworkElement.Focus();
        //}
    }
}
