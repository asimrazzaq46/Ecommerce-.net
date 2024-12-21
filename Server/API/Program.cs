using Core.Interfaces;
using Infastructure.Data;
using Infastructure.Repositery;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<StoreContext>(opt=>
opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepositery,ProductRepositery>();


var app = builder.Build();


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