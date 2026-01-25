//using FluentAssertions;
//using TaskBoard.Core.Models;
//using TaskBoard.Core.Models.Enums;
//using TaskBoard.Infrastructure.Repositories;
//using TaskBoard.Tests.Fixtures;
//using Xunit;

//namespace TaskBoard.Tests.Advanced;

///// <summary>
///// Esercizi Modulo 7: Test Avanzati
/////
///// ISTRUZIONI:
///// - Questi test coprono scenari avanzati: concorrenza, eccezioni, validazione enum
///// - Focus su comportamenti edge-case e race condition
///// - Esegui: dotnet test --filter "FullyQualifiedName~AdvancedTests"
/////
///// NOTA: I test di concorrenza usano MongoDB reale via Testcontainers!
///// </summary>
//[Collection("MongoDB")]
//public class AdvancedTests : IAsyncLifetime
//{
//    private readonly MongoDbFixture _fixture;
//    private readonly MongoTaskRepository _sut;

//    public AdvancedTests(MongoDbFixture fixture)
//    {
//        _fixture = fixture;
//        _sut = new MongoTaskRepository(_fixture.GetSettings());
//    }

//    public async Task InitializeAsync()
//    {
//        await _fixture.ClearCollectionAsync();
//    }

//    public Task DisposeAsync() => Task.CompletedTask;

//    /// <summary>
//    /// Modulo 7: Test Avanzati
//    /// Esercizio 1: Concorrenza - Last Writer Wins
//    ///
//    /// Obiettivo: Verificare che con update concorrenti, l'ultimo scrittura vinca
//    /// </summary>
//    [Fact]
//    public async Task ConcurrentUpdates_LastWriterWins()
//    {
//        // Arrange
//        // TODO: Crea un task iniziale
//        // HINT: var task = new TaskItem
//        //       {
//        //           Title = "Original",
//        //           Description = "Original Desc",
//        //           Status = KanbanStatus.Todo,
//        //           Priority = TaskPriority.Medium,
//        //           CreatedAt = DateTime.UtcNow,
//        //           UpdatedAt = DateTime.UtcNow
//        //       };
//        //       var created = await _sut.CreateAsync(task);

//        // TODO: Crea due versioni di update diverse
//        // HINT: var update1 = new TaskItem
//        //       {
//        //           Title = "Update 1",
//        //           Description = "Desc 1",
//        //           Status = KanbanStatus.InProgress,
//        //           Priority = TaskPriority.High,
//        //           CreatedAt = created.CreatedAt,
//        //           UpdatedAt = DateTime.UtcNow
//        //       };
//        //
//        //       var update2 = new TaskItem
//        //       {
//        //           Title = "Update 2",
//        //           Description = "Desc 2",
//        //           Status = KanbanStatus.Done,
//        //           Priority = TaskPriority.Low,
//        //           CreatedAt = created.CreatedAt,
//        //           UpdatedAt = DateTime.UtcNow
//        //       };

//        // Act
//        // TODO: Esegui i due update in parallelo usando Task.WhenAll
//        // HINT: var task1 = _sut.UpdateAsync(created.Id, update1);
//        //       var task2 = _sut.UpdateAsync(created.Id, update2);
//        //       await Task.WhenAll(task1, task2);

//        // Assert
//        // TODO: Verifica che il task finale abbia uno dei due titoli
//        // HINT: var final = await _sut.GetByIdAsync(created.Id);
//        //       final.Should().NotBeNull();
//        //       final!.Title.Should().BeOneOf("Update 1", "Update 2");

//        throw new NotImplementedException("Esercizio 1: Implementa il test concorrenza update");
//    }

//    /// <summary>
//    /// Modulo 7: Test Avanzati
//    /// Esercizio 2: Concorrenza - Solo un Delete ha successo
//    ///
//    /// Obiettivo: Verificare che con delete concorrenti, solo uno restituisca true
//    /// </summary>
//    [Fact]
//    public async Task ConcurrentDeletes_OnlyOneSucceeds()
//    {
//        // Arrange
//        // TODO: Crea un task da eliminare
//        // HINT: var task = new TaskItem
//        //       {
//        //           Title = "To Delete",
//        //           Description = "Will be deleted",
//        //           Status = KanbanStatus.Todo,
//        //           Priority = TaskPriority.Medium,
//        //           CreatedAt = DateTime.UtcNow,
//        //           UpdatedAt = DateTime.UtcNow
//        //       };
//        //       var created = await _sut.CreateAsync(task);

//        // Act
//        // TODO: Esegui due delete in parallelo
//        // HINT: var delete1Task = _sut.DeleteAsync(created.Id);
//        //       var delete2Task = _sut.DeleteAsync(created.Id);
//        //       var results = await Task.WhenAll(delete1Task, delete2Task);

//        // Assert
//        // TODO: Verifica che:
//        // - Esattamente uno dei risultati sia true
//        // - Esattamente uno dei risultati sia false
//        // - Il task non esista più nel database
//        // HINT: results.Count(r => r).Should().Be(1);
//        //       results.Count(r => !r).Should().Be(1);
//        //       var deleted = await _sut.GetByIdAsync(created.Id);
//        //       deleted.Should().BeNull();

//        throw new NotImplementedException("Esercizio 2: Implementa il test concorrenza delete");
//    }

//    /// <summary>
//    /// Modulo 7: Test Avanzati
//    /// Esercizio 3: CreateAsync con titolo null lancia eccezione
//    ///
//    /// Obiettivo: Verificare che la validazione lanci un'eccezione appropriata
//    /// </summary>
//    [Fact]
//    public async Task CreateAsync_WithNullTitle_ThrowsException()
//    {
//        // Arrange
//        // TODO: Crea un task con Title null
//        // HINT: var task = new TaskItem
//        //       {
//        //           Title = null!,
//        //           Description = "Description",
//        //           Status = KanbanStatus.Todo,
//        //           Priority = TaskPriority.Medium,
//        //           CreatedAt = DateTime.UtcNow,
//        //           UpdatedAt = DateTime.UtcNow
//        //       };

//        // Act & Assert
//        // TODO: Usa FluentAssertions per verificare che venga lanciata un'eccezione
//        // Metodo 1 - Assert.ThrowsAsync:
//        // HINT: await Assert.ThrowsAsync<Exception>(() => _sut.CreateAsync(task));
//        //
//        // Metodo 2 - FluentAssertions (preferito):
//        // HINT: var act = () => _sut.CreateAsync(task);
//        //       await act.Should().ThrowAsync<Exception>();

//        throw new NotImplementedException("Esercizio 3: Implementa il test eccezione");
//    }

//    /// <summary>
//    /// Modulo 7: Test Avanzati
//    /// Esercizio 4: Verifica valori numerici dell'enum TaskPriority
//    ///
//    /// Obiettivo: Verificare che l'enum TaskPriority abbia i valori numerici corretti
//    /// </summary>
//    [Fact]
//    public void TaskPriority_HasCorrectNumericValues()
//    {
//        // Assert
//        // TODO: Verifica che i valori numerici dell'enum siano:
//        // - Low = 1
//        // - Medium = 2
//        // - High = 3
//        // HINT: ((int)TaskPriority.Low).Should().Be(1);
//        //       ((int)TaskPriority.Medium).Should().Be(2);
//        //       ((int)TaskPriority.High).Should().Be(3);

//        throw new NotImplementedException("Esercizio 4: Implementa il test valori enum");
//    }
//}
