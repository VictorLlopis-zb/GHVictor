using AppWpf1.Atributos;
using AppWpf1.Interfaces;
using System.Collections.ObjectModel;
namespace AppWpf1.Modelos
{
    [Persistente("identidades.json")]
    //public record PersonaIdentidad
    public record PersonaIdentidad : IMPersistente<PersonaIdentidad>
    {
        public string Cedula { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;

        // Implementación de la lista observable
        public static ObservableCollection<PersonaIdentidad> ListaPersistente { get; set; }
            = new ObservableCollection<PersonaIdentidad>();
        //public static List<PersonaIdentidad> ListaPersistente { get; set; } = new();

        // Constructor vacío explícito
        public PersonaIdentidad() { }


        // Constructor extendido: asegura separadores duros
        public PersonaIdentidad(string cedula, string nombre, string apellido1, string apellido2)
        {
            Cedula = cedula;
            NombreCompleto = $"{nombre}\u00A0{apellido1}\u00A0{apellido2}";
        }


        public override string ToString() => NombreCompleto;
        // Método estático que devuelve el puntero a la lista
        public static IList<PersonaIdentidad> ObtenerLista()
        {
            return ListaPersistente;
        }

        public string NombrePlano() => (NombreCompleto?.Replace("\u00A0", " ")) ?? string.Empty;
        private string[] Partes()
        {
            if (string.IsNullOrWhiteSpace(NombreCompleto)) return Array.Empty<string>();
            var partes = NombreCompleto.Split('\u00A0', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < partes.Length; i++) partes[i] = partes[i].Trim();
            return partes;
        }

        public string Nombre() =>
            Partes().Length >= 1 ? Partes()[0] : string.Empty;

        public string PrimerApellido() =>
            Partes().Length >= 2 ? Partes()[1] : string.Empty;

        public string SegundoApellido() =>
            Partes().Length >= 3 ? Partes()[2] : string.Empty;
        
    }
}