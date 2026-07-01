using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HotelReservation.Adapter.WebApi.OpenApi;

public sealed class SwaggerDefaultsOperationFilter : IOperationFilter
{
    private const string CheckIn = "2026-08-01";
    private const string CheckOut = "2026-08-05";
    private const string Room101Id = "11111111-1111-1111-1111-111111111111";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var path = context.ApiDescription.RelativePath;

        if (path == "rooms/available")
        {
            SetParameterExample(operation, "checkIn", CheckIn);
            SetParameterExample(operation, "checkOut", CheckOut);
            return;
        }

        if (path == "reservations" &&
            string.Equals(context.ApiDescription.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
        {
            SetCreateReservationExample(operation);
        }
    }

    private static void SetParameterExample(
        OpenApiOperation operation,
        string parameterName,
        string value)
    {
        var parameter = operation.Parameters?
            .OfType<OpenApiParameter>()
            .SingleOrDefault(parameter => parameter.Name == parameterName);

        if (parameter is null)
        {
            return;
        }

        var jsonValue = JsonValue.Create(value);
        parameter.Example = jsonValue;

        if (parameter.Schema is OpenApiSchema schema)
        {
            schema.Default = jsonValue;
        }
    }

    private static void SetCreateReservationExample(OpenApiOperation operation)
    {
        if (operation.RequestBody?.Content is null ||
            !operation.RequestBody.Content.TryGetValue("application/json", out var mediaType))
        {
            return;
        }

        mediaType.Example = JsonNode.Parse(
            $$"""
            {
              "roomId": "{{Room101Id}}",
              "guestFirstName": "Ada",
              "guestLastName": "Lovelace",
              "guestEmail": "ada.lovelace@example.com",
              "checkIn": "{{CheckIn}}",
              "checkOut": "{{CheckOut}}"
            }
            """);
    }
}
