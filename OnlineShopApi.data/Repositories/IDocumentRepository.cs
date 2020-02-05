using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OnlineShopApi.data.Repositories
{
    public interface IDocumentRepository<T>
    {
        IFindFluent<T, T> GetBy(Expression<Func<T, bool>> predicate);
        List<T> Get();
        Task CreateAsync(T match);
    }
}
