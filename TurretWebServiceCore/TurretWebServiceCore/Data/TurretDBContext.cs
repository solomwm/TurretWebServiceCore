using Microsoft.EntityFrameworkCore;
using TurretWebServiceCore.Models;

namespace TurretWebServiceCore.Data
{
    public class TurretDBContext: DbContext
    {
        public TurretDBContext(DbContextOptions<TurretDBContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
