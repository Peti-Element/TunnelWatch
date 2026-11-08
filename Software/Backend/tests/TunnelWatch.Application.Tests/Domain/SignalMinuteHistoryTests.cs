using TunnelWatch.Domain.Signals;

namespace TunnelWatch.Application.Tests.Domain
{
    /// <summary>
    /// Testovi zaokruživanja na minutu i ažuriranja prosjeka, minimuma i maksimuma.
    /// </summary>
    public class SignalMinuteHistoryTests
    {
        private static readonly DateTimeOffset Minute = new(2026, 11, 7, 4, 26, 0, TimeSpan.Zero);

        [Fact]
        public void ToMinute_DropsSecondsAndMilliseconds()
        {
            var timestamp = new DateTimeOffset(2026, 11, 7, 4, 26, 19, 665, TimeSpan.Zero);

            Assert.Equal(Minute, SignalMinuteHistory.ToMinute(timestamp));
        }

        [Fact]
        public void ToMinute_LastMillisecondOfMinute_StaysInSameMinute()
        {
            var timestamp = new DateTimeOffset(2026, 11, 7, 4, 26, 59, 999, TimeSpan.Zero);

            Assert.Equal(Minute, SignalMinuteHistory.ToMinute(timestamp));
        }

        [Fact]
        public void ToMinute_NonUtcOffset_ReturnsUtcMinute()
        {
            var timestamp = new DateTimeOffset(2026, 11, 7, 5, 26, 30, TimeSpan.FromHours(1));

            var minute = SignalMinuteHistory.ToMinute(timestamp);

            Assert.Equal(Minute, minute);
            Assert.Equal(TimeSpan.Zero, minute.Offset);
        }

        [Fact]
        public void Start_UsesFirstValueForAllAggregates()
        {
            var history = SignalMinuteHistory.CreateFromSignal("tdce-01", "engine_speed", Minute.AddSeconds(19), 769.25, "rpm");

            Assert.Equal(Minute, history.Minute);
            Assert.Equal(769.25, history.Average);
            Assert.Equal(769.25, history.Minimum);
            Assert.Equal(769.25, history.Maximum);
            Assert.Equal(1, history.SampleCount);
            Assert.Equal("rpm", history.Unit);
        }

        [Fact]
        public void AddSample_UpdatesAverageMinMaxAndCount()
        {
            var history = SignalMinuteHistory.CreateFromSignal("tdce-01", "engine_speed", Minute, 10, "rpm");

            history.AddSample(20, "rpm");
            history.AddSample(60, "rpm");

            Assert.Equal(30, history.Average, precision: 10);
            Assert.Equal(10, history.Minimum);
            Assert.Equal(60, history.Maximum);
            Assert.Equal(3, history.SampleCount);
        }

        [Fact]
        public void AddSample_ManyValues_AverageMatchesArithmeticMean()
        {
            double[] values = [769.25, 769, 769.375, 801.5, 1801.75, 0, 12.125];
            var history = SignalMinuteHistory.CreateFromSignal("tdce-01", "engine_speed", Minute, values[0], "rpm");

            foreach (var value in values.Skip(1))
            {
                history.AddSample(value, "rpm");
            }

            Assert.Equal(values.Average(), history.Average, precision: 9);
            Assert.Equal(values.Min(), history.Minimum);
            Assert.Equal(values.Max(), history.Maximum);
            Assert.Equal(values.Length, history.SampleCount);
        }

        [Fact]
        public void AddSample_WithoutUnit_KeepsExistingUnit()
        {
            var history = SignalMinuteHistory.CreateFromSignal("tdce-01", "engine_speed", Minute, 10, "rpm");

            history.AddSample(20, null);

            Assert.Equal("rpm", history.Unit);
        }
    }
}
