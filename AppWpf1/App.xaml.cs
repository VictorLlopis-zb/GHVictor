using AppWpf1.Datos;
using AppWpf1.Vistas;
using System.Windows;

namespace AppWpf1
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
           base.OnStartup(e);
            
            // Inicializar listas persistentes
            BaseLocal.Inicializar();

            // Abrir login
            var login = new Vistas.Login(); //hasta aqui quito para FLogFalso
            
            //var login = new Vistas.FLogFalso(); // y esto lo pongo para FLogFalso
            login.ShowDialog();
            
        }
    }
}