using Core.DAL.Models.Guild;
using Core.DAL.Models.Infractions;
using Core.DAL.Models.Quotes;
using Microsoft.EntityFrameworkCore;

namespace Core.DAL
{
    public class CoreDBContext : DbContext
    {
        public CoreDBContext(DbContextOptions<CoreDBContext> options) : base(options) { }
        public DbSet<Quote> Quotes { get; set; }
        
        public DbSet<Infraction> Infractions { get; set; }
        
        public DbSet<Guild> Guilds { get; set; }
    }
}

