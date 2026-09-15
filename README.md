# Devsu - Prueba Tecnica Backend

Sistema de gestion de clientes y cuentas bancarias, implementado como 2 microservicios independientes en .NET 10, siguiendo Clean Architecture (Domain/Application/Infrastructure/Api en proyectos separados), comunicacion asincrona via RabbitMQ y despliegue en contenedores Docker.

## Arquitectura

- **`Devsu.Clients`** - CRUD de cliente/persona
- **`Devsu.Accounts`** - CRU de cuenta/movimiento mas el reporte de estado de cuenta
- **Clean Architecture** - cada microservicio separado en 4 capas (Domain, Application, Infrastructure, Api), cada una en su propio proyecto
- **Database per Service** - cada microservicio tiene su propia base de datos en PostgreSQL, sin Foreign Key fisica entre ellas
- **RabbitMQ + MassTransit** - comunicacion asincrona: Clients publica eventos que Accounts consume para mantener una copia local de los clientes (`known_clients`)
- **`Contracts`** - proyecto compartido minimo, solo con la forma de los eventos, sin logica de negocio

## Requisitos

- [Docker](https://www.docker.com/) y Docker Compose
- Si se va a modificar codigo: [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Despliegue

### 1. Configurar variables de entorno

```bash
cp .env.template .env
```

### 2. Levantar el proyecto (se recomienda con Docker)

```bash
docker compose up -d --build
```

Esto levanta Postgres, RabbitMQ, y las 2 APIs (`Devsu.Clients` en el puerto `5047`, `Devsu.Accounts` en el `5004`).

### 3. Correr las migraciones

Las migraciones son manuales, no se aplican solas al arrancar:

```bash
cd Devsu.Clients
dotnet ef database update --project src/Infrastructure --startup-project src/Api

cd ../Devsu.Accounts
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Si no tienes el SDK de .NET instalado, tambien se puede aplicar el esquema corriendo `BaseDatos.sql` directo contra cada base.

### 4. Ejecutar el seed (data de prueba del enunciado)

```bash
curl -X POST http://localhost:5047/seed
curl -X POST http://localhost:5004/seed
```

El de Clients tiene que correr primero.

### 5. Probar los endpoints

Se incluye una coleccion de Postman con todos los endpoints ya armados (`coleccion-postman/`), lista para importar junto con su environment.
