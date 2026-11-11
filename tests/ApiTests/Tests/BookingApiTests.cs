using System.Net;
using ApiTests.Client;
using ApiTests.Models;
using ApiTests.TestData;

namespace ApiTests.Tests;

[Category("API")]
public class BookingApiTests
{
    private BookingApiClient _api = null!;
    private readonly List<int> _createdIds = [];

    [SetUp]
    public void SetUp()
    {
        _api = new BookingApiClient();
        _createdIds.Clear();
    }

    /// <summary>Removes bookings the test created, so the shared demo API is left clean.</summary>
    [TearDown]
    public async Task CleanUp()
    {
        try
        {
            if (_createdIds.Count > 0)
            {
                var token = await GetTokenAsync();
                foreach (var id in _createdIds)
                    (await _api.DeleteBookingAsync(id, token)).Dispose();
            }
        }
        finally
        {
            _api.Dispose();
        }
    }

    [Test]
    [Description("POST /booking creates a booking, and GET /booking/{id} returns exactly the same data.")]
    public async Task CreateBooking_ThenGetById_ReturnsSameData()
    {
        var booking = BookingData.NewBooking();

        using var createResponse = await _api.CreateBookingAsync(booking);

        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var created = await BookingApiClient.ReadAsync<CreatedBooking>(createResponse);
        _createdIds.Add(created.BookingId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(created.BookingId, Is.GreaterThan(0), "a new booking id is returned");
            Assert.That(created.Booking, Is.EqualTo(booking), "the response echoes the sent booking");
        }

        using var getResponse = await _api.GetBookingAsync(created.BookingId);

        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(getResponse.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
        var stored = await BookingApiClient.ReadAsync<Booking>(getResponse);
        Assert.That(stored, Is.EqualTo(booking), "the stored booking matches what was sent");
    }

    [Test]
    [Description("An authorized user deletes a booking, after which GET /booking/{id} returns 404.")]
    public async Task DeleteBooking_WithValidToken_RemovesBooking()
    {
        using var createResponse = await _api.CreateBookingAsync(BookingData.NewBooking());
        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK), "precondition: booking is created");
        var id = (await BookingApiClient.ReadAsync<CreatedBooking>(createResponse)).BookingId;

        var token = await GetTokenAsync();

        using var deleteResponse = await _api.DeleteBookingAsync(id, token);

        // Restful-Booker answers 201 Created on a successful delete (its documented behavior).
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        using var getResponse = await _api.GetBookingAsync(id);
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "the deleted booking is no longer available");
    }

    private async Task<string> GetTokenAsync()
    {
        using var response = await _api.CreateTokenAsync(BookingData.AdminCredentials);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var auth = await BookingApiClient.ReadAsync<AuthResponse>(response);
        Assert.That(auth.Token, Is.Not.Null.And.Not.Empty, $"a token is issued (reason: {auth.Reason})");
        return auth.Token!;
    }
}
