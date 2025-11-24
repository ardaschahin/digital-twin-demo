using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        // OPC UA connection, background
        _ = OpcUaClient.ConnectAndSubscribe();

        var builder = WebApplication.CreateBuilder(args);

        // CORS: Frontend (Vite) comes from another port, so give an access.
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy
                    .AllowAnyOrigin()   // demo reasons...
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        // JSON  camelCase (for React)
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy =
                System.Text.Json.JsonNamingPolicy.CamelCase;
        });

        var app = builder.Build();

        // CORS middleware
        app.UseCors();

        // Digital Zwillinge endpoint
        app.MapGet("/twin", () => Results.Json(TwinStore.Get()));

        // health check 
        app.MapGet("/health", () => Results.Ok("UP"));

        Console.WriteLine("REST API running at http://localhost:5000/twin");
        Console.WriteLine("OPC UA Subscription streaming in background...\n");

        await app.RunAsync();
    }
}
