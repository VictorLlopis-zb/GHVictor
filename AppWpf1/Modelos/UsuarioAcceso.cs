using AppWpf1.Atributos;
using AppWpf1.Datos;
using AppWpf1.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace AppWpf1.Modelos
{
    [Persistente("usuarios.json")]
    public record UsuarioAcceso : IPersistente<UsuarioAcceso>
    {
        [CampoFormulario("Cédula", "TextBox", true, 80)]
        public string Cedula { get; set; } = string.Empty;

        [CampoFormulario("Rol", "ComboBox", true, 60)]
        public string Rol { get; set; } = string.Empty;

        [CampoFormulario("Clave", "PasswordBox", true, 120)]
        public string ClaveCodificada { get; set; } = string.Empty;
        public static ObservableCollection<UsuarioAcceso> ListaPersistente { get; set; } = new();

        public void SetClave(string clavePlano)
        {
            if (string.IsNullOrWhiteSpace(clavePlano))
                throw new ArgumentException("La clave no puede estar vacía.", nameof(clavePlano));

            // Aquí puedes aplicar la codificación/hasheo que prefieras
            ClaveCodificada = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(clavePlano));
        }
        // ==== Constructores ====
        //Constructor vacío explícito
        public UsuarioAcceso() { }


        public UsuarioAcceso(string cedula, string rol, string claveCodificada)
        {
            if (rol != "A" && rol != "S" && rol != "O")
                throw new ArgumentException("El rol debe ser A (Administrador), S (Supervisor) u O (Operador).", nameof(rol));

            Cedula = cedula;
            Rol = rol;
            ClaveCodificada = claveCodificada;
        }
        // ==== Métodos auxiliares ====
        public static string NombreRol(string rol) => rol switch
        {
            "A" => "Administrador",
            "S" => "Supervisor",
            "O" => "Operador",
            _ => "Desconocido"
        };
        
        public override string ToString()
        {
            var identidad = BaseLocal.ObtenerLista<PersonaIdentidad>()
                .FirstOrDefault(i => i.Cedula == Cedula);

            string nombre = identidad?.NombrePlano() ?? Cedula;
            return $"{nombre} - {NombreRol(Rol)}";
        }

        // ==== Métodos del interfaz ====
        public bool EsPIF() => false;  // No es PersonaIdentidadFormulario

        public void Aceptar(string operacion, UsuarioAcceso elemento)
        {
            // Convertir al tipo concreto
            var usuario = elemento as UsuarioAcceso;
            if (usuario == null)
                throw new ArgumentException("Elemento inválido: no es UsuarioAcceso");

            switch (operacion)
            {
                case "crear":
                    // Agregar nuevo usuario si no existe
                    if (!ListaPersistente.Any(u => u.Cedula == elemento.Cedula))
                        ListaPersistente.Add(usuario);
                    else
                        throw new InvalidOperationException("Ya existe un UsuarioAcceso con esa cédula.");
                    break;

                case "modificar":
                    // Buscar y reemplazar el usuario existente
                    var existente = ListaPersistente.FirstOrDefault(u => u.Cedula == elemento.Cedula);
                    if (existente != null)
                    {
                        int index = ListaPersistente.IndexOf(existente);
                        ListaPersistente[index] = usuario;
                    }
                    else
                        throw new InvalidOperationException("No existe un UsuarioAcceso con esa cédula para modificar.");
                    break;

                case "eliminar":
                    // Eliminar el usuario si existe
                    var eliminar = ListaPersistente.FirstOrDefault(u => u.Cedula == elemento.Cedula);
                    if (eliminar != null)
                        ListaPersistente.Remove(eliminar);
                    else
                        throw new InvalidOperationException("No existe un UsuarioAcceso con esa cédula para eliminar.");
                    break;

                default:
                    throw new NotSupportedException($"Operación '{operacion}' no soportada en UsuarioAcceso.");
            }
        }


        public void Salir()
        {
            // No hace nada
        }
        
        // ==== Utilidades ====

        public List<string> ValidarCampos()
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(Cedula)) errores.Add("Cédula es requerida.");
            if (string.IsNullOrWhiteSpace(Rol)) errores.Add("Rol es requerido.");
            if (string.IsNullOrWhiteSpace(ClaveCodificada)) errores.Add("Clave es requerida.");
            if (Rol != "A" && Rol != "S" && Rol != "O") errores.Add("Rol inválido.");

            return errores;
        }

        
        
        private void MostrarErrores(string titulo, List<string> errores)
        {
            MessageBox.Show(string.Join(Environment.NewLine, errores),
                titulo, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        
    }
}