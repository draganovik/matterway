using AutoMapper;
using Customers.API.Entities;
using Customers.API.Models.CustomerModels;
using Customers.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Customers.API.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Customers").WithTags(nameof(Customer));

        group.MapGet("/", QueryCustomers)
            .WithName("QueryCustomers").WithOpenApi(operation => new(operation)
            {
                Summary = "Query Customers",
            });

        group.MapGet("/VerifyBy", VerifyCustomer)
            .WithName("VerifyCustomer").WithOpenApi(operation => new(operation)
            {
                Summary = "Verify Customer",
            });

        group.MapGet("/{id}", GetCustomerById)
            .WithName("GetCustomerById").WithOpenApi(operation => new(operation)
            {
                Summary = "Get Customer by Id",
            });

        group.MapPatch("/{id}", UpdateCustomerById)
            .WithName("UpdateCustomerById").WithOpenApi(operation => new(operation)
            {
                Summary = "Update Customer by Id",
            });

        group.MapPost("/", CreateCustomer)
            .WithName("CreateCustomer").WithOpenApi(operation => new(operation)
            {
                Summary = "Create Customer",
            });

        group.MapDelete("/{id}", DeleteCustomer)
            .WithName("DeleteCustomer").WithOpenApi(operation => new(operation)
            {
                Summary = "Delete Customer",
            });
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<IEnumerable<CustomerBaseResponseModel>>, NoContent>> QueryCustomers([FromQuery] int pageIndex, [FromQuery] int pageSize, ICustomerRepository customerRepository, IMapper mapper)
    {
        return await customerRepository.Query(pageIndex, pageSize)
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

    [Authorize]
    public static async Task<Results<Ok<CustomerBaseResponseModel>, NotFound, BadRequest<object>, ForbidHttpResult>> UpdateCustomerById(Guid id, CustomerUpdateRequestModel requestModel, HttpContext httpContext, ICustomerRepository customerRepository, IMapper mapper)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid systemUserId))
        {
            return TypedResults.Forbid();
        }

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
        {
            return TypedResults.Forbid();
        }

        var updateEntity = mapper.Map<Customer>(requestModel);

        if (userRole == SystemUserRole.Customer)
        {
            if (updateEntity.SystemUserId != systemUserId)
            {
                return TypedResults.Forbid();
            }
        }

        var results = new List<ValidationResult>();
        var context = new ValidationContext(updateEntity);
        var isValid = Validator.TryValidateObject(updateEntity, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        var updatedUser = await customerRepository.Update(id, updateEntity);
        return updatedUser is not null ? TypedResults.Ok(mapper.Map<CustomerBaseResponseModel>(updatedUser)) : TypedResults.NotFound();
    }

    [Authorize]
    public static async Task<Results<Created<CustomerBaseResponseModel>, BadRequest<object>, ForbidHttpResult>> CreateCustomer(CustomerCreateRequestModel requestModel, HttpContext httpContext, ICustomerRepository customerRepository, IMapper mapper)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        if (!Guid.TryParse(identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid systemUserId))
        {
            return TypedResults.Forbid();
        }

        if (!Enum.TryParse(identity?.FindFirst(ClaimTypes.Role)?.Value, out SystemUserRole userRole))
        {
            return TypedResults.Forbid();
        }

        var newEntity = mapper.Map<Customer>(requestModel);

        if (userRole == SystemUserRole.Customer)
        {
            if (newEntity.SystemUserId != systemUserId)
            {
                return TypedResults.Forbid();
            }
        }

        var results = new List<ValidationResult>();
        var context = new ValidationContext(newEntity);
        var isValid = Validator.TryValidateObject(newEntity, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        var createdCustomer = await customerRepository.Create(newEntity);
        if (createdCustomer is null)
        {
            return TypedResults.BadRequest<object>(new { message = "Cannot create Customer" });
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
