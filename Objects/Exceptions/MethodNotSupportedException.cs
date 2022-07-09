using izolabella.Backend.Objects.Exceptions.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.Objects.Exceptions
{
    public class MethodNotSupportedException : IzolabellaServerException
    {
        public MethodNotSupportedException(string MethodFromClient) : base($"{MethodFromClient} is not a method supported by this server.")
        {
        }
    }
}
