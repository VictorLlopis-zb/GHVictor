using AppWpf1.Atributos;
using AppWpf1.Interfaces;
using AppWpf1.Modelos;
using AppWpf1.Servicios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace AppWpf1.DTO
{
    [Persistente("personaidentidad_formulario.json")]
    public record PersonaIdentidadFormulario : IPersistente<PersonaIdentidadFormulario>
    {
        [CampoFormulario("Cédula", "TextBox", true, 80)]
        public string Cedula { get; set; } = string.Empty;

        [CampoFormulario("Nombre", "TextBox", true, 120)]
        public string Nombre { get; set; } = string.Empty;

        [CampoFormulario("Primer Apellido", "TextBox", true, 120)]
        public string Apellido1 { get; set; } = string.Empty;

        [CampoFormulario("Segundo Apellido", "TextBox", false, 120)]
        public string? Apellido2 { get; set; }

        [CampoFormulario("Fecha de Nacimiento", "DatePicker", true, 120)]
        public DateTime? FechaNacimiento { get; set; }

        [CampoFormulario("Sexo", "ComboBox", true, 60)]
        public SexoEnum Sexo { get; set; }


        public string NombreCompleto =>
            string.Join(" ", new[] { Nombre, Apellido1, Apellido2 ?? string.Empty }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
        
        // Tipo correcto
        public static ObservableCollection<PersonaIdentidadFormulario> ListaPersistente { get; set; } = new();

        // ==== Métodos del interfaz ====
        public bool EsPIF() => true; // Sí es PersonaIdentidadFormulario

        public void Aceptar(string operacion, PersonaIdentidadFormulario elemento)
        {
            MessageBox.Show("entro a aceptar PIF");
            switch (operacion)
            {
                case "crear":
                    Crear(elemento);
                    break;
                case "modificar":
                    Modificar(elemento);
                    break;
                case "eliminar":
                    Eliminar(elemento.Cedula);
                    break;
                default:
                    MessageBox.Show($"Operación {operacion} no reconocida.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        public void Salir()
        {
            // lógica de salida
        }

        public List<string> ValidarCampos()
        {
            var errores = new List<string>();
            if (string.IsNullOrWhiteSpace(Nombre)) errores.Add("Nombre es requerido.");
            if (string.IsNullOrWhiteSpace(Apellido1)) errores.Add("Primer Apellido es requerido.");
            if (FechaNacimiento == null) errores.Add("Fecha de Nacimiento es requerida.");
            if (!Enum.IsDefined(typeof(SexoEnum), Sexo)) errores.Add("Sexo es inválido.");
            return errores;
        }

        // ==== Métodos CRUD internos ====
        private bool Crear(PersonaIdentidadFormulario elemento)
        {
            var errores = elemento.ValidarCampos();
            if (errores.Any())
            {
                MostrarErrores("Errores al crear registro", errores);
                return false;
            }
            ListaPersistente.Add(elemento);
            return true;
        }

        private bool Modificar(PersonaIdentidadFormulario elemento)
        {
            var existente = ListaPersistente.FirstOrDefault(p => p.Cedula == elemento.Cedula);
            if (existente == null) return false;

            int index = ListaPersistente.IndexOf(existente);
            ListaPersistente[index] = elemento;
            return true;
        }

        private bool Eliminar(string cedula)
        {
            var existente = ListaPersistente.FirstOrDefault(p => p.Cedula == cedula);
            if (existente == null) return false;
            ListaPersistente.Remove(existente);
            return true;
        }

        private void MostrarErrores(string titulo, List<string> errores)
        {
            MessageBox.Show(string.Join(Environment.NewLine, errores),
                titulo, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
    public enum SexoEnum { M, F }
}