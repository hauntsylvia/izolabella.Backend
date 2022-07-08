using izolabella.Storage.Objects.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.REST.Objects.Structures.Entities
{
    public interface IPartialDataStoreEntity<T> where T : IDataStoreEntity
    {
        T? Wrapped { get; }
        ulong Id { get; }
    }
}
