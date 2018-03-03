using System;

namespace ESource.Base
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        T GetById(Guid id);
        void Save(T aggregate, int expectedVersion);
    }
}
