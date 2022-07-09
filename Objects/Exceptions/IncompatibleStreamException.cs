using izolabella.Backend.Objects.Exceptions.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.Objects.Exceptions
{
    public class IncompatibleStreamException : IzolabellaServerException
    {
        public IncompatibleStreamException() : base("The client's stream could not be read.")
        {

        }
    }
}
