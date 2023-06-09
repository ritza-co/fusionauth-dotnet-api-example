var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Events = new()
        {
            OnMessageReceived = context =>
            {
                // Get the token from a cookie
                context.Token = context.Request.Cookies["app.idt"];

                return Task.CompletedTask;
            }
        };
        options.Authority = "http://localhost:9011";
        options.TokenValidationParameters.ValidateAudience = false;
        options.RequireHttpsMetadata = false; // For testing only, set true in production.
                                                  
    });
builder.Services.AddAuthorization(options =>
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        //policy.RequireClaim("scope", "api1");
    })
);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowLocalFrontendCORS",
                      policy =>
                      {
                          policy
                            .WithOrigins("http://localhost:3000")
                            .AllowCredentials();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalFrontendCORS");
//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

