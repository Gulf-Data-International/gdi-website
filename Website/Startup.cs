using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Canvas
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            // services.Configure<RouteOptions>(opts => opts.ConstraintMap.Add("culturecode", typeof(CultureRouteConstraint)));
            // services.AddMvc();
            services.AddLocalization(options => options.ResourcesPath = "Resources");


            services.AddMvc(
            //opts => {
            //    opts.Conventions.Insert(0, new LocalizationConvention());
            //    opts.Filters.Add(new MiddlewareFilterAttribute(typeof(LocalizationPipeline)));
            //}
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();
            var supportedCultures = new[]
               {
                    new CultureInfo("en-US"),
                    new CultureInfo("ar")
                };

            //RequestLocalizationOptions locOptions = new RequestLocalizationOptions()
            //{
            //    DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US"),
            //    SupportedCultures = supportedCultures,
            //    SupportedUICultures = supportedCultures

            //};
            ////locOptions.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider(){ Options = locOptions });
            //locOptions.RequestCultureProviders = new[]{ new RouteDataRequestCultureProvider{
            //    IndexOfCulture=1,
            //    IndexofUICulture=1
            //}};
            // services.AddSingleton(locOptions);
            // Configure supported cultures and localization options
            services.Configure<RequestLocalizationOptions>(options =>
            {
                //options.RequestCultureProviders = new[]
                //{
                //    new RouteDataRequestCultureProvider
                //    {
                //        RouteDataStringKey = "lang",
                //        Options = options
                //    }
                //};

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                var requestProvider = new RouteDataRequestCultureProvider() {
                    IndexOfCulture = 1,
                    IndexofUICulture = 1
                };
                options.RequestCultureProviders.Insert(0, requestProvider);

            });
            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("culture", typeof(LanguageRouteConstraint));
                options.AppendTrailingSlash = true;
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            //app.UseRequestLocalization(locOptions.Value);
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();


            app.UseRouter(routes =>
            {
                routes.MapMiddlewareRoute("{culture=culture}/{*mvcRoute}", subApp =>
                {
                    subApp.UseRequestLocalization(locOptions.Value);

                    subApp.UseMvc(mvcRoutes =>
                    {
                        mvcRoutes.MapRoute(
                            name: "default",
                            template: "{culture=culture}/{controller=Home}/{action=Index}/{id?}");
                    });
                });
            });


            //app.UseMvc(routes =>
            //{
            //    //routes.MapRoute(
            //    //    name: "default",
            //    //    template: "{controller=Home}/{action=Index}/{id?}");
            //    ////////////////////////////////////////////////////////////////////////

            //    //routes.MapRoute(
            //    //    name: "default",
            //    //    template: "{culture:culture}/{controller=Home}/{action=Index}/{id?}");
            //    ////routes.MapGet("{culture:culture}/{*path}", appBuilder => { return Task.CompletedTask; });
            //    //routes.MapRoute("AppBulider","{culture:culture}/{*path}");
            //    //routes.MapGet("{*path}", (RequestDelegate)(ctx =>
            //    //{
            //    //    var defaultCulture = new RequestCulture("en-US").Culture.TwoLetterISOLanguageName;
            //    //    var path = ctx.GetRouteValue("path") ?? string.Empty;
            //    //    var culturedPath = $"/{defaultCulture}/{path}";
            //    //    ctx.Response.Redirect(culturedPath);
            //    //    return Task.CompletedTask;
            //    //}));
            //    ////////////////////////////////////////////////////////////////////////
            //    routes.MapRoute(
            //            name: "LocalizedDefault",
            //            template: "{culture:culture}/{controller=Home}/{action=Index}/{id?}"
            //    );
            //    routes.MapRoute(
            //          name: "default",
            //          template: "{*catchall}",
            //          defaults: new { controller = "Home", action = "RedirectToDefaultLanguage", culture = "en" });


            //});



        }
    }
}
