using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Payments.API.Entities;
using Payments.API.Models.PaymentModels;
using Payments.API.Repository;
using Shared.Enums;
using SharedProject.ModelTemplates;
using System.ComponentModel.DataAnnotations;

namespace Payments.API.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Payments").WithTags(nameof(Payment));

        group.MapGet("/", QueryPayments)
            .WithName("QueryPayments").WithOpenApi(operation => new(operation)
            {
                Summary = "Query Payments",
            });

        group.MapGet("/{id}", GetPaymentById)
            .WithName("GetPaymentById").WithOpenApi(operation => new(operation)
            {
                Summary = "Get Payment By Id",
            });

        group.MapPatch("/{id}", UpdatePaymentById)
            .WithName("UpdatePaymentById").WithOpenApi(operation => new(operation)
            {
                Summary = "Update Payment By Id",
            });

        group.MapPost("/", CreatePayment)
            .WithName("CreatePayment").WithOpenApi(operation => new(operation)
            {
                Summary = "Create Payment",
            });

        group.MapDelete("/{id}", DeletePayment)
            .WithName("DeletePayment").WithOpenApi(operation => new(operation)
            {
                Summary = "Delete Payment",
            });
    }


    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)},{nameof(SystemUserRole.Manager)}")]
    public static async Task<Results<Ok<PaginationResponse<PaymentBaseResponseModel>>, NoContent, BadRequest<ProblemDetails>>> QueryPayments([FromQuery] int page, [FromQuery] int pageSize, HttpContext httpContext, IPaymentRepository paymentRepository, IMapper mapper)
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

        var total = await paymentRepository.GetTotalEntities();
        var entities = await paymentRepository.Query(page, pageSize);
        var baseUri = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/api/Payments");

        var paginationResponse = new PaginationResponse<PaymentBaseResponseModel>(total, page, pageSize, mapper.Map<IEnumerable<PaymentBaseResponseModel>>(entities).ToList(), baseUri);

        return entities is IEnumerable<Payment> value && value.Any()
                ? TypedResults.Ok(paginationResponse)
                : TypedResults.NoContent();
    }

    [Authorize]
    public static async Task<Results<Ok<PaymentBaseResponseModel>, NotFound>> GetPaymentById(Guid id, IPaymentRepository paymentRepository, IMapper mapper)
    {
        return await paymentRepository.GetById(id)
            is Payment entity
                ? TypedResults.Ok(mapper.Map<PaymentBaseResponseModel>(entity))
                : TypedResults.NotFound();
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)}")]
    public static async Task<Results<Ok<PaymentBaseResponseModel>, NotFound, BadRequest<ProblemDetails>>> UpdatePaymentById(Guid id, PaymentBaseRequestModel requestModel, IPaymentRepository paymentRepository, IMapper mapper)
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

        var updateEntity = await paymentRepository.Update(id, requestModel);
        if (updateEntity is null)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Ok(mapper.Map<PaymentBaseResponseModel>(updateEntity));
    }

    public static async Task<Results<Created<PaymentBaseResponseModel>, BadRequest<ProblemDetails>>> CreatePayment(PaymentBaseRequestModel requestModel, IPaymentRepository paymentRepository, IMapper mapper)
    {
        var newEntity = mapper.Map<Payment>(requestModel);
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

        newEntity = await paymentRepository.Create(newEntity);
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
        return TypedResults.Created($"/api/Payments/{newEntity.Id}", mapper.Map<PaymentBaseResponseModel>(newEntity));
    }

    [Authorize(Roles = $"{nameof(SystemUserRole.Admin)}")]
    public static async Task<Results<NoContent, NotFound>> DeletePayment(Guid id, IPaymentRepository paymentRepository, IMapper mapper)
    {
        var isDeleted = await paymentRepository.Delete(id);

        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
