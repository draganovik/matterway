using AutoMapper;
using Customers.API.Entities;
using Customers.API.Models.CustomerModels;
using Customers.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using SharedProject.ModelTemplates;
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
    public static async Task<Results<Ok<PaginationResponse<CustomerBaseResponseModel>>, NoContent, BadRequest<ProblemDetails>>> QueryCustomers([FromQuery] int page, [FromQuery] int pageSize, HttpContext httpContext, ICustomerRepository customerRepository, IMapper mapper)
    {
        if (page < 1 || pageSize < 1)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Invalid page or pageSize.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Page and pageSize must be greater than zero.",
            };
            var results = new List<ValidationResult>();
            if (page < 1)
            {
                results.Add(new ValidationResult("Page must be greater than zero.", new[] { nameof(page) }));
            }
            if (pageSize < 1)
            {
                results.Add(new ValidationResult("PageSize must be greater than zero.", new[] { nameof(pageSize) }));
            }

            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var total = await customerRepository.GetTotalEntities();
        var entities = await customerRepository.Query(page, pageSize);
        var baseUri = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/api/Customers");

        var paginationResponse = new PaginationResponse<CustomerBaseResponseModel>(total, page, pageSize, mapper.Map<IEnumerable<CustomerBaseResponseModel>>(entities).ToList(), baseUri);

        return entities is IEnumerable<Customer> value && value.Any()
                ? TypedResults.Ok(paginationResponse)
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
    public static async Task<Results<Ok<CustomerBaseResponseModel>, NotFound, BadRequest<ProblemDetails>, ForbidHttpResult>> UpdateCustomerById(Guid id, CustomerUpdateRequestModel requestModel, HttpContext httpContext, ICustomerRepository customerRepository, IMapper mapper)
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
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred."
            };
            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var updatedUser = await customerRepository.Update(id, updateEntity);
        return updatedUser is not null ? TypedResults.Ok(mapper.Map<CustomerBaseResponseModel>(updatedUser)) : TypedResults.NotFound();
    }

    [Authorize]
    public static async Task<Results<Created<CustomerBaseResponseModel>, BadRequest<ProblemDetails>, ForbidHttpResult>> CreateCustomer(CustomerCreateRequestModel requestModel, HttpContext httpContext, ICustomerRepository customerRepository, IMapper mapper)
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
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred."
            };
            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        newEntity.Id = newEntity.SystemUserId;

        var createdCustomer = await customerRepository.Create(newEntity);
        if (createdCustomer is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
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
