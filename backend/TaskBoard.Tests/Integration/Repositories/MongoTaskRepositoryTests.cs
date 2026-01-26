//using FluentAssertions;
//using TaskBoard.Core.Models;
//using TaskBoard.Core.Models.Enums;
//using TaskBoard.Core.Ports;
//using TaskBoard.Infrastructure.Repositories;
//using TaskBoard.Tests.Fixtures;
//using Xunit;

//namespace TaskBoard.Tests.Integration.Repositories;

///// <summary>
///// Esercizi Modulo 5: Integration Test MongoDB
/////
///// ISTRUZIONI:
///// - Questi test usano Testcontainers per avviare un MongoDB reale
///// - La fixture MongoDbFixture gestisce il ciclo di vita del container
///// - Focus su query, filtri e operazioni CRUD reali
///// - Esegui: dotnet test --filter "FullyQualifiedName~MongoTaskRepositoryTests"
/////
///// NOTA: I test richiedono Docker in esecuzione!
///// </summary>
//[Collection("MongoDB")]
//public class MongoTaskRepositoryTests : IAsyncLifetime
//{
//    private readonly MongoDbFixture _fixture;
//    private readonly MongoTaskRepository _sut;

//    public MongoTaskRepositoryTests(MongoDbFixture fixture)
//    {
//        _fixture = fixture;
//        _sut = new MongoTaskRepository(_fixture.GetSettings());
//    }

//    public async Task InitializeAsync()
//    {
//        // Pulisce la collection prima di ogni test
//        await _fixture.ClearCollectionAsync();
//    }

//    public Task DisposeAsync() => Task.CompletedTask;

//    /// <summary>
//    /// Modulo 5: Integration Test MongoDB
//    /// Esercizio 1: Filtro per status
//    ///
//    /// Obiettivo: Verificare che GetAllAsync filtri correttamente per status
//    /// </summary>
//    [Fact]
//    public async Task GetAllAsync_WithStatusFilter_ReturnsMatchingTasks()
//    {
//        // Arrange
//        // TODO: Inserisci task con status diversi
//        // HINT: await _fixture.InsertTaskAsync("Todo Task 1", status: "todo");
//        //       await _fixture.InsertTaskAsync("Todo Task 2", status: "todo");
//        //       await _fixture.InsertTaskAsync("Done Task", status: "done");

//        // TODO: Crea un TaskQueryParams con filtro status
//        // HINT: var query = new TaskQueryParams(Status: KanbanStatus.Todo);

//        // Act
//        // TODO: Chiama GetAllAsync con il filtro
//        // HINT: var result = await _sut.GetAllAsync(query);

//        // Assert
//        // TODO: Verifica che:
//        // - Il risultato contenga solo 2 task
//        // - Tutti i task abbiano Status = KanbanStatus.Todo
//        // HINT: result.Should().HaveCount(2);
//        //       result.Should().AllSatisfy(t => t.Status.Should().Be(KanbanStatus.Todo));

//        throw new NotImplementedException("Esercizio 1: Implementa il test filtro status");
//    }

//    /// <summary>
//    /// Modulo 5: Integration Test MongoDB
//    /// Esercizio 2: Filtro per priority
//    ///
//    /// Obiettivo: Verificare che GetAllAsync filtri correttamente per priority
//    /// </summary>
//    [Fact]
//    public async Task GetAllAsync_WithPriorityFilter_ReturnsMatchingTasks()
//    {
//        // Arrange
//        // TODO: Inserisci task con priority diverse
//        // HINT: await _fixture.InsertTaskAsync("High Priority", priority: "high");
//        //       await _fixture.InsertTaskAsync("Low Priority", priority: "low");
//        //       await _fixture.InsertTaskAsync("Another High", priority: "high");

//        // TODO: Crea un TaskQueryParams con filtro priority
//        // HINT: var query = new TaskQueryParams(Priority: TaskPriority.High);

//        // Act
//        // TODO: Chiama GetAllAsync
//        // HINT: var result = await _sut.GetAllAsync(query);

//        // Assert
//        // TODO: Verifica che restituisca solo i 2 task con priority High
//        // HINT: result.Should().HaveCount(2);
//        //       result.Should().AllSatisfy(t => t.Priority.Should().Be(TaskPriority.High));

//        throw new NotImplementedException("Esercizio 2: Implementa il test filtro priority");
//    }

//    /// <summary>
//    /// Modulo 5: Integration Test MongoDB
//    /// Esercizio 3: Filtro per archived (Theory)
//    ///
//    /// Obiettivo: Verificare i diversi valori del filtro archived: "true", "false", "all"
//    /// </summary>
//    [Theory]
//    [InlineData("true", 2)]   // Solo task archiviati
//    [InlineData("false", 3)]  // Solo task non archiviati
//    [InlineData("all", 5)]    // Tutti i task
//    public async Task GetAllAsync_WithArchivedFilter_ReturnsCorrectTasks(string archivedFilter, int expectedCount)
//    {
//        // Arrange
//        // TODO: Inserisci 3 task attivi e 2 archiviati
//        // HINT: await _fixture.InsertTaskAsync("Active 1", archived: false);
//        //       await _fixture.InsertTaskAsync("Active 2", archived: false);
//        //       await _fixture.InsertTaskAsync("Active 3", archived: false);
//        //       await _fixture.InsertTaskAsync("Archived 1", archived: true);
//        //       await _fixture.InsertTaskAsync("Archived 2", archived: true);

//        // TODO: Crea il TaskQueryParams con il filtro archived
//        // HINT: var query = new TaskQueryParams(Archived: archivedFilter);

//        // Act
//        // TODO: Chiama GetAllAsync
//        // HINT: var result = await _sut.GetAllAsync(query);

//        // Assert
//        // TODO: Verifica che il count corrisponda a expectedCount
//        // HINT: result.Should().HaveCount(expectedCount);

//        throw new NotImplementedException("Esercizio 3: Implementa il test filtro archived");
//    }

//    /// <summary>
//    /// Modulo 5: Integration Test MongoDB
//    /// Esercizio 4: Search case-insensitive
//    ///
//    /// Obiettivo: Verificare che la ricerca testuale sia case-insensitive
//    /// </summary>
//    [Fact]
//    public async Task GetAllAsync_WithSearch_FindsCaseInsensitive()
//    {
//        // Arrange
//        // TODO: Inserisci task con titoli in case diverse
//        // HINT: await _fixture.InsertTaskAsync("IMPORTANT Task");
//        //       await _fixture.InsertTaskAsync("Important Note");
//        //       await _fixture.InsertTaskAsync("Something else");

//        // TODO: Crea il query con search in minuscolo
//        // HINT: var query = new TaskQueryParams(Search: "important", Archived: "all");

//        // Act
//        // TODO: Chiama GetAllAsync
//        // HINT: var result = await _sut.GetAllAsync(query);

//        // Assert
//        // TODO: Verifica che trovi entrambi i task con "important" (case-insensitive)
//        // HINT: result.Should().HaveCount(2);
//        //       result.Should().AllSatisfy(t =>
//        //           t.Title.Should().ContainEquivalentOf("important"));

//        throw new NotImplementedException("Esercizio 4: Implementa il test search case-insensitive");
//    }

//    /// <summary>
//    /// Modulo 5: Integration Test MongoDB
//    /// Esercizio 5: Ordinamento per CreatedAt (Theory)
//    ///
//    /// Obiettivo: Verificare l'ordinamento ascendente e discendente per data creazione
//    /// </summary>
//    [Theory]
//    [InlineData("asc")]
//    [InlineData("desc")]
//    public async Task GetAllAsync_OrderByCreatedAt_ReturnsSorted(string order)
//    {
//        // Arrange
//        // TODO: Inserisci task con un piccolo delay per avere timestamp diversi
//        // HINT: await _fixture.InsertTaskAsync("First");
//        //       await Task.Delay(10);
//        //       await _fixture.InsertTaskAsync("Second");
//        //       await Task.Delay(10);
//        //       await _fixture.InsertTaskAsync("Third");

//        // TODO: Crea il query con ordinamento
//        // HINT: var query = new TaskQueryParams(OrderBy: "createdAt", Order: order, Archived: "all");

//        // Act
//        // TODO: Chiama GetAllAsync e converti in lista
//        // HINT: var result = (await _sut.GetAllAsync(query)).ToList();

//        // Assert
//        // TODO: Verifica l'ordine corretto in base ad "asc" o "desc"
//        // HINT: if (order == "asc")
//        //       {
//        //           result[0].Title.Should().Be("First");
//        //           result[2].Title.Should().Be("Third");
//        //       }
//        //       else
//        //       {
//        //           result[0].Title.Should().Be("Third");
//        //           result[2].Title.Should().Be("First");
//        //       }

//        throw new NotImplementedException("Esercizio 5: Implementa il test ordinamento CreatedAt");
//    }

//    /// <summary>
//    /// Modulo 5: Integration Test MongoDB
//    /// Esercizio 6: Ordinamento per Priority (Theory)
//    ///
//    /// Obiettivo: Verificare l'ordinamento ascendente e discendente per priorità
//    /// </summary>
//    [Theory]
//    [InlineData("asc")]
//    [InlineData("desc")]
//    public async Task GetAllAsync_OrderByPriority_ReturnsSorted(string order)
//    {
//        // Arrange
//        // TODO: Inserisci task con priorità diverse
//        // HINT: await _fixture.InsertTaskAsync("Medium Task", priority: "medium");
//        //       await _fixture.InsertTaskAsync("High Task", priority: "high");
//        //       await _fixture.InsertTaskAsync("Low Task", priority: "low");

//        // TODO: Crea il query con ordinamento per priority
//        // HINT: var query = new TaskQueryParams(OrderBy: "priority", Order: order, Archived: "all");

//        // Act
//        // TODO: Chiama GetAllAsync
//        // HINT: var result = (await _sut.GetAllAsync(query)).ToList();

//        // Assert
//        // TODO: Verifica l'ordine corretto
//        // HINT: if (order == "asc")
//        //       {
//        //           result[0].Priority.Should().Be(TaskPriority.Low);
//        //           result[2].Priority.Should().Be(TaskPriority.High);
//        //       }
//        //       else
//        //       {
//        //           result[0].Priority.Should().Be(TaskPriority.High);
//        //           result[2].Priority.Should().Be(TaskPriority.Low);
//        //       }

//        throw new NotImplementedException("Esercizio 6: Implementa il test ordinamento Priority");
//    }

//    /// <summary>
//    /// Modulo 5: Integration Test MongoDB
//    /// Esercizio 7: CreateAsync genera un ObjectId valido
//    ///
//    /// Obiettivo: Verificare che MongoDB generi un ObjectId di 24 caratteri hex
//    /// </summary>
//    [Fact]
//    public async Task CreateAsync_GeneratesValidObjectId()
//    {
//        // Arrange
//        var task = new TaskItem
//        {
//            Title = "New Task",
//            Description = "Description",
//            Status = KanbanStatus.Todo,
//            Priority = TaskPriority.Medium,
//            CreatedAt = DateTime.UtcNow,
//            UpdatedAt = DateTime.UtcNow
//        };

//        // Act
//        // TODO: Chiama CreateAsync
//        // HINT: var result = await _sut.CreateAsync(task);

//        // Assert
//        // TODO: Verifica che l'ID sia un ObjectId valido:
//        // - Non nullo o vuoto
//        // - Lunghezza 24 caratteri
//        // - Solo caratteri esadecimali (a-f, 0-9)
//        // HINT: result.Id.Should().NotBeNullOrEmpty();
//        //       result.Id.Should().HaveLength(24);
//        //       result.Id.Should().MatchRegex("^[a-f0-9]{24}$");

//        throw new NotImplementedException("Esercizio 7: Implementa il test ObjectId valido");
//    }

//    /// <summary>
//    /// Modulo 5: Integration Test MongoDB
//    /// Esercizio 8: DeleteAsync con ObjectId invalido restituisce false
//    ///
//    /// Obiettivo: Verificare che DeleteAsync non lanci eccezioni con ID invalidi
//    /// </summary>
//    [Fact]
//    public async Task DeleteAsync_WithInvalidObjectId_ReturnsFalse()
//    {
//        // Arrange
//        // TODO: Crea un ID che non è un ObjectId valido
//        // HINT: var invalidId = "not-a-valid-objectid";

//        // Act
//        // TODO: Chiama DeleteAsync con l'ID invalido
//        // HINT: var result = await _sut.DeleteAsync(invalidId);

//        // Assert
//        // TODO: Verifica che restituisca false (senza lanciare eccezioni)
//        // HINT: result.Should().BeFalse();

//        throw new NotImplementedException("Esercizio 8: Implementa il test DeleteAsync con ID invalido");
//    }
//}
