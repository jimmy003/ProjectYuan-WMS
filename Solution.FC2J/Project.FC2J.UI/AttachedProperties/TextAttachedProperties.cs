﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Project.FC2J.UI.AttachedProperties
{
    /// <summary>
    /// Focuses (keyboard focus) this element on load
    /// </summary>
    public class IsFocusedProperty : BaseAttachedProperty<IsFocusedProperty, bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            
            if(!(sender is Control))
                return;

            var control = ((Control) sender);

            // Focus this control once loaded
            control.Loaded += (s, se) => control.Focus();
        }
    }

    /// <summary>
    /// Focuses (keyboard focus) this element if true
    /// </summary>
    public class FocusProperty : BaseAttachedProperty<FocusProperty, bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Control))
                return;

            var control = ((Control)sender);

            if ((bool)e.NewValue)
                // Focus this control
                control.Focus();
        }
    }

    /// <summary>
    /// Focuses (keyboard focus) and selects all text in this element if true
    /// </summary>
    public class FocusAndSelectProperty : BaseAttachedProperty<FocusAndSelectProperty, bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

            // If we don't have a control, return
            if (sender is TextBoxBase )
            {
                var control = ((TextBoxBase) sender);
                if ((bool)e.NewValue)
                {
                    // Focus this control
                    control.Focus();

                    // Select all text
                    control.SelectAll();
                }
            }
            if (sender is PasswordBox )
            {
                var password = ((PasswordBox) sender);

                if ((bool)e.NewValue)
                {
                    // Focus this control
                    password.Focus();

                    // Select all text
                    password.SelectAll();
                }
            }
        }
    }
}
