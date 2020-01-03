using NUnit.Framework;
using StupidFirewallManager.Core;
using System;

namespace StupidFirewallManager.Tests
{
    [TestFixture]
    public class DateTimeEncodingHelperTests
    {
        [Test]
        public void BasicParsing() 
        {
            var parsing = DateTimeEncodingHelper.TryParse(Constants.TcpRulePrefix + "201010111230", out var date);
            Assert.That(parsing);
            Assert.That(date, Is.EqualTo(new DateTime(2010, 10, 11, 12, 30, 00, DateTimeKind.Utc)));
        }

        [TestCase(Constants.TcpRulePrefix + "201010111270")]
        [TestCase(Constants.TcpRulePrefix + "20101011")]
        public void not_correct_Parsing(String dateToParse)
        {
            var parsing = DateTimeEncodingHelper.TryParse(dateToParse, out var date);
            Assert.That(parsing, Is.False);
        }
    }
}
