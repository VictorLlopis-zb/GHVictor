using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows;

namespace AppWpf1.Servicios
{
    public static class SerializadorOptimizado
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            Encoder = JavaScriptEncoder.Create(
                UnicodeRanges.BasicLatin,
                UnicodeRanges.Latin1Supplement,   // á, é, í, ó, ú, ñ
                UnicodeRanges.GeneralPunctuation  // incluye \u00A0
            ),
            WriteIndented = false   // compacto
        };

        // Serialización genérica sobre IEnumerable<T> en formato tabular
        public static string Serializar<T>(IEnumerable<T> lista)
        {
            var propiedades = typeof(T).GetProperties()
                                       .Select(p => p.Name)
                                       .ToList();

            var datos = lista.Select(item =>
                propiedades.Select(p => typeof(T).GetProperty(p)?.GetValue(item)).ToList()
            ).ToList();

            var resultado = new
            {
                campos = propiedades,
                datos = datos
            };

            return JsonSerializer.Serialize(resultado, Options);
        }

        // Sobrecarga para listas como object + Type (tabular)
        public static string Serializar(object lista, Type tipo)
        {
            var propiedades = tipo.GetProperties()
                                  .Select(p => p.Name)
                                  .ToList();

            var enumerable = ((IEnumerable)lista).Cast<object>().ToList();

            var datos = enumerable.Select(item =>
                propiedades.Select(p => tipo.GetProperty(p)?.GetValue(item)).ToList()
            ).ToList();

            var resultado = new
            {
                campos = propiedades,
                datos = datos
            };

            return JsonSerializer.Serialize(resultado, Options);
        }

        // Deserialización genérica (tabular)
        public static List<T> Deserializar<T>(string json) where T : new()
        {
            using var doc = JsonDocument.Parse(json);
            // Detectar formato tabular
            if (doc.RootElement.TryGetProperty("campos", out var camposElement) &&
                doc.RootElement.TryGetProperty("datos", out var datosElement))
            {
                var campos = camposElement.EnumerateArray()
                                          .Select(x => x.GetString() ?? string.Empty)
                                          .ToList();

                var lista = new List<T>();
                foreach (var fila in datosElement.EnumerateArray())
                {
                    var obj = new T();
                    int i = 0;
                    foreach (var valor in fila.EnumerateArray())
                    {
                        var nombreCampo = campos[i];
                        if (!string.IsNullOrEmpty(nombreCampo))
                        {
                            var prop = typeof(T).GetProperty(nombreCampo);
                            if (prop != null && valor.ValueKind != JsonValueKind.Null)
                            {
                                object? convertido = valor.ValueKind == JsonValueKind.String
                                    ? valor.GetString()
                                    : valor.ToString();

                                if (convertido != null)
                                    prop.SetValue(obj, Convert.ChangeType(convertido, prop.PropertyType));
                            }
                        }
                        i++;
                    }
                    lista.Add(obj);
                }
                return lista;
            }

            // Si no es tabular, intentar deserialización estándar
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        // Deserialización con reflexión (Type dinámico)
        public static object Deserializar(string json, Type tipo)
        {
            using var doc = JsonDocument.Parse(json);

            // Detectar formato tabular
            if (doc.RootElement.TryGetProperty("campos", out var camposElement) &&
                doc.RootElement.TryGetProperty("datos", out var datosElement))
            {
                var campos = camposElement.EnumerateArray()
                                          .Select(x => x.GetString() ?? string.Empty)
                                          .ToList();

                var lista = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(tipo))!;

                foreach (var fila in datosElement.EnumerateArray())
                {
                    var obj = Activator.CreateInstance(tipo)!;
                    int i = 0;
                    foreach (var valor in fila.EnumerateArray())
                    {
                        var nombreCampo = campos[i];
                        var prop = tipo.GetProperty(nombreCampo);
                        if (prop != null && valor.ValueKind != JsonValueKind.Null)
                        {
                            object? convertido = valor.ValueKind == JsonValueKind.String
                                ? valor.GetString()
                                : valor.ToString();

                            if (convertido != null)
                                prop.SetValue(obj, Convert.ChangeType(convertido, prop.PropertyType));
                        }
                        i++;
                    }
                    lista.Add(obj);
                }

                return lista;
            }

            // Si no es tabular, usar deserialización estándar
            var generic = typeof(List<>).MakeGenericType(tipo);
            return JsonSerializer.Deserialize(json, generic) ?? Activator.CreateInstance(generic)!;
        }

        public static ObservableCollection<T> DeserializarObservable<T>(string json) where T : new()
        {
            // Reutiliza tu deserialización genérica existente
            var lista = Deserializar<T>(json);

            // Convierte a ObservableCollection
            return new ObservableCollection<T>(lista);
        }

    }
}