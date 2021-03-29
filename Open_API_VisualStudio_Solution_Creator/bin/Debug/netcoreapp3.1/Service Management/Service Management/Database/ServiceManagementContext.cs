using Microsoft.EntityFrameworkCore;
using Service_Management.Models;

namespace Service_Management.Database
{
    public class ServiceManagementContext : DbContext
    {
        
        public ServiceManagementContext(DbContextOptions<ServiceManagementContext> options) : base(options)
        {

        }

        public DbSet<Service> Services { get; set; }

    }
}
