namespace AppWpf1.Interfaces
{
    public interface IPersistente<T> : IMPersistente<T>
    {
        // Métodos base
        bool EsPIF(); // Indica si es PersonaIdentidadFormulario

        void Aceptar(string operacion, T elemento); // Acepta operación CRUD
        void Salir(); // Cierra el flujo CRUD

        
        // Utilidades de validación y búsqueda
        List<string> ValidarCampos();
    }      
}