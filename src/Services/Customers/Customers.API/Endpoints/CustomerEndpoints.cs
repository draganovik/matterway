using AutoMapper;
using Customers.API.Entities;
using Customers.API.Models.CustomerModels;
using Customers.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;

namespace Customers.API.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Customers").WithTags(nameof(Customer));

        group.MapGet("/", QueryCustomers)
            .WithName("QueryCustomers").WithOpenApi();

        group.MapGet("/VerifyBy", VerifyCustomer)
            .WithName("VerifyCustomer").WithOpenApi();

        group.MapGet("/{id}", GetCustomerById)
            .WithName("GetCustomerById").WithOpenApi();

        group.MapPut("/{id}", UpdateCustomerById)
            .WithName("UpdateCustomerById").WithOpenApi();

        group.MapPost("/", CreateCustomer)
            .WithName("CreateCustomer").WithOpenApi();

        group.MapDelete("/{id}", DeleteCustomer)
            .WithName("DeleteCustomer").WithOpenApi();
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<IEnumerable<CustomerBaseResponseModel>>, NoContent>> QueryCustomers(ICustomerRepository customerRepository, IMapper mapper)
    {
        return await customerRepository.Query()
            is IEnumerable<Customer> value && value.Any()
                ? TypedResults.Ok(mapper.Map<IEnumerable<CustomerBaseResponseModel>>(value))
                : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<CustomerBaseResponseModel>, NotFound>> GetCustomerById(Guid id, ICustomerRepository customerRepository, IMapper mapper)
    {
        return await customerRepository.GetById(id)
            is Customer value
                ? TypedResults.Ok(mapper.Map<CustomerBaseResponseModel>(value))
                : TypedResults.NotFound();
    }

    public static async Task<Results<Ok<CustomerBaseResponseModel>, NotFound, BadRequest>> VerifyCustomer([FromQuery] Guid? customerId, [FromQuery] Guid? systemUserId, ICustomerRepository customerRepository, IMapper mapper)
    {
        if (customerId.HasValue)
        {
            var customer = await customerRepository.GetById(customerId.Value);
            return customer != null
                ? TypedResults.Ok(mapper.Map<CustomerBaseResponseModel>(customer))
                : TypedResults.NotFound();
        }
        if (systemUserId.HasValue)
        {
            var customer = await customerRepository.GetBySystemUserId(systemUserId.Value);
            return customer != null
                ? TypedResults.Ok(mapper.Map<CustomerBaseResponseModel>(customer))
                : TypedResults.NotFound();
        }
        return TypedResults.BadRequest();

    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<CustomerBaseResponseModel>, NotFound>> UpdateCustomerById(Guid id, CustomerUpdateRequestModel requestModel, ICustomerRepository customerRepository, IMapper mapper)
    {
        var updatedUser = await customerRepository.Update(id, requestModel);
        return updatedUser is not null ? TypedResults.Ok(mapper.Map<CustomerBaseResponseModel>(updatedUser)) : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<CustomerBaseResponseModel>, BadRequest>> CreateCustomer(CustomerCreateRequestModel requestModel, ICustomerRepository customerRepository, IMapper mapper)
    {
        var customerModel = mapper.Map<Customer>(requestModel);
        var createdCustomer = await customerRepository.Create(customerModel);
        if (createdCustomer is null)
        {
            return TypedResults.BadRequest();
        }
        return TypedResults.Created($"/api/Customers/{createdCustomer.Id}", mapper.Map<CustomerBaseResponseModel>(createdCustomer));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteCustomer(Guid id, ICustomerRepository customerRepository, IMapper mapper)
    {
        var isDeleted = await customerRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
