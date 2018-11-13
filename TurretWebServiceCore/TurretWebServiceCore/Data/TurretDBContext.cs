using Microsoft.EntityFrameworkCore;
using Params;
using TurretWebServiceCore.Models;

namespace TurretWebServiceCore.Data
{
    public class TurretDBContext: DbContext
    {
        public TurretDBContext(DbContextOptions<TurretDBContext> options) : base(options)
        {
            Database.EnsureCreated();   
        }

        //Инициализация контекста начальными данными, если они отсутствуют.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User[]
                {
                    new User { Id = 1, Name = "Винни-Пух", Password = PasswordService.GetPasswordHash("123456"), MaxLevel = 0, MaxScore = 0 },
                    new User { Id = 2, Name = "Пятачок", Password = PasswordService.GetPasswordHash("123456"), MaxLevel = 0, MaxScore = 0 },
                    new User { Id = 3, Name = "Кролик", Password = PasswordService.GetPasswordHash("123456"), MaxLevel = 0, MaxScore = 0 }
                });
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}