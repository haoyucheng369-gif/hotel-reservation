using HotelReservation.Adapter.WebApi.Contracts.Rooms;
using HotelReservation.Core.UseCases.Rooms.SearchAvailableRooms;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservation.Adapter.WebApi.Controllers;

[ApiController]
[Route("rooms")]
public sealed class RoomsController(ISender sender) : ControllerBase
{
    [HttpGet("available")]
    [ProducesResponseType<IReadOnlyList<AvailableRoomResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchAvailable(
        [FromQuery] DateOnly checkIn,
        [FromQuery] DateOnly checkOut,
        CancellationToken cancellationToken)
    {
        var query = new SearchAvailableRoomsQuery(checkIn, checkOut);
        var result = await sender.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                SearchAvailableRoomsErrors.InvalidDateRange => BadRequest(),
                _ => BadRequest()
            };
        }

        var response = result.Value!
            .Select(room => new AvailableRoomResponse(
                room.Id.Value,
                room.RoomNumber,
                room.Capacity,
                room.BasePrice))
            .ToList();

        return Ok(response);
    }
}
