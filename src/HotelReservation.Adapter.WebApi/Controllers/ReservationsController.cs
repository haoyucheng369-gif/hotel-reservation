using HotelReservation.Adapter.WebApi.Contracts.Reservations;
using HotelReservation.Core.Domain.Reservations;
using HotelReservation.Core.Domain.Rooms;
using HotelReservation.Core.UseCases.Reservations.CancelReservation;
using HotelReservation.Core.UseCases.Reservations.ConfirmReservation;
using HotelReservation.Core.UseCases.Reservations.CreateReservation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservation.Adapter.WebApi.Controllers;

[ApiController]
[Route("reservations")]
public sealed class ReservationsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CreateReservationResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateReservationCommand(
            new RoomId(request.RoomId),
            request.GuestFirstName,
            request.GuestLastName,
            request.GuestEmail,
            request.CheckIn,
            request.CheckOut);

        var result = await sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                CreateReservationErrors.RoomNotFound => NotFound(),
                CreateReservationErrors.RoomAlreadyBooked => Conflict(),
                _ => BadRequest()
            };
        }

        return Created(
            $"/reservations/{result.Value!.Value}",
            new CreateReservationResponse(result.Value.Value));
    }

    [HttpPost("{reservationId:guid}/confirm")]
    [ProducesResponseType<ConfirmReservationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Confirm(
        Guid reservationId,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmReservationCommand(new ReservationId(reservationId));
        var result = await sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                ConfirmReservationErrors.ReservationNotFound => NotFound(),
                ConfirmReservationErrors.ReservationAlreadyCancelled => Conflict(),
                _ => BadRequest()
            };
        }

        return Ok(new ConfirmReservationResponse(reservationId));
    }

    [HttpPost("{reservationId:guid}/cancel")]
    [ProducesResponseType<CancelReservationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel(
        Guid reservationId,
        CancellationToken cancellationToken)
    {
        var command = new CancelReservationCommand(new ReservationId(reservationId));
        var result = await sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error switch
            {
                CancelReservationErrors.ReservationNotFound => NotFound(),
                CancelReservationErrors.ReservationAlreadyCancelled => Conflict(),
                _ => BadRequest()
            };
        }

        return Ok(new CancelReservationResponse(reservationId));
    }
}
