using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K_miner.Core.DataBase
{

    class SqlQueryParameter
    {
        private string parameter;
        private string value;

        public SqlQueryParameter()
        {
            this.Parameter = this.Value = string.Empty;
        }
        public SqlQueryParameter(string param, string value)
        {
            this.Parameter = param;
            this.value = value;
        }
        public void Add(string param, string value)
        {
            this.Parameter = param;
            this.value = value;
        }
        public string Parameter
        {
            get { return parameter; }
            set { parameter = value; }
        }
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
