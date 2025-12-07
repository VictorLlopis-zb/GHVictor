using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AppWpf1.Servicios
{
    public static class BindingHelper
    {
        public static T ApplyBinding<T>(this T control, DependencyProperty dp, string propertyName) where T : FrameworkElement
        {
            var binding = new Binding(propertyName)
            {
                Mode = BindingMode.TwoWay,
                StringFormat = dp == DatePicker.SelectedDateProperty ? "dd/MM/yyyy" : null
            };
            control.SetBinding(dp, binding);
            return control;
        }

        public static PasswordBox ApplyPasswordBinding(this PasswordBox pwd, string propertyName, Grid grid)
        {
            pwd.PasswordChanged += (s, e) =>
            {
                var box = (PasswordBox)s;
                var data = grid.DataContext;
                if (data != null)
                {
                    var prop = data.GetType().GetProperty(propertyName);
                    prop?.SetValue(data, box.Password);
                }
            };
            return pwd;
        }

    }
}
