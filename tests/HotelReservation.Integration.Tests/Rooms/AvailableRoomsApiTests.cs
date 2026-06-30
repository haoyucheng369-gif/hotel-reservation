using System.Net;
using System.Net.Http.Json;
using HotelReservation.Adapter.WebApi.Contracts.Rooms;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelReservation.Integration.Tests.Rooms;

public sealed class AvailableRoomsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AvailableRoomsApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAvailableRooms_ShouldReturnSeededAvailableRooms()
    {
        var response = await _client.GetAsync("/rooms/available?checkIn=2026-07-01&checkOut=2026-07-05");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rooms = await response.Content.ReadFromJsonAsync<IReadOnlyList<AvailableRoomResponse>>();

        Assert.NotNull(rooms);
        Assert.Equal(3, rooms.Count);
        Assert.Contains(rooms, room => room.RoomNumber == "101");
        Assert.Contains(rooms, room => room.RoomNumber == "102");
        Assert.Contains(rooms, room => room.RoomNumber == "201");
    }
}
