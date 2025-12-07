using System.Windows;

namespace Servicios
{
    public static class HelperConfirmar
    {
        /// <summary>
        /// Muestra un cuadro de diálogo de confirmación con Sí/No.
        /// Devuelve true si el usuario confirma (Sí), false si responde No.
        /// </summary>
        /// <param name="mensaje">Texto del mensaje a mostrar.</param>
        /// <returns>True si el usuario presiona Sí, False si presiona No.</returns>
        public static bool Preguntar(string mensaje)
        {
            var resultado = MessageBox.Show(
                mensaje,
                "Confirmar",                // título fijo
                MessageBoxButton.YesNo,     // botones fijos
                MessageBoxImage.Question    // icono fijo
            );

            return resultado == MessageBoxResult.Yes;
        }
    }
}
