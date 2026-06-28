using HotelReservation.Core.UseCases.Common;
using MediatR;

namespace HotelReservation.Core.UseCases.Rooms.SearchAvailableRooms;

public sealed record SearchAvailableRoomsQuery(
    DateOnly CheckIn,
    DateOnly CheckOut) : IRequest<Result<IReadOnlyList<AvailableRoomDto>>>;
