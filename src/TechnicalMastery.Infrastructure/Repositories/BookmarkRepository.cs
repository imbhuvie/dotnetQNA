using Microsoft.EntityFrameworkCore;
using TechnicalMastery.Application.Interfaces;
using TechnicalMastery.Domain.Entities;
using TechnicalMastery.Infrastructure.Data;

namespace TechnicalMastery.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IBookmarkRepository"/>.
/// </summary>
public class BookmarkRepository : IBookmarkRepository
{
    private readonly AppDbContext context;

    public BookmarkRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyList<Bookmark>> GetAllAsync(CancellationToken cancellationToken)
    {
        List<Bookmark> bookmarks = await this.context.Bookmarks
            .AsNoTracking()
            .Include(bookmark => bookmark.Question)
            .OrderByDescending(bookmark => bookmark.CreatedAt)
            .ToListAsync(cancellationToken);

        return bookmarks;
    }

    public Task<bool> ExistsAsync(int questionId, CancellationToken cancellationToken)
    {
        return this.context.Bookmarks.AnyAsync(bookmark => bookmark.QuestionId == questionId, cancellationToken);
    }

    public async Task AddAsync(Bookmark bookmark, CancellationToken cancellationToken)
    {
        await this.context.Bookmarks.AddAsync(bookmark, cancellationToken);
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RemoveAsync(int questionId, CancellationToken cancellationToken)
    {
        Bookmark? bookmark = await this.context.Bookmarks
            .FirstOrDefaultAsync(item => item.QuestionId == questionId, cancellationToken);

        if (bookmark is null)
        {
            return false;
        }

        this.context.Bookmarks.Remove(bookmark);
        await this.context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
