using API.Middlewares;
using Core.Interfaces;
using Core.Models;
using Infastructure.Data;
using Infastructure.Repositery;
using Infastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<StoreContext>(opt=>
opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepositery,ProductRepositery>();
builder.Services.AddScoped(typeof(IGenericRepositery<>),typeof(GenericRepositery<>));


builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
{
	var connString = builder.Configuration.GetConnectionString("Redis")
	?? throw new Exception("Cannot Get Reddis connection string");
    
	var configuration = ConfigurationOptions.Parse(connString,true);
	
	return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<ICartService, CartService>();


builder.Services.AddCors(policy =>
{
	policy.AddPolicy("cors-policy", opt =>
	{
		opt.AllowAnyHeader()
		.AllowAnyMethod()
		.WithOrigins("http://localhost:4200", "https://localhost:4200");
	});
}
);


var app = builder.Build();


app.UseMiddleware<ErrorHandling>();
app.UseCors("cors-policy");
app.UseAuthorization();

app.MapControllers();



try
{
	using var scope = app.Services.CreateScope();
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<StoreContext>();
	await context.Database.MigrateAsync();

	await SeedStoreContext.SeedAsync(context);

}
catch (Exception ex)
{
    Console.WriteLine(ex);

	throw;
}





app.Run();
