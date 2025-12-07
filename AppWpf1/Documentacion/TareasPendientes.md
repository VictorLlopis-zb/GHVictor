## 🔧 Desarrollo
- Revisar que **Login** sea quien hace las actualizaciones y no **PanelPrincipal**.

- Arreglar el formulario **Login** en lo que respecta al *focus* y controles de entrada.

## 📂 Organización
- Preparar guía completa con el proceso de **Git**, que incluya:
  - Borrar todo y quedarnos con el último commit.
- Tener una carpeta con los **scripts más usados**.
  - Analizar si los scripts de Git se ponen aquí o en la carpeta superior.

## 🚀 Nuevas Ramas
- Empezar la otra rama del **PanelPrincipal** donde se comienzan las tareas.
- Trabajar con las clases:
  - `PersonaIdentidad` es la primera.




ya habiamos resuelto que el login aliniciar el admin lo guardara para que se mantenga y llegamos a la conclusion que los botones del panel principal Debian cambiarse ya que no tenian sentido despues de eso el proximo paso es definir los botones del panelprincipal esta claro que ya sera pera trabajar con las clases en donde a cada una le podremos nadir miembros, editarlos o eliminarlos. Antes de eso debemos definir como sera el proceso de incluir nuevas clases en el proyecto segun mi analisis basta con incluir las clases en el proyecto con la pertenencia ya esto permitira que las listass se incorporen vacias en el login y pueden entrar a definirse sus nuevos entidades para la listas. Tambien debe verse como se crean los formularios de edicion de las clases a trabajar. Pudiera ser que as ckases tuvieran un metodo que de aalguna manera defina estos boxes donde se pondran cadenas y restricciones de las mismas.

Checklist de Migración y Flujo
- Login
- [ ] Inicializa listas vacías si no existe JSON.
- [ ] Crea automáticamente el JSON vacío ([]) en disco.
- [ ] Carga PersonaIdentidad desde BaseLocal con datos desmenuzados.
- [ ] Vincula las demás clases al ComboBox de cédulas.
- Diccionario central (GestorClases)
- [ ] Registrar todas las clases con su nombre y lista asociada.
- [ ] Validar que no existan duplicados.
- [ ] Permitir iteración dinámica para generar botones en PanelPrincipal.
- PanelPrincipal
- [ ] Generar botones dinámicos según las clases registradas.
- [ ] Cada botón abre el formulario de edición correspondiente.
- [ ] Validar que PersonaIdentidad tenga su flujo especial (sin ComboBox).
- Formularios de edición
- [ ] PersonaIdentidad: campos desmenuzados desde BaseLocal.
- [ ] Otras clases: ComboBox de cédulas + campos dinámicos definidos en DefinirCampos().
- [ ] Validaciones: cédula obligatoria, no duplicada en la misma lista.
- Persistencia
- [ ] Guardar en JSON correspondiente (ya creado vacío en Login).
- [ ] Actualizar listas en memoria y reflejar cambios en disco.
- [ ] Mantener trazabilidad: cada clase ↔ su JSON ↔ su lista.
- [ ] PersonaIdentidad persiste directamente en BaseLocal.

PARA MAÑANA LO DE ABAJO SON LAS INSTRIUCCIONES.


Cada clase persistente debe exponer un método DefinirCampos() que devuelva un diccionario de metadatos. Estos metadatos describen cómo se construye cada control en el formulario:
public Dictionary<string, CampoDefinicion> DefinirCampos()
{
    return new Dictionary<string, CampoDefinicion>
    {
        { "Cedula", new CampoDefinicion("string", requerido:true, origen:"PersonaIdentidad", tipoControl:"ComboBox") },
        { "Cargo", new CampoDefinicion("string", requerido:true, maxLength:100, tipoControl:"TextBox") },
        { "Salario", new CampoDefinicion("decimal", requerido:true, rango:"0-999999", tipoControl:"NumericBox") },
        { "FechaIngreso", new CampoDefinicion("DateTime", requerido:true, tipoControl:"DatePicker") }
    };
}



🔹 Atributos clave para cada campo
- Tipo → string, decimal, DateTime, etc.
- Requerido → si el campo es obligatorio.
- MaxLength → longitud máxima para cadenas.
- Rango → límites para números.
- Origen → si el valor proviene de otra clase (ej. PersonaIdentidad).
- TipoControl → define el control WPF que se debe generar (TextBox, ComboBox, DatePicker, etc.).


 cuando pasamos en las clases los atributos de cada campo ya eso se puede contemplar


. Ya con la infraestructura de persistencia funcionando, ahora sí podemos contemplar los atributos de cada campo en las clases para que el motor de formularios dinámicos sepa cómo construir los controles.

🔹 Cómo se contemplan los atributos
Cada clase persistente expone un método DefinirCampos() que devuelve un diccionario de metadatos. Esos metadatos describen:
- Tipo de dato (string, decimal, DateTime, etc.).
- Requerido (si el campo es obligatorio).
- MaxLength (longitud máxima para cadenas).
- Rango (límites para números).
- Origen (si el valor proviene de otra clase, ej. PersonaIdentidad).
- TipoControl (qué control WPF se debe generar: TextBox, ComboBox, DatePicker, etc.).

🔹 Ejemplo aplicado a Empleo
public Dictionary<string, CampoDefinicion> DefinirCampos()
{
    return new Dictionary<string, CampoDefinicion>
    {
        { "Cedula", new CampoDefinicion(
            tipo:"string",
            requerido:true,
            origen:"PersonaIdentidad",
            tipoControl:"ComboBox") },

        { "Cargo", new CampoDefinicion(
            tipo:"string",
            requerido:true,
            maxLength:100,
            tipoControl:"TextBox") },

        { "Salario", new CampoDefinicion(
            tipo:"decimal",
            requerido:true,
            rango:"0-999999",
            tipoControl:"NumericBox") },

        { "FechaIngreso", new CampoDefinicion(
            tipo:"DateTime",
            requerido:true,
            tipoControl:"DatePicker") }
    };
}



🔹 Qué logra esto
- El motor de formularios puede leer los atributos y decidir qué control crear.
- Se asegura que las validaciones estén centralizadas en la definición de la clase.
- Cada clase persistente queda autodescriptiva: no necesitas codificar manualmente cada formulario, se genera dinámicamente.

🔹 Checklist auditable
- [ ] Cada clase persistente implementa DefinirCampos() con atributos completos.
- [ ] El motor de formularios interpreta tipoControl y genera el control adecuado.
- [ ] Validaciones aplicadas según Requerido, MaxLength, Rango.
- [ ] Cedula siempre se carga desde PersonaIdentidad vía ComboBox.
- [ ] Persistencia se mantiene sincronizada al guardar/editar/eliminar.

📌 En resumen: sí, ya se puede contemplar. Los atributos de cada campo se definen en la clase y el motor de formularios los interpreta para construir la interfaz de edición.



Ahora quisiera ver como seria a la pantalla del login como hacemos los metodos para darle mas coherencia esto seria que la cedula solo puede ser numero de 10 digitos ni unomas nin uno menos y que al final en lugar de dar click en la clave al daar return pase el focus a la clave e igual pase con el return a l boton de aceptar

Reglas que quieres implementar
- Validación de la cédula
- Solo se permiten números.
- Debe tener exactamente 10 dígitos (ni más ni menos).
- Navegación con Enter (Return)
- Al presionar Enter en el campo de cédula → el foco pasa al campo de clave.
- Al presionar Enter en el campo de clave → el foco pasa al botón de aceptar (y opcionalmente dispara el click).

🔹 Ejemplo en WPF (XAML + Code-behind)
XAML
<StackPanel Margin="20">
    <!-- Campo Cédula -->
    <TextBox x:Name="txtCedula"
             Width="200"
             MaxLength="10"
             PreviewTextInput="TxtCedula_PreviewTextInput"
             KeyDown="TxtCedula_KeyDown"
             PlaceholderText="Ingrese cédula (10 dígitos)" />

    <!-- Campo Clave -->
    <PasswordBox x:Name="txtClave"
                 Width="200"
                 KeyDown="TxtClave_KeyDown"
                 PlaceholderText="Ingrese clave" />

    <!-- Botón Aceptar -->
    <Button x:Name="btnAceptar"
            Width="200"
            Content="Aceptar"
            Click="BtnAceptar_Click" />
</StackPanel>



Code-behind (C#)
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace AppWpf1
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        // Validar que solo se ingresen números en la cédula
        private void TxtCedula_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        // Al presionar Enter en cédula → pasar foco a clave
        private void TxtCedula_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtCedula.Text.Length == 10)
                    txtClave.Focus();
                else
                    MessageBox.Show("La cédula debe tener exactamente 10 dígitos.");
            }
        }

        // Al presionar Enter en clave → pasar foco al botón aceptar
        private void TxtClave_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnAceptar.Focus();
            }
        }

        // Acción del botón aceptar
        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            // Aquí validas usuario/clave
            MessageBox.Show("Login procesado.");
        }
    }
}



🔹 Checklist auditable para Login
- [ ] Campo cédula solo acepta números.
- [ ] Longitud exacta de 10 dígitos validada.
- [ ] Enter en cédula → foco en clave.
- [ ] Enter en clave → foco en botón aceptar.
- [ ] Botón aceptar dispara la validación final.

📌 Con esto tu pantalla de Login queda coherente, intuitiva y blindada contra errores de entrada.


👌. Todo ese code‑behind que te mostré para validar la cédula y manejar el flujo con Enter va dentro del archivo Login.xaml.cs, que es el code‑behind de tu ventana de login en WPF.
El esquema completo quedaría así:

🔹 Login.xaml
<Window x:Class="AppWpf1.LoginWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Login" Height="250" Width="300">
    <StackPanel Margin="20">
        <!-- Campo Cédula -->
        <TextBox x:Name="txtCedula"
                 Width="200"
                 MaxLength="10"
                 PreviewTextInput="TxtCedula_PreviewTextInput"
                 KeyDown="TxtCedula_KeyDown"
                 PlaceholderText="Ingrese cédula (10 dígitos)" />

        <!-- Campo Clave -->
        <PasswordBox x:Name="txtClave"
                     Width="200"
                     KeyDown="TxtClave_KeyDown"
                     PlaceholderText="Ingrese clave" />

        <!-- Botón Aceptar -->
        <Button x:Name="btnAceptar"
                Width="200"
                Content="Aceptar"
                Click="BtnAceptar_Click" />
    </StackPanel>
</Window>



🔹 Login.xaml.cs
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace AppWpf1
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        // Validar que solo se ingresen números en la cédula
        private void TxtCedula_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        // Al presionar Enter en cédula → pasar foco a clave
        private void TxtCedula_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtCedula.Text.Length == 10)
                    txtClave.Focus();
                else
                    MessageBox.Show("La cédula debe tener exactamente 10 dígitos.");
            }
        }

        // Al presionar Enter en clave → pasar foco al botón aceptar
        private void TxtClave_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnAceptar.Focus();
            }
        }

        // Acción del botón aceptar
        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            // Aquí validas usuario/clave contra tu infraestructura
            MessageBox.Show("Login procesado.");
        }
    }
}



🔹 Checklist auditable para Login
- [ ] Login.xaml define los controles (TextBox, PasswordBox, Button).
- [ ] Login.xaml.cs contiene la lógica de validación y navegación con Enter.
- [ ] Cédula solo acepta números y debe tener 10 dígitos.
- [ ] Enter en cédula → foco en clave.
- [ ] Enter en clave → foco en botón aceptar.
- [ ] Botón aceptar dispara validación final.

📌 Con esto tu pantalla de Login queda coherente y funcional.
Mañana podemos extenderlo para que el botón aceptar valide contra tu BaseLocal (usuarios y claves encriptadas), en lugar de solo mostrar un MessageBox.


Visual Studio ya te genera la estructura básica automáticamente cuando creas una Window WPF llamada LoginWindow:
- Login.xaml → el archivo de interfaz, donde defines los controles (TextBox, PasswordBox, Button, etc.).
- Login.xaml.cs → el code‑behind, donde escribes la lógica de eventos como validación de cédula, navegación con Enter y el click del botón aceptar.
Cuando en el diseñador de VS agregas un TextBox y le asignas eventos (PreviewTextInput, KeyDown, etc.), Visual Studio te crea los handlers en el Login.xaml.cs con la firma correcta. Tú solo completas el cuerpo del método.

🔹 Flujo típico en Visual Studio
- Creas un nuevo Window WPF → VS genera Login.xaml y Login.xaml.cs.
- En el diseñador (XAML), agregas controles (TextBox, PasswordBox, Button).
- En el panel de propiedades, asignas eventos (KeyDown, Click).
- VS crea automáticamente los métodos en Login.xaml.cs.
- Tú escribes la lógica dentro de esos métodos (validación, cambio de foco, etc.).

🔹 Resultado
- La validación de cédula (solo números, 10 dígitos) se hace en el PreviewTextInput y KeyDown.
- La navegación con Enter se controla en los eventos KeyDown de cada control.
- El botón aceptar se dispara con Click.
Todo esto se integra de forma natural porque Visual Studio ya enlaza el XAML con el code‑behind.

📌 En resumen: sí, con el interface de VS se genera toda la estructura automáticamente. Tú solo defines los controles en el XAML y completas la lógica en el Login.xaml.cs.

