using HotelReservation.Adapter.Persistence.InMemory;
using HotelReservation.Core.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace HotelReservation.Adapter.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceAdapter(this IServiceCollection services)
    {
        services.AddSingleton<IReservationRepository, InMemoryReservationRepository>();
        services.AddSingleton<IRoomRepository, InMemoryRoomRepository>();
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
