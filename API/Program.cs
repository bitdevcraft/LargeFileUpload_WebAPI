using API.Extensions;
using Microsoft.AspNetCore.Http.Features;
using tusdotnet;
using tusdotnet.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
// app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles();
app.Use((context, next) =>
{
    // Default limit was changed some time ago. Should work by setting MaxRequestBodySize to null using ConfigureKestrel but this does not seem to work for IISExpress.
    // Source: https://github.com/aspnet/Announcements/issues/267
    context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = null;
    return next.Invoke();
});
app.UseTus(httpContext => Task.FromResult(httpContext.RequestServices.GetService<DefaultTusConfiguration>()));
app.UseRouting();

app.MapControllers();

app.Run();
