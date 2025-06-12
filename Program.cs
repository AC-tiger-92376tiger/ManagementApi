using ManagementApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
/*
// Set the connection string from the environment variable
builder.Configuration["ConnectionStrings:DefaultConnection"] = 
    Environment.GetEnvironmentVariable("DefaultConnection");
*/
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
/*
builder.Services.AddDbContext<StudentContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("StudentConnection")));
*/
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Replace with your frontend's URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
//builder.WebHost.UseUrls("http://0.0.0.0:8080");
var app = builder.Build();
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};



app.UseAuthorization();

app.MapControllers();

app.Run();
