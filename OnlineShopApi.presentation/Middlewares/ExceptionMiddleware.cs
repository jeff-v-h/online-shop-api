using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OnlineShopApi.presentation.RequestModels;
using System;
using System.Net;
using System.Threading.Tasks;

namespace OnlineShopApi.presentation.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var errorId = $"urn:bdf:error:{Guid.NewGuid()}";
                _logger.LogError($"ErrorId: {errorId}, Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex, errorId);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, string errorInstance)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(
                new ErrorResponse(context.Response.StatusCode, exception.Message, errorInstance).ToString());
        }
    }
}
