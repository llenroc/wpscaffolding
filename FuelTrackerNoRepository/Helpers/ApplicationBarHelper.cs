using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace FuelTracker.Helpers
{
    public static class ApplicationBarHelper
    {
        public static void UpdateBindingOnFocussedControl()
        {
            object focusedElement = FocusManager.GetFocusedElement();
            if (focusedElement != null && focusedElement is TextBox)
            {
                var binding = (focusedElement as TextBox).GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                    binding.UpdateSource();
            }
        }
    }
}
