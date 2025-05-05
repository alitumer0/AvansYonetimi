using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using VarlikYönetimi.Services.Interfaces;

namespace VarlikYönetimi.API.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RateLimitingMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var rateLimitService = scope.ServiceProvider.GetRequiredService<IRateLimitService>();
            
            var clientId = context.Request.Headers["X-Client-Id"].ToString();
            var endpoint = $"{context.Request.Method}:{context.Request.Path}";

            if (string.IsNullOrEmpty(clientId))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("X-Client-Id header is required");
                return;
            }

            if (!await rateLimitService.IsRequestAllowed(clientId, endpoint))
            {
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Rate limit exceeded");
                return;
            }

            await _next(context);
        }
    }
} 