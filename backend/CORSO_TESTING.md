# Corso: Testing con xUnit in .NET

Corso pratico sul testing in .NET usando xUnit, basato sull'applicazione Kanban Board con architettura Clean Architecture.

## Modulo 1: Fondamenti xUnit

- Anatomia di un test: Arrange, Act, Assert
- `[Fact]` vs `[Theory]` con `[InlineData]`
- Convenzioni di naming dei test
- **Esercizio:** Test base per `TaskService.ParsePriority()` e `ParseStatus()`

## Modulo 2: Unit Test del Domain Layer

- Testare la logica di business isolata
- Test del `TaskService` con mock di `ITaskRepository`
- Introduzione a **Moq**
- **Esercizi:**
  - `CreateAsync` crea task con valori corretti
  - `PatchAsync` aggiorna solo i campi specificati
  - `UpdateAsync` ritorna null se task non esiste

## Modulo 3: Test dei Controller (Unit)

- Mock di `ITaskService`
- Verifica status code e response body
- Test dei casi di errore (404, 400)
- **Esercizi:**
  - `GetById` ritorna 404 se non trovato
  - `Create` ritorna 400 se title mancante
  - `Delete` ritorna 204 su successo

## Modulo 4: Integration Test con WebApplicationFactory

- `WebApplicationFactory<Program>` per test end-to-end
- Configurare servizi mock per i test
- `HttpClient` per chiamare gli endpoint
- **Esercizi:**
  - Test completo CRUD via HTTP
  - Verifica serializzazione JSON camelCase

## Modulo 5: Integration Test con Database Reale

- **Testcontainers** per MongoDB in Docker
- Setup/teardown del database per test
- Test del `MongoTaskRepository` reale
- **Esercizi:**
  - Filtri per status, priority, archived
  - Ricerca per titolo case-insensitive
  - Ordinamento per createdAt, priority

## Modulo 6: Fixture e Condivisione Risorse

- `IClassFixture<T>` per risorse condivise nella classe
- `ICollectionFixture<T>` per risorse tra classi
- `IAsyncLifetime` per setup/teardown async
- **Esercizio:** Fixture MongoDB condivisa tra test

## Modulo 7: Test Avanzati

- Test di concorrenza
- Verifica delle eccezioni con `Assert.Throws`
- Custom assertions e extension methods
- **Esercizio:** Test che due update concorrenti non perdano dati

## Modulo 8: Code Coverage e Best Practices

- Configurare coverage con Coverlet
- Analisi del report con ReportGenerator
- Cosa testare e cosa no
- Evitare test fragili (flaky tests)

---

## Struttura Progetto Test Suggerita

```
TaskBoard.Tests/
├── Unit/
│   ├── Core/
│   │   └── TaskServiceTests.cs
│   └── WebApi/
│       └── TasksControllerTests.cs
├── Integration/
│   ├── Api/
│   │   └── TasksEndpointTests.cs
│   └── Repositories/
│       └── MongoTaskRepositoryTests.cs
└── Fixtures/
    ├── MongoDbFixture.cs
    └── WebAppFixture.cs
```

## Pacchetti NuGet Richiesti

```xml
<PackageReference Include="xunit" Version="2.9.0" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.0" />
<PackageReference Include="Moq" Version="4.20.70" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.8" />
<PackageReference Include="Testcontainers.MongoDb" Version="3.9.0" />
<PackageReference Include="coverlet.collector" Version="6.0.2" />
```

## Comandi Utili

```bash
# Eseguire tutti i test
dotnet test

# Eseguire test con output dettagliato
dotnet test --logger "console;verbosity=detailed"

# Eseguire test di una classe specifica
dotnet test --filter "FullyQualifiedName~TaskServiceTests"

# Eseguire test con coverage
dotnet test --collect:"XPlat Code Coverage"

# Generare report HTML coverage
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```
