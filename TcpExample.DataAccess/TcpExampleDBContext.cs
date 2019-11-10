using TcpExample.Domain.DBModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using TcpExample.Domain.Common;

namespace TcpExample.DataAccessCore
{
    public partial class TcpExampleDBContext : DbContext, ITcpExampleDBContext
    {
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DataCollection> DataCollections { get; set; }
        public DbSet<User> Users { get; set; }

        public TcpExampleDBContext(DbContextOptions<TcpExampleDBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            Guid currentUsername = ConstantsVariables.AdminstratorId;// null;// "we moust get current user name or null";

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).DateCreated = DateTime.UtcNow;
                    ((BaseEntity)entity.Entity).UserCreated = currentUsername;
            
                    ((BaseEntity)entity.Entity).IsDeleted = false;
                }

                ((BaseEntity)entity.Entity).DateModified = DateTime.UtcNow;
                ((BaseEntity)entity.Entity).UserModified = currentUsername;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Device>()
                .HasKey(c => new { c.MarkCode, c.SerialNumber });

            modelBuilder.Entity<DataCollection>().HasOne(p => p.Device).WithMany(a => a.DataCollections)
                .HasForeignKey(p => new { p.MarkCode, p.SerialNumber });


            modelBuilder.Entity<DataCollection>()
            .HasIndex(b => b.DateCreated);

            #region Default data

            var AdminId = ConstantsVariables.AdminstratorId;
           
            var now = DateTime.Now;
            modelBuilder.Entity<User>().HasData(
             new User
             {
                 Id = AdminId,
                 FirstName = "admin",
                 LastName = "admin",
                 BirthDate = now,
                 DateCreated = now,
                 DateModified = now,
                 UserModified = AdminId,
                 UserCreated = AdminId,
                 IsDeleted = false,
                 UserName = "admin",
                 Password = HashConvertor.GetHashString("admin"),
                 
                 Picture = ""

             });


            for (int i = 65; i<= 75; i++)
            {
                modelBuilder.Entity<Mark>().HasData(
                new Mark
                {
                    Code =  Convert.ToChar(i).ToString() ,
                    Name = "Mark "+(i-64),
                    DateCreated = now,
                    DateModified = now,
                    UserModified = AdminId,
                    UserCreated = AdminId,
                    IsDeleted = false
                });
                modelBuilder.Entity<Device>().HasData(
                new Device
                {
                    MarkCode = Convert.ToChar(i).ToString(),
                    SerialNumber = 123456,
                    Port="8091",
                    IpAddress="127.0.0.1",
                    DateCreated = now,
                    DateModified = now,
                    UserModified = AdminId,
                    UserCreated = AdminId,
                    IsDeleted = false
                });
                modelBuilder.Entity<Device>().HasData(
                new Device
                {
                    MarkCode = Convert.ToChar(i).ToString(),
                    SerialNumber = 123457,
                    Port = "8091",
                    IpAddress = "127.0.0.1",
                    DateCreated = now,
                    DateModified = now,
                    UserModified = AdminId,
                    UserCreated = AdminId,
                    IsDeleted = false
                });
            }
            #endregion
        }
    }

}





