using AppWpf1.Atributos;
namespace AppWpf1.Modelos
{
   // [Persistente("empleo.json")]
    public record Empleo
    {
        public required string Cedula { get; set; }
        public required string Cargo { get; set; }
        public decimal Salario { get; set; }
        public DateTime FechaIngreso { get; set; }
    }
}