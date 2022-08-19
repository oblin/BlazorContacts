using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlazorContacts.Web.Data;
using System.Runtime.InteropServices;

namespace BlazorContacts.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();

            // register the HttpClientFactory interface
            services.AddHttpClient<Services.ApiService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5010");
            })
            .AddClientAccessTokenHandler();

            services.AddAccessTokenManagement(options => {
                options.Client.Clients.Add("auth", new IdentityModel.Client.ClientCredentialsTokenRequest{
                    Address = "http://localhost:5020/connect/token",
                    ClientId = "blazorcontacts-web",
                    ClientSecret = "thisismysecrets"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //運行於 Linux 時啟用 Reverse Proxy 模式 
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor 
                    | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
                });
            }

            app.UsePathBase(Configuration["pathBase"] ?? "/web");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
