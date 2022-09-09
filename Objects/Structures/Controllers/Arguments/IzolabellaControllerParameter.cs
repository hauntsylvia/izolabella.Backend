using izolabella.Storage.Objects.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.Objects.Structures.Controllers.Arguments;

public class IzolabellaControllerParameter
{
    public IzolabellaControllerParameter(string Name, string Description, bool IsRequired)
    {
        this.Name = Name;
        this.Description = Description;
        this.IsRequired = IsRequired;
    }

    public string Name { get; }

    public string Description { get; }

    public bool IsRequired { get; }
}
