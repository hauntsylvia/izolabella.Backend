using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.Objects.Exceptions.Bases
{
    public class IzolabellaServerException : Exception
    {
        public IzolabellaServerException(string Message) : base(Message)
        {
        }
    }
}
