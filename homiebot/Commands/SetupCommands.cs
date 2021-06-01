using System;
using System.Threading.Tasks;
using Core.Services;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Core.Commands
{
    public class SetupCommands : BaseCommandModule
    {
        private readonly IGuildService _guildService;
        public SetupCommands(IGuildService guildService)
        {
            _guildService = guildService;
        }

        [Command("setup")]
        [RequirePermissions(Permissions.Administrator)]

        public async Task SetupGuild(CommandContext ctx, ulong muteroleid, ulong novcroleid, ulong botbanid)
        {
            await _guildService.SetupGuild(ctx.Guild.Id, ctx.Guild.Name, muteroleid, novcroleid,botbanid, ctx.Guild.MemberCount);
        }

        [Command("updategi")]
        [RequirePermissions(Permissions.ManageChannels)]

        public async Task Updategi(CommandContext ctx, ulong muteroleid, ulong novcroleid, ulong botbanid)
        {
            try
            {
                await _guildService.Updategi(ctx.Guild.Id, ctx.Guild.Name, muteroleid, novcroleid, botbanid,
                    ctx.Guild.MemberCount);
            }catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}