using AppWpf1.Datos;
using AppWpf1.Modelos;
using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace AppWpf1.Vistas
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            txtCedula.Focus(); // foco inicial en cédula
        }

        // Abrir el panel principal y cerrar el login
        private void AbrirPanelPrincipal()
        {
            var panel = new PanelPrincipal();
            Application.Current.MainWindow = panel;
            panel.Show();
            this.Close();
        }

        // Solo números en cédula, un dígito por pulsación
        private void TxtCedula_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]$");
        }

        // Enter en cédula: validar longitud, mostrar bienvenida dinámica y pasar foco a clave
        private void TxtCedula_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string cedula = txtCedula.Text.Trim();

                if (cedula.Length == 10)
                {
                    // Asegurar que el admin exista (infraestructura base)
                    //BaseLocal.InicializarAdministrador();
                    // Cargar datos persistentes
                    var personas = BaseLocal.ObtenerLista<PersonaIdentidad>();
                    var usuarios = BaseLocal.ObtenerLista<UsuarioAcceso>();
                    /*
                    // 📢 Diagnóstico: imprimir qué listas se obtienen
                    MessageBox.Show(
                        $"Obtenidas listas:\n" +
                        $"PersonaIdentidad → {personas.GetType().Name}, Count={personas.Count}\n" +
                        $"UsuarioAcceso → {usuarios.GetType().Name}, Count={usuarios.Count}"
                    );
                    */
                    var persona = personas.FirstOrDefault(p => p.Cedula == cedula);
                    var usuario = usuarios.FirstOrDefault(u => u.Cedula == cedula);

                    if (persona != null && usuario != null)
                    {
                        txtBienvenida.Visibility = Visibility.Visible;
                        txtBienvenida.Text = $"Bienvenido {persona.NombreCompleto}\nRol: {UsuarioAcceso.NombreRol(usuario.Rol)}\nTeclee su clave:";
                        txtClave.Focus();
                    }
                    else
                    {
                        MessageBox.Show("La cédula no aparece en PersonaIdentidad Rectifíquela.");
                    }
                }
                else
                {
                    MessageBox.Show("La cédula debe tener exactamente 10 dígitos.");
                }
            }
        }

        // Enter en clave: pasar foco al botón ingresar
        private void TxtClave_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string cedula = txtCedula.Text.Trim();
                string clave = txtClave.Password.Trim();

                var usuarios = BaseLocal.ObtenerLista<UsuarioAcceso>();
                var encontrado = usuarios.FirstOrDefault(u => u.Cedula == cedula);

                if (encontrado != null && encontrado.ClaveCodificada == CodificarClave(clave))
                {

                    // PersonaIdentidad
                    //var listaIdentidad = PersonaIdentidad.ListaPersistente;
                    //MessageBox.Show($"PersonaIdentidad tiene {listaIdentidad.Count} elementos", "Diagnóstico");

                    // UsuarioAcceso
                    //var listaAcceso = UsuarioAcceso.ListaPersistente;
                    //MessageBox.Show($"UsuarioAcceso tiene {listaAcceso.Count} elementos", "Diagnóstico");




                    AbrirPanelPrincipal();
                }
                else
                {
                    MessageBox.Show("Credenciales inválidas. Intente nuevamente.");
                    txtClave.Clear();
                    txtClave.Focus();
                }
            }
        }
        
        // SHA256 → Base64 para comparar con ClaveCodificada
        private static string CodificarClave(string clave)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(clave);
                byte[] hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}