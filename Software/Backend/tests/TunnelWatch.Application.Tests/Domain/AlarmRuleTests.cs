using TunnelWatch.Domain.Alarms;

namespace TunnelWatch.Application.Tests.Domain
{
    /// <summary>
    /// Testovi pravila alarma: ispravno pravilo se sprema u svim poljima, neispravno se odbija bez izmjena.
    /// </summary>
    public class AlarmRuleTests
    {
        private static AlarmRule CoolantRule() =>
            new("tdce-01", "engine_coolant_temperature", ComparisonOperator.GreaterThan, 95, 10, 3, AlarmSeverity.Critical, 300, true);

        [Fact]
        public void Constructor_ValidRule_SetsAllFields()
        {
            var rule = CoolantRule();

            Assert.Equal("tdce-01", rule.GatewayId);
            Assert.Equal("engine_coolant_temperature", rule.SignalName);
            Assert.Equal(ComparisonOperator.GreaterThan, rule.Operator);
            Assert.Equal(95, rule.Threshold);
            Assert.Equal(10, rule.DurationSeconds);
            Assert.Equal(3, rule.Hysteresis);
            Assert.Equal(AlarmSeverity.Critical, rule.Severity);
            Assert.Equal(300, rule.RepeatSuppressionSeconds);
            Assert.True(rule.Enabled);
        }

        [Fact]
        public void Constructor_TrimsGatewayAndSignal()
        {
            var rule = new AlarmRule(" tdce-01 ", " engine_speed ", ComparisonOperator.LessThan, 500, 0, 0, AlarmSeverity.Info, 0, true);

            Assert.Equal("tdce-01", rule.GatewayId);
            Assert.Equal("engine_speed", rule.SignalName);
        }

        [Theory]
        [InlineData("", "engine_speed")]
        [InlineData("   ", "engine_speed")]
        [InlineData("tdce-01", "")]
        public void Constructor_EmptyGatewayOrSignal_Throws(string gatewayId, string signalName)
        {
            Assert.ThrowsAny<ArgumentException>(() => new AlarmRule(gatewayId, signalName, ComparisonOperator.GreaterThan, 1, 0, 0, AlarmSeverity.Info, 0, true));
        }

        [Fact]
        public void Constructor_NegativeDuration_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlarmRule("tdce-01", "engine_speed", ComparisonOperator.GreaterThan, 1, -1, 0, AlarmSeverity.Info, 0, true));
        }

        [Fact]
        public void Constructor_NegativeHysteresis_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlarmRule("tdce-01", "engine_speed", ComparisonOperator.GreaterThan, 1, 0, -0.5, AlarmSeverity.Info, 0, true));
        }

        [Fact]
        public void Constructor_NegativeRepeatSuppression_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlarmRule("tdce-01", "engine_speed", ComparisonOperator.GreaterThan, 1, 0, 0, AlarmSeverity.Info, -1, true));
        }

        [Fact]
        public void Constructor_UnknownOperator_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AlarmRule("tdce-01", "engine_speed", (ComparisonOperator)99, 1, 0, 0, AlarmSeverity.Info, 0, true));
        }

        [Fact]
        public void Update_ValidValues_ChangesRuleAndCanDisableIt()
        {
            var rule = CoolantRule();

            rule.Update("tdce-02", "fuel_level", ComparisonOperator.LessThanOrEqual, 10, 30, 2, AlarmSeverity.Warning, 600, false);

            Assert.Equal("tdce-02", rule.GatewayId);
            Assert.Equal("fuel_level", rule.SignalName);
            Assert.Equal(ComparisonOperator.LessThanOrEqual, rule.Operator);
            Assert.Equal(10, rule.Threshold);
            Assert.Equal(30, rule.DurationSeconds);
            Assert.Equal(2, rule.Hysteresis);
            Assert.Equal(AlarmSeverity.Warning, rule.Severity);
            Assert.Equal(600, rule.RepeatSuppressionSeconds);
            Assert.False(rule.Enabled);
        }

        [Fact]
        public void Update_InvalidValue_LeavesRuleUnchanged()
        {
            var rule = CoolantRule();

            Assert.Throws<ArgumentOutOfRangeException>(() => rule.Update("tdce-02", "fuel_level", ComparisonOperator.LessThan, 10, -5, 2, AlarmSeverity.Warning, 600, false));

            Assert.Equal("tdce-01", rule.GatewayId);
            Assert.Equal(10, rule.DurationSeconds);
            Assert.True(rule.Enabled);
        }
    }
}
