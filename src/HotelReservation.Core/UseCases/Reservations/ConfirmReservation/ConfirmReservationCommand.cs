using HotelReservation.Core.Domain.Reservations;
using HotelReservation.Core.UseCases.Common;
using MediatR;

namespace HotelReservation.Core.UseCases.Reservations.ConfirmReservation;

public sealed record ConfirmReservationCommand(
    ReservationId ReservationId) : IRequest<Result>;
