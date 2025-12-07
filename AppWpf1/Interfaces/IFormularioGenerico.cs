namespace AppWpf1.Interfaces
{
    public interface IFormularioGenerico<T>
    {
        System.Windows.FrameworkElement GenerarFormulario();
        T ObtenerInstancia();
    }
}