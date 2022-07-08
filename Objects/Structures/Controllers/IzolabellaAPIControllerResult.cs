using izolabella.Storage.Objects.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.REST.Objects.Structures.Controllers
{
    public class IzolabellaAPIControllerResult
    {
        public IzolabellaAPIControllerResult()
        {
        }

        public IzolabellaAPIControllerResult(IDataStoreEntity? Entity)
        {
            this.Entity = Entity;
        }

        public IDataStoreEntity? Entity { get; }
    }
}
