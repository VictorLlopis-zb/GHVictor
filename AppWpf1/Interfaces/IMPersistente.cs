using System.Collections.ObjectModel;

namespace AppWpf1.Interfaces
{
    public interface IMPersistente<T>
    {
        string Cedula { get; set; }

        // Contrato: cada clase concreta debe implementar esta propiedad estática
        static abstract ObservableCollection<T> ListaPersistente { get; set; }
    }
}

