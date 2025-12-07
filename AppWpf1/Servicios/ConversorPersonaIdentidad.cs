using AppWpf1.DTO;
using AppWpf1.Modelos;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AppWpf1.Servicios
{
    public static class ConversorPersonaIdentidad
    {
        public static PersonaIdentidadFormulario Convertir(PersonaIdentidad p)
        {
            var partes = p.NombreCompleto.Split('\u00A0', StringSplitOptions.RemoveEmptyEntries);

            string nombre = partes.Length > 0 ? partes[0] : string.Empty;
            string apellido1 = partes.Length > 1 ? partes[1] : string.Empty;
            string apellido2 = partes.Length > 2 ? partes[2] : string.Empty;

            // Usar helper para descomponer la cédula
            var (fechaNacimiento, sexo) = CedulaHelper.Descomponer(p.Cedula);

            return new PersonaIdentidadFormulario
            {
                Cedula = p.Cedula,
                Nombre = nombre,
                Apellido1 = apellido1,
                Apellido2 = apellido2,
                Sexo = sexo,
                FechaNacimiento = fechaNacimiento
            };
        }
        public static ObservableCollection<PersonaIdentidadFormulario> GenerarListaFormulario(IEnumerable<PersonaIdentidad> listaModelo)
        {
            return new ObservableCollection<PersonaIdentidadFormulario>(
                listaModelo.Select(p => Convertir(p))
            );
        }

        public static PersonaIdentidad ConvertirBack(PersonaIdentidadFormulario f)
        {
            string nombreCompleto = $"{f.Nombre}\u00A0{f.Apellido1}\u00A0{f.Apellido2}".Trim();

            return new PersonaIdentidad
            {
                Cedula = f.Cedula,
                NombreCompleto = nombreCompleto
            };
        }
        public static List<PersonaIdentidadFormulario> GenerarListaFormulario(List<PersonaIdentidad> listaModelo)
        {
            return listaModelo
                .Select(p => Convertir(p))
                .ToList();
        }
    }
    public static class CedulaHelper
    {
        //Calcular 7 primeros dígitos de la cédula
        public static string Calcular7Digs(DateTime fecha, SexoEnum sexo)
        => fecha.ToString("yyMMdd") + ((sexo == SexoEnum.M ? 1 : 2) + 
        (fecha.Year < 2000 ? 0 : 4) + (fecha.Month % 2 == 0  ? 0 : 2)).ToString();            
       

        // Generar cédula a partir de fecha, sexo y lista intermedia
        public static string Generar(DateTime? fecha, SexoEnum sexo,string nombre, 
            List<PersonaIdentidadFormulario> listaFormulario)
        {
            if (fecha == null) return string.Empty;
            
            string base7 = Calcular7Digs(fecha.Value, sexo);

            // Contar cuántos ya existen con esos 7 dígitos en la lista PIF
            int count = listaFormulario.Count(f => f.Cedula.StartsWith(base7));

            string orden = (count + 1).ToString("D1");
            string base8 = base7 + orden;
            int checksum = int.Parse(base8) % 97;
            string chequeo = checksum.ToString("D2");

            return base8 + chequeo;
        }

        // Descomponer cédula en fecha y sexo
        public static (DateTime fechaNacimiento, SexoEnum sexo) Descomponer(string cedula)
        {
            string cedula8 = cedula.PadLeft(8, '0');
            int septimo = int.Parse(cedula8[6].ToString());

            SexoEnum sexo = (septimo % 2 == 0) ? SexoEnum.F : SexoEnum.M;
            int siglo = (septimo < 5) ? 1900 : 2000;

            string yy = cedula8.Substring(0, 2);
            string mm = cedula8.Substring(2, 2);
            string dd = cedula8.Substring(4, 2);

            int year = siglo + int.Parse(yy);
            int month = int.Parse(mm);
            int day = int.Parse(dd);

            return (new DateTime(year, month, day), sexo);
        }
    }

}