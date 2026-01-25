//using FluentAssertions;
//using Moq;
//using TaskBoard.Core.Models;
//using TaskBoard.Core.Models.Enums;
//using TaskBoard.Core.Ports;
//using TaskBoard.Core.Services;
//using Xunit;

//namespace TaskBoard.Tests.Unit.Core;

///// <summary>
///// Esercizi Modulo 1 e 2: Fondamenti xUnit e Unit Test Domain Layer
/////
///// ISTRUZIONI:
///// - Ogni test ha un TODO che indica cosa implementare
///// - Il primo esercizio usa Assert classico (xUnit nativo), tutti gli altri FluentAssertions
///// - Rimuovi "throw new NotImplementedException(...)" quando completi l'esercizio
///// - Esegui: dotnet test --filter "FullyQualifiedName~TaskServiceTests"
///// </summary>
//public class TaskServiceTests
//{
//    private readonly Mock<ITaskRepository> _repositoryMock;
//    private readonly TaskService _sut;

//    public TaskServiceTests()
//    {
//        _repositoryMock = new Mock<ITaskRepository>();
//        _sut = new TaskService(_repositoryMock.Object);
//    }

//    #region Modulo 1: Fondamenti xUnit

//    /// <summary>
//    /// Modulo 1: Fondamenti xUnit
//    /// Esercizio 1: Test con Assert classico (xUnit nativo)
//    ///
//    /// Obiettivo: Verificare che ParsePriority("low") restituisca TaskPriority.Low
//    /// </summary>
//    [Fact]
//    public void ParsePriority_WithLowValue_ReturnsLow()
//    {
//        // Arrange
//        // TODO: Crea una variabile input con valore "low"
//        // HINT: var input = "low";

//        // Act
//        // TODO: Chiama TaskService.ParsePriority(input) e salva il risultato
//        // HINT: var result = TaskService.ParsePriority(input);

//        // Assert
//        // TODO: Usa Assert.Equal per verificare che il risultato sia TaskPriority.Low
//        // HINT: Assert.Equal(TaskPriority.Low, result);

//        throw new NotImplementedException("Esercizio 1: Implementa il test usando Assert.Equal()");
//    }

//    /// <summary>
//    /// Modulo 1: Fondamenti xUnit
//    /// Esercizio 2: Theory con InlineData e FluentAssertions
//    ///
//    /// Obiettivo: Testare ParsePriority con diversi input usando [Theory] e [InlineData]
//    /// </summary>
//    [Theory]
//    [InlineData("low", TaskPriority.Low)]
//    [InlineData("medium", TaskPriority.Medium)]
//    [InlineData("high", TaskPriority.High)]
//    [InlineData("LOW", TaskPriority.Low)]
//    [InlineData("MEDIUM", TaskPriority.Medium)]
//    [InlineData("invalid", TaskPriority.Medium)] // default per input non validi
//    public void ParsePriority_WithVariousInputs_ReturnsCorrectPriority(string input, TaskPriority expected)
//    {
//        // Act
//        // TODO: Chiama TaskService.ParsePriority(input) e salva il risultato
//        // HINT: var result = TaskService.ParsePriority(input);

//        // Assert
//        // TODO: Usa FluentAssertions per verificare che result sia uguale a expected
//        // HINT: result.Should().Be(expected);

//        throw new NotImplementedException("Esercizio 2: Implementa il test usando FluentAssertions");
//    }

//    /// <summary>
//    /// Modulo 1: Fondamenti xUnit
//    /// Esercizio 3: Theory per ParseStatus
//    ///
//    /// Obiettivo: Testare ParseStatus con vari input validi
//    /// </summary>
//    [Theory]
//    [InlineData("todo", KanbanStatus.Todo)]
//    [InlineData("in-progress", KanbanStatus.InProgress)]
//    [InlineData("done", KanbanStatus.Done)]
//    [InlineData("TODO", KanbanStatus.Todo)]
//    [InlineData("IN-PROGRESS", KanbanStatus.InProgress)]
//    public void ParseStatus_WithValidValues_ReturnsCorrectStatus(string input, KanbanStatus expected)
//    {
//        // Act
//        // TODO: Chiama TaskService.ParseStatus(input) e salva il risultato
//        // HINT: var result = TaskService.ParseStatus(input);

//        // Assert
//        // TODO: Usa FluentAssertions per verificare che result sia uguale a expected
//        // HINT: result.Should().Be(expected);

//        throw new NotImplementedException("Esercizio 3: Implementa il test per ParseStatus");
//    }

//    /// <summary>
//    /// Modulo 1: Fondamenti xUnit
//    /// Esercizio 4: Test del valore di default
//    ///
//    /// Obiettivo: Verificare che ParseStatus restituisca KanbanStatus.Todo per input non validi
//    /// </summary>
//    [Fact]
//    public void ParseStatus_WithInvalidValue_ReturnsDefault()
//    {
//        // Arrange
//        // TODO: Crea una variabile con un valore di status non valido
//        // HINT: var input = "invalid-status";

//        // Act
//        // TODO: Chiama TaskService.ParseStatus(input)
//        // HINT: var result = TaskService.ParseStatus(input);

//        // Assert
//        // TODO: Verifica che il risultato sia KanbanStatus.Todo (valore di default)
//        // HINT: result.Should().Be(KanbanStatus.Todo);

//        throw new NotImplementedException("Esercizio 4: Implementa il test per il valore di default");
//    }

//    #endregion

//    #region Modulo 2: Unit Test Domain Layer con Moq

//    /// <summary>
//    /// Modulo 2: Unit Test Domain Layer
//    /// Esercizio 5: CreateAsync verifica tutti i valori
//    ///
//    /// Obiettivo: Verificare che CreateAsync imposti correttamente tutti i campi del task
//    /// </summary>
//    [Fact]
//    public async Task CreateAsync_WithValidData_CreatesTaskWithCorrectValues()
//    {
//        // Arrange
//        var title = "Test Task";
//        var description = "Test Description";
//        var priority = "high";

//        // TODO: Configura il mock del repository per restituire il task passato
//        // HINT: _repositoryMock
//        //     .Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
//        //     .ReturnsAsync((TaskItem task) => task);

//        // Act
//        // TODO: Chiama _sut.CreateAsync(title, description, priority)
//        // HINT: var result = await _sut.CreateAsync(title, description, priority);

//        // Assert
//        // TODO: Verifica che:
//        // - result.Title sia uguale a title
//        // - result.Description sia uguale a description
//        // - result.Priority sia TaskPriority.High
//        // - result.Status sia KanbanStatus.Todo
//        // - result.Archived sia false
//        // HINT: result.Title.Should().Be(title);
//        //       result.Priority.Should().Be(TaskPriority.High);
//        //       result.Status.Should().Be(KanbanStatus.Todo);
//        //       result.Archived.Should().BeFalse();

//        throw new NotImplementedException("Esercizio 5: Verifica tutti i valori creati da CreateAsync");
//    }

//    /// <summary>
//    /// Modulo 2: Unit Test Domain Layer
//    /// Esercizio 6: CreateAsync imposta i timestamp
//    ///
//    /// Obiettivo: Verificare che CreatedAt e UpdatedAt siano impostati correttamente
//    /// </summary>
//    [Fact]
//    public async Task CreateAsync_SetsTimestamps()
//    {
//        // Arrange
//        // TODO: Salva il timestamp corrente PRIMA di chiamare CreateAsync
//        // HINT: var before = DateTime.UtcNow;

//        // TODO: Configura il mock del repository
//        // HINT: _repositoryMock
//        //     .Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
//        //     .ReturnsAsync((TaskItem task) => task);

//        // Act
//        // TODO: Chiama CreateAsync
//        // HINT: var result = await _sut.CreateAsync("Task", "Desc", null);

//        // TODO: Salva il timestamp corrente DOPO la chiamata
//        // HINT: var after = DateTime.UtcNow;

//        // Assert
//        // TODO: Verifica che CreatedAt e UpdatedAt siano tra before e after
//        // HINT: result.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
//        //       result.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);

//        throw new NotImplementedException("Esercizio 6: Verifica i timestamp");
//    }

//    /// <summary>
//    /// Modulo 2: Unit Test Domain Layer
//    /// Esercizio 7: PatchAsync aggiorna solo i campi specificati
//    ///
//    /// Obiettivo: Verificare che PatchAsync modifichi solo il title lasciando gli altri campi invariati
//    /// </summary>
//    [Fact]
//    public async Task PatchAsync_UpdatesOnlySpecifiedFields()
//    {
//        // Arrange
//        var existingTask = new TaskItem
//        {
//            Id = "123",
//            Title = "Original Title",
//            Description = "Original Desc",
//            Status = KanbanStatus.Todo,
//            Priority = TaskPriority.Medium,
//            Archived = false
//        };

//        var updates = new Dictionary<string, object?>
//        {
//            ["title"] = "Updated Title"
//        };

//        // TODO: Configura GetByIdAsync per restituire existingTask
//        // HINT: _repositoryMock
//        //     .Setup(r => r.GetByIdAsync("123"))
//        //     .ReturnsAsync(existingTask);

//        // TODO: Configura UpdateAsync per restituire il task aggiornato
//        // HINT: _repositoryMock
//        //     .Setup(r => r.UpdateAsync("123", It.IsAny<TaskItem>()))
//        //     .ReturnsAsync((string id, TaskItem task) => task);

//        // Act
//        // TODO: Chiama PatchAsync
//        // HINT: var result = await _sut.PatchAsync("123", updates);

//        // Assert
//        // TODO: Verifica che:
//        // - Title sia "Updated Title"
//        // - Description sia rimasto "Original Desc"
//        // - Status sia rimasto KanbanStatus.Todo
//        // - Priority sia rimasto TaskPriority.Medium
//        // HINT: result!.Title.Should().Be("Updated Title");
//        //       result.Description.Should().Be("Original Desc");

//        throw new NotImplementedException("Esercizio 7: Verifica che PatchAsync aggiorni solo i campi specificati");
//    }

//    /// <summary>
//    /// Modulo 2: Unit Test Domain Layer
//    /// Esercizio 8: PatchAsync aggiorna più campi
//    ///
//    /// Obiettivo: Verificare che PatchAsync aggiorni correttamente title, description e priority insieme
//    /// </summary>
//    [Fact]
//    public async Task PatchAsync_WithMultipleFields_UpdatesAll()
//    {
//        // Arrange
//        var existingTask = new TaskItem
//        {
//            Id = "123",
//            Title = "Old Title",
//            Description = "Old Desc",
//            Priority = TaskPriority.Low
//        };

//        var updates = new Dictionary<string, object?>
//        {
//            ["title"] = "New Title",
//            ["description"] = "New Desc",
//            ["priority"] = "high"
//        };

//        // TODO: Configura i mock (GetByIdAsync e UpdateAsync)
//        // HINT: _repositoryMock.Setup(r => r.GetByIdAsync("123")).ReturnsAsync(existingTask);
//        //       _repositoryMock.Setup(r => r.UpdateAsync("123", It.IsAny<TaskItem>()))
//        //           .ReturnsAsync((string id, TaskItem task) => task);

//        // Act
//        // TODO: Chiama PatchAsync con updates
//        // HINT: var result = await _sut.PatchAsync("123", updates);

//        // Assert
//        // TODO: Verifica tutti e tre i campi aggiornati
//        // HINT: result!.Title.Should().Be("New Title");
//        //       result.Description.Should().Be("New Desc");
//        //       result.Priority.Should().Be(TaskPriority.High);

//        throw new NotImplementedException("Esercizio 8: Verifica PatchAsync con più campi");
//    }

//    /// <summary>
//    /// Modulo 2: Unit Test Domain Layer
//    /// Esercizio 9: UpdateAsync quando il task non esiste
//    ///
//    /// Obiettivo: Verificare che UpdateAsync restituisca null e NON chiami il repository.UpdateAsync
//    /// </summary>
//    [Fact]
//    public async Task UpdateAsync_WhenTaskNotExists_ReturnsNull()
//    {
//        // Arrange
//        // TODO: Configura GetByIdAsync per restituire null (task non trovato)
//        // HINT: _repositoryMock
//        //     .Setup(r => r.GetByIdAsync("nonexistent"))
//        //     .ReturnsAsync((TaskItem?)null);

//        // Act
//        // TODO: Chiama UpdateAsync con un id che non esiste
//        // HINT: var result = await _sut.UpdateAsync("nonexistent", new TaskItem());

//        // Assert
//        // TODO: Verifica che:
//        // - Il risultato sia null
//        // - UpdateAsync del repository NON sia stato chiamato (Times.Never)
//        // HINT: result.Should().BeNull();
//        //       _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<TaskItem>()), Times.Never);

//        throw new NotImplementedException("Esercizio 9: Verifica comportamento quando task non esiste");
//    }

//    /// <summary>
//    /// Modulo 2: Unit Test Domain Layer
//    /// Esercizio 10: DeleteAsync verifica chiamata al repository
//    ///
//    /// Obiettivo: Verificare che DeleteAsync chiami il repository con l'ID corretto
//    /// </summary>
//    [Fact]
//    public async Task DeleteAsync_CallsRepositoryWithCorrectId()
//    {
//        // Arrange
//        // TODO: Configura DeleteAsync del repository per restituire true
//        // HINT: _repositoryMock
//        //     .Setup(r => r.DeleteAsync(It.IsAny<string>()))
//        //     .ReturnsAsync(true);

//        // Act
//        // TODO: Chiama DeleteAsync con un ID specifico
//        // HINT: await _sut.DeleteAsync("specific-id");

//        // Assert
//        // TODO: Verifica che il repository sia stato chiamato con l'ID corretto, esattamente una volta
//        // HINT: _repositoryMock.Verify(r => r.DeleteAsync("specific-id"), Times.Once);

//        throw new NotImplementedException("Esercizio 10: Verifica chiamata DeleteAsync al repository");
//    }

//    #endregion
//}
