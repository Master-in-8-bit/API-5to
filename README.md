# API de Clientes

API REST desarrollada en ASP.NET Core para gestionar clientes con registro, login, consulta, actualización y eliminación. La aplicación utiliza SQLite como base de datos, JWT para autenticación, Swagger para documentación y GraphQL para consultas/mutaciones adicionales.

## Descripción

Esta API permite:

- Registrar nuevos clientes.
- Iniciar sesión con email y contraseña.
- Obtener la lista de clientes.
- Consultar un cliente por su identificador.
- Actualizar datos de un cliente.
- Eliminar un cliente.
- Exponer consultas y mutaciones adicionales a través de GraphQL.

La lógica principal de negocio está separada en servicios y validaciones, y la base de datos se maneja con Entity Framework Core.

## Requisitos

- .NET SDK 9.0 o superior
- SQLite
- Visual Studio 2022 o VS Code

## Instalación y ejecución

1. Clona el repositorio o abre la carpeta del proyecto.
2. Desde la raíz del proyecto, restaura los paquetes:

```bash
dotnet restore
```

3. Ejecuta la API:

```bash
dotnet run --project ApiClientes.Api
```

4. La API queda disponible en la siguiente URL por defecto:

- HTTP: http://localhost:5176
- Swagger: http://localhost:5176/swagger
- GraphQL: http://localhost:5176/graphql

También puede levantarse con el perfil HTTPS de launchSettings, en cuyo caso la URL será:

- HTTPS: https://localhost:7120
- Swagger: https://localhost:7120/swagger

## Estructura del proyecto

```text
api-5to/
├── ApiClientes.Api/
│   ├── Controllers/
│   ├── DTOs/
│   ├── GraphQL/
│   ├── Infrastructure/
│   │   ├── Authentication/
│   │   └── Data/
│   ├── Migrations/
│   ├── Models/
│   ├── Services/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── ApiClientes.Api.csproj
│   └── Properties/
├── ApiClientes.Tests/
│   ├── ClienteServiceTests.cs
│   ├── UnitTest1.cs
│   └── ApiClientes.Tests.csproj
├── ApiClientes.slnx
├── README.md
├── clientes.db
└── .gitignore
```

## Estructura de la base de datos

La API usa SQLite con una tabla llamada `Clientes`.

### Tabla: Clientes

| Campo | Tipo | Restricciones |
|---|---|---|
| Id | integer | Primary key, autoincremental |
| Nombre | text | Obligatorio, máximo 100 caracteres |
| Email | text | Obligatorio, máximo 200 caracteres, único |
| PasswordHash | text | Obligatorio |
| FechaRegistro | datetime | Obligatorio |

### Regla importante

El campo `Email` tiene un índice único, por lo que no se permiten clientes duplicados con el mismo correo.

### Connection string

La conexión por defecto se define en `Program.cs` como:

```csharp
Data Source=clientes.db
```

Al iniciar la API, Entity Framework Core ejecuta la migración inicial y crea la base de datos automáticamente si no existe.

## Endpoints REST

La base de la ruta es:

```text
/api/clientes
```

### 1) Registro de cliente

- Método: `POST`
- URL: `/api/clientes/register`
- Autenticación: No requiere

#### JSON de entrada

```json
{
  "nombre": "Carlos",
  "email": "carlos@test.com",
  "password": "123456"
}
```

#### JSON de respuesta

```json
{
  "id": 1,
  "nombre": "Carlos",
  "email": "carlos@test.com",
  "fechaRegistro": "2026-11-06T00:00:00"
}
```

Si el email ya existe o la contraseña no cumple la validación, la API responde con `400 Bad Request`.

### 2) Login

- Método: `POST`
- URL: `/api/clientes/login`
- Autenticación: No requiere

#### JSON de entrada

```json
{
  "email": "carlos@test.com",
  "password": "123456"
}
```

#### JSON de respuesta

```json
{
  "success": true,
  "message": "Login exitoso.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "cliente": {
    "id": 1,
    "nombre": "Carlos",
    "email": "carlos@test.com",
    "fechaRegistro": "2026-11-06T00:00:00"
  }
}
```

### 3) Listar clientes

- Método: `GET`
- URL: `/api/clientes`
- Autenticación: Requiere JWT

#### Respuesta esperada

```json
[
  {
    "id": 1,
    "nombre": "Carlos",
    "email": "carlos@test.com",
    "fechaRegistro": "2026-11-06T00:00:00"
  }
]
```

### 4) Obtener cliente por ID

- Método: `GET`
- URL: `/api/clientes/{id}`
- Ejemplo: `/api/clientes/1`
- Autenticación: Requiere JWT

#### Respuesta esperada

```json
{
  "id": 1,
  "nombre": "Carlos",
  "email": "carlos@test.com",
  "fechaRegistro": "2026-11-06T00:00:00"
}
```

Si no existe el cliente, la API responde con `404 Not Found`.

### 5) Actualizar cliente

- Método: `PUT`
- URL: `/api/clientes/{id}`
- Ejemplo: `/api/clientes/1`
- Autenticación: Requiere JWT

#### JSON de entrada

```json
{
  "nombre": "Carlos Actualizado",
  "email": "carlos.nuevo@test.com",
  "password": "654321"
}
```

> El campo `password` es opcional. Si se envía, debe tener al menos 6 caracteres.

#### Respuesta esperada

```json
{
  "id": 1,
  "nombre": "Carlos Actualizado",
  "email": "carlos.nuevo@test.com",
  "fechaRegistro": "2026-11-06T00:00:00"
}
```

### 6) Eliminar cliente

- Método: `DELETE`
- URL: `/api/clientes/{id}`
- Ejemplo: `/api/clientes/1`
- Autenticación: Requiere JWT

#### Respuesta

- `204 No Content` si el cliente fue eliminado correctamente.
- `404 Not Found` si el cliente no existe.

## Endpoints con argumento en la dirección

Los endpoints que incluyen un parámetro en la ruta son:

- `GET /api/clientes/{id}`
- `PUT /api/clientes/{id}`
- `DELETE /api/clientes/{id}`

Ejemplos:

```text
GET /api/clientes/5
PUT /api/clientes/5
DELETE /api/clientes/5
```

El valor `{id}` debe ser un número entero que represente el identificador del cliente.

## GraphQL

La API también expone GraphQL en la ruta:

```text
/graphql
```

### Consulta de clientes

```graphql
query {
  clientes {
    id
    nombre
    email
    fechaRegistro
  }
}
```

### Consulta por ID

```graphql
query {
  clienteById(id: 1) {
    id
    nombre
    email
    fechaRegistro
  }
}
```

### Registro desde GraphQL

```graphql
mutation {
  registerCliente(nombre: "Carlos", email: "carlos@test.com", password: "123456") {
    id
    nombre
    email
    fechaRegistro
  }
}
```

### Login desde GraphQL

```graphql
mutation {
  loginCliente(email: "carlos@test.com", password: "123456") {
    success
    message
    token
    cliente {
      id
      nombre
      email
    }
  }
}
```

## Autenticación

La autenticación está basada en JWT.

Cuando el usuario hace login, la API devuelve un `token` en la respuesta. Ese token debe enviarse en la cabecera:

```http
Authorization: Bearer <token>
```

Ejemplo:

```http
GET /api/clientes HTTP/1.1
Host: localhost:5176
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

La validación se configura con `JwtBearer` y usa la clave, issuer y audience definidos en la configuración de la aplicación.

## Seguridad

- Las contraseñas no se guardan en texto plano.
- Se usa BCrypt para generar el `PasswordHash`.
- La autenticación protegida por JWT evita acceso no autorizado a los endpoints sensibles.
- El email es único en la base de datos.

## Pruebas

El proyecto incluye pruebas unitarias en la carpeta `ApiClientes.Tests`.

### Archivos de prueba

- `ClienteServiceTests.cs`
- `UnitTest1.cs` (archivo base de ejemplo)

### Comando para ejecutar pruebas

```bash
dotnet test
```

### Cobertura de pruebas

Las pruebas verifican principalmente:

- Registro de clientes con contraseña hasheada.
- Validación de que el password guardado no es el texto plano original.
- Inicio de sesión exitoso con credenciales correctas.
- Generación de token JWT al autenticar un cliente.

## Resultado verificado

Se ejecutó la validación real del proyecto con:

```bash
dotnet test --nologo
```

Resultado verificado: 3 pruebas ejecutadas, 3 correctas, 0 fallidas.
