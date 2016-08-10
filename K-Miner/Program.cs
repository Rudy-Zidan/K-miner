using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K_miner;

namespace K_Miner
{
    class Program
    {
        static void Main(string[] args)
        {
            K_miner.K_miner miner = new K_miner.K_miner();
            miner.Start();
        }
    }
}
