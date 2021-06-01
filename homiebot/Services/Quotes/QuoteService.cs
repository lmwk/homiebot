using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DAL;
using Core.DAL.Models.Quotes;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Quotes
{
    public interface IQuoteService
    {
        Task<Quote> GetQuoteByIdAsync(int id);

        Task<Quote> GetQuoteByTextAndOwnerAsync(string text, ulong id);

        Task AddQuoteAsync(string text, ulong ownerid);

        Task<List<Quote>> GetQuoteListAsync();

        Task<string> GetUserQuotesStringAsync(ulong ownerid);

        Task<int> GetUserQuoteCount(ulong userid);

        Task<Quote> GetQuoteByContentAsync(string text);

    }

    public class QuoteService : IQuoteService
    {
        private readonly CoreDBContext _context;

        public QuoteService(CoreDBContext context)
        {
            _context = context;
        }
        
        public async Task<Quote> GetQuoteByIdAsync(int id)
        {
            try
            {
                var quote = await _context.Quotes.FirstOrDefaultAsync(x => x.id.Equals(id));

                return quote;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return null;
        }

        public async Task<Quote> GetQuoteByTextAndOwnerAsync(string text, ulong id)
        {
            try
            {
                var quote = await _context.Quotes.FirstOrDefaultAsync(x => x.Text.Equals(text) && x.owner == id);
                return quote;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return null;

        }

        public async Task AddQuoteAsync(string text, ulong ownerid)
        {
            await _context.Quotes.AddAsync(new Quote { Text = text, owner = ownerid });
            await _context.SaveChangesAsync();
        }

        public async Task<List<Quote>> GetQuoteListAsync()
        {
            return await _context.Quotes.ToListAsync();
        }

        public async Task<string> GetUserQuotesStringAsync(ulong ownerid)
        {
            try
            {
                var quotes = await GetQuoteListAsync();
                var userquote = quotes.Where(x => x.owner.Equals(ownerid));
                
                var descriptiontext = $"";
                foreach (var quote in userquote)
                {
                    quote.Text = quote.Text.Replace('>', ' ');
                    quote.Text = quote.Text.Replace('<', ' ');
                    quote.Text = quote.Text.Insert(1, "\n");
                    quote.Text = quote.Text.Insert(quote.Text.Length, "\n");
                    descriptiontext = descriptiontext.Insert(descriptiontext.Length, $" {quote.Text}");
                }

                return descriptiontext;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return null;
        }

        public async Task<int> GetUserQuoteCount(ulong userid)
        {
            try
            {
                var quotes = await GetQuoteListAsync();
                var userquote = quotes.Where(x => x.owner.Equals(userid));
                return userquote.Count();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return 0;
        }

        public async Task<Quote> GetQuoteByContentAsync(string text)
        {
            var quote = await _context.Quotes.FirstOrDefaultAsync(x => x.Text.Contains(text));
            return quote;
        }
    }
    
}