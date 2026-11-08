using Microsoft.EntityFrameworkCore;
using TunnelWatch.Application.Signals;
using TunnelWatch.Infrastructure.Mqtt;
using TunnelWatch.Infrastructure.Persistence;
using TunnelWatch.Application.Gateways;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// MQTT: postavke iz sekcije "Mqtt", parser poruka i pozadinska pretplata na broker.
builder.Services.AddOptions<MqttOptions>()
    .Bind(builder.Configuration.GetSection(MqttOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Host) && o.Port > 0 && !string.IsNullOrWhiteSpace(o.TopicFilter),
        "Sekcija 'Mqtt' mora imati Host, Port i TopicFilter.")
    .ValidateOnStart();
builder.Services.AddSingleton<IMachineMessageParser, MachineMessageParser>();
builder.Services.AddHostedService<MqttSubscriptionService>();

builder.Services.AddDbContext<TunnelWatchDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IMachineSignalStore, MachineSignalStore>();
builder.Services.AddScoped<IGatewayQueries, GatewayQueries>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<TunnelWatchDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    app.MapOpenApi();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "TunnelWatch API"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
