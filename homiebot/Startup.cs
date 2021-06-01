using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Core.DAL;
using System.IO;
using System.Text;
using Core.Services;
using Core.Services.Quotes;
using Newtonsoft.Json;

namespace Core
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            {
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                {
                    json = sr.ReadToEnd();
                }
            }

            var ConfigJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            
            services.AddDbContext<CoreDBContext>(options =>
            {

                options.UseSqlServer(ConfigJson.connectionstring,
                    x => x.MigrationsAssembly("Core.DAL.Migrations"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                options.EnableSensitiveDataLogging();
            });

            services.AddScoped<IQuoteService, QuoteService>();
            services.AddTransient<IGuildService, GuildService>();
            services.AddTransient<IInfractionService, InfractionService>();
            
#pragma warning disable ASP0000
            var serviceprovider = services.BuildServiceProvider();
#pragma warning restore ASP0000

            var bot = new bot(serviceprovider, ConfigJson);
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton(bot);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
    
}
