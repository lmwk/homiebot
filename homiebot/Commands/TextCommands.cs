using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using Core.DAL.Models.Quotes;
using System.Collections.Generic;
using System.Globalization;
using Core.Services.Quotes;

namespace Core.Commands
{
    class TextCommands : BaseCommandModule
    {
        readonly IQuoteService _quoteService;

        public TextCommands(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        [Command("tldr")]
        public async Task tldr(CommandContext ctx, string messagelink, [RemainingText] string tldr)
        {
            try
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"tldr by {ctx.Member.DisplayName}",
                    Description = $"\n" + $"{tldr}",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = ctx.Member.AvatarUrl, Height = 400, Width = 400
                    },
                    Url = messagelink,
                    Color = DiscordColor.DarkRed,
                };

                await ctx.Channel.SendMessageAsync(embed: embed);
                await ctx.Message.DeleteAsync();
            }
            catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync($"`{e}`");
            }
        }

        [Command("quote")]
        public async Task Quote(CommandContext ctx, [RemainingText] string quotetext)
        {
            try
            {
                Console.WriteLine(ctx.Message.Attachments.Count);

                if (ctx.Message.Attachments.Count > 0)
                {
                    foreach (var attach in ctx.Message.Attachments)
                    {
                        quotetext = quotetext.Insert(quotetext.Length, $" {attach.Url}");
                    }

                    await _quoteService.AddQuoteAsync(quotetext, ctx.Member.Id);
                    var quote1 = await _quoteService.GetQuoteByTextAndOwnerAsync(quotetext, ctx.Member.Id);
                    var msg1 = await new DiscordMessageBuilder()
                        .WithContent($"Quote was created and saved to the database, id for the quote is {quote1.id}")
                        .WithReply(ctx.Message.Id)
                        .SendAsync(ctx.Channel);
                    Console.WriteLine($"Quote was created and saved to the database, id for the quote is {quote1.id}");

                    return;
                }
                else
                {
                    await _quoteService.AddQuoteAsync(quotetext, ctx.Member.Id);
                    var quote = await _quoteService.GetQuoteByTextAndOwnerAsync(quotetext, ctx.Member.Id);
                    var msg = await new DiscordMessageBuilder()
                        .WithContent($"Quote was created and saved to the database, id for the quote is {quote.id}")
                        .WithReply(ctx.Message.Id)
                        .SendAsync(ctx.Channel);
                    Console.WriteLine($"Quote was created and saved to the database, id for the quote is {quote.id}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        [Command("quotes")]
        public async Task Quotes(CommandContext ctx, string text)
        {
            try
            {
                if (int.TryParse(text, out int result))
                {
                    var quote = await _quoteService.GetQuoteByIdAsync(result);
                    var msg = await new DiscordMessageBuilder().WithContent(quote.Text)
                        .WithReply(ctx.Message.Id)
                        .SendAsync(ctx.Channel);
                    Console.WriteLine($"Quote {quote.id} was requested in channel: {ctx.Channel.Id}");
                }
                else
                {
                    var quote = await _quoteService.GetQuoteByContentAsync(text);
                    var msg = await new DiscordMessageBuilder().WithContent(quote.Text)
                        .WithReply(ctx.Message.Id)
                        .SendAsync(ctx.Channel);
                    Console.WriteLine($"Quote {quote.id} was requested in channel: {ctx.Channel.Id}");
                }
            }
            catch (NullReferenceException)
            {
                var msg = await new DiscordMessageBuilder().WithContent("Error: Tag doesn't exist")
                    .WithReply(ctx.Message.Id, true)
                    .SendAsync(ctx.Channel);
            }
        }

        [Command("qlist")]
        public async Task Qlist(CommandContext ctx, ulong id = ulong.MinValue)
        {
            try
            {
                if (id == ulong.MinValue)
                {
                    var count = await _quoteService.GetUserQuoteCount(ctx.Member.Id);
                    var description = await _quoteService.GetUserQuotesStringAsync(ctx.Member.Id);
                    var embed = new DiscordEmbedBuilder
                    {
                        Author = new DiscordEmbedBuilder.EmbedAuthor {IconUrl = ctx.Client.CurrentUser.AvatarUrl},
                        Title = $"**Quotes found: {count}**",
                        Description = description.ToString(),
                        Color = ctx.Member.Color,
                    };

                    if (ctx.Message.ReferencedMessage != null)
                    {
                        var origmessage = ctx.Message.ReferencedMessage;
                        var message = await new DiscordMessageBuilder().WithEmbed(embed)
                            .WithReply(origmessage.Id, mention: true)
                            .SendAsync(ctx.Channel);
                        return;
                    }

                    var msg = await new DiscordMessageBuilder().WithEmbed(embed)
                        .WithReply(ctx.Message.Id, mention: true)
                        .SendAsync(ctx.Channel);
                    return;
                }
                else
                {
                    var count = _quoteService.GetUserQuoteCount(id);
                    var description = _quoteService.GetUserQuotesStringAsync(id);
                    var member = await ctx.Guild.GetMemberAsync(id);

                    var embed = new DiscordEmbedBuilder
                    {
                        Author = new DiscordEmbedBuilder.EmbedAuthor {IconUrl = ctx.Client.CurrentUser.AvatarUrl},
                        Title = $"**Quotes found: {count}**",
                        Description = description.ToString(),
                        Color = member.Color
                    };

                    if (ctx.Message.ReferencedMessage != null)
                    {
                        var origmessage = ctx.Message.ReferencedMessage;
                        var message = await new DiscordMessageBuilder().WithEmbed(embed)
                            .WithReply(origmessage.Id, mention: true)
                            .SendAsync(ctx.Channel);
                        return;
                    }

                    var msg = await new DiscordMessageBuilder().WithEmbed(embed)
                        .WithReply(ctx.Message.Id, mention: true)
                        .SendAsync(ctx.Channel);
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        [Command("qinfo")]
        public async Task Qinfo(CommandContext ctx, string text)
        {
            try
            {
                if (int.TryParse(text, out int result))
                {
                    var quote = await _quoteService.GetQuoteByIdAsync(result);
                    var user = await ctx.Client.GetUserAsync(quote.owner);

                    var embed = new DiscordEmbedBuilder
                    {
                        Author = new DiscordEmbedBuilder.EmbedAuthor {IconUrl = user.AvatarUrl},
                        Title = $"Quote info from quote {quote.id}",
                        Description = $"text: {quote.Text}\n\nowner's id: {quote.owner}",
                        Color = DiscordColor.Red,
                    };
                    var msg = await new DiscordMessageBuilder()
                        .WithEmbed(embed)
                        .WithReply(ctx.Message.Id)
                        .SendAsync(ctx.Channel);
                    Console.WriteLine($"Quote info from quote {quote.id} was requested in channel: {ctx.Channel.Id}");
                }
                else
                {
                    var quote = await _quoteService.GetQuoteByIdAsync(result);
                    var user = await ctx.Client.GetUserAsync(quote.owner);

                    var embed = new DiscordEmbedBuilder
                    {
                        Author = new DiscordEmbedBuilder.EmbedAuthor {IconUrl = user.AvatarUrl},
                        Title = $"Quote info from quote {quote.id}",
                        Description = $"text: {quote.Text}\n\nowner's id: {quote.owner}",
                        Color = DiscordColor.Red,
                    };
                    var msg = await new DiscordMessageBuilder()
                        .WithEmbed(embed)
                        .WithReply(ctx.Message.Id)
                        .SendAsync(ctx.Channel);
                    Console.WriteLine($"Quote info from quote {quote.id} was requested in channel: {ctx.Channel.Id}");
                }
            }
            catch (NullReferenceException)
            {
                var msg = await new DiscordMessageBuilder()
                    .WithContent("Error: Tag doesn't exist")
                    .WithReply(ctx.Message.Id, true)
                    .SendAsync(ctx.Channel);
            }
        }
    }
}