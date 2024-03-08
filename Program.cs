using SignalR.Chat.Server.Hubs;
using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddRouting();

builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy", policy => {
        policy.WithHeaders("*")
                .AllowAnyMethod()
                .WithOrigins("http://localhost:4200")
                .AllowCredentials();
    });
});

var app = builder.Build();
app.UseRouting();
app.UseCors("CorsPolicy");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chat");
});

app.Run();
