using HotelReservation.Core.Domain.Common;
using HotelReservation.Core.Ports;
using HotelReservation.Core.UseCases.Common;
using MediatR;

namespace HotelReservation.Core.UseCases.Rooms.SearchAvailableRooms;

public sealed class SearchAvailableRoomsQueryHandler(
    IRoomRepository roomRepository)
    : IRequestHandler<SearchAvailableRoomsQuery, Result<IReadOnlyList<AvailableRoomDto>>>
{
    public async Task<Result<IReadOnlyList<AvailableRoomDto>>> Handle(
        SearchAvailableRoomsQuery request,
        CancellationToken cancellationToken)
    {
        DateRange period;
        try
        {
            period = new DateRange(request.CheckIn, request.CheckOut);
        }
        catch (ArgumentException)
        {
            return Result<IReadOnlyList<AvailableRoomDto>>.Failure(
                SearchAvailableRoomsErrors.InvalidDateRange);
        }

        var rooms = await roomRepository.SearchAvailableRoomsAsync(period, cancellationToken);
        var result = rooms
            .Select(room => new AvailableRoomDto(
                room.Id,
                room.RoomNumber,
                room.Capacity,
                room.BasePrice))
            .ToList();

        return Result<IReadOnlyList<AvailableRoomDto>>.Success(result);
    }
}
