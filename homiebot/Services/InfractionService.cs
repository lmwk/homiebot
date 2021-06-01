using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Core.DAL;
using Core.DAL.Models.Infractions;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public interface IInfractionService
    {
        public Task<Infraction> CreateInfraction(string reason,string type, ulong ownerid, ulong guildid, DateTime expirationdate);
    }

    public class InfractionService : IInfractionService
    {
        private readonly CoreDBContext _context;
        private readonly IGuildService _guildService;
        
        public InfractionService(CoreDBContext context, IGuildService guildService)
        {
            _context = context;
            _guildService = guildService;
            
            Timer timer = new(TimeSpan.FromSeconds(30).TotalMilliseconds);
            timer.Elapsed += async (_, _) => await tick();
            timer.Start();
        }

        private async Task ManageMute(ulong guildid, ulong userid, Infraction infraction)
        {
            try
            {
                var guild = await bot.client.GetGuildAsync(guildid);
                if (!guild.Members.TryGetValue(userid, out DiscordMember member))
                {
                    Console.WriteLine($"Couldn't unmute user with id {userid}, user wasn't found in guild {guild.Name}: {guild.Id}");
                    _context.Infractions.Remove(infraction);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var guilddata = await _guildService.GetGuild(guildid);
                    await member.RevokeRoleAsync(guild.Roles[guilddata.muteroleid], "Mute expired");
                    _context.Infractions.Remove(infraction);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<Infraction> CreateInfraction(string reason,string type, ulong ownerid, ulong guildid, DateTime expirationdate)
        {
            var infraction = new Infraction
                {ExpirationDate = expirationdate, guildid = guildid, ownerid = ownerid, Reason = reason, Type = type};
            await _context.Infractions.AddAsync(infraction);
            await _context.SaveChangesAsync();
            return infraction;
        }

        private async Task tick()
        {
            if (_context.Infractions.Count() is 0) return;
            var expiredinfractions = await _context.Infractions.ToListAsync();
               var expiredinfractions1 = expiredinfractions.Where(x => x.ExpirationDate.Second - DateTime.Now.Second < 0).GroupBy(x => x.guildid); 

            foreach (var expired in expiredinfractions1)
            {
                foreach (Infraction infraction in expired)
                {
                     switch (infraction.Type)
                    {
                        case "TempMute":
                            await ManageMute(infraction.guildid, infraction.ownerid, infraction);
                            break;
                        case "Warn":
                            break;
                        case "BotBan":
                            await ManageBotBan(infraction.guildid, infraction.ownerid, infraction);
                            break;
                        default:
                            Console.WriteLine($"something went wrong and we cant process the expired infraction with the id {infraction.id}");
                            break;
                    }
                }
                
            }
        }

        private async Task ManageBotBan(ulong infractionGuildid, ulong infractionOwnerid, Infraction infraction)
        {
            try
            {
                var guild = await bot.client.GetGuildAsync(infractionGuildid);
                if (!guild.Members.TryGetValue(infractionOwnerid, out DiscordMember member))
                {
                    Console.WriteLine($"Couldn't unmute user with id {infractionOwnerid}, user wasn't found in guild {guild.Name}: {guild.Id}");
                    _context.Infractions.Remove(infraction);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var guilddata = await _guildService.GetGuild(infractionGuildid);
                    await member.RevokeRoleAsync(guild.Roles[guilddata.botbanroleid], "Bot commands ban expired");
                    _context.Infractions.Remove(infraction);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    
}