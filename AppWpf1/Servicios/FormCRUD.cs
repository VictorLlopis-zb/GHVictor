using AppWpf1.Interfaces;
using AppWpf1.Modelos;
using AppWpf1.Servicios;
using AppWpf1.Vistas;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppWpf1.Servicios
{
    public class FormCRUD<TEntidad>
    where TEntidad : class, IPersistente<TEntidad>
    {
        private readonly TEntidad gestor;
        private readonly DataGrid dgvRegistros;
        private readonly IList<TEntidad> lista;
        private string operacionActual = null;
        private bool ventanaEntidadCorrecta = true;

        // Propiedad pública para exponer el Grid
        public Grid Formulario { get; }

        public FormCRUD(TEntidad gestor,
                    DataGrid dgvRegistros,
                    IList<TEntidad> lista)
        {
            this.gestor = gestor ?? throw new ArgumentNullException(nameof(gestor));
            this.dgvRegistros = dgvRegistros ?? throw new ArgumentNullException(nameof(dgvRegistros));
            this.lista = lista ?? throw new ArgumentNullException(nameof(lista));

            dgvRegistros.ItemsSource = lista;

            // Aquí usas tu generador genérico
            var generador = new FormularioGeneral<TEntidad>();
            Formulario = (Grid)generador.GenerarFormulario();
        }

        private FrameworkElement? ObtenerControl(string nombrePropiedad)
        {
            // El formulario incrustado es un Grid con los controles generados
            var grid = Formulario;

            // Convención: prefijo según tipo de control (Txt, Cmb, Dp, Pwd)
            // Aquí buscamos cualquier control cuyo Name termine con el nombre de la propiedad
            return grid.Children
                       .OfType<FrameworkElement>()
                       .FirstOrDefault(c => c.Name.EndsWith(nombrePropiedad, StringComparison.OrdinalIgnoreCase));
        }

        private bool ValidarSeleccion()
        {
            // 🔑 Diagnóstico: imprimir si hay item seleccionado            
            if (dgvRegistros.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Debe seleccionar un elemento antes de continuar.");
                return false;
            }
            return true;
        }
        public void Cancelar()
        {
            operacionActual = null;
            Formulario.DataContext = null;
        }
        public void Crear()
        {
            operacionActual = "crear";
            if (!gestor.EsPIF()) MessageBox.Show("No implementado");
            else
            {
                // 1) Limpiar contexto previo
                Formulario.DataContext = null;

            }
        }
        

        public void Modificar()
        {
            operacionActual = "modificar";
            // 🔑 Vincular al formulario incrustado
            // ya en Formpersistente se validó la selección
            var seleccionado = (TEntidad)dgvRegistros.SelectedItem;
            Formulario.DataContext = seleccionado;
            
        }

        public void Eliminar()
        {
            operacionActual = "eliminar";

            // 🔑 Limpiar el formulario incrustado
            Formulario.DataContext = null;

            // 🔑 Cargar solo la cédula del seleccionado
            var txtCedula = ObtenerControl("Cedula") as TextBox;
            var propCedula = typeof(TEntidad).GetProperty("Cedula");
            var seleccionado = (TEntidad)dgvRegistros.SelectedItem;
            var valorCedula = propCedula.GetValue(seleccionado)?.ToString();
            txtCedula.Text = valorCedula;


        }

        public void Aceptar(TEntidad entidad)
        {
            gestor.Aceptar(operacionActual!, entidad);
            dgvRegistros.Items.Refresh();
            operacionActual = null;
        }

        public void Salir() => gestor.Salir();

        private void CambiarVentanaAClase()
        {
            if (ventanaEntidadCorrecta)
            {
                dgvRegistros.ItemsSource = PersonaIdentidad.ListaPersistente;
            }
            else
            {
                dgvRegistros.ItemsSource = lista;
            }

            ventanaEntidadCorrecta = !ventanaEntidadCorrecta;
        }

        private bool ValidarVentanaYSeleccion(bool esperado)
        {
            if (ventanaEntidadCorrecta != esperado)
            {
                CambiarVentanaAClase();
                MessageBox.Show("La ventana no era correcta. Seleccione nuevamente el elemento.");
                return false;
            }

            if (dgvRegistros.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un elemento antes de continuar.");
                return false;
            }

            return true;
        }
    }
}