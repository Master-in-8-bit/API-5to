# API de Clientes con Registro y Login

Proyecto desarrollado en ASP.NET Core Web API con Entity Framework Core, Swagger, JWT y GraphQL. La API permite registrar clientes, iniciar sesión, consultar, actualizar y eliminar información de clientes.

## Requisitos

- .NET SDK 9.0 o superior
- Visual Studio 2022 o VS Code
- SQLite

## Estructura del proyecto

```text
ApiClientes/
├── ApiClientes.Api/
│   ├── Controllers/
│   ├── DTOs/
│   ├── GraphQL/
│   ├── Infrastructure/
│   ├── Migrations/
│   ├── Models/
│   ├── Services/
│   ├── appsettings.json
│   ├── Program.cs
│   └── ApiClientes.Api.csproj
├── ApiClientes.Tests/
│   └── ClienteServiceTests.cs
├── ApiClientes.slnx
├── README.md
└── clientes.db
```

## Instalación y ejecución

1. Clonar o abrir el proyecto en la carpeta del repositorio.
2. Restaurar paquetes:

```bash
dotnet restore
```

3. Ejecutar la API:

```bash
dotnet run --project ApiClientes.Api
```

4. La API quedará disponible en:

- Swagger: https://localhost:5001/swagger
- GraphQL: https://localhost:5001/graphql

## Base de datos

La aplicación usa SQLite por defecto. Al iniciar la API, Entity Framework crea la base de datos y aplica la migración automáticamente.

## Endpoints REST

### Registro
- POST /api/clientes/register

### Login
- POST /api/clientes/login

### Listar clientes
- GET /api/clientes

### Obtener cliente por ID
- GET /api/clientes/{id}

### Editar cliente
- PUT /api/clientes/{id}

### Eliminar cliente
- DELETE /api/clientes/{id}

> Los endpoints de listado, consulta por ID, actualización y eliminación requieren autenticación JWT.

## Autenticación

El login devuelve un token JWT que se debe enviar en el header:

```http
Authorization: Bearer <token>
```

## GraphQL

La API expone operaciones GraphQL en `/graphql`.

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
  }
}
```

### Mutación de registro
```graphql
mutation {
  registerCliente(nombre: "Carlos", email: "carlos@test.com", password: "123456") {
    id
    nombre
    email
  }
}
```

### Login GraphQL
```graphql
mutation {
  loginCliente(email: "carlos@test.com", password: "123456") {
    success
    message
    token
  }
}
```

## SOLID y Clean Code aplicados

- Single Responsibility: cada servicio y controlador tiene una responsabilidad concreta.
- Open/Closed: los endpoints y servicios se pueden extender sin tocar lógica central innecesaria.
- Liskov: las implementaciones respetan la interfaz `IClienteService`.
- Interface Segregation: se usan interfaces para desacoplar servicios y token generation.
- Dependency Inversion: la capa de control depende de abstracciones (`IClienteService` / `ITokenService`).
- Clean Code: nombres descriptivos, validaciones claras, métodos pequeños y separación por capas.

## Seguridad

- Las contraseñas se almacenan con hash mediante BCrypt.
- La autenticación está protegida con JWT.
- El email se valida contra duplicados en la base de datos.

## Pruebas

Se incluyen pruebas unitarias para validar las reglas principales:

```bash
dotnet test
```

## Integrantes

- Grupo 2
- API de clientes con login y CRUD
