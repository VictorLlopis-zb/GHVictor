readme
## 🧬 Clase base de datos personales

La clase principal representa los datos personales de cada individuo. Se divide en dos grupos:

### 🔹 Grupo 1: Identidad
- `Nombre1` (obligatorio, inicia en mayúscula)
- `Nombre2` (opcional, inicia en mayúscula)
- `Apellido1` (obligatorio, inicia en mayúscula)
- `Apellido2` (opcional, inicia en mayúscula)

### 🔹 Grupo 2: Fecha de nacimiento
- `Día`, `Mes`, `Año` (listas desplegables)
- `Sexo` (codificado como 0/1)

### 🔹 Cédula generada automáticamente
Formato: `xxyyzzsgotcc` (12 dígitos)
- Año, mes, día
- Siglo (0/1), género (0/1)
- Orden (1–9)
- Status (fallecido, emigrado, etc.)
- Dígitos de control

La clase será extensible mediante subclases que implementen una interfaz común.