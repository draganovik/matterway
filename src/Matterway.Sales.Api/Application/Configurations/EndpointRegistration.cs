using Asp.Versioning;
using Matterway.Sales.Api.Features.Orders.AddOrderStatus;
using Matterway.Sales.Api.Features.Orders.CreateOrder;
using Matterway.Sales.Api.Features.Orders.GetOrderById;
using Matterway.Sales.Api.Features.Orders.QueryOrders;
using Matterway.Sales.Api.Features.Payments.GetPaymentById;
using Matterway.Sales.Api.Features.Payments.QueryPayments;
using Matterway.Sales.Api.Features.Payments.RegisterPayment;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scalar.AspNetCore;

namespace Matterway.Sales.Api.Application.Configurations;

public static class EndpointRegistration
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureApiVersioning()
        {
            builder.Services.AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            return builder;
        }

        public IHostApplicationBuilder ConfigureFeatures()
        {
            var uniqueTypes = new HashSet<Type>();

            var serviceDescriptors = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.DefinedTypes;
                    }
                    catch
                    {
                        return [];
                    }
                })
                .Where(type =>
                    type is { IsAbstract: false, IsInterface: false } &&
                    type.IsAssignableTo(typeof(IEndpoint)) &&
                    uniqueTypes.Add(type.AsType()))
                .Select(type =>
                    ServiceDescriptor.Transient(typeof(IEndpoint), type.AsType()))
                .ToArray();

            builder.Services.TryAddEnumerable(serviceDescriptors);
            builder.Services.AddTransient<CreateOrderService>();
            builder.Services.AddTransient<AddOrderStatusService>();
            builder.Services.AddTransient<GetOrderByIdService>();
            builder.Services.AddTransient<QueryOrdersService>();
            builder.Services.AddTransient<RegisterPaymentService>();
            builder.Services.AddTransient<GetPaymentByIdService>();
            builder.Services.AddTransient<QueryPaymentsService>();

            return builder;
        }
    }

    extension(WebApplication app)
    {
        public WebApplication ApplyEndpoints()
        {
            var versionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            var apiGroup = app.MapGroup("/api")
                .DisableAntiforgery();

            var versionedApiGroup = apiGroup
                .MapGroup("/v{version:apiVersion}")
                .WithApiVersionSet(versionSet);

            var endpoints = app.Services
                .GetRequiredService<IEnumerable<IEndpoint>>();

            foreach (var endpoint in endpoints) endpoint.MapEndpoint(versionedApiGroup);

            return app;
        }

        public WebApplication ApplyScalar()
        {
            app.MapScalarApiReference("/", options =>
            {
                options.WithOpenApiRoutePattern("/openapi/{documentName}.yaml");
                options.WithTitle("Matterway Sales API");
            });

            return app;
        }
    }
}