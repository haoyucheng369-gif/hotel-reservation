using HotelReservation.Adapter.Persistence;
using HotelReservation.Core;
using Microsoft.Extensions.DependencyInjection;

namespace HotelReservation.Bootstrapper;

public static class DependencyInjection
{
    public static IServiceCollection AddHotelReservationCore(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining<HotelReservation.Core.AssemblyMarker>();
        });

        services.AddPersistenceAdapter();

        return services;
    }
}
