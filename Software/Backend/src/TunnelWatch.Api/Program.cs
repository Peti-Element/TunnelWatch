using TunnelWatch.Application.Signals;
using TunnelWatch.Infrastructure.Mqtt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// MQTT: postavke iz sekcije "Mqtt", parser poruka i pozadinska pretplata na broker.
builder.Services.AddOptions<MqttOptions>()
    .Bind(builder.Configuration.GetSection(MqttOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Host) && o.Port > 0 && !string.IsNullOrWhiteSpace(o.TopicPrefix),
        "Sekcija 'Mqtt' mora imati Host, Port i TopicPrefix.")
    .ValidateOnStart();
builder.Services.AddSingleton<IMachineMessageParser, MachineMessageParser>();
builder.Services.AddHostedService<MqttSubscriptionService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "TunnelWatch API"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
