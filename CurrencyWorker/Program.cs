using System.Text;
using CurrencyWorker;
using CurrencyWorker.Persistence;
using Microsoft.EntityFrameworkCore;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<WorkerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient("cbr");

builder.Services.AddSingleton<CbrXmlParser>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
