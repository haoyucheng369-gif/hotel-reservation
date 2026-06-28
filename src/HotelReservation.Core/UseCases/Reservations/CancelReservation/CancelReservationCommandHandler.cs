using HotelReservation.Core.Ports;
using HotelReservation.Core.UseCases.Common;
using MediatR;

namespace HotelReservation.Core.UseCases.Reservations.CancelReservation;

public sealed class CancelReservationCommandHandler(
    IReservationRepository reservationRepository)
    : IRequestHandler<CancelReservationCommand, Result>
{
    public async Task<Result> Handle(
        CancelReservationCommand request,
        CancellationToken cancellationToken)
    {
        var reservation = await reservationRepository.GetByIdAsync(
            request.ReservationId,
            cancellationToken);

        if (reservation is null)
        {
            return Result.Failure(CancelReservationErrors.ReservationNotFound);
        }

        try
        {
            reservation.Cancel();
        }
        catch (InvalidOperationException)
        {
            return Result.Failure(CancelReservationErrors.ReservationAlreadyCancelled);
        }

        await reservationRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
