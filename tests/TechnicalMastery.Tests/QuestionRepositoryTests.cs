using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Infrastructure.Repositories;

namespace TechnicalMastery.Tests;

public class QuestionRepositoryTests
{
    [Fact]
    public async Task GetPaged_Paginates()
    {
        using TestDatabase database = new TestDatabase();
        IQuestionRepository repository = new QuestionRepository(database.Context);

        (IReadOnlyList<Question> page1, int total) = await repository.GetPagedAsync(
            1, 2, null, null, null, null, null, null, false, CancellationToken.None);
        (IReadOnlyList<Question> page2, int totalAgain) = await repository.GetPagedAsync(
            2, 2, null, null, null, null, null, null, false, CancellationToken.None);

        Assert.Equal(3, total);
        Assert.Equal(3, totalAgain);
        Assert.Equal(2, page1.Count);
        Assert.Single(page2);
        Assert.NotEqual(page1[0].Id, page2[0].Id);
    }

    [Fact]
    public async Task GetCategoryStats_Aggregates()
    {
        using TestDatabase database = new TestDatabase();
        IQuestionRepository repository = new QuestionRepository(database.Context);

        IReadOnlyList<(int CategoryId, int Total, int Completed)> stats =
            await repository.GetCategoryStatsAsync(CancellationToken.None);

        Assert.Single(stats);
        Assert.Equal(3, stats[0].Total);
        Assert.Equal(0, stats[0].Completed);
    }

    [Fact]
    public async Task Count_ReturnsActiveOnly()
    {
        using TestDatabase database = new TestDatabase();
        IQuestionRepository repository = new QuestionRepository(database.Context);

        Assert.Equal(3, await repository.CountAsync(CancellationToken.None));
    }
}
