using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.REST.Objects.Structures.Controllers
{
    public abstract class IzolabellaAPIController
    {
        public abstract string Route { get; }

        public abstract Task<IzolabellaAPIControllerResult> RunAsync();
    }
}
