using AppWpf1.Atributos;
using AppWpf1.Modelos;
using AppWpf1.Servicios;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace AppWpf1.Datos
{
    public static class BaseLocal
    {
        private static readonly Dictionary<Type, IList> Colecciones = new();
        private static readonly Dictionary<Type, string> Rutas = new();

        public static string CarpetaDatos { get; } =
            Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Datos"));

        static BaseLocal()
        {
            try
            {
                Directory.CreateDirectory(CarpetaDatos);
                CargarTodo();
                //GuardarTodo();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar BaseLocal:\n{ex.Message}");
            }            
        }
        /// <summary>
        /// Método explícito para inicializar las colecciones en memoria.
        /// Fuerza la ejecución del constructor estático.
        /// </summary>
        public static void Inicializar()
        {
            // No hace nada, pero garantiza que el constructor estático ya corrió.
            // Si querés, podés llamar directamente a CargarTodo() para mayor claridad:
            //CargarTodo();
            // Acceder a la lista tipada de personas
            

        }


        // 🔑 Obtener lista tipada
        public static ObservableCollection<T> ObtenerLista<T>() where T : class
        {
            if (Colecciones.TryGetValue(typeof(T), out var lista))
                return (ObservableCollection<T>)lista; // ✔ correcto

            // Si no existe, inicializa una nueva colección observable
            var nueva = new ObservableCollection<T>();
            Colecciones[typeof(T)] = nueva;
            return nueva;
        }

        // 🔑 Guardar una lista específica
        public static void Guardar<T>()
        {
            if (Colecciones.TryGetValue(typeof(T), out var lista))
            {
                string ruta = Rutas[typeof(T)];
                string json = SerializadorOptimizado.Serializar((IEnumerable<T>)lista);
                File.WriteAllText(ruta, json);
            }
        }

        // 🔑 Guardar todas las listas
        public static void GuardarTodo()
        {
            foreach (var kvp in Colecciones)
            {
                var tipo = kvp.Key;
                var lista = kvp.Value;

                string ruta = Rutas[tipo];

                // Usar dynamic para que se resuelva Serializar<T> automáticamente
                string json = SerializadorOptimizado.Serializar((dynamic)lista);
                File.WriteAllText(ruta, json);

                /*MessageBox.Show($"Guardado {tipo.Name} con {lista.Count} elementos en {ruta}",
                    "Debug GuardarTodo", MessageBoxButton.OK, MessageBoxImage.Information);*/
            }
        }

        // 🔑 Cargar todas las listas dinámicamente
        public static void CargarTodo()
        {
            var tiposPersistentes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(PersistenteAttribute), false).Any());

            foreach (var tipo in tiposPersistentes)
            {
                try
                {
                    //MessageBox.Show($"Procesando tipo: {tipo.FullName}");
                    var atributo = (PersistenteAttribute)(tipo
                        .GetCustomAttributes(typeof(PersistenteAttribute), false)
                        .FirstOrDefault() ?? throw new InvalidOperationException($"Tipo {tipo.Name} sin atributo Persistente"));

                    string ruta = Path.Combine(CarpetaDatos, atributo.Archivo);
                    Rutas[tipo] = ruta;

                    IList lista = (IList)Activator.CreateInstance(typeof(ObservableCollection<>).MakeGenericType(tipo))!;

                    if (File.Exists(ruta))
                    {
                        string json = File.ReadAllText(ruta);

                        // Invocar DeserializarObservable<T>(json) por reflection usando 'tipo'
                        var metodoGenerico = typeof(SerializadorOptimizado)
                            .GetMethod("DeserializarObservable", new[] { typeof(string) })!
                            .MakeGenericMethod(tipo);
                        var resultado = metodoGenerico.Invoke(null, new object[] { json });
                        lista = (IList)resultado!;
                        // 📢 Imprimir el número de elementos cargados
                        /*MessageBox.Show($"Tipo {tipo.Name}: se cargaron {lista.Count} elementos.");

                        // 📢 Mostrar contenido si es PersonaIdentidad
                        if (tipo == typeof(PersonaIdentidad))
                        {
                            var sb = new System.Text.StringBuilder();
                            foreach (PersonaIdentidad p in lista)
                            {
                                sb.AppendLine($"Cédula: {p.Cedula} | Nombre: {p.NombreCompleto}");
                            }

                            MessageBox.Show(sb.Length > 0 ? sb.ToString() : "No se cargaron identidades.");
                        }
                        */
                    }

                    // Guardar en el diccionario central
                    Colecciones[tipo] = lista;

                    // 🔑 Sincronizar con la ListaPersistente estática de la clase
                    var propListaPersistente = tipo.GetProperty("ListaPersistente",
                        BindingFlags.Static | BindingFlags.Public);
                    if (propListaPersistente != null)
                    {
                        // Asignar directamente la referencia observable
                        propListaPersistente.SetValue(null, lista);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar tipo {tipo.Name}:\n{ex.Message}");
                }
            }

            //InicializarAdministrador();
        }


        // 🔑 Factory centralizada para construir cédulas
        public static PersonaIdentidad GenerarPersonaIdentidad(
    string nombre,
    string apellido1,
    string apellido2,
    int año,
    int mes,
    int dia,
    string sexo
)
        {
            // Validaciones mínimas
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido1) ||
                string.IsNullOrWhiteSpace(apellido2))
                throw new ArgumentException("Nombre y apellidos son obligatorios.");

            if (sexo != "M" && sexo != "F")
                throw new ArgumentException("Sexo debe ser 'M' o 'F'.");

            // Año en dos dígitos (AA)
            int aa = año % 100;
            string año2 = $"{aa:D2}";

            // Raíz AA MM DD (6 dígitos)
            string raizFecha = $"{año2}{mes:D2}{dia:D2}";

            // Dígito sexo (1 dígito)
            string digitoSexo = sexo == "M" ? "1" : "2";

            // Buscar coincidencias por raíz AA MM DD + sexo
            var lista = BaseLocal.ObtenerLista<PersonaIdentidad>();
            var baseRaiz = $"{raizFecha}{digitoSexo}";
            var coincidencias = lista.Where(i => i.Cedula.StartsWith(baseRaiz)).ToList();

            int orden = coincidencias.Any() ? coincidencias.Count + 1 : 1;
            if (orden > 9)
                throw new InvalidOperationException("Se excedió el máximo de 9 identidades por raíz AA MM DD + sexo.");

            // Checksum de 2 dígitos (módulo 97 sobre AA MM DD + sexo + orden)
            string parcial = $"{baseRaiz}{orden}";
            int checksum = int.Parse(parcial) % 97;
            string cc = $"{checksum:D2}";

            // Cédula final: AA MM DD + S + O + CC = 10 dígitos
            string cedula = $"{parcial}{cc}";

            var identidad = new PersonaIdentidad(cedula, nombre, apellido1, apellido2);
            lista.Add(identidad);

            // Persistir inmediatamente
            BaseLocal.GuardarTodo();

            return identidad;
        }


        // 🔑 Inicializar administrador
        public static void InicializarAdministrador()
        {
            const string CedulaAutor = "4412041101";
            //const string NombreAutor = "Victor Manuel\u00A0Llopis\u00A0Zayas Bazan";

            var identidades = ObtenerLista<PersonaIdentidad>();
            var usuarios = ObtenerLista<UsuarioAcceso>();

            if (!identidades.Any(i => i.Cedula == CedulaAutor))
                identidades.Add(new PersonaIdentidad(CedulaAutor, "Victor Manuel","Llopis","Zayas Bazan"));
            // 🔎 Contador para verificar
                /*MessageBox.Show($"Identidades actuales: {identidades.Count}",
                    "Debug InicializarAdministrador",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                */
            if (!usuarios.Any(u => u.Cedula == CedulaAutor))
            {
                string clavePlano = "admin123";
                string claveCodificada;
                using (var sha = System.Security.Cryptography.SHA256.Create())
                {
                    var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(clavePlano));
                    claveCodificada = Convert.ToBase64String(bytes);
                }
                usuarios.Add(new UsuarioAcceso(CedulaAutor, "A", claveCodificada));
                //GuardarTodo();
            }
        }

    }
}