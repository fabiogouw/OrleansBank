using Orleans;
using Orleans.Hosting;
using OrleansBank.Adapters;
using OrleansBank.UseCases;
using OrleansBank.UseCases.Ports.In;
using OrleansBank.UseCases.Ports.Out;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ITransferMoneyUseCase, TransferMoneyUseCaseImpl>();
builder.Services.AddSingleton<IGetBalanceUseCase, GetBalanceUseCaseImpl>();
builder.Services.AddSingleton<IAccountRepository, OrleansAccountRepository>();

builder.Host.UseOrleans(siloBuilder => 
{
    siloBuilder.UseLocalhostClustering();
    //siloBuilder.AddMemoryGrainStorage("Accounts");
    siloBuilder.AddIdempotentySqlServerGrainStorage("Accounts", options =>
    {
        options.ConnectionString = builder.Configuration["Orleans:Storage:Accounts:ConnectionString"];
    });
});

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
