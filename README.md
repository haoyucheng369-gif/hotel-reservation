# HotelReservation

HotelReservation is a .NET 10 backend for a hotel room reservation core module. It is designed around Hexagonal Architecture, Tactical DDD, CQRS, MediatR, automated tests, and production-oriented observability.

The system focuses on the reservation workflow: searching availability, creating reservations, confirming reservations, cancelling reservations, and protecting the booking calendar from overlapping reservations.

## Architecture

The solution uses Hexagonal Architecture, also known as Ports and Adapters.

```text
Driving side
  HotelReservation.Adapter.WebApi
        |
        v
Hexagon core
  HotelReservation.Core
    - Domain
    - UseCases
    - Ports
        ^
        |
Driven side
  HotelReservation.Adapter.Persistence
  HotelReservation.Adapter.Notifications

Composition root
  HotelReservation.Bootstrapper
```

## Solution Structure

```text
HotelReservation/
  src/
    HotelReservation.Core/
    HotelReservation.Adapter.WebApi/
    HotelReservation.Adapter.Persistence/
    HotelReservation.Adapter.Notifications/
    HotelReservation.Bootstrapper/
  tests/
    HotelReservation.Core.Tests/
    HotelReservation.UseCases.Tests/
    HotelReservation.Integration.Tests/
  HotelReservation.slnx
```

## Dependency Rule

The core owns the business model and the application ports. Adapters depend on the core, never the opposite.

```text
HotelReservation.Core
  depends on no adapter

HotelReservation.Adapter.WebApi
  depends on Core and Bootstrapper

HotelReservation.Adapter.Persistence
  depends on Core

HotelReservation.Adapter.Notifications
  depends on Core

HotelReservation.Bootstrapper
  depends on Core and adapters
```

Forbidden dependencies:

```text
Core -> WebApi
Core -> Persistence
Core -> Notifications
Core -> EF Core
Core -> ASP.NET Core
```

## Project Responsibilities

### HotelReservation.Core

The hexagon core contains:

- Domain entities
- Value objects
- Aggregates
- Domain events
- Use cases
- Commands and queries
- MediatR handlers
- Ports required by the core

Expected internal structure:

```text
Domain/
UseCases/
Ports/
```

### HotelReservation.Adapter.WebApi

Driving adapter for HTTP clients.

Responsibilities:

- API endpoints or controllers
- request and response DTOs
- HTTP status code mapping
- ProblemDetails-style error responses
- OpenAPI configuration

Business rules do not belong in this adapter. API handlers dispatch commands and queries through MediatR.

### HotelReservation.Adapter.Persistence

Driven adapter for data storage.

Responsibilities:

- EF Core DbContext
- entity mappings
- repository implementations
- database migrations

This adapter implements repository ports defined by the core.

### HotelReservation.Adapter.Notifications

Driven adapter for reservation notifications.

Responsibilities:

- reservation confirmation notification implementation
- local no-op or console notification implementation
- future external email provider integration

This adapter implements notification ports defined by the core.

### HotelReservation.Bootstrapper

Composition root for dependency injection.

Responsibilities:

- Core service registration
- MediatR registration
- persistence adapter registration
- notification adapter registration
- infrastructure implementation binding for core ports

## Reservation Module

Initial capabilities:

- Search available rooms
- Create a reservation
- Confirm a reservation
- Cancel a reservation
- Reject overlapping reservations for the same room

Business rules:

- Check-out must be after check-in.
- Adjacent date ranges are allowed.
- Overlapping reservations for the same room are rejected.
- A new reservation starts as pending.
- A pending reservation can be confirmed.
- A pending or confirmed reservation can be cancelled.
- A cancelled reservation cannot be confirmed.
- A cancelled reservation cannot be cancelled again.

## Domain Model

Value objects:

- `DateRange`
- `GuestInfo`
- `Money`

Entities:

- `Room`
- `Reservation`

Aggregate root:

- `Reservation`

Domain events:

- `ReservationCreatedEvent`
- `ReservationConfirmedEvent`
- `ReservationCancelledEvent`

Ports:

- `IReservationRepository`
- `IRoomRepository`
- `IClock`
- `INotificationSender`

## CQRS And MediatR

Commands change state:

- `CreateReservationCommand`
- `ConfirmReservationCommand`
- `CancelReservationCommand`

Queries read state:

- `SearchAvailableRoomsQuery`
- `GetReservationByIdQuery`

Request flow:

```text
HTTP request
  -> WebApi adapter
  -> mediator.Send(...)
  -> Core use case handler
  -> Domain model
  -> Port interface
  -> Driven adapter implementation
```

MediatR pipeline behaviors will be used for cross-cutting concerns such as validation, logging, performance timing, and transaction boundaries.

## Quality Strategy

Core domain behavior is covered with fast unit tests. Use cases are covered with fake ports. End-to-end behavior is covered through integration tests against the Web API and configured adapters.

Development flow:

```text
Red -> Green -> Refactor
```

Each business rule is introduced with a failing test before or alongside the implementation.

## Implementation Plan

### 1. Hexagonal baseline

- Verify project structure
- Verify project references
- Verify build and test execution

### 2. Domain foundations

- `DateRange`
- strongly typed IDs
- `GuestInfo`
- `Room`
- `Reservation` aggregate
- domain events

### 3. Ports

- reservation repository
- room repository
- clock
- notification sender

### 4. Use cases

- create reservation command
- confirm reservation command
- cancel reservation command
- search available rooms query

### 5. Web API adapter

- reservation endpoints
- room availability endpoint
- request and response contracts
- API error mapping

### 6. Persistence adapter

- EF Core DbContext
- entity configurations
- repository implementations
- local database setup

### 7. Integration tests

- create reservation through API
- reject overlapping reservations
- search available rooms
- cancel reservation

### 8. Observability

- structured logging
- correlation ID
- health checks
- OpenTelemetry tracing
- metrics

### 9. Delivery polish

- architecture diagram
- API examples
- GitHub Actions CI
- Docker support if useful

## Commands

Build:

```bash
dotnet build HotelReservation.slnx
```

Run tests:

```bash
dotnet test HotelReservation.slnx
```

Run API:

```bash
dotnet run --project src/HotelReservation.Adapter.WebApi/HotelReservation.Adapter.WebApi.csproj
```

## Current Status

The hexagonal baseline is in place. The next implementation target is the `DateRange` value object in the core domain.
