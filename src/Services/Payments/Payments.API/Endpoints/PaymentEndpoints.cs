using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Payments.API.Entities;
using Payments.API.Models.PaymentModels;
using Payments.API.Repository;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Payments.API.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Payments").WithTags(nameof(Payment));

        group.MapGet("/", QueryPayments)
            .WithName("QueryPayments").WithOpenApi();

        group.MapGet("/{id}", GetPaymentById)
            .WithName("GetPaymentById").WithOpenApi();

        group.MapPatch("/{id}", UpdatePaymentById)
            .WithName("UpdatePaymentById").WithOpenApi();

        group.MapPost("/", CreatePayment)
            .WithName("CreatePayment").WithOpenApi();

        group.MapDelete("/{id}", DeletePayment)
            .WithName("DeletePayment").WithOpenApi();
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<IEnumerable<PaymentBaseResponseModel>>, NoContent>> QueryPayments([FromQuery] int pageIndex, [FromQuery] int pageSize, IPaymentRepository addressRepository, IMapper mapper)
    {
        return await addressRepository.Query(pageIndex, pageSize)
            is IEnumerable<Payment> entityCollection && entityCollection.Any()
                ? TypedResults.Ok(mapper.Map<IEnumerable<PaymentBaseResponseModel>>(entityCollection))
                : TypedResults.NoContent();
    }

    [Authorize]
    public static async Task<Results<Ok<PaymentBaseResponseModel>, NotFound>> GetPaymentById(Guid id, IPaymentRepository addressRepository, IMapper mapper)
    {
        return await addressRepository.GetById(id)
            is Payment entity
                ? TypedResults.Ok(mapper.Map<PaymentBaseResponseModel>(entity))
                : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)}")]
    public static async Task<Results<Ok<PaymentBaseResponseModel>, NotFound<object>, BadRequest<object>>> UpdatePaymentById(Guid id, PaymentBaseRequestModel requestModel, IPaymentRepository addressRepository, IMapper mapper)
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
        return TypedResults.Ok(mapper.Map<PaymentBaseResponseModel>(updateEntity));
    }

    [Authorize]
    public static async Task<Results<Created<PaymentBaseResponseModel>, BadRequest<object>>> CreatePayment(PaymentBaseRequestModel requestModel, IPaymentRepository addressRepository, IMapper mapper)
    {
        var newEntity = mapper.Map<Payment>(requestModel);
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
        return TypedResults.Created($"/api/Payments/{newEntity.Id}", mapper.Map<PaymentBaseResponseModel>(newEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)}")]
    public static async Task<Results<NoContent, NotFound>> DeletePayment(Guid id, IPaymentRepository addressRepository, IMapper mapper)
    {
        var isDeleted = await addressRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
