using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AqualogicJumper.Services;
using JsonFlatFileDataStore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AqualogicJumper
{
    public class Startup
    {
        public const string FileName = "state.json";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddTransient(s => JsonSerializer.Create(new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            }));
            services.AddSingleton(sp => new DataStore(FileName));
            services.AddSingleton<PoolStatusStore>();
            services.AddSingleton<IAqualogicStream, SerialAqualogicStream>();
            services.AddSingleton<MenuService>();
            services.AddSingleton<SensorService>();
            services.AddSingleton<SettingService>();
            services.AddSingleton<StatusUpdateService>();
            services.AddSingleton<SwitchService>();
            services.AddSingleton<CommandService>();
            services.AddSingleton<AqualogicMessageWriter>();
            services.AddSingleton(s => s.GetService<MenuService>()?.Sensors);
            services.AddSingleton(s => s.GetService<MenuService>()?.Menu);
            services.AddSingleton(s => s.GetService<MenuService>()?.Switches);
            services.AddSingleton<AquaLogicProtocolService>();
            services.AddSingleton<AqualogicHostedService>();
            services.AddHostedService(sp => sp.GetService<AqualogicHostedService>());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
