# Proyecto de Gestión de Identidades en C# WPF

📌 Descripción del Proyecto
El único objetivo del proyecto es desarrollar habilidades de programación en C#, Visual Studio y GitHub. No tiene aplicación práctica.
Se persigue armar una estructura que abarque datos de diferentes personas.  
Estos datos se repartirán en distintas clases, todas vinculadas por un identificador único (**cédula**).
Las clases que marcan el objetivo del proyecto tendrán características especiales reflejadas con el atributo de Persistente.
La clase principal PersonaIdentidad contiene el nombre completo y la cédula.
Las demás clases se asociarán a esta por la cédula y se irán definiendo con el desarrollo.
Se define además la clase UsuarioAcceso, que representa a las personas autorizadas para introducir datos, con su rol jerárquico y clave.
El sistema permite:
- Crear nuevas clases con distintos tipos de datos.
- Introducir información en cada clase.
- Definir qué datos se mostrarán en pantalla.
- Guardar las listas al finalizar una sesión, para retomarlas en la siguiente.
El programa se desarrolla en C# con WPF mediante formularios.

🧩 Clases con el atributo persistente.
Estas clases son el objetivo del proyecto. Tienen en común para su creación:
 - Todas tienen la propiedad Cedula que las enlaza.
 - Son Record que solo hay que crearlos con las características establecidas:
   - Atributo [Persistente("nombre.json")]
   - El nombre.json indica la lista que representa a la clase y con la que se trabaja.
   - Cada clase tiene su propio nombre para su lista.
   - Las propiedades se recrean con [CampoFormulario("Cédula", "TextBox", true, 80)] que define como se crearán, modificarán o eliminarán las instancias.
   - Tendrán métodos recreados que validarán los datos de cuando sea necesario.
Luego de creadas las clases el resto del trabajo será automático.
Las listas que representan las Clases se mantienen en memoria actualizada para su uso.
Se guardan en disco simplificadas, especialmente al final y se cargan al inicio.
Todo el trabajo se hará  con las listas.
A cada se le puede añadir, modificar y eliminar datos.
Pueden crearse formatos que mezclen datos de las clases para mostrarlos en pantalla.
El Usuario puede crear las clases con los datos que desee con el objetivo de probar el programa y ver su funcionamiento. Por diseño hay tres clases con el atributo persistente creadas previamente. PersonaIdentidad y UsuarioAcceso necesarias para el Login inicial donde la primera vez que accedieramos al programa debíamos tener una persona y un administrador en el sistema. La otra PersonaIdentidadFormulario ya que PersonaIdentidad se diferenciaba de las demás en que generaba la cedula, las otras usaban la que ella ya tenía, y ella sirve de acople para trabajar dentro del sistema automatizado con PersonaIdentidad.
 

1. PersonaIdentidad
- [Persistente("identidades.json")]
- Cedula (string): identificador único de 10 dígitos.
- 6 primeros: fecha de nacimiento (AAAAMMDD).
- 7º: sexo, siglo y paridad del mes.
- 8º: orden de personas con misma fecha/sexo.
- 9º–10º: chequeo (módulo 97 de los 8 primeros).
- Ejemplo: "4412041101".
- NombreCompleto (string): nombre y apellidos separados por \u00A0.
- Métodos: obtener nombre plano, partes (nombre, apellido1, apellido2).
- ToString(): devuelve NombreCompleto.
2. UsuarioAcceso
- [Persistente("usuarios.json")]
- Cedula (string): asociada a PersonaIdentidad.
- Rol (string): derechos de acceso. Solo puede ser:
- "A" → Administrador
- "S" → Subadministrador
- "O" → Operario
- ClaveCodificada (string): autenticidad del usuario, almacenada en formato codificado.
- ToString(): devuelve el nombre completo de la persona y el rol descriptivo.
3. PersonaIdentidadFormulario
- [Persistente("personaidentidad_formulario.json")]
- Cedula (string):  [CampoFormulario("Cédula", "TextBox", true, 80)]
- Nombre (string): [CampoFormulario("Nombre", "TextBox", true, 120)]
- Apellido1 (string): [CampoFormulario("Primer Apellido", "TextBox", true,120)]
- Apellido2 (string?): [CampoFormulario("Segundo Apellido", "TextBox", false, 120)]
- FechaNacimiento (DateTime?): [CampoFormulario("Fecha de Nacimiento", "DatePicker", true, 120)]
- Sexo (SexoEnum): [CampoFormulario("Sexo", "ComboBox", true, 60)] 
- NombreCompleto (string): NombreCompleto => $"{Nombre} {Apellido1} {Apellido2}"
---

## 🏗️ Estructura del Programa

### Formularios. Definidos en la Carpeta Vistas.

### 1. Formulario Login. Inicio de sesión permite que los miembros de **UsuarioAccesos** entren al sistema con su cédula y clave.
- **Campos:**
  - `txtCedula`: TextBox para ingresar la cédula (10 dígitos, solo números, foco inicial).
  - `txtClave`: PasswordBox para ingresar la clave.
  - `txtBienvenida`: TextBox invisible que se muestra al validar la cédula, con el mensaje dinámico:
                   `Bienvenido {NombreCompleto} Rol: {UsuarioAcceso.NombreRol(Rol)} Teclee su clave:`
- **Flujo:**
  1. El usuario ingresa la cédula y presiona Enter.
  2. Si la cédula existe en `PersonaIdentidad` y `UsuarioAccesos`, se muestra el mensaje de bienvenida y el foco va a clave.
  3. El usuario ingresa la clave y presiona Enter.
  4. Si la clave coincide con la codificada en `usuarios.json`, se abre el `PanelPrincipal`.
  5. Si la clave es incorrecta, se muestra un mensaje de error y el foco vuelve a la clave.

### Otros Formularios. Definidos en otras carpetas

1 FormularioGeneral en la carpeta Servicios. Se usa para generar de forma automática los formularios donde se realizarán las acciones de Crear, Eliminar y Modificar (CEM) sobre las listas de las clases con el atributo Persistente utilizando los [CampoFormulario] de las propiedades y los métodos de validación y conversión de las clases. Se inserta en la parte inferior del FormularioContenedor ya explicado y conjuntamente hacen todo el trabajo.
La generación del de este formulario se hace utilizando métodos de la clase para obtener la lista 

FormularioGeneral<T> : IFormularioGenerico<T>
tenemos dos formularios el formulario contenedor y el formulario general en lo que hicimos ayer para la base PersonaIdentidadFormulario se genera su json en base al de personaidentidad en las demas clases su json sera el propio igual al salir se convierte su json al de personaidentidad.en las demas clases no es necesario ni convertir y para salir sera siempre solo salir ya que la lista se actualiza cuando hacemos los cambios Tambien hay ddiferencias conla cedula que en la primera se tiene que calcular la nueva cedula si cambian los campos y en las otras no creo que eso se podria unifica

Diferencias actuales
- Formulario contenedor (PersonaIdentidadFormulario)
- Su JSON no es propio: se deriva del de PersonaIdentidad.
- Al salir, se convierte de vuelta a PersonaIdentidad para mantener coherencia.
- La cédula se recalcula si cambian campos (fecha, sexo, etc.), porque es generada a partir de reglas internas.
- Formulario general (otras clases persistentes)
- Cada clase tiene su propio JSON independiente.
- No necesitan conversión al salir: la lista ya está actualizada en memoria y se guarda directamente.
- La cédula no se recalcula: se usa la que ya existe como clave de enlace.

🔧 Posible unificación
Podés definir un contrato común para todas las clases persistentes:
public interface IPersistente
{
    string Cedula { get; set; }
    string ToJson();
    void FromJson(string json);
    void RecalcularCedula(); // opcional, según clase
}


- En PersonaIdentidadFormulario, RecalcularCedula() implementa la lógica de generación.
- En las demás clases, RecalcularCedula() puede quedar vacío (no hace nada).
- El FormularioGeneral invoca siempre RecalcularCedula() al aceptar cambios → se unifica el flujo, aunque cada clase decida si recalcula o no.




### Clases adicionales
- Se irán creando nuevas clases según los datos que se requiera manejar.  
- Todas tendrán la **cédula** como clave primaria y obtendrán el nombre desde `PersonaIdentidad`.  
- Ejemplos futuros: `PersonaContacto`, `PersonaHistorial`, `PersonaDireccion`.

---

## 📈 Evolución del Proyecto
- Definición inicial de las clases base (`PersonaIdentidad`, `PersonaAutorizada`).  
- Desarrollo del primer formulario en WPF.  
- Implementación de guardado de listas al finalizar sesión.  
- Extensión progresiva con nuevas clases de datos.  
- Documentación consolidada en este README como referencia única.


ersona y se codifica con.


# Proyecto de Blindaje y Auditoría Técnica

## 📌 Estado del Proyecto
Este proyecto se encuentra en **desarrollo activo**. El objetivo principal es:
- Blindar la lógica de inicialización.
- Consolidar la normativa de generación de identidades.
- Documentar exhaustivamente cada flujo crítico.
- Eliminar documentación escueta y dispersa, centralizando todo en este README.

---

## ⚙️ Arquitectura Actual

### Componentes principales
- **Instancia BaseLocal**  
  Punto de inicialización rastreado y documentado. Se audita el momento exacto en que se dispara para evitar duplicidad.

- **Modelos de identidad**  
  Evolución progresiva:
  - Tuplas → primera aproximación rápida.
  - Records → mayor trazabilidad y compatibilidad con colecciones.
  - Clases → cuando se requieren constructores y propiedades calculadas.

- **Normativa centralizada**  
  - Uso de funciones *factory* y clases estáticas para generación de cédulas y nombres completos.  
  - Evita dispersión de reglas en distintos módulos.

---

## 🧩 Procesos en Desarrollo

- **Auditoría de inicialización**  
  Rastrear y documentar el punto exacto donde se crea la instancia clave.  
  Comparar variantes técnicas y asegurar que nunca se cree de forma ambigua.

- **Consolidación de documentación**  
  Reemplazo de archivos escuetos por un README único y completo.  
  Incluir diagramas de arquitectura y flujos.

- **Blindaje del entorno**  
  Scripts y rutinas auditables para diagnóstico y reinstalación de componentes críticos (ej. Web Experience Pack).  
  Validación de procesos residentes (`WidgetBoard`, `WidgetService`).

- **Migración progresiva de estructuras de datos**  
  Evolución hacia clases con propiedades calculadas, manteniendo compatibilidad con UI y colecciones.

---

## 📈 Evolución Documentada

- Migración de tuplas → records → clases.  
- Adopción de *factory* y clases estáticas como patrón central.  
- Auditoría de procesos residentes y reinstalación quirúrgica de componentes críticos.  
- Consolidación de documentación técnica en un único README.

---

## 🧭 Decisiones Técnicas

- **Record vs Clase**  
  - Records: trazabilidad simple, binding limpio en UI.  
  - Clases: necesarias cuando se requieren constructores y propiedades calculadas.  
  - Decisión: usar records por defecto, migrar a clases cuando el flujo lo exija.

- **Factory vs Dispersión de reglas**  
  - Factory centralizado asegura replicabilidad y evita inconsistencias.  
  - Reglas dispersas generan duplicidad y errores.  
  - Decisión: consolidar en funciones factory y clases estáticas.

---

## 🚧 Próximos Pasos

- Completar la normativa de generación de identidades.  
- Documentar flujos de inicialización con trazabilidad completa.  
- Ampliar el README con diagramas de arquitectura y flujos.  
- Validar compatibilidad con UI y binding en colecciones.  
- Automatizar auditorías de procesos críticos con scripts residentes.