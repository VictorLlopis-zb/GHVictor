using System;

namespace AppWpf1.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CampoFormularioAttribute : Attribute
    {
        public string Etiqueta { get; }
        public string TipoControl { get; }
        public bool Obligatorio { get; }
        public double Ancho { get; }   // nuevo

        public CampoFormularioAttribute(
            string etiqueta,
            string tipoControl = "TextBox",
            bool obligatorio = false,
            double ancho = 200) // valor por defecto
        {
            Etiqueta = etiqueta;
            TipoControl = tipoControl;
            Obligatorio = obligatorio;
            Ancho = ancho;
        }
    }
}