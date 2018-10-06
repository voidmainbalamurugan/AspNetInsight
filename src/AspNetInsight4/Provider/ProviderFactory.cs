using AspNetInsight;
using AspNetInsight.Repo;
using AspNetInsight4.Module;
using AspNetInsight4.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetInsight4
{
    /// <summary>
    /// Factory used to choose repositories for configured data provider
    /// </summary>
    public class ProviderFactory
    {
        public IResponseInstrumentation GetInstrumentRepoByProvider(Provider provider)
        {
            switch (provider)
            {
                case Provider.InMemory:
                    return new ResponseInstrumentation(new CacheAppRepo(),
                        new CacheResponseTimeRepo(),
                        new CacheResponseSizeRepo(),
                        new CacheResponseLog()
                        );
                case Provider.SqLite:
                    return new ResponseInstrumentation(new SQLiteAppRepo(),
                        new SQLiteResponseTimeRepo(),
                        new SQLiteResponseSizeRepo(),
                        new LogRepo()
                        );
                case Provider.Others:
                    return null;
            }

            return null;
        }
    }
}
