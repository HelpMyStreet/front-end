using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using HelpMyStreet.Utils.CoordinatedResetCache;
using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HelpMyStreetFE.Models.Yoti;
using HelpMyStreetFE.Models.Email;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Rewrite;
using System;
using Microsoft.Extensions.Internal;
using Polly;
using HelpMyStreet.Utils.PollyPolicies;
using HelpMyStreetFE.Models.RequestHelp.Enum;

namespace HelpMyStreetFE
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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {            
                    options.Cookie.HttpOnly = true;                    
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Strict;                        
                });
            services.AddControllersWithViews();
            services.Configure<YotiOptions>(Configuration.GetSection("Yoti"));
            services.Configure<EmailConfig>(Configuration.GetSection("SendGrid"));
            services.Configure<RequestSettings>(Configuration.GetSection("RequestSettings"));


            PollyHttpPolicies pollyHttpPolicies = new PollyHttpPolicies(new PollyHttpPoliciesConfig());

            services.AddHttpClient<IUserRepository, UserRepository>(client =>
            {
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }).AddPolicyHandler(pollyHttpPolicies.InternalHttpRetryPolicy);

            services.AddHttpClient<IValidationRepository, ValidationRepository>(client =>
            {
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }).AddPolicyHandler(pollyHttpPolicies.InternalHttpRetryPolicy);

            services.AddHttpClient<IAddressRepository, AddressRepository>(client =>
            {
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }).AddPolicyHandler(pollyHttpPolicies.InternalHttpRetryPolicy);

            services.AddHttpClient<IRequestHelpRepository, RequestHelpRepository>(client =>
            {
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }).AddPolicyHandler(pollyHttpPolicies.InternalHttpRetryPolicy);

            services.AddHttpClient<IAddressService, AddressService>(client =>
            {
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }).AddPolicyHandler(pollyHttpPolicies.InternalHttpRetryPolicy);

            services.AddHttpClient<IValidationService, ValidationService>(client =>
            {
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }).AddPolicyHandler(pollyHttpPolicies.InternalHttpRetryPolicy);

            services.AddHttpClient<IGoogleService, GoogleService>(client =>
            {
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

            services.AddSingleton<ICommunityRepository, CommunityRepository>();
            services.AddSingleton<IUserService, Services.UserService>();
            services.AddSingleton<IAuthService, AuthService>();            
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IRequestHelpBuilder, RequestHelpBuilder>();
            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession();

            services.AddSingleton<IRequestService, RequestService>();

            // cache
            services.AddSingleton<IPollyMemoryCacheProvider, PollyMemoryCacheProvider>();
            services.AddTransient<ISystemClock, MockableDateTime>();
            services.AddSingleton<ICoordinatedResetCache, CoordinatedResetCache>();

            services.AddControllers();
            services.AddRazorPages()
            .AddRazorRuntimeCompilation()
            .AddRazorOptions(opt => {
                opt.ViewLocationFormats.Add("/Views/Account/Verification/{0}.cshtml");
                opt.ViewLocationFormats.Add("/Views/RequestHelp/RequestStage/{0}.cshtml");
                opt.ViewLocationFormats.Add("/Views/RequestHelp/DetailStage/{0}.cshtml");
                opt.ViewLocationFormats.Add("/Views/RequestHelp/ReviewStage/{0}.cshtml");
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
                app.UseStatusCodePagesWithReExecute("/Error/{0}");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRewriter(new RewriteOptions().AddRedirectToWwwPermanent("helpmystreet.org"));
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";
            app.UseStaticFiles(new StaticFileOptions{
                ContentTypeProvider = provider
            });
            app.UseSession();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "about",
                    pattern: "about-us",
                    defaults: new { controller = "Pages", action = "AboutUs" });

                endpoints.MapControllerRoute(
                    name: "community-organisers",
                    pattern: "community-organisers",
                    defaults: new { controller = "Pages", action = "CommunityOrganisers" });

                endpoints.MapControllerRoute(
                    name: "community",
                    pattern: "community/{communityName}",
                    defaults: new { controller = "Community", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "privacy",
                    pattern: "privacy-policy",
                    defaults: new { controller = "Pages", action = "PrivacyPolicy" });
                endpoints.MapControllerRoute(
                    name: "terms",
                    pattern: "terms-conditions",
                    defaults: new { controller = "Pages", action = "Terms" });
                endpoints.MapControllerRoute(
                    name: "resources",
                    pattern: "resources",
                    defaults: new { controller = "Pages", action = "Resources" });
                endpoints.MapControllerRoute(
                    name: "questions",
                    pattern: "questions",
                    defaults: new { controller = "Pages", action = "Questions" });
                endpoints.MapControllerRoute(
                    name: "contact",
                    pattern: "contact-us",
                    defaults: new { controller = "Pages", action = "ContactUs" });
                        endpoints.MapControllerRoute(
                    name: "request-help/v4v",
                    pattern: "request-help/v4v",
                    defaults: new { controller = "RequestHelp", action = "RequestHelp", source = RequestHelpSource.VitalsForVeterans  });
                endpoints.MapControllerRoute(
                name: "request-help/diy",
                pattern: "request-help/diy",
                defaults: new { controller = "RequestHelp", action = "RequestHelp", source = RequestHelpSource.DIY });
                endpoints.MapControllerRoute(
                    name: "request-help",
                    pattern: "request-help",
                    defaults: new { controller = "RequestHelp", action = "RequestHelp", source = RequestHelpSource.Default });
                endpoints.MapControllerRoute(
                    name: "login",
                    pattern: "login",
                    defaults: new { controller = "Account", action = "Login" });
                endpoints.MapControllerRoute(
                    name: "ForgottenPassword",
                    pattern: "forgotten-password",
                    defaults: new { controller = "Home", action = "ForgottenPassword" });

                // Community placeholders
                endpoints.MapControllerRoute(
                    name: "Kimberley",
                    pattern: "kimberley",
                    defaults: new { controller = "Home", action = "Index",  });

                endpoints.MapControllerRoute(
                    name: "Tankersley",
                    pattern: "tankersley",
                    defaults: new { controller = "Home", action = "Index" });

                endpoints.MapControllerRoute(
                   name: "face-masks",
                   pattern: "face-masks",
                   defaults: new { controller = "Community", action = "FaceMasks" });

                endpoints.MapControllerRoute(
                 name: "OpenRequests",
                 pattern: "account/open-requests",
                 defaults: new { controller = "Account", action = "OpenRequests" });

                endpoints.MapControllerRoute(
                 name: "AcceptedRequests",
                 pattern: "account/accepted-requests",
                 defaults: new { controller = "Account", action = "AcceptedRequests" });

                endpoints.MapControllerRoute(
                name: "registration/stepone/hlp",
                pattern: "registration/stepone/hlp",
                defaults: new { controller = "Registration", action = "StepOne", source  = RegistrationSource.HLP });

                // Enable attribute routing
                //endpoints.MapControllers();
            });
        }
    }
}
