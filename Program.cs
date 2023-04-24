using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var serializeOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase

};

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("igx-openapi", new OpenApiInfo { Title = "IGX Service External CSharp", Version = "v1" });
    }
);

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
    ("BasicAuthentication", null);
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IUsers, Users>();

var app = builder.Build();


app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

// Replace template with your own name here
app.MapPost(
    "/template/igx-service/{customerKey}/{orgKey}/addone",
    [Authorize] async (HttpRequest req) =>
    {
        app.Logger.LogInformation("Fisk");
        var dat = await req.ReadFromJsonAsync<Request>(serializeOptions);
        if (dat == null || dat.InParams == null ) {
            return Results.BadRequest();
        }
        var param1 = dat.InParams["1"];

        if (param1 != null && Int32.TryParse(param1, out int conv)) {
            var resp = new Response {
                Ref = dat.Ref,
                OutParams = new Dictionary<string, string>()
            };

            resp.OutParams["1"] = (conv + 1).ToString();

            return Results.Json(resp, serializeOptions);
        } else {
            return Results.BadRequest();
        }
    });

// Configure the HTTP request pipeline.
app.UseSwagger(c =>
{
    c.RouteTemplate = "/template/{documentName}";
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/template/igx-openapi", "IGX CSharp Template v1");
});

app.Run();
