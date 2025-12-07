using AppWpf1.DTO;
using AppWpf1.Modelos;
using AppWpf1.Servicios;
using System.Windows;

namespace AppWpf1.Vistas
{
    public partial class PanelPrincipal : Window
    {
        public PanelPrincipal()
        {
            InitializeComponent();
        }

        private void BtnAbrirFormularioIdentidad_Click(object sender, RoutedEventArgs e)
        {
            // Diagnóstico de listas
            //var listaIdentidad = PersonaIdentidad.ListaPersistente;
            //MessageBox.Show($"PersonaIdentidad tiene {listaIdentidad.Count} elementos", "Diagnóstico");

            //var listaAcceso = UsuarioAcceso.ListaPersistente;
            //MessageBox.Show($"UsuarioAcceso tiene {listaAcceso.Count} elementos", "Diagnóstico");

            MessageBox.Show("Hola2", "Mensaje");

            // 🔎 Abrir el formulario de prueba con el cuadrado incrustado
            var ventana = new FormPruebaCuadrado();

            ventana.ShowDialog();
        }

        private void BtnAbrirFormularioUsuario_Click(object sender, RoutedEventArgs e)
        {
            // Generar formulario de UsuarioAcceso
            var ventana = new FormPersistente(typeof(PersonaIdentidadFormulario));
            //var ventana = new FormPersistente(typeof(UsuarioAcceso));
            ventana.ShowDialog();

            /*var generador = new FormularioGeneral<UsuarioAcceso>();
            var formulario = generador.GenerarFormulario(); // UserControl
            contenedorDinamico.Content = formulario;*/

            // Si quieres mantener el mensaje temporal:
            // MessageBox.Show("Hola1", "Mensaje");
        }
    }
}