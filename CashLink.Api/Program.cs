using CashLink.Api.Services;
using System.Data.SqlClient;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register connection factory and payment service
builder.Services.AddSingleton<IDbConnectionFactory, SqlServerConnectionFactory>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Initialize database on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var connectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Initializing database...");
        
        // Read and execute schema
        var schemaPath = Path.Combine(AppContext.BaseDirectory, "Database", "schema.sql");
        if (File.Exists(schemaPath))
        {
            var schema = await File.ReadAllTextAsync(schemaPath);
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            
            foreach (var batch in schema.Split("GO", StringSplitOptions.RemoveEmptyEntries))
            {
                if (!string.IsNullOrWhiteSpace(batch))
                {
                    await connection.ExecuteAsync(batch);
                }
            }
            
            logger.LogInformation("Database initialized successfully");
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database");
    }
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
