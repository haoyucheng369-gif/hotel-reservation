using System.Net;
using System.Net.Http.Json;
using HotelReservation.Adapter.WebApi.Contracts.Reservations;
using HotelReservation.Adapter.WebApi.Contracts.Rooms;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelReservation.Integration.Tests.Reservations;

public sealed class ReservationsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly Guid Room101Id = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly HttpClient _client;

    public ReservationsApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ReservationLifecycle_ShouldCreateConfirmCancelAndReleaseRoom()
    {
        var checkIn = new DateOnly(2026, 8, 1);
        var checkOut = new DateOnly(2026, 8, 5);

        var initiallyAvailableRooms = await GetAvailableRoomsAsync(checkIn, checkOut);
        Assert.Equal(3, initiallyAvailableRooms.Count);

        var createRequest = new CreateReservationRequest(
            Room101Id,
            "Ada",
            "Lovelace",
            "ada.lovelace@example.com",
            checkIn,
            checkOut);

        var createResponse = await _client.PostAsJsonAsync("/reservations", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdReservation = await createResponse.Content.ReadFromJsonAsync<CreateReservationResponse>();
        Assert.NotNull(createdReservation);
        Assert.NotEqual(Guid.Empty, createdReservation.ReservationId);

        var availableRoomsAfterCreate = await GetAvailableRoomsAsync(checkIn, checkOut);
        Assert.Equal(2, availableRoomsAfterCreate.Count);
        Assert.DoesNotContain(availableRoomsAfterCreate, room => room.Id == Room101Id);

        var confirmResponse = await _client.PostAsync(
            $"/reservations/{createdReservation.ReservationId}/confirm",
            content: null);

        Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);

        var confirmedReservation = await confirmResponse.Content.ReadFromJsonAsync<ConfirmReservationResponse>();
        Assert.NotNull(confirmedReservation);
        Assert.Equal(createdReservation.ReservationId, confirmedReservation.ReservationId);

        var cancelResponse = await _client.PostAsync(
            $"/reservations/{createdReservation.ReservationId}/cancel",
            content: null);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);

        var cancelledReservation = await cancelResponse.Content.ReadFromJsonAsync<CancelReservationResponse>();
        Assert.NotNull(cancelledReservation);
        Assert.Equal(createdReservation.ReservationId, cancelledReservation.ReservationId);

        var availableRoomsAfterCancel = await GetAvailableRoomsAsync(checkIn, checkOut);
        Assert.Equal(3, availableRoomsAfterCancel.Count);
        Assert.Contains(availableRoomsAfterCancel, room => room.Id == Room101Id);
    }

    [Fact]
    public async Task CreateReservation_WhenRoomDoesNotExist_ShouldReturnNotFound()
    {
        var request = new CreateReservationRequest(
            Guid.Parse("99999999-9999-9999-9999-999999999999"),
            "Grace",
            "Hopper",
            "grace.hopper@example.com",
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 9, 5));

        var response = await _client.PostAsJsonAsync("/reservations", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateReservation_WhenRoomAlreadyBooked_ShouldReturnConflict()
    {
        var request = new CreateReservationRequest(
            Room101Id,
            "Katherine",
            "Johnson",
            "katherine.johnson@example.com",
            new DateOnly(2026, 10, 1),
            new DateOnly(2026, 10, 5));

        var firstResponse = await _client.PostAsJsonAsync("/reservations", request);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondResponse = await _client.PostAsJsonAsync("/reservations", request);

        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact]
    public async Task ConfirmReservation_WhenReservationDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.PostAsync(
            "/reservations/99999999-9999-9999-9999-999999999999/confirm",
            content: null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CancelReservation_WhenReservationDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.PostAsync(
            "/reservations/99999999-9999-9999-9999-999999999999/cancel",
            content: null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<IReadOnlyList<AvailableRoomResponse>> GetAvailableRoomsAsync(
        DateOnly checkIn,
        DateOnly checkOut)
    {
        var response = await _client.GetAsync($"/rooms/available?checkIn={checkIn:yyyy-MM-dd}&checkOut={checkOut:yyyy-MM-dd}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rooms = await response.Content.ReadFromJsonAsync<IReadOnlyList<AvailableRoomResponse>>();

        Assert.NotNull(rooms);
        return rooms;
    }
}
