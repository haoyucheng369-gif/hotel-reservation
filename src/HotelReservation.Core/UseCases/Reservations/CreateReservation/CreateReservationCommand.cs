using HotelReservation.Core.Domain.Reservations;
using HotelReservation.Core.Domain.Rooms;
using HotelReservation.Core.UseCases.Common;
using MediatR;

namespace HotelReservation.Core.UseCases.Reservations.CreateReservation;

public sealed record CreateReservationCommand(
    RoomId RoomId,
    string GuestFirstName,
    string GuestLastName,
    string GuestEmail,
    DateOnly CheckIn,
    DateOnly CheckOut) : IRequest<Result<ReservationId>>;
