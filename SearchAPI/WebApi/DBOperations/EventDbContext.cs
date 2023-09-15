using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.DBOperations
{
    public class EventDbContext : DbContext
    {
        public EventDbContext(DbContextOptions<EventDbContext>options):base(options)
        {

        }
        public DbSet<Event> Events { get; set; }
    }
}