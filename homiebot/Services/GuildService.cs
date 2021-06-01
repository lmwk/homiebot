using System;
using System.Linq;
using System.Threading.Tasks;
using Core.DAL;
using Core.DAL.Models.Guild;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public interface IGuildService
    {
        public Task<Guild> GetGuild(ulong guildid);
        public Task SetupGuild(ulong guildid, string name, ulong muteroleid, ulong novcroleid,ulong botbanid, int usercount);
        public Task Updategi(ulong guildid, string name, ulong muteroleid, ulong novcroleid, ulong botbanid, int usercount);
    }
    
    public class GuildService : IGuildService
    {
        private readonly CoreDBContext _context;
        public GuildService(CoreDBContext context)
        {
            _context = context;
        }

        public async Task<Guild> GetGuild(ulong guildid)
        {
            try
            {
                var guilds = await _context.Guilds.ToListAsync();
                return guilds.Find(x => x.guildid == guildid);
            }
            catch
            {
                Console.WriteLine($"Data for a guild with the id {guildid} was requested but not found");
            }

            return null;
        }

        public async Task SetupGuild(ulong guildid, string name, ulong muteroleid, ulong novcroleid,ulong botbanid, int usercount)
        {
            var guilddata = new Guild
            {
                guildid = guildid, muteroleid = muteroleid, name = name, usercount = usercount, novcroleid = novcroleid, botbanroleid = botbanid
            };
            
            await _context.Guilds.AddAsync(guilddata);
            await _context.SaveChangesAsync();
        }

        public async Task Updategi(ulong guildid, string name, ulong muteroleid, ulong novcroleid, ulong botbanid, int usercount)
        {

            var guilddata = await GetGuild(guildid);

            _context.Guilds.Remove(guilddata);
            
            await _context.Guilds.AddAsync(new Guild
            {
                id = guilddata.id, name = name, guildid = guildid, novcroleid = novcroleid,
                botbanroleid = botbanid, muteroleid = muteroleid, usercount = usercount
            });
            await _context.SaveChangesAsync();
        }
    }
}