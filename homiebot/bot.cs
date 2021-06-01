using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Core.Commands;

namespace Core
{
    class bot
    {
        public static DiscordClient client { get; private set; }
        private CommandsNextExtension commands { get; set; }
        public InteractivityExtension Interactivity { get; private set; }

        public static ConfigJson configJson { get; private set; }

        public bot(IServiceProvider services, ConfigJson ConfigJson)
        {
            configJson = ConfigJson;

            var config = new DiscordConfiguration
            {
                Token = ConfigJson.Token,
                AutoReconnect = true,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug,
                Intents = DiscordIntents.Guilds | DiscordIntents.GuildMessages | DiscordIntents.GuildMembers | DiscordIntents.GuildEmojis | DiscordIntents.GuildMessageReactions,
                UseRelativeRatelimit = true,
                LargeThreshold = 300,
            };

            client = new DiscordClient(config);
            client.Ready += OnclientReady;
            client.MessageCreated += MessageCreated;
            client.MessageUpdated += MessageUpdated;
            client.GuildMemberAdded += PersonJoined;
            client.GuildMemberRemoved += PersonRemoved;

            client.UseInteractivity(new InteractivityConfiguration
            {
                PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromMinutes(5),
            });

            var commandsconfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { ConfigJson.Prefix },
                EnableDms = true,
                EnableMentionPrefix = true,
                Services = services
            };

            commands = client.UseCommandsNext(commandsconfig);

            commands.RegisterCommands<Rolecommands>();
            commands.RegisterCommands<UserCommands>();
            commands.RegisterCommands<PollCommand>();
            commands.RegisterCommands<TextCommands>();
            commands.RegisterCommands<RedditCommands>();
            commands.RegisterCommands<ModCommands>();
            commands.RegisterCommands<TenorCommands>();
            commands.RegisterCommands<SetupCommands>();

            client.ConnectAsync();
        }

        private static Task OnclientReady(DiscordClient d, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        private static async Task MessageCreated(DiscordClient d, MessageCreateEventArgs m)
        {
            try
            {
                var message = m.Message.Content;
                var replacevalues = new char[] {'-', '/', '.', '{', '}', ';', ':', '"', '[', ']', '-', '_', '(', ')', '*', '&', '^', '%', '$', '#', '@', '!', ',', '<', '>', '?', '+', '='};
                foreach (var t in replacevalues)
                {
                    message.Replace(t, ' ');
                }

                var split = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (split.Any(t => configJson.bannedwords.Any(t1 => t == t1)))
                {
                    await m.Message.DeleteAsync("Contains banned word");
                    var person = m.Message.Author;
                    await m.Channel.SendMessageAsync($"Watch what you say {person.Mention}");
                    var messge = await new DiscordMessageBuilder()
                        .WithContent("Watch what you say")
                        .WithReply(m.Message.Id, mention: true)
                        .SendAsync(m.Channel);
                }
            }
            catch (Exception e)
            {
                await m.Channel.SendMessageAsync($"`{e}`");
            }
        }
        
        private static async Task MessageUpdated(DiscordClient d, MessageUpdateEventArgs m)
        {
            try
            {
                var message = m.Message.Content;
                var replacevalues = new char[] { '-', '/', '.', '{', '}', ';', ':', '"', '[', ']', '-', '_', '(', ')', '*', '&', '^', '%', '$', '#', '@', '!', ',', '<', '>', '?', '+', '=' };
                foreach (var t in replacevalues)
                {
                    message.Replace(t, ' ');
                }
                var split = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split.Any(t => configJson.bannedwords.Any(t1 => t == t1)))
                {
                    await m.Message.DeleteAsync("Contains banned word");
                    var person = m.Message.Author;
                    await m.Channel.SendMessageAsync($"Watch your language and dont try and circumvent the filter {person.Mention}");
                }
            }
            catch (Exception e)
            {
                await m.Channel.SendMessageAsync($"`{e}`");
            }
        }

        private static async Task PersonJoined(DiscordClient d, GuildMemberAddEventArgs g)
        {
            try
            {
                var welcome = g.Guild.GetChannel(797666458007502858);
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"{g.Member.Username} is here!",
                    Description = $"{g.Member.Mention} has joined the server, dont be mean to them",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = g.Member.AvatarUrl,
                        Height = 300,
                        Width = 300
                    },
                    ImageUrl = "https://lmwkasked.wheres-my-ta.co/sUfh0a.gif",
                    Color = DiscordColor.DarkRed,
                    Timestamp = g.Member.JoinedAt,
                };
                await welcome.SendMessageAsync(embed: embed);
            }catch (Exception e)
            {
                var lmwk = await g.Guild.GetMemberAsync(501894809485705217);
                var channel = await d.GetChannelAsync(765383439356133426);
                await channel.SendMessageAsync($"`{e}` {lmwk.Mention}");
            }
        }

        private static async Task PersonRemoved(DiscordClient d, GuildMemberRemoveEventArgs g)
        {
            try
            {
                var welcome = g.Guild.GetChannel(808207243450384394);
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"{g.Member.Username} has left!",
                    Description = $"{g.Member.Mention} has left the server, goodbye",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = g.Member.AvatarUrl,
                        Height = 300,
                        Width = 300
                    },
                    ImageUrl = "https://lmwkasked.wheres-my-ta.co/sUfh0a.gif",
                    Color = DiscordColor.DarkRed,
                    Timestamp = DateTimeOffset.UtcNow,
                };
                await welcome.SendMessageAsync(embed: embed);
            }
            catch (Exception e)
            {
                var lmwk = await g.Guild.GetMemberAsync(501894809485705217);
                var channel = await d.GetChannelAsync(765383439356133426);
                await channel.SendMessageAsync($"`{e}` {lmwk.Mention}");
            }
        }

    }
}
