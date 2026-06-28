using HotelReservation.Core.Domain.Guests;

namespace HotelReservation.Core.Tests.Domain.Guests;

public class GuestInfoTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrow_WhenFirstNameIsEmpty(string firstName)
    {
        Assert.Throws<ArgumentException>(() => new GuestInfo(firstName, "Cheng", "guest@example.com"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrow_WhenLastNameIsEmpty(string lastName)
    {
        Assert.Throws<ArgumentException>(() => new GuestInfo("Haoyu", lastName, "guest@example.com"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-email")]
    [InlineData("guest@")]
    [InlineData("@example.com")]
    public void Constructor_ShouldThrow_WhenEmailIsInvalid(string email)
    {
        Assert.Throws<ArgumentException>(() => new GuestInfo("Haoyu", "Cheng", email));
    }

    [Fact]
    public void Constructor_ShouldCreateGuestInfo_WhenValuesAreValid()
    {
        var guest = new GuestInfo("Haoyu", "Cheng", "guest@example.com");

        Assert.Equal("Haoyu", guest.FirstName);
        Assert.Equal("Cheng", guest.LastName);
        Assert.Equal("guest@example.com", guest.Email);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenGuestInfoHasSameValues()
    {
        var first = new GuestInfo("Haoyu", "Cheng", "guest@example.com");
        var second = new GuestInfo("Haoyu", "Cheng", "guest@example.com");

        Assert.Equal(first, second);
    }
}
