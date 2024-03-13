using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;
using TestTaskApi.Database;
using TestTaskApi.Helper;
using TestTaskApi.Interface;
using TestTaskApi.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("conn")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
string secretKey = builder.Configuration.GetSection("JwtSettings:SecretKey").Value;
int tokenExpirationInMinutes = int.Parse(builder.Configuration.GetSection("JwtSettings:TokenExpirationInMinutes").Value);
var key = Encoding.ASCII.GetBytes(secretKey);
builder.Services.AddScoped<JwtHelper>(sp =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    return new JwtHelper(secretKey, tokenExpirationInMinutes, httpContextAccessor);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://localhost:5001")); // Replace with the origin of your MVC application
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();

app.MapControllers();

app.Run();
