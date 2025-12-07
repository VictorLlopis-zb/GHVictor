## 0. Formato de envío de reglas
- Todos los archivos de reglas deben enviarse en **bloques de código Markdown**.
- Esto garantiza que los encabezados (`##`) y demás sintaxis se copien literalmente al archivo `.md`.
- Cada vez que el usuario envíe una versión nueva de una regla (aunque sea parcial), la IA debe actualizarla automáticamente en memoria y aplicarla como la versión vigente, sin necesidad de que el usuario lo pida explícitamente.
- Todo cambio que se realice en un objeto ya reglamentado debe preguntarse al usuario si se incorpora a las reglas vigentes o queda en fase de prueba.
  - Si se incorpora el usuario debe reglamentarlo y mandar el arreglo
  - Si queda en fase de prueba al reiniciar el trabajo al día siguiente se vuelve a realizar la pregunta al usuaro
## A. Caracteristicas Generales  del proyecto.
- La solución es una WPF y esta en la carpeta C:/lomau/source/repos/ProyectoWpf1
- El proyecto se llama AppWpf1.
- Tiene las carpetas: Atributos, Datos, Documentacion, DTO, Interfaces, Recursos, Servicios, Vistas y Windows.
- Recursos y Windows se mantienes sin utilzar. El App.xaml está en la raíz.
## 1. Reglas generales (aplican a todas las clases persistentes)
- Cada clase persistente se define como `record` con atributo `[Persistente("nombre.json")]`.
- Tienen una propiedad llamada `Cedula` que actúa como identificador único y las relaciona.
- Tienen una lista asociada que lleva el nombre.json puesto en el atributo.
- Esta se carga al iniciar el programa y se guarda al cerrar siempre en formato Simplificado.
- Definen una propiedad estática `ListaPersistente` que materializa esta regla.
- Se pueden incluir nuevas clases persistentes en cualquier momento; todas ellas se integran al sistema bajo las mismas reglas.
- Todas excepto `PersonaIdentidad` implementan el interface `IPersistente`.
### 1.1  Clases persistentes con Interface
- Permiten operaciones CRUD que se realizan sobre la lista en memoria.
- Su formulario CRUD se genera automáticamente a partir de los atributos `[CampoFormulario(...)]`.
- Este formulario se inserta en un formulario principal que gestiona la lista de la clase.
- Los métodos de validación y conversión deben estar centralizados en la clase correspondiente.

## 2. Reglas específicas por clase. Las propiedades y metodos impuestos por el Interface empiezan con -IT. 
## 2.1 PersonaIdentidad
- Clase raíz, necesaria para enlazar todas las demás mediante la cédula.
- Forma un todo único junto con `PersonaIdentidadFormulario` que es la que gestiona el CRUD de `PersonaIdentidad`'
  - `PersonaIdentidad` mantiene la cédula oficial que usan todas las otras clases persistentes.
- **Propiedades:**
 - **Cedula (string):** identificador único de 10 caracteres. El sexo esta implicito en el dígito 7.
  - 1–6 → Fecha de nacimiento en formato `AAMMDD`.
  - 7 → (sexo == "M"?1:2) + (año>=2000?4:0) + (mes%2==1?2:0)).
  - 8 → control de orden: número correlativo para personas con misma fecha y mismo valor en 7.
  - 9–10 → verificación: módulo 97 de los 8 primeros.
- **NombreCompleto (string):** nombre y apellidos separados por `\u00A0`. Al final del nombre y del apellido1.
- **ListaPersistente (static):** `public static List<PersonaIdentidad> ListaPersistente { get; } = new()`
 -- **Métodos auxiliares:**
  - **public string NombrePlano():** Obtener nombre completo substituyendo sepradores por espacios.
  - **public string Nombre():** Obtener nombre.
  - **public string PrimerApellido():** Obtener Primer Apellido. 
  - **public string SegundoApellido():** Obtener Segundo Apellido.
  - **public override string ToString():** devuelve `NombreCompleto` como representación textual de la instancia.

### 2.2 UsuarioAcceso
- **Propiedades:**
 -IT **Cedula (string):** [CampoFormulario("Cédula", "TextBox", true, 80)]. Asociada a PersonaIdentidad.
 - **Rol (string):** [CampoFormulario("Rol", "ComboBox", true, 60)]. Solo puede ser:
   - "A" → Administrador
   - "S" → Subadministrador
   - "O" → Operario
 - **ClaveCodificada (string):** [CampoFormulario("Clave", "PasswordBox", true, 120)].Autenticidad del usuario, almacenada en formato codificado.
 -IT **EsPIF (Bool)** => false; Indica al Formulario CRUD que no es `PersonaIdentidadFormulario`.
- **Métodos auxiliares:**
 - **public static string NombreRol(string rol):** devuelve el nombre del rol. 
 - **public override string ToString():** devuelve NombrePlano en `PersonaIdentidad` + NombreRol.
 -IT **
 - **	  
 - Devolver nombre completo de la persona asociada.
 - Devolver rol descriptivo según el código.
### 2.3 PersonaIdentidadFormulario
- Clase necesaria para realizar el CRUD de `PersonaIdentidad` 
- Forma un todo único junto con `PersonaIdentidad`:
  - `PersonaIdentidadFormulario` es la responsable de generar y recalcular la cédula cuando cambian fecha o sexo.
-IT **Cedula (string):** [CampoFormulario("Cédula", "TextBox", true, 80)]
- **Nombre, Apellido1, Apellido2, FechaNacimiento, Sexo:** definidos con `[CampoFormulario]`.
- **NombreCompleto:** se construye automáticamente con los campos anteriores.
-IT **EsPIF (Bool)** => true; Indica al Formulario CRUD que es `PersonaIdentidadFormulario`.
- **ListaPersistente (static):** `public static List<PersonaIdentidadFormulario> ListaPersistente { get; } = new()`
- **Métodos auxiliares:**

-  **Reglas de recalculo:** si cambian fecha o sexo, la cédula se recalcula siguiendo las reglas oficiales.---

## 3. Interface y Formularios CRUD
## 3.1 Interface IPersistente
- Todas las clases persistentes (excepto PersonaIdentidad) deben implementarlo.
- Define operaciones CRUD y utilidades de validación/búsqueda.
- Propieddes obligatorias:
  - string Cedula { get; set; } string de 10 digitos que existe en `PersonaIdentidad`.
  - bool EsPIF { get; }  True para `PersonaIdentidadFormulario` False para las demás.
- Métodos obligatorios que delegan operaciones del Formulario principal:
  - Aceptar(string operacion, string jsonElemento) → ejecuta crear, modificar y eliminar.
  - Salir() → cierra el flujo CRUD.
  - ValidarCampos() → devuelve lista de errores.
## 3.2 Atributo [CampoFormulario]
- Define metadatos para generación automática de controles en el formulario CRUD.
- Parámetros: etiqueta, tipo de control, obligatoriedad, ancho.
  - Los tipos de control permitidos son: "TextBox", "DatePicker", "ComboBox" y "PasswordBox".
- Ejemplo:
[CampoFormulario("Cédula", "TextBox", true, 80)]
public string Cedula { get; set; }
## 3.3 Formulario CRUD generado
- Se construye dinámicamente a partir de [CampoFormulario].
- Valida campos al presionar Aceptar.
- No debe contener referencias manuales a propiedades específicas.
- Se incrusta en un contenedor (ContentControl o StackPanel) dentro del formulario principal.
- Debe limpiar controles al cerrar o cambiar de entidad.
- private void btnCrear_Click(object sender, RoutedEventArgs e)
        {   operacionActual = "crear";
            formularioGeneral.LimpiarCampos(); // siempre limpiar primero
            if (ValidarVentanaYSeleccion(!gestor.EsPIF != ventanaEntidadCorrecta))
                if (!gestor.EsPIF) formularioGeneral.FijarCedula(persona.Cedula);
        }  //esta correcta
## 3.4 Formulario principal
- Gestiona la lista asociada a la clase persistente.
### 3.4.1 Parte Superior
- Contiene los botones que controlan el proceso estos son: Crear, Modificar, Eliminar, Aceptar y Salir.
 - Los tres primeros botones gestionan la preparación de la operación indicada.
 - El botón Aceptar ejecuta la operación en la clase persistente mediante el método Aceptar delegado del Interface.
	- Cada clase gestiona la operación según sus reglas internas.
 - El botón Salir delegado del Interface ejecuta el metodo Salir y cierra el formulario principal.
	- Solo la clase `PersonaIdentidadFormulario` ejecuta acciones en este boton.
- Tiene la ventana donde se selecciona el elemento sobre el que va a trabajar el CRUD.
  - En el caso de modificar y eliminar sera de la lista asociada a la clase persistente.
  - Para crear en `PersonaIdentidadFormulario` no se selecciona nada ya que sera alguien completamente nuevo.
  - Para crear en las otras clases sera de la lista asociada a `PersonaIdentidad`.
### 3.4.2 Parte inferior
- Contiene panel dinámico donde se incrusta el FormularioCRUD generado para la clase asociada.


## 4. Gobernanza del archivo
- Este archivo es la fuente única de verdad para las reglas oficiales.
- Cada cambio debe documentarse aquí antes de aplicarse.
- Si el archivo supera el límite de caracteres permitido para compartir, se divide en secciones (Generales, PersonaIdentidad, UsuarioAcceso, Formularios, Checklist).
 
