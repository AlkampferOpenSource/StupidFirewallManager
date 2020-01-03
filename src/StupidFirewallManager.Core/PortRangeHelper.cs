using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidFirewallManager.Core
{
    public static class PortRangeHelper
    {
        /// <summary>
        /// Given a list of port that we want to be open, this method
        /// will return a series of ranges of port to close.
        /// </summary>
        /// <param name="ports"></param>
        /// <returns></returns>
        public static Range[] GetRangeExclusive(params Int32[] ports) 
        {
            List<Range> ranges = new List<Range>();
            Int32 actualPortSequence = 1;
            Int32 portIndex = 0;
            while (portIndex < ports.Length) 
            {
                var currentPort = ports[portIndex];
                if (currentPort > actualPortSequence)
                {
                    ranges.Add(new Range(actualPortSequence, currentPort - 1));
                }
                actualPortSequence = currentPort + 1;
                portIndex++;
            }
            if (actualPortSequence <= 65535)
            {
                ranges.Add(new Range(actualPortSequence, 65535));
            }
            return ranges.ToArray();
        }

        public struct Range 
        {
            public Range(int lowerPortInclusive, int upperPortInclusive)
            {
                LowerPortInclusive = lowerPortInclusive;
                UpperPortInclusive = upperPortInclusive;
            }

            public Int32 LowerPortInclusive { get; set; }
            public Int32 UpperPortInclusive { get; set; }

            public override bool Equals(object obj)
            {
                return obj is Range range &&
                       LowerPortInclusive == range.LowerPortInclusive &&
                       UpperPortInclusive == range.UpperPortInclusive;
            }

            public static bool operator ==(Range left, Range right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Range left, Range right)
            {
                return !(left == right);
            }

            public override string ToString()
            {
                return $"Range: {LowerPortInclusive}-{UpperPortInclusive}";
            }
        }
    }
}
