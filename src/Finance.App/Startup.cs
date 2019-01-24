using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.App
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSystem();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Use(async (context, func) =>
            {
                try
                {
                    await func();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
            app.UseMvc();
        }
    }
}