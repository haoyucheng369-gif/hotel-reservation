using HotelReservation.Core.Domain.Common;
using HotelReservation.Core.Domain.Guests;
using HotelReservation.Core.Domain.Reservations;
using HotelReservation.Core.Ports;
using HotelReservation.Core.UseCases.Common;
using MediatR;

namespace HotelReservation.Core.UseCases.Reservations.CreateReservation;

public sealed class CreateReservationCommandHandler(
    IRoomRepository roomRepository,
    IReservationRepository reservationRepository,
    IClock clock) : IRequestHandler<CreateReservationCommand, Result<ReservationId>>
{
    public async Task<Result<ReservationId>> Handle(
        CreateReservationCommand request,
        CancellationToken cancellationToken)
    {
        var room = await roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room is null)
        {
            return Result<ReservationId>.Failure(CreateReservationErrors.RoomNotFound);
        }

        var period = new DateRange(request.CheckIn, request.CheckOut);
        var isAlreadyBooked = await reservationRepository.HasOverlappingReservationAsync(
            request.RoomId,
            period,
            cancellationToken);

        if (isAlreadyBooked)
        {
            return Result<ReservationId>.Failure(CreateReservationErrors.RoomAlreadyBooked);
        }

        var reservationId = new ReservationId(Guid.NewGuid());
        var guestInfo = new GuestInfo(
            request.GuestFirstName,
            request.GuestLastName,
            request.GuestEmail);
        var reservation = Reservation.Create(
            reservationId,
            request.RoomId,
            guestInfo,
            period,
            clock.UtcNow);

        await reservationRepository.AddAsync(reservation, cancellationToken);
        await reservationRepository.SaveChangesAsync(cancellationToken);

        return Result<ReservationId>.Success(reservationId);
    }
}
