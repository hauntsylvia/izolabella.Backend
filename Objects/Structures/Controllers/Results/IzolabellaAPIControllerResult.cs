using izolabella.Storage.Objects.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.Objects.Structures.Controllers.Results
{
    public class IzolabellaAPIControllerResult
    {
        public IzolabellaAPIControllerResult()
        {
        }

        public IzolabellaAPIControllerResult(object? Entity)
        {
            this.Entity = Entity;
        }

        public IzolabellaAPIControllerResult(byte[]? Bytes)
        {
            this.Bytes = Bytes;
        }

        public object? Entity { get; }

        public byte[]? Bytes { get; }
    }
}
