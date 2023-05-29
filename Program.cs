using Microsoft.EntityFrameworkCore;
using System.Reflection;
using trackingApi.Data;
using trackingApi.GenericRepository;
using trackingApi.NonGenericRepository;
using trackingApi.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Add SQLite database connection string
var connectionString = builder.Configuration.GetConnectionString("Issues") ?? "Data Source=Issues.db";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Add DbContext to the SQLite
builder.Services.AddSqlite<IssueDbContext>(connectionString);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
builder.Services.AddAutoMapper(typeof(Program));

//builder.Services.AddDbContext<IssueDbContext>(options=>options.UseSqlServer(
//    builder.Configuration.GetConnectionString("myDb1")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
