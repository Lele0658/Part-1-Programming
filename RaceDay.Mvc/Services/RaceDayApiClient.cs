using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using RaceDay.Api.DTOs;

namespace RaceDay.Mvc.Services;

public class RaceDayApiClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContext;
    private readonly JsonSerializerOptions _jsonOptions;

    public RaceDayApiClient(IHttpClientFactory factory, IHttpContextAccessor httpContext)
    {
        _http = factory.CreateClient("RaceDayApi");
        _httpContext = httpContext;
        // This is crucial! It ensures case-insensitive property mapping
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    private string? GetToken()
    {
        return _httpContext.HttpContext?.Session.GetString("JwtToken");
    }

    // A helper method to create authorized requests safely
    private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        var token = GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return request;
    }

    // --- AUTH ---
    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var res = await _http.PostAsJsonAsync("api/auth/login", dto);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<AuthResponseDto>(_jsonOptions) : null;
    }

    // --- EVENTS ---
    public async Task<List<EventResponseDto>> GetEventsAsync()
    {
        var req = CreateAuthorizedRequest(HttpMethod.Get, "api/events");
        var res = await _http.SendAsync(req);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<List<EventResponseDto>>(_jsonOptions) ?? new() : new();
    }

    public async Task<EventResponseDto?> GetEventByIdAsync(int id)
    {
        var req = CreateAuthorizedRequest(HttpMethod.Get, $"api/events/{id}");
        var res = await _http.SendAsync(req);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<EventResponseDto>(_jsonOptions) : null;
    }

    // --- ORGANISER METHODS ---
    public async Task<EventResponseDto?> CreateEventAsync(EventDto dto)
    {
        var req = CreateAuthorizedRequest(HttpMethod.Post, "api/events");
        req.Content = JsonContent.Create(dto);

        var res = await _http.SendAsync(req);

        if (!res.IsSuccessStatusCode)
        {
            var errorText = await res.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"API ERROR ({(int)res.StatusCode}): " + errorText);
            return null;
        }

        return await res.Content.ReadFromJsonAsync<EventResponseDto>(_jsonOptions);
    }

    public async Task<bool> UploadBannerAsync(int eventId, IFormFile file)
    {
        var req = CreateAuthorizedRequest(HttpMethod.Post, $"api/events/{eventId}/banner");

        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "File", file.FileName);

        req.Content = content;

        var res = await _http.SendAsync(req);
        return res.IsSuccessStatusCode;
    }

    // --- ENROLMENTS & RESULTS (Participant) ---
    public async Task<bool> EnrolAsync(EnrolmentDto dto)
    {
        var req = CreateAuthorizedRequest(HttpMethod.Post, "api/enrolments");
        req.Content = JsonContent.Create(dto);
        var res = await _http.SendAsync(req);
        return res.IsSuccessStatusCode;
    }

    public async Task<List<EnrolmentResponseDto>> GetMyEnrolmentsAsync()
    {
        var req = CreateAuthorizedRequest(HttpMethod.Get, "api/enrolments/my");
        var res = await _http.SendAsync(req);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<List<EnrolmentResponseDto>>(_jsonOptions) ?? new() : new();
    }

    public async Task<List<ResultResponseDto>> GetMyResultsAsync()
    {
        var req = CreateAuthorizedRequest(HttpMethod.Get, "api/results/my");
        var res = await _http.SendAsync(req);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<List<ResultResponseDto>>(_jsonOptions) ?? new() : new();
    }

    public async Task<List<EnrolmentResponseDto>> GetEventEnrolmentsAsync(int eventId)
    {
        var req = CreateAuthorizedRequest(HttpMethod.Get, $"api/enrolments/event/{eventId}");
        var res = await _http.SendAsync(req);
        return res.IsSuccessStatusCode
            ? await res.Content.ReadFromJsonAsync<List<EnrolmentResponseDto>>(_jsonOptions) ?? new()
            : new();
    }
}