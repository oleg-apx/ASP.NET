var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "ViteFrontend",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5174")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseCors("ViteFrontend");
app.MapControllers();
app.Run();
