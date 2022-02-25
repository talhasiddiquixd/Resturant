using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Models
{
    public class UnitOfWork : IunitOfWork
    {
        public RestaurantPosDBContext Context { get; }
        public RestaurantLivePosDBContext LiveContext { get; }

        public UnitOfWork(RestaurantPosDBContext context, RestaurantLivePosDBContext liveContext)
        {
            Context = context;
            LiveContext = liveContext;
        }

        public async Task<int> Commit()
        {
            return await Context.SaveChangesAsync();
        }
        public async Task<int> CommitLive()
        {
            return await LiveContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            Context.Dispose();
        }
    }
    public interface IunitOfWork : IDisposable
    {
        RestaurantPosDBContext Context { get; }
        RestaurantLivePosDBContext LiveContext { get; }
        Task<int> Commit();
        Task<int> CommitLive();
    }
}
