using CodigoDelSurApi.Infrastructure.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CodigoDelSurApi.Infrastructure
{
    public interface ICodigoDelSurDbContext
    {
        DbSet<UserPreferences> UserPreferences { get; set; }
        DbSet<User> Users { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}