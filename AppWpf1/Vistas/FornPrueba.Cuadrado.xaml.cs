using AppWpf1.DTO;
using AppWpf1.Modelos;
using AppWpf1.Servicios;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;

namespace AppWpf1.Vistas
{
    public partial class FormPruebaCuadrado : Window
    {
        // Campo de clase
        private ObservableCollection<PersonaIdentidadFormulario> listaFormulario;
        private dynamic? crud;

        public FormPruebaCuadrado()
        {
            InitializeComponent();

            try
            {
                // 1. Generar lista intermedia desde la persistente
                //listaFormulario = ConversorPersonaIdentidad.GenerarListaFormulario(PersonaIdentidad.ListaPersistente);
                string resumen = string.Join(Environment.NewLine, listaFormulario.Select(p =>
    $"{p.Cedula} - {p.Nombre} {p.Apellido1} {p.Apellido2} - {p.FechaNacimiento:dd/MM/yyyy}"
));

                MessageBox.Show(resumen, "Diagnóstico listaFormulario");
                // 2. Mostrar en el DataGrid
                dgvRegistros.ItemsSource = listaFormulario;

                // 🔧 Arreglo: aplicar formato a columnas de fecha
                

                // 3. Generar formulario dinámico
                var generador = new FormularioGeneral<PersonaIdentidadFormulario>();
                var formulario = generador.GenerarFormulario();

                // Diagnóstico
                MessageBox.Show($"El formulario es de tipo: {formulario.GetType().FullName}", "Diagnóstico");

                // Vincular datos si corresponde
                if (listaFormulario.Count > 0)
                {
                    formulario.DataContext = listaFormulario[0];
                }

                var tipoCrud = typeof(FormCRUD<>)
                    .MakeGenericType(typeof(PersonaIdentidadFormulario));

                var entidad = new PersonaIdentidadFormulario();
                crud = (FormCRUD<PersonaIdentidadFormulario>)Activator.CreateInstance(
                    tipoCrud,
                    entidad,
                    dgvRegistros,
                    listaFormulario
                );
                MessageBox.Show($"Filas: {crud.Formulario.RowDefinitions.Count}, Controles: {crud.Formulario.Children.Count}");
                contenedorPrueba.Content = crud.Formulario;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear CRUD: {ex.Message}", "Diagnóstico");
            }
        }

        /// <summary>
        /// Recorre las columnas del DataGrid y aplica formato dd/MM/yyyy
        /// a las que tengan encabezado que empiece con "Fecha".
        /// </summary>
        private void dgvRegistros_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var propertyType = (e.PropertyDescriptor as System.ComponentModel.PropertyDescriptor)?.PropertyType;

            // Ocultar Password
            if (e.PropertyName == "Password")
            {
                e.Cancel = true;
                return;
            }

            // Formatear fechas
            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            {
                if (e.Column is DataGridTextColumn col && col.Binding is Binding binding)
                {
                    binding.StringFormat = "dd/MM/yyyy";
                }
            }
        }
    }
}