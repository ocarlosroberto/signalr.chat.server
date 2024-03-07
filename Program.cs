using SignalR.Chat.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddRouting();

builder.Services.AddCors(options => {
    options.AddPolicy("All", policy => {
        policy.AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins("https://ashy-smoke-0846a5f0f.5.azurestaticapps.net")
                .AllowCredentials();
    });
});

var app = builder.Build();
app.UseRouting();
app.UseCors("All");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chat");
});

app.Run();
