//using System.Net;
//using System.Text.Json;
//using Dsw2025Tpi.Application.Exceptions;
//using Microsoft.AspNetCore.Diagnostics;
//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.AspNetCore.Mvc;

//namespace Dsw2025Tpi.Api;
//public class CustomExceptionHandlingMiddleware : IMiddleware
//{
//    private readonly ILogger<CustomExceptionHandlingMiddleware> _logger;
//    public CustomExceptionHandlingMiddleware(ILogger<CustomExceptionHandlingMiddleware> logger)
//    {
//        _logger = logger ?? throw new Application.Exceptions.ArgumentNullException(nameof(logger));
//    }
//    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
//    {
//        try
//        {
//            await next(context);
//        }
//        catch (Exception e)
//        {
//            _logger.LogError(e.Message);
//            context.Response.ContentType = "application/json";

//            var statusCode = e switch
//            {
//                Application.Exceptions.EntityNotFoundException => "200",
//                Application.Exceptions.ArgumentOutOfRangeException => "300",
//                Application.Exceptions.ArgumentNullException => "300",
//                Application.Exceptions.ArgumentException => "300",
//                Application.Exceptions.DuplicatedEntityException => "300",
//                Application.Exceptions.InvalidStatusException => "300",
//                Application.Exceptions.NotAuthenticatedException => "300",
//                Application.Exceptions.NotFoundException => "200",
//                Application.Exceptions.NoContentException => "100",
//                InvalidOperationException => "300",
//                Application.Exceptions.ApplicationException => "300",
//                _ => "400"
//            };

//            context.Response.StatusCode = int.Parse(statusCode);

//            var errorResponse = new
//            {
//                status = statusCode,
//                detail = e.Message
//            };

//            var json = JsonSerializer.Serialize(errorResponse);
//            await context.Response.WriteAsync(json);
//        }
//    }
//}

using System.Net;
using System.Text.Json;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api;
public class CustomExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<CustomExceptionHandlingMiddleware> _logger;
    public CustomExceptionHandlingMiddleware(ILogger<CustomExceptionHandlingMiddleware> logger)
    {
        _logger = logger ?? throw new Application.Exceptions.ArgumentNullException(nameof(logger));
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            context.Response.ContentType = "application/json";


            var statusCode = e switch
            {
                Application.Exceptions.EntityNotFoundException => HttpStatusCode.NotFound,
                Application.Exceptions.ArgumentOutOfRangeException => HttpStatusCode.BadRequest,
                Application.Exceptions.ArgumentNullException => HttpStatusCode.BadRequest,
                Application.Exceptions.ArgumentException => HttpStatusCode.BadRequest,
                Application.Exceptions.DuplicatedEntityException => HttpStatusCode.BadRequest,
                Application.Exceptions.InvalidStatusException => HttpStatusCode.BadRequest,
                Application.Exceptions.NotAuthenticatedException => HttpStatusCode.BadRequest,
                Application.Exceptions.NotFoundException => HttpStatusCode.NotFound,
                Application.Exceptions.NoContentException => HttpStatusCode.NoContent,
                InvalidOperationException => HttpStatusCode.BadRequest,
                Dsw2025Tpi.Application.Exceptions.ApplicationException => HttpStatusCode.BadRequest,
                System.ApplicationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };
            //prodctos = 3000-3100

            var internalErrorCode = e switch
            {
                Application.Exceptions.EntityNotFoundException => "01",
                Application.Exceptions.ArgumentOutOfRangeException => "02",
                Application.Exceptions.ArgumentNullException => "03",
                Application.Exceptions.ArgumentException => "04",
                Application.Exceptions.DuplicatedEntityException => "05",
                Application.Exceptions.InvalidStatusException => "06",
                Application.Exceptions.NotAuthenticatedException => "07",
                Application.Exceptions.NotFoundException => "08",
                Application.Exceptions.NoContentException => "09",
                InvalidOperationException => "10",
                Application.Exceptions.ApplicationException => "11",
                _ => "00"
            };

            var path = context.Request.Path.Value?.ToLower() ?? "";
            string source;
            if (path.Contains("orders"))
                source = "orders";
            else if (path.Contains("products"))
                source = "product";
            else if (path.Contains("auth"))
                source = "auth";
            else
                source = "app";

            internalErrorCode = $"{source}-{internalErrorCode}";
            
            context.Response.StatusCode = 500;

            var errorResponse = new
            {
                status = (int)statusCode,
                title = statusCode.ToString(),
                detail = e.Message,
                code = internalErrorCode
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
    }
}