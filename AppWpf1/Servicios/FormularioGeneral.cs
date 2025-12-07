using AppWpf1.Atributos;
using AppWpf1.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AppWpf1.Servicios
{
    public class FormularioGeneral<T> : IFormularioGenerico<T>
    {
        private Grid? grid; // mantener referencia al grid generado

        public FrameworkElement GenerarFormulario()
        {
            grid = new Grid { Margin = new Thickness(10) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });

            int rowIndex = 0;

            foreach (var prop in typeof(T).GetProperties())
            {
                var atributo = prop.GetCustomAttribute<CampoFormularioAttribute>();
                if (atributo == null) continue;

                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var label = new TextBlock
                {
                    Text = atributo.Etiqueta,
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(label, rowIndex);
                Grid.SetColumn(label, 0);
                grid.Children.Add(label);

                FrameworkElement control = atributo.TipoControl switch
                {
                    "TextBox" => new TextBox { Name = $"Txt{prop.Name}", Width = atributo.Ancho },
                    "ComboBox" => new ComboBox
                    {
                        Name = $"Cmb{prop.Name}",
                        Width = atributo.Ancho,
                        ItemsSource = prop.PropertyType.IsEnum
                                ? Enum.GetValues(prop.PropertyType) : null
                    },
                    "DatePicker" => new DatePicker { Name = $"Dp{prop.Name}", Width = atributo.Ancho },
                    "PasswordBox" => new PasswordBox { Name = $"Pwd{prop.Name}", Width = atributo.Ancho },
                    _ => new TextBox { Name = $"Txt{prop.Name}", Width = atributo.Ancho }
                };

                // Binding automático (excepto PasswordBox)
                if (control is TextBox tb)
                    tb.SetBinding(TextBox.TextProperty, new Binding(prop.Name) { Mode = BindingMode.TwoWay });
                else if (control is ComboBox cb)
                    cb.SetBinding(ComboBox.SelectedItemProperty, new Binding(prop.Name) { Mode = BindingMode.TwoWay });
                else if (control is DatePicker dp)
                    dp.SetBinding(DatePicker.SelectedDateProperty, new Binding(prop.Name) { Mode = BindingMode.TwoWay });
                else if (control is PasswordBox pwd)
                    pwd.PasswordChanged += (s, e) =>
                    {
                        var box = (PasswordBox)s;
                        var data = grid.DataContext;
                        if (data != null)
                            prop.SetValue(data, box.Password);
                    };

                Grid.SetRow(control, rowIndex);
                Grid.SetColumn(control, 1);
                grid.Children.Add(control);

                rowIndex++;
            }

            return grid;
        }

        // Limpiar todos los controles
        public void LimpiarCampos()
        {
            foreach (var control in grid.Children.OfType<FrameworkElement>())
            {
                switch (control)
                {
                    case TextBox txt:
                        txt.Text = string.Empty;
                        break;
                    case ComboBox cmb:
                        cmb.SelectedIndex = -1;
                        break;
                    case DatePicker dp:
                        dp.SelectedDate = null;
                        break;
                    case PasswordBox pwd:
                        pwd.Password = string.Empty;
                        break;
                }
            }
        }

        // Cargar valores desde una instancia
        public void CargarCampos(T instancia)
        {
            var tipo = typeof(T);
            foreach (var prop in tipo.GetProperties())
            {
                var valor = prop.GetValue(instancia);
                var control = grid.Children.OfType<FrameworkElement>()
                                           .FirstOrDefault(c => c.Name.EndsWith(prop.Name));

                switch (control)
                {
                    case TextBox txt:
                        txt.Text = valor?.ToString() ?? string.Empty;
                        break;
                    case ComboBox cmb:
                        cmb.SelectedValue = valor;
                        break;
                    case DatePicker dp when valor is DateTime fecha:
                        dp.SelectedDate = fecha;
                        break;
                    case PasswordBox pwd:
                        pwd.Password = valor?.ToString() ?? string.Empty;
                        break;
                }
            }
        }

        // Obtener instancia desde los controles
        public T ObtenerInstancia()
        {
            var instancia = Activator.CreateInstance<T>();
            var tipo = typeof(T);

            foreach (var prop in tipo.GetProperties())
            {
                var control = grid.Children.OfType<FrameworkElement>()
                                           .FirstOrDefault(c => c.Name.EndsWith(prop.Name));

                if (control is TextBox txt)
                    prop.SetValue(instancia, txt.Text);
                else if (control is ComboBox cmb)
                    prop.SetValue(instancia, cmb.SelectedValue);
                else if (control is DatePicker dp)
                    prop.SetValue(instancia, dp.SelectedDate);
                else if (control is PasswordBox pwd)
                    prop.SetValue(instancia, pwd.Password);
            }

            return instancia;
        }
    }
}