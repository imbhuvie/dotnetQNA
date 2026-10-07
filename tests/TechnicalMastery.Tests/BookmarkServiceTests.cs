using TechnicalMastery.Application.DTOs;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Application.Services;
using TechnicalMastery.Infrastructure.Repositories;

namespace TechnicalMastery.Tests;

public class BookmarkServiceTests
{
    private static BookmarkService CreateService(TestDatabase database)
    {
        return new BookmarkService(
            new BookmarkRepository(database.Context),
            new QuestionRepository(database.Context));
    }

    [Fact]
    public async Task Add_CreatesBookmark()
    {
        using TestDatabase database = new TestDatabase();
        BookmarkService service = CreateService(database);
        int id = database.QuestionId("What is a variable?");

        BookmarkDto created = await service.AddAsync(id, CancellationToken.None);

        Assert.Equal(id, created.QuestionId);

        IReadOnlyList<BookmarkDto> all = await service.GetAllAsync(CancellationToken.None);
        Assert.Single(all);
    }

    [Fact]
    public async Task Add_Duplicate_ThrowsConflict()
    {
        using TestDatabase database = new TestDatabase();
        BookmarkService service = CreateService(database);
        int id = database.QuestionId("What is a variable?");

        await service.AddAsync(id, CancellationToken.None);

        await Assert.ThrowsAsync<Application.Common.Exceptions.ConflictException>(
            () => service.AddAsync(id, CancellationToken.None));
    }

    [Fact]
    public async Task Add_MissingQuestion_ThrowsNotFound()
    {
        using TestDatabase database = new TestDatabase();
        BookmarkService service = CreateService(database);

        await Assert.ThrowsAsync<Application.Common.Exceptions.NotFoundException>(
            () => service.AddAsync(999999, CancellationToken.None));
    }

    [Fact]
    public async Task Remove_DeletesBookmark()
    {
        using TestDatabase database = new TestDatabase();
        BookmarkService service = CreateService(database);
        int id = database.QuestionId("What is a variable?");

        await service.AddAsync(id, CancellationToken.None);
        await service.RemoveAsync(id, CancellationToken.None);

        IReadOnlyList<BookmarkDto> all = await service.GetAllAsync(CancellationToken.None);
        Assert.Empty(all);
    }

    [Fact]
    public async Task Remove_Missing_ThrowsNotFound()
    {
        using TestDatabase database = new TestDatabase();
        BookmarkService service = CreateService(database);

        await Assert.ThrowsAsync<Application.Common.Exceptions.NotFoundException>(
            () => service.RemoveAsync(999999, CancellationToken.None));
    }
}
