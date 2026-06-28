using HotelReservation.Core.Domain.Reservations;
using HotelReservation.Core.Ports;
using HotelReservation.Core.UseCases.Common;
using MediatR;

namespace HotelReservation.Core.UseCases.Reservations.ConfirmReservation;

public sealed class ConfirmReservationCommandHandler(
    IReservationRepository reservationRepository)
    : IRequestHandler<ConfirmReservationCommand, Result>
{
    public async Task<Result> Handle(
        ConfirmReservationCommand request,
        CancellationToken cancellationToken)
    {
        var reservation = await reservationRepository.GetByIdAsync(
            request.ReservationId,
            cancellationToken);

        if (reservation is null)
        {
            return Result.Failure(ConfirmReservationErrors.ReservationNotFound);
        }

        try
        {
            reservation.Confirm();
        }
        catch (InvalidOperationException)
        {
            return Result.Failure(ConfirmReservationErrors.ReservationAlreadyCancelled);
        }

        await reservationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
