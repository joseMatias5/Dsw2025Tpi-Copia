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
                Application.Exceptions.EntityNotFoundException => "200",
                Application.Exceptions.ArgumentOutOfRangeException => "300",
                Application.Exceptions.ArgumentNullException => "300",
                Application.Exceptions.ArgumentException => "300",
                Application.Exceptions.DuplicatedEntityException => "300",
                Application.Exceptions.InvalidStatusException => "300",
                Application.Exceptions.NotAuthenticatedException => "300",
                Application.Exceptions.NotFoundException => "200",
                Application.Exceptions.NoContentException => "100",
                InvalidOperationException => "300",
                Application.Exceptions.ApplicationException => "300",
                _ => "400"
            };

            context.Response.StatusCode = int.Parse(statusCode);

            var errorResponse = new
            {
                status = statusCode,
                detail = e.Message
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
    }
}