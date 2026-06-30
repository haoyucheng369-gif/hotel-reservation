using HotelReservation.Core;
using Microsoft.Extensions.DependencyInjection;

namespace HotelReservation.Bootstrapper;

public static class DependencyInjection
{
    public static IServiceCollection AddHotelReservationCore(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining<AssemblyMarker>();
        });

        return services;
    }
}
