using HotelReservation.Core.Domain.Reservations;
using HotelReservation.Core.UseCases.Common;
using MediatR;

namespace HotelReservation.Core.UseCases.Reservations.CancelReservation;

public sealed record CancelReservationCommand(
    ReservationId ReservationId) : IRequest<Result>;
