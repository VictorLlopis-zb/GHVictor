using System.Windows;

namespace AppWpf1.Vistas
{
    public partial class FrmPersonaIdentidadCRUD : Window
    {
        public FrmPersonaIdentidadCRUD()
        {
            InitializeComponent();
        }

        private void btnCrear_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Crear registro");
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Modificar registro");
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eliminar registro");
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aceptar cambios");
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}