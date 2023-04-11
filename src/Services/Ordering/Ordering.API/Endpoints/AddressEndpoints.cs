using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Entities;
using Ordering.API.Models.AddressModels;
using Ordering.API.Repository;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Endpoints;

public static class AddressEndpoints
{
    public static void MapAddressEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Addresses").WithTags(nameof(Address));

        group.MapGet("/", QueryAddresses)
            .WithName("QueryAddresses").WithOpenApi();

        group.MapGet("/{id}", GetAddressById)
            .WithName("GetAddressById").WithOpenApi();

        group.MapPut("/{id}", UpdateAddressById)
            .WithName("UpdateAddressById").WithOpenApi();

        group.MapPost("/", CreateAddress)
            .WithName("CreateAddress").WithOpenApi();

        group.MapDelete("/{id}", DeleteAddress)
            .WithName("DeleteAddress").WithOpenApi();
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<IEnumerable<AddressBaseResponseModel>>, NoContent>> QueryAddresses([FromQuery] int pageIndex, [FromQuery] int pageSize, IAddressRepository addressRepository, IMapper mapper)
    {
        return await addressRepository.Query(pageIndex, pageSize)
            is IEnumerable<Address> entityCollection && entityCollection.Any()
                ? TypedResults.Ok(mapper.Map<IEnumerable<AddressBaseResponseModel>>(entityCollection))
                : TypedResults.NoContent();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<AddressBaseResponseModel>, NotFound>> GetAddressById(Guid id, IAddressRepository addressRepository, IMapper mapper)
    {
        return await addressRepository.GetById(id)
            is Address entity
                ? TypedResults.Ok(mapper.Map<AddressBaseResponseModel>(entity))
                : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<AddressBaseResponseModel>, NotFound<object>, BadRequest<object>>> UpdateAddressById(Guid id, AddressBaseRequestModel requestModel, IAddressRepository addressRepository, IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(requestModel);
        var isValid = Validator.TryValidateObject(requestModel, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        var updateEntity = await addressRepository.Update(id, requestModel);
        if (updateEntity is null)
        {
            return TypedResults.NotFound<object>(new { message = "Entity not found" });
        }
        return TypedResults.Ok(mapper.Map<AddressBaseResponseModel>(updateEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Created<AddressBaseResponseModel>, BadRequest<object>>> CreateAddress(AddressBaseRequestModel requestModel, IAddressRepository addressRepository, IMapper mapper)
    {
        var newEntity = mapper.Map<Address>(requestModel);
        var results = new List<ValidationResult>();
        var context = new ValidationContext(newEntity);
        var isValid = Validator.TryValidateObject(newEntity, context, results, true);

        if (!isValid)
        {
            var errors = results.Select(r => r.ErrorMessage).ToList();
            return TypedResults.BadRequest<object>(new { message = "Bad Request", errors });
        }

        newEntity = await addressRepository.Create(newEntity);
        if (newEntity is null)
        {
            return TypedResults.BadRequest<object>(new { message = "Cannot create entity" });
        }
        return TypedResults.Created($"/api/Addresses/{newEntity.Id}", mapper.Map<AddressBaseResponseModel>(newEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<NoContent, NotFound>> DeleteAddress(Guid id, IAddressRepository addressRepository, IMapper mapper)
    {
        var isDeleted = await addressRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
