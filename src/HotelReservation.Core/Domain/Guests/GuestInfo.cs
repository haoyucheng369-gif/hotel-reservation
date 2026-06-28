using System.Net.Mail;

namespace HotelReservation.Core.Domain.Guests;

public sealed record GuestInfo
{
    public GuestInfo(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name is required.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name is required.", nameof(lastName));
        }

        if (!IsValidEmail(email))
        {
            throw new ArgumentException("Email is invalid.", nameof(email));
        }

        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public string FirstName { get; }

    public string LastName { get; }

    public string Email { get; }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var address = new MailAddress(email);
            return address.Address == email;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
