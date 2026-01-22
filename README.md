# TodoApp

A comprehensive Todo List application built with .NET 8 backend and modern Vue 3 frontend, following Clean Architecture and Domain-Driven Design (DDD) principles with CQRS pattern using MediatR.

## 🛠️ Stack Tecnológico

### Backend

- **.NET 8** - Latest .NET framework
- **ASP.NET Core** - RESTful API framework
- **Entity Framework Core 8.0** - ORM with SQLite provider
- **MediatR 12.2.0** - CQRS implementation and mediator pattern
- **SQLite** - Lightweight database engine
- **xUnit** - Testing framework
- **Moq 4.20.70** - Mocking framework for unit tests
- **Polly v8** - Resilience and fault tolerance library

### Frontend

- **Vue 3** - Progressive JavaScript framework
- **TypeScript** - Type-safe JavaScript superset
- **Vite** - Fast build tool and dev server
- **Pinia** - State management library
- **TanStack Query (Vue Query)** - Data fetching and caching
- **Axios** - HTTP client for API requests
- **Tailwind CSS** - Utility-first CSS framework
- **Vue Router** - Official routing library
- **Vitest** - Unit testing framework
- **Playwright** - End-to-end testing framework

## 🏗️ Arquitectura

### Clean Architecture con DDD

The solution follows Clean Architecture with clear separation of concerns and dependency inversion:

- **TodoApp.Domain** - Core business logic, entities, value objects, and domain services
- **TodoApp.Application** - CQRS implementation with MediatR (Commands, Queries, Handlers)
- **TodoApp.Infrastructure** - Data access with EF Core and SQLite
- **TodoApp.Api** - RESTful API with ASP.NET Core
- **TodoApp.Console** - Console UI and dependency injection configuration
- **frontend/** - Modern Vue 3 + TypeScript frontend application

### Patrones de Diseño Implementados

- **Clean Architecture** - Separation of concerns with dependency rule
- **Domain-Driven Design (DDD)** - Rich domain model with aggregates, entities, and value objects
- **CQRS** - Command Query Responsibility Segregation using MediatR
- **Repository Pattern** - Data access abstraction
- **Factory Pattern** - TodoItem creation with business rules enforcement
- **Domain Services** - Category validation and ID generation
- **Dependency Injection** - Microsoft.Extensions.DependencyInjection
- **SOLID Principles** - Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion

### Arquitectura Frontend

- **Composition API** - Modern Vue 3 composition-based components
- **Pinia Store** - Centralized state management with TypeScript support
- **Composables** - Reusable composition functions for shared logic
- **TanStack Query** - Server state management with automatic caching and refetching
- **Component-Based** - Modular and reusable UI components

## 📦 Instalación del Proyecto

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) - Backend development
- [Node.js 18+](https://nodejs.org/) - Frontend development and package management

### 1. Clonar el Repositorio

```bash
git clone https://github.com/JulianLondonoD/TodoApp.git
cd TodoApp
```

### 2. Backend API

1. Restaurar dependencias y construir la solución:
```bash
dotnet restore
dotnet build TodoApp.sln
```

2. Ejecutar la API:
```bash
cd TodoApp.Api
dotnet run
```

La API estará disponible en `http://localhost:5000`

### 3. Frontend Application

1. Navegar al directorio frontend:
```bash
cd frontend
```

2. Instalar dependencias:
```bash
npm install
```

3. Iniciar el servidor de desarrollo:
```bash
npm run dev
```

El frontend estará disponible en `http://localhost:3000`

Para más detalles, consultar [frontend/README.md](frontend/README.md)

### 4. Aplicación Console (Opcional)

```bash
cd TodoApp.Console
dotnet run
```

## 🔧 Configuración

### Base de Datos

La aplicación utiliza SQLite con el archivo de base de datos `todoapp.db` creado automáticamente en el directorio de la aplicación. La base de datos se inicializa con categorías predefinidas en la primera ejecución.

### Configuración de Desarrollo

El proyecto incluye políticas de resiliencia configurables en `appsettings.json`:

- **Retry Policy**: Reintentos automáticos con backoff exponencial
- **Circuit Breaker**: Protección contra fallos en cascada
- **Timeout Protection**: Timeouts configurables para operaciones de base de datos

### Variables de Entorno

**Frontend** (archivo `.env.development`):
```
VITE_API_BASE_URL=http://localhost:5000
```

**Frontend** (archivo `.env.production`):
```
VITE_API_BASE_URL=https://your-production-api.com
```

## 🚀 Comandos Útiles

### Backend

```bash
# Restaurar dependencias
dotnet restore

# Construir la solución
dotnet build TodoApp.sln

# Ejecutar tests
dotnet test TodoApp.sln

# Ejecutar la API
cd TodoApp.Api && dotnet run

# Ejecutar la consola
cd TodoApp.Console && dotnet run
```

### Frontend

```bash
# Instalar dependencias
npm install

# Servidor de desarrollo
npm run dev

# Build de producción
npm run build

# Type checking
npm run type-check

# Tests unitarios
npm run test

# Tests con UI
npm run test:ui

# Tests de cobertura
npm run test:coverage

# Tests E2E
npm run test:e2e

# Formato de código
npm run format

# Vista previa de build
npm run preview
```


