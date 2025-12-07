namespace AppWpf1.Atributos
{
[AttributeUsage(AttributeTargets.Class)]
    public class PersistenteAttribute : Attribute
    {
        public string Archivo { get; }
        public PersistenteAttribute(string archivo)
        {
            Archivo = archivo;
        }
    }
}

