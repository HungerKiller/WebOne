using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OnePiece.Data;
using System.Globalization;

namespace OnePiece
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            // 在这里设置supportedCultures，是把supportedCultures当做service给注册了。
            // 所以之后LocOptions.Value.SupportedUICultures可以获取到这里设置好的supportedCultures。
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]{
                    new CultureInfo("zh-CN"),
                    new CultureInfo("en-US")
                };
                options.DefaultRequestCulture = new RequestCulture(culture: "zh-CN", uiCulture: "zh-CN");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddDbContext<OnePieceContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("OnePieceContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // 如果在这里设置supportedCultures，那么是通过中间件直接设置的。
            // 所以LocOptions.Value.SupportedUICultures并不能获取到各种supportedCultures。所以_SelectLanguagePartial.cshtml里边就只有一个supportedCultures，不能切换语言。
            //var supportedCultures = new[]{
            //    new CultureInfo("zh-CN"),
            //    new CultureInfo("en-US"),
            //    new CultureInfo("fr-FR")
            //};
            //app.UseRequestLocalization(new RequestLocalizationOptions
            //{
            //    DefaultRequestCulture = new RequestCulture(supportedCultures[0]),
            //    // Formatting numbers, dates, etc.
            //    SupportedCultures = supportedCultures,
            //    // UI strings that we have localized.
            //    SupportedUICultures = supportedCultures
            //});

            // 此处指明UseRequestLocalization即可，因为supportedCultures已经被当做service注册了。
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
