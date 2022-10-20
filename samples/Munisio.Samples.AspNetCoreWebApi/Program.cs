using Munisio;
using Munisio.Samples.AspNetCoreWebApi.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(x => x.AddHateoas()); // 1: Call AddHateoas!

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<UserDatabase>();
builder.Services.AddSingleton<TweetDatabase>();

builder.Services.AddHateoasProviders(); // 2: Let munisio search and register your providers automatically OR
//services.AddTransient<IHateoasProvider<Tweet>, TweetHateoasProvider>(); // 3: Do it yourself!

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
