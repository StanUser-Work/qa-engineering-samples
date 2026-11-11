using ApiTests.Models;

namespace ApiTests.TestData;

public static class BookingData
{
    /// <summary>Public demo credentials published in the Restful-Booker docs (not real secrets).</summary>
    public static readonly AuthRequest AdminCredentials = new("admin", "password123");

    /// <summary>A valid booking with a unique last name, so it is easy to tell apart in the shared demo API.</summary>
    public static Booking NewBooking() => new(
        FirstName: "QA",
        LastName: "Auto-" + Guid.NewGuid().ToString("N")[..8],
        TotalPrice: 150,
        DepositPaid: true,
        BookingDates: new BookingDates(new DateOnly(2027, 1, 10), new DateOnly(2027, 1, 15)),
        AdditionalNeeds: "Breakfast");
}
