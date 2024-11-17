using Factory.Api.Auth;
using Factory.Api.Database;
using Factory.Api.Extensions;
using Factory.Api.Modules;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

var MyPolicy = "_myPolicy";

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddAuthorization();

//builder.Services.AddIdentityApiEndpoints<IdentityUser>()
//    .AddEntityFrameworkStores<AuthDbContext>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyPolicy, policy =>
    {
        policy.WithOrigins("https://localhost:7065", "http://localhost:5092")
        .AllowAnyMethod()
        .WithHeaders(HeaderNames.ContentType);
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Read connection string from User Secrets json file
var connString = builder.Configuration["APISecrets:SQLiteConnectionString"]!;

builder.AddServicesToContainer(connString);

var app = builder.Build();

//app.MapIdentityApi<IdentityUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyPolicy);

app.UseHttpsRedirection();

// Register endpoints
app.MapCategoryEndpoint();
app.MapCustomerEndpoint();
app.MapMaterialEndpoint();
app.MapProductEndpoint();
app.MapOrderEndpoint();
app.MapSupplierEndpoint();
app.MapPurchaseEndpoint();
app.MapProductionEndpoint();

app.Run();

