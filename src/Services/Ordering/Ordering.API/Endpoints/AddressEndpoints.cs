using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Ordering.API.Entities;
using Ordering.API.Models.AddressModels;
using Ordering.API.Repository;
using Shared.Enums;
using SharedProject.ModelTemplates;

namespace Ordering.API.Endpoints;

public static class AddressEndpoints
{
    public static void MapAddressEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Addresses").WithTags(nameof(Address));

        group.MapGet("/", QueryAddresses)
            .WithName("QueryAddresses").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Query Addresses"
            });

        group.MapGet("/{id}", GetAddressById)
            .WithName("GetAddressById").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get Address By Id"
            });

        group.MapPatch("/{id}", UpdateAddressById)
            .WithName("UpdateAddressById").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Update Address By Id"
            });

        group.MapPost("/", CreateAddress)
            .WithName("CreateAddress").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Create Address"
            });

        group.MapDelete("/{id}", DeleteAddress)
            .WithName("DeleteAddress").WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Delete Address"
            });
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async
        Task<Results<Ok<PaginationResponse<AddressBaseResponseModel>>, NoContent, BadRequest<ProblemDetails>>>
        QueryAddresses([FromQuery] int page, [FromQuery] int pageSize, HttpContext httpContext,
            IAddressRepository addressRepository, IMapper mapper)
    {
        if (page < 1 || pageSize < 1)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Invalid page or pageSize.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Page and pageSize must be greater than zero."
            };
            var results = new List<ValidationResult>();
            if (page < 1) results.Add(new ValidationResult("Page must be greater than zero.", [nameof(page)]));
            if (pageSize < 1)
                results.Add(new ValidationResult("PageSize must be greater than zero.", [nameof(pageSize)]));

            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var total = await addressRepository.GetTotalEntities();
        var entities = await addressRepository.Query(page, pageSize);
        var baseUri =
            new Uri(
                $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/api/Addresses");
        var paginationResponse = new PaginationResponse<AddressBaseResponseModel>(total, page, pageSize,
            mapper.Map<IEnumerable<AddressBaseResponseModel>>(entities).ToList(), baseUri);

        return entities is IEnumerable<Address> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<AddressBaseResponseModel>, NotFound>> GetAddressById(Guid id,
        IAddressRepository addressRepository, IMapper mapper)
    {
        return await addressRepository.GetById(id)
            is Address entity
            ? TypedResults.Ok(mapper.Map<AddressBaseResponseModel>(entity))
            : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<AddressBaseResponseModel>, NotFound, BadRequest<ProblemDetails>>>
        UpdateAddressById(Guid id, AddressBaseRequestModel requestModel, IAddressRepository addressRepository,
            IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

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

        var updateEntity = await addressRepository.Update(id, requestModel);
        if (updateEntity is null) return TypedResults.NotFound();
        return TypedResults.Ok(mapper.Map<AddressBaseResponseModel>(updateEntity));
    }

    public static async Task<Results<Created<AddressBaseResponseModel>, BadRequest<ProblemDetails>>> CreateAddress(
        AddressBaseRequestModel requestModel, IAddressRepository addressRepository, IMapper mapper)
    {
        var newEntity = mapper.Map<Address>(requestModel);
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

        newEntity = await addressRepository.Create(newEntity);
        if (newEntity is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        return TypedResults.Created($"/api/Addresses/{newEntity.Id}", mapper.Map<AddressBaseResponseModel>(newEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteAddress(Guid id, IAddressRepository addressRepository,
        IMapper mapper)
    {
        var isDeleted = await addressRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}