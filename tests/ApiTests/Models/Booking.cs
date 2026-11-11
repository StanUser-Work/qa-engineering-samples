using System.Text.Json.Serialization;

namespace ApiTests.Models;

// Records give value equality, so a whole booking can be compared with one assertion.

public record Booking(
    [property: JsonPropertyName("firstname")] string FirstName,
    [property: JsonPropertyName("lastname")] string LastName,
    [property: JsonPropertyName("totalprice")] int TotalPrice,
    [property: JsonPropertyName("depositpaid")] bool DepositPaid,
    [property: JsonPropertyName("bookingdates")] BookingDates BookingDates,
    [property: JsonPropertyName("additionalneeds")] string AdditionalNeeds);

public record BookingDates(
    [property: JsonPropertyName("checkin")] DateOnly CheckIn,
    [property: JsonPropertyName("checkout")] DateOnly CheckOut);

/// <summary>Response of POST /booking.</summary>
public record CreatedBooking(
    [property: JsonPropertyName("bookingid")] int BookingId,
    [property: JsonPropertyName("booking")] Booking Booking);

public record AuthRequest(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("password")] string Password);

/// <summary>Response of POST /auth. On wrong credentials the API returns 200 with "reason" instead of "token".</summary>
public record AuthResponse(
    [property: JsonPropertyName("token")] string? Token,
    [property: JsonPropertyName("reason")] string? Reason);
