using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using TunnelWatch.Application.Signals;

namespace TunnelWatch.Application.Tests.Signals;

/// <summary>
/// Testovi parsera na stvarnim snimkama s uređaja i na ručno složenim rubnim slučajevima.
/// </summary>
public class MachineMessageParserTests
{
    private const string Topic = "tdce-01/can0/j1939-default";

    private readonly MachineMessageParser _parser = new(NullLogger<MachineMessageParser>.Instance);

    /// <summary>
    /// Svaki redak obje snimke kao zaseban slučaj testa: (datoteka, broj retka).
    /// </summary>
    public static TheoryData<string, int> SampleLines()
    {
        var data = new TheoryData<string, int>();
        foreach (var file in new[] { SampleFiles.Truck, SampleFiles.DrillRig })
        {
            var count = SampleFiles.ReadLines(file).Count();
            for (var i = 0; i < count; i++)
            {
                data.Add(file, i);
            }
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(SampleLines))]
    public void Parse_SampleLine_ReturnsSignalsWithoutDuplicates(string file, int lineIndex)
    {
        var (topic, payload) = SampleFiles.ReadLines(file).ElementAt(lineIndex);

        var signals = _parser.Parse(topic, payload);

        Assert.NotEmpty(signals);
        Assert.All(signals, s =>
        {
            Assert.Equal("tdce-01", s.GatewayId);
            Assert.Equal("can0", s.Port);
            Assert.Equal("j1939-default", s.Profile);
        });
        Assert.Equal(signals.Count, signals.Select(s => (s.Name, s.Timestamp)).Distinct().Count());
    }

    [Fact]
    public void Parse_TruckSample_ReadsUnits()
    {
        var signals = SampleFiles.ReadLines(SampleFiles.Truck)
            .SelectMany(line => _parser.Parse(line.Topic, line.Payload))
            .ToList();

        Assert.Contains(signals, s => s.Name == "engine_speed");
        Assert.All(signals.Where(s => s.Name == "engine_speed"), s => Assert.Equal("rpm", s.Unit));

        Assert.Contains(signals, s => s.Name == "parking_brake_switch");
        Assert.All(signals.Where(s => s.Name == "parking_brake_switch"), s => Assert.Null(s.Unit));
    }

    [Fact]
    public void Parse_UnixMilliseconds_ConvertsToUtc()
    {
        const string payload = """
            {"version":1,"port":"can0","profile":"j1939-default","messages":[
              {"id":217056256,"timestamp":1794025579665,"signals":{"engine_speed":{"value":769.25,"unit":"rpm"}}}
            ]}
            """;

        var signal = Assert.Single(_parser.Parse(Topic, payload));

        Assert.Equal(new DateTimeOffset(2026, 11, 7, 4, 26, 19, 665, TimeSpan.Zero), signal.Timestamp);
        Assert.Equal(TimeSpan.Zero, signal.Timestamp.Offset);
    }

    [Fact]
    public void Parse_IntegerValue_IsReadAsDouble()
    {
        const string payload = """
            {"messages":[{"id":1,"timestamp":1794025579665,"signals":{"engine_speed":{"value":769,"unit":"rpm"}}}]}
            """;

        var signal = Assert.Single(_parser.Parse(Topic, payload));

        Assert.Equal(769.0, signal.Value);
    }

    [Fact]
    public void Parse_DuplicateMessages_AreRemoved()
    {
        const string payload = """
            {"messages":[
              {"id":217056256,"timestamp":1794025579665,"signals":{"engine_speed":{"value":769.25,"unit":"rpm"},"drivers_demand_engine_percent_torque":{"value":9,"unit":"%"}}},
              {"id":217056256,"timestamp":1794025579665,"signals":{"engine_speed":{"value":769.25,"unit":"rpm"},"drivers_demand_engine_percent_torque":{"value":9,"unit":"%"}}},
              {"id":217056256,"timestamp":1794025579682,"signals":{"engine_speed":{"value":769,"unit":"rpm"}}}
            ]}
            """;

        var signals = _parser.Parse(Topic, payload);

        Assert.Equal(3, signals.Count);
        Assert.Equal(2, signals.Count(s => s.Name == "engine_speed"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not json")]
    [InlineData("{\"messages\":[")]
    [InlineData("[]")]
    [InlineData("42")]
    [InlineData("{\"version\":1}")]
    public void Parse_InvalidJson_ReturnsEmpty(string payload)
    {
        var signals = _parser.Parse(Topic, payload);

        Assert.Empty(signals);
    }

    [Theory]
    [InlineData("")]
    [InlineData("tdce-01")]
    [InlineData("tdce-01/can0")]
    [InlineData("tdce-01//j1939-default")]
    [InlineData("tdce-01/can0/j1939-default/extra")]
    public void Parse_InvalidTopic_ReturnsEmpty(string topic)
    {
        var (_, payload) = SampleFiles.ReadLines(SampleFiles.Truck).First();

        var signals = _parser.Parse(topic, payload);

        Assert.Empty(signals);
    }

    [Fact]
    public void Parse_NonNumericValue_IsSkippedAndOthersAreParsed()
    {
        const string payload = """
            {"messages":[{"id":1,"timestamp":1794025579665,"signals":{
              "text_value":{"value":"abc","unit":"rpm"},
              "null_value":{"value":null},
              "missing_value":{"unit":"%"},
              "engine_speed":{"value":769.25,"unit":"rpm"}
            }}]}
            """;

        var signal = Assert.Single(_parser.Parse(Topic, payload));

        Assert.Equal("engine_speed", signal.Name);
    }

    [Fact]
    public void Parse_Utf8Bytes_GivesSameResultAsString()
    {
        var (topic, payload) = SampleFiles.ReadLines(SampleFiles.Truck).First();

        var fromString = _parser.Parse(topic, payload);
        var fromBytes = _parser.Parse(topic, Encoding.UTF8.GetBytes(payload));

        Assert.Equal(fromString, fromBytes);
    }
}