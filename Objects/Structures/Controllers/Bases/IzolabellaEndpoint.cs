using izolabella.Backend.Objects.Structures.Controllers.Arguments;
using izolabella.Backend.Objects.Structures.Controllers.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.Objects.Structures.Controllers.Bases
{
    public abstract class IzolabellaEndpoint
    {
        public abstract string Route { get; }

        public virtual List<IzolabellaControllerParameter> Parameters { get; } = new();

        public abstract Task<IzolabellaAPIControllerResult> RunAsync(IzolabellaControllerArgument Arguments);

        public virtual Task OnErrorAsync(Exception Ex)
        {
            return Task.CompletedTask;
        }
    }
}
