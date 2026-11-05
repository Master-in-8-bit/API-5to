# API 5to

API REST para gestión de clientes con autenticación JWT, SQLite y Swagger. El proyecto incluye registro, login, consulta y administración de clientes.

## Descripción

Esta API permite:

- Registrar nuevos clientes
- Iniciar sesión con email y contraseña
- Generar token JWT para autenticación
- Consultar todos los clientes
- Consultar un cliente por ID
- Actualizar datos de un cliente
- Eliminar un cliente

## Stack

- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- SQLite
- JWT Bearer
- Swagger / OpenAPI

## Requisitos

- .NET SDK 10.0 o superior
- Un editor como VS Code o Visual Studio

## Estructura del proyecto

- `Program.cs`: configuración de servicios, autenticación, base de datos y pipeline HTTP
- `Controllers/`: endpoints de la API
- `Services/`: lógica de negocio
- `Repositories/`: acceso a datos
- `Data/`: DbContext
- `Models/`: entidades del dominio
- `DTOs/`: request y response models
- `Migrations/`: migraciones de Entity Framework

## Configuración

La API usa SQLite por defecto con la cadena de conexión en `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=clientes.db"
}
```

También se configuran los valores JWT en el mismo archivo:

```json
"Jwt": {
  "Key": "ClaveSuperSecretaParaApiDeClientes1234567890",
  "Issuer": "APIClientes",
  "Audience": "APIClientesClient"
}
```

> En un entorno real, se recomienda usar secretos o variables de entorno en lugar de valores fijos.

## Ejecutar la API

Desde la raíz del proyecto:

```bash
dotnet restore
dotnet run
```

La aplicación puede abrir Swagger en el entorno de desarrollo y creará la base de datos automáticamente si no existe.

## Endpoints

### Autenticación

| Método | Ruta | Descripción |
| --- | --- | --- |
| POST | `/api/Clientes/register` | Registra un nuevo cliente |
| POST | `/api/Clientes/login` | Inicia sesión y devuelve un token JWT |

### Clientes

| Método | Ruta | Descripción |
| --- | --- | --- |
| GET | `/api/Clientes` | Obtiene todos los clientes (requiere autenticación) |
| GET | `/api/Clientes/{id}` | Obtiene un cliente por ID (requiere autenticación) |
| PUT | `/api/Clientes/{id}` | Actualiza un cliente existente (requiere autenticación) |
| DELETE | `/api/Clientes/{id}` | Elimina un cliente (requiere autenticación) |

## Modelos principales

### Registro

```json
{
  "nombre": "Juan Pérez",
  "email": "juan@correo.com",
  "password": "123456"
}
```

### Login

```json
{
  "email": "juan@correo.com",
  "password": "123456"
}
```

### Respuesta de login

```json
{
  "token": "jwt-token",
  "id": 1,
  "nombre": "Juan Pérez",
  "email": "juan@correo.com",
  "fechaRegistro": "2026-11-05T00:00:00"
}
```

## Seguridad

- Los endpoints de consulta y actualización requieren JWT.
- El token se valida contra issuer, audience y firma.
- La clave `Jwt:Key` debe mantenerse privada.

## Notas

- La base de datos se crea automáticamente con `EnsureCreated()` en `Program.cs`.
- El proyecto incluye Swagger habilitado en desarrollo para facilitar pruebas.
