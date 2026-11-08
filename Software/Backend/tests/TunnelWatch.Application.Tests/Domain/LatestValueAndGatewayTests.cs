using System;
using System.Collections.Generic;
using TunnelWatch.Domain.Gateways;
using TunnelWatch.Domain.Signals;

namespace TunnelWatch.Application.Tests.Domain
{
    /// <summary>
    /// Testovi pravila "ne prepisuj novije starijim" za zadnju vrijednost i vrijeme zadnje poruke uređaja.
    /// </summary>
    public class LatestValueAndGatewayTests
    {
        private static readonly DateTimeOffset Time = new(2026, 11, 7, 4, 26, 19, 665, TimeSpan.Zero);

        [Fact]
        public void LatestValue_NewerTimestamp_IsUpdated()
        {
            var latest = new LatestSignalValue("tdce-01", "engine_speed", "rpm", Time, 769.25);

            var updated = latest.UpdateIfNotOlder("rpm", Time.AddSeconds(1), 801.5);

            Assert.True(updated);
            Assert.Equal(801.5, latest.Value);
            Assert.Equal(Time.AddSeconds(1), latest.Timestamp);
        }

        [Fact]
        public void LatestValue_SameTimestamp_IsUpdated()
        {
            var latest = new LatestSignalValue("tdce-01", "engine_speed", "rpm", Time, 769.25);

            var updated = latest.UpdateIfNotOlder("rpm", Time, 770);

            Assert.True(updated);
            Assert.Equal(770, latest.Value);
        }

        [Fact]
        public void LatestValue_OlderTimestamp_IsIgnored()
        {
            var latest = new LatestSignalValue("tdce-01", "engine_speed", "rpm", Time, 769.25);

            var updated = latest.UpdateIfNotOlder("rpm", Time.AddSeconds(-1), 1);

            Assert.False(updated);
            Assert.Equal(769.25, latest.Value);
            Assert.Equal(Time, latest.Timestamp);
        }

        [Fact]
        public void Gateway_NewerMessage_MovesLastMessageAt()
        {
            var gateway = new Gateway("tdce-01", Time);

            gateway.RegisterMessage(Time.AddMinutes(1));

            Assert.Equal(Time.AddMinutes(1), gateway.LastMessageAt);
        }

        [Fact]
        public void Gateway_OlderMessage_KeepsLastMessageAt()
        {
            var gateway = new Gateway("tdce-01", Time);

            gateway.RegisterMessage(Time.AddMinutes(-1));

            Assert.Equal(Time, gateway.LastMessageAt);
        }
    }
}
