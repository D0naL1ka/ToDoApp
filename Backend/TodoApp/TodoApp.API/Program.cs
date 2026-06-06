using TodoApp.API.Extensions;
using TodoApp.API.Middleware;
using TodoApp.Domain.Interfaces;
using TodoApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.RegisterAutoMapper(); 
builder.Services.AddValidation();
builder.Services.AddCorsPolicy();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    }); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();