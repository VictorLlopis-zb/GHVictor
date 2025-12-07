using AppWpf1.Modelos;
using AppWpf1.Servicios;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel; // <-- necesario

namespace AppWpf1.Vistas
{
    public partial class FLogFalso : Window
    {
        public FLogFalso()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var ruta = Path.Combine(
                    Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Datos")),
                    "identidades.json"
                );
                var json = File.Exists(ruta) ? File.ReadAllText(ruta) : "[]";

                ObservableCollection<PersonaIdentidad> listaObservable;

                try
                {
                    var metodoGenerico = typeof(SerializadorOptimizado)
                        .GetMethods()
                        .First(m => m.Name == "Deserializar" && m.IsGenericMethod)
                        .MakeGenericMethod(typeof(PersonaIdentidad));

                    var resultado = metodoGenerico.Invoke(null, new object[] { json });
                    listaObservable = new ObservableCollection<PersonaIdentidad>(
                        (IEnumerable<PersonaIdentidad>)resultado!
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error en FLogFalso: {ex.InnerException?.Message ?? ex.Message}");
                    listaObservable = new ObservableCollection<PersonaIdentidad>();
                }

                // 📢 Mostrar contenido rápido
                var sb = new StringBuilder();
                foreach (var p in listaObservable)
                    sb.AppendLine($"Cédula: {p.Cedula} | Nombre: {p.NombreCompleto}");

                MessageBox.Show(sb.Length > 0 ? sb.ToString() : "No se cargaron identidades.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error general en FLogFalso: {ex.Message}");
            }
        }
    }
}