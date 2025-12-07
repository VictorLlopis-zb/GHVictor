using System;
using System.Collections.Generic;
using System.Linq;
using AppWpf1.Modelos;

namespace AppWpf1.DTO
{
    public static class PersonaIdentidadMapper
    {
        public static PersonaIdentidadFormulario ToFormulario(this PersonaIdentidad p)
        {
            var partes = p.NombreCompleto.Split('\u00A0', StringSplitOptions.RemoveEmptyEntries);

            return new PersonaIdentidadFormulario
            {
                Cedula = p.Cedula,
                Nombre = partes.Length > 0 ? partes[0] : string.Empty,
                Apellido1 = partes.Length > 1 ? partes[1] : string.Empty,
                Apellido2 = partes.Length > 2 ? string.Join(" ", partes.Skip(2)) : null,
                FechaNacimiento = DateTime.MinValue,
                Sexo = SexoEnum.M
            };
        }

        public static PersonaIdentidad ToDesglosada(this PersonaIdentidadFormulario f)
        {
            var nombreCompleto = $"{f.Nombre}\u00A0{f.Apellido1}";
            if (!string.IsNullOrWhiteSpace(f.Apellido2))
                nombreCompleto += $"\u00A0{f.Apellido2}";

            var cedula = string.IsNullOrWhiteSpace(f.Cedula)
                ? $"{f.Nombre[0]}{f.Apellido1[0]}{f.FechaNacimiento:yyyy}"
                : f.Cedula;

            return new PersonaIdentidad
            {
                Cedula = cedula,
                NombreCompleto = nombreCompleto
            };
        }

        public static List<PersonaIdentidadFormulario> ToFormularioList(this IEnumerable<PersonaIdentidad> lista)
            => lista.Select(p => p.ToFormulario()).ToList();

        public static List<PersonaIdentidad> ToDesglosadaList(this IEnumerable<PersonaIdentidadFormulario> lista)
            => lista.Select(f => f.ToDesglosada()).ToList();
    }
}
