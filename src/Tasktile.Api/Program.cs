using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Tasktile.Api.Auth;
using Tasktile.Api.Data;
using Tasktile.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// ---------- Database ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// ---------- Auth ----------
// Dùng được cho cả Firebase lẫn Sign in with Apple, chỉ khác Authority/Audience trong config.
var authOptions = builder.Configuration.GetSection("Auth").Get<AuthOptions>() ?? new AuthOptions();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authOptions.Authority;
        options.MapInboundClaims = false; // giữ nguyên claim "sub"
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authOptions.Authority,
            ValidateAudience = true,
            ValidAudience = authOptions.Audience,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();

// Chỉ tự migrate khi đã có migration (tránh lỗi lần chạy đầu tiên).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.GetMigrations().Any())
    {
        db.Database.Migrate();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // /openapi/v1.json
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTimeOffset.UtcNow }));
app.MapMeEndpoints();
app.MapTaskEndpoints();

app.Run();
