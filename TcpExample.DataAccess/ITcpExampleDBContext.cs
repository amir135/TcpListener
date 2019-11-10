using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TcpExample.Domain.DBModel;


namespace TcpExample.DataAccessCore
{
    public interface ITcpExampleDBContext
    {

        //DbSet<UserProperty> UserProperties { get; set; }
        DbSet<Mark> Marks { get; set; }
        DbSet<Device> Devices { get; set; }
        DbSet<DataCollection> DataCollections { get; set; }
        DbSet<User> Users { get; set; }
        int SaveChanges();
        EntityEntry Entry(object entity);

    }
}