using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TechnicalMastery.Wpf.Models;

namespace TechnicalMastery.Wpf.Services;

/// <summary>
/// The single HTTP gateway to the backend. One typed <see cref="HttpClient"/>
/// (created by the factory — never newed per request), JSON over the wire,
/// envelope unwrapping in one place. Implements every narrow client interface.
/// </summary>
public class StudyApiClient :
    ICatalogApiClient, IQuestionApiClient, IBookmarkApiClient,
    IProgressApiClient, INotesApiClient, IDashboardApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient http;

    public StudyApiClient(HttpClient http)
    {
        this.http = http;
    }

    public async Task<IReadOnlyList<CategoryModel>> GetCategoriesAsync(CancellationToken ct)
    {
        return await GetAsync<List<CategoryModel>>("api/categories", ct) ?? new List<CategoryModel>();
    }

    public async Task<IReadOnlyList<TopicModel>> GetTopicsAsync(int? categoryId, CancellationToken ct)
    {
        string url = categoryId.HasValue ? "api/topics?categoryId=" + categoryId.Value : "api/topics";
        return await GetAsync<List<TopicModel>>(url, ct) ?? new List<TopicModel>();
    }

    public Task<PagedResult<QuestionSummaryModel>> GetPagedAsync(
        int page, int pageSize, int? categoryId, int? topicId,
        DifficultyLevel? difficulty, string? search, string? sortBy, bool descending,
        CancellationToken ct)
    {
        List<string> parts = new List<string>
        {
            "page=" + page,
            "pageSize=" + pageSize,
            "descending=" + (descending ? "true" : "false")
        };

        if (categoryId.HasValue)
        {
            parts.Add("categoryId=" + categoryId.Value);
        }

        if (topicId.HasValue)
        {
            parts.Add("topicId=" + topicId.Value);
        }

        if (difficulty.HasValue)
        {
            parts.Add("difficulty=" + difficulty.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            parts.Add("search=" + Uri.EscapeDataString(search));
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            parts.Add("sortBy=" + Uri.EscapeDataString(sortBy));
        }

        return GetAsync<PagedResult<QuestionSummaryModel>>("api/questions?" + string.Join("&", parts), ct);
    }

    public Task<QuestionDetailModel> GetByIdAsync(int id, CancellationToken ct)
    {
        return GetAsync<QuestionDetailModel>("api/questions/" + id, ct);
    }

    public async Task<IReadOnlyList<QuestionSummaryModel>> GetRandomAsync(int count, CancellationToken ct)
    {
        return await GetAsync<List<QuestionSummaryModel>>("api/questions/random?count=" + count, ct)
            ?? new List<QuestionSummaryModel>();
    }

    public async Task<IReadOnlyList<QuestionSummaryModel>> GetRelatedAsync(int questionId, int count, CancellationToken ct)
    {
        return await GetAsync<List<QuestionSummaryModel>>("api/questions/" + questionId + "/related?count=" + count, ct)
            ?? new List<QuestionSummaryModel>();
    }

    public async Task<IReadOnlyList<BookmarkModel>> GetAllAsync(CancellationToken ct)
    {
        return await GetAsync<List<BookmarkModel>>("api/bookmarks", ct) ?? new List<BookmarkModel>();
    }

    public async Task AddAsync(int questionId, CancellationToken ct)
    {
        await SendAsync<BookmarkModel>(HttpMethod.Post, "api/bookmarks/" + questionId, null, ct);
    }

    public async Task RemoveAsync(int questionId, CancellationToken ct)
    {
        await SendAsync<object>(HttpMethod.Delete, "api/bookmarks/" + questionId, null, ct);
    }

    public async Task SetStatusAsync(int questionId, StudyStatus status, CancellationToken ct)
    {
        await SendAsync<StudyProgressModel>(HttpMethod.Post, "api/progress/" + questionId, new { status = status.ToString() }, ct);
    }

    public async Task<IReadOnlyList<QuestionNoteModel>> GetByQuestionAsync(int questionId, CancellationToken ct)
    {
        return await GetAsync<List<QuestionNoteModel>>("api/notes/question/" + questionId, ct)
            ?? new List<QuestionNoteModel>();
    }

    public async Task CreateAsync(int questionId, string noteText, CancellationToken ct)
    {
        await SendAsync<QuestionNoteModel>(HttpMethod.Post, "api/notes/question/" + questionId, new { noteText = noteText }, ct);
    }

    public async Task UpdateAsync(int noteId, string noteText, CancellationToken ct)
    {
        await SendAsync<QuestionNoteModel>(HttpMethod.Put, "api/notes/" + noteId, new { noteText = noteText }, ct);
    }

    public async Task DeleteAsync(int noteId, CancellationToken ct)
    {
        await SendAsync<object>(HttpMethod.Delete, "api/notes/" + noteId, null, ct);
    }

    public Task<DashboardSummaryModel> GetSummaryAsync(CancellationToken ct)
    {
        return GetAsync<DashboardSummaryModel>("api/dashboard/summary", ct);
    }

    private Task<T> GetAsync<T>(string url, CancellationToken ct)
    {
        return SendAsync<T>(HttpMethod.Get, url, null, ct);
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string url, object? body, CancellationToken ct)
    {
        using HttpRequestMessage request = new HttpRequestMessage(method, url);

        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }

        using HttpResponseMessage response = await this.http.SendAsync(request, ct);

        ApiResponse<T>? envelope;
        try
        {
            envelope = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(JsonOptions, ct);
        }
        catch (JsonException)
        {
            throw new ApiException("The server returned an unreadable response.", response.StatusCode);
        }

        if (envelope is null)
        {
            throw new ApiException("The server returned an empty response.", response.StatusCode);
        }

        if (!response.IsSuccessStatusCode || !envelope.Success)
        {
            string message = string.IsNullOrWhiteSpace(envelope.Message)
                ? "Request failed with status " + (int)response.StatusCode + "."
                : envelope.Message;

            if (envelope.Errors.Count > 0)
            {
                message += " " + string.Join(" ", envelope.Errors);
            }

            throw new ApiException(message, response.StatusCode);
        }

        if (envelope.Data is null)
        {
            throw new ApiException("The server returned no data.", response.StatusCode);
        }

        return envelope.Data;
    }
}
