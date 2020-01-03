using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidFirewallManager.Core
{
    public class Sealer
    {
        private readonly Configuration _configuration;
        private const String SealRulePrefix = "_sfm_block_";

        public Sealer(Configuration configuration)
        {
            _configuration = configuration;
        }

        public void Seal() 
        {
                
        }
    }
}
