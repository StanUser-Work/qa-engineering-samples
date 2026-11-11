using System.Net.Http.Headers;
using System.Net.Http.Json;
using ApiTests.Models;

namespace ApiTests.Client;

/// <summary>
/// Thin wrapper over the Restful-Booker API (https://restful-booker.herokuapp.com/apidoc).
/// Methods return the raw <see cref="HttpResponseMessage"/> so tests can check status codes and headers,
/// and <see cref="ReadAsync{T}"/> turns a response body into a typed model.
/// </summary>
public sealed class BookingApiClient : IDisposable
{
    public const string DefaultBaseUrl = "https://restful-booker.herokuapp.com/";

    private readonly HttpClient _http;

    public BookingApiClient(string? baseUrl = null)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl ?? DefaultBaseUrl),
            Timeout = TimeSpan.FromSeconds(30),
        };
        // Without this header the API answers POST /booking with 418 "I'm a teapot".
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public Task<HttpResponseMessage> CreateTokenAsync(AuthRequest credentials) =>
        _http.PostAsJsonAsync("auth", credentials);

    public Task<HttpResponseMessage> CreateBookingAsync(Booking booking) =>
        _http.PostAsJsonAsync("booking", booking);

    public Task<HttpResponseMessage> GetBookingAsync(int id) =>
        _http.GetAsync($"booking/{id}");

    /// <summary>Deleting needs an auth token, which the API expects in a cookie.</summary>
    public Task<HttpResponseMessage> DeleteBookingAsync(int id, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"booking/{id}");
        request.Headers.Add("Cookie", $"token={token}");
        return _http.SendAsync(request);
    }

    public static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<T>();
        return body ?? throw new InvalidOperationException($"Response body could not be read as {typeof(T).Name}.");
    }

    public void Dispose() => _http.Dispose();
}
