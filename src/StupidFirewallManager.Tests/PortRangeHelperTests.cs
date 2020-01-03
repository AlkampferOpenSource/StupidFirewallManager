using NUnit.Framework;
using StupidFirewallManager.Core;
using System;
using System.Collections.Generic;

namespace StupidFirewallManager.Tests
{
    [TestFixture]
    public class PortRangeHelperTests
    {
        [TestCase(new[] { 1 }, new[] { 2, 65535 })]
        [TestCase(new[] { 65535 }, new[] { 1, 65534 })]
        [TestCase(new[] { 1,2 }, new[] { 3, 65535 })]
        [TestCase(new[] { 65534, 65535 }, new[] { 1, 65533 })]
        [TestCase(new[] { 1, 3 }, new[] { 2, 2, 4, 65535 })]
        [TestCase(new[] { 1, 65534, 65535 }, new[] { 2, 65533 })]
        public void Edge_cases(Int32[] ports, Int32[] rangesExpected)
        {
            PortRangeHelper.Range[] expected = CreateRange(rangesExpected);
            var ranges = PortRangeHelper.GetRangeExclusive(ports);
            Assert.That(ranges, Is.EquivalentTo(expected));
        }

        [TestCase(new[] { 1, 5 }, new[] { 2, 4, 6, 65535 })]
        [TestCase(new[] { 10, 3456 }, new[] { 1, 9, 11, 3455, 3457, 65535 })]
        public void Standard_ranges(Int32[] ports, Int32[] rangesExpected)
        {
            PortRangeHelper.Range[] expected = CreateRange(rangesExpected);
            var ranges = PortRangeHelper.GetRangeExclusive(ports);
            Assert.That(ranges, Is.EquivalentTo(expected));
        }

        [TestCase("1000", new[] { 1000 }, true)]
        [TestCase("1000", new[] { 2000, 1000 }, true)]
        [TestCase("1000", new[] { 2000, 3000 }, false)]
        [TestCase("1000-5000", new[] { 2000 }, true)]
        [TestCase("1000,5000", new[] { 2000 }, false)]
        [TestCase("1000,5000", new[] { 5000 }, true)]
        [TestCase("1000,5000-6000", new[] { 5500 }, true)]
        public void Contains_checker(String firewallPortDefinition, Int32[] ports, Boolean expected)
        {
            var containsRange = PortRangeHelper.RangeContainsPort(firewallPortDefinition, ports);
            Assert.That(containsRange, Is.EqualTo(expected));
        }

        private PortRangeHelper.Range[] CreateRange(int[] rangesExpected)
        {
            var rangeList = new List<PortRangeHelper.Range>();
            Int32 i = 0;
            while (i < rangesExpected.Length)
            {
                rangeList.Add(new PortRangeHelper.Range(rangesExpected[i++], rangesExpected[i++]));
            }
            return rangeList.ToArray();
        }
    }
}
