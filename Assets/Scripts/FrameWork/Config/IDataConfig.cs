using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface IDataConfig
{
    public void Init(List<string[]> data);

   // public IDataConfigLine GetConfigByKey(string keyName, string value);

    //public List<IDataConfigLine> GetConfigsByKey(string keyName, string value);
}
