using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services
builder.Services.AddControllers();

builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("igx-openapi", new OpenApiInfo { Title = "IGX Service External CSharp", Version = "v1" });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Basic authenication handler
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
    ("BasicAuthentication", null);
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IUsers, Users>();

// End of services
var app = builder.Build();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.UseSwagger(c => {
    c.RouteTemplate = "/template/{documentName}";
});

app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/template/igx-openapi", "IGX CSharp Template v1");
});

app.MapControllers();

app.Run();
