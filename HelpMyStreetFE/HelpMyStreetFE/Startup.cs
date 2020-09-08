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
using HelpMyStreet.Utils.PollyPolicies;
using HelpMyStreet.Cache.Extensions;
using HelpMyStreet.Cache;
using System.Collections.Generic;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account;

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
            services.Configure<EmailConfig>(Configuration.GetSection("EmailConfig"));
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

            services.AddHttpClient<IGroupRepository, GroupRepository>(client =>
            {
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });


            services.AddHttpClient<ICommunicationService, CommunicationService>(client =>
            {
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

            services.AddSingleton<ICommunityRepository, CommunityRepository>();
            services.AddSingleton<IFeedbackRepository, FeedbackRepository>();
            services.AddSingleton<IUserService, Services.UserService>();
            services.AddSingleton<IAuthService, AuthService>();
            //services.AddSingleton<ICommunicationService, CommunicationService>();
            services.AddSingleton<IRequestHelpBuilder, RequestHelpBuilder>();
            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession();

            services.AddSingleton<IRequestService, RequestService>();
            services.AddSingleton<IGroupService, GroupService>();
            services.AddSingleton<IFilterService, FilterService>();

            // cache
            services.AddSingleton<IPollyMemoryCacheProvider, PollyMemoryCacheProvider>();
            services.AddTransient<ISystemClock, MockableDateTime>();
            services.AddSingleton<ICoordinatedResetCache, CoordinatedResetCache>();
            services.AddMemCache();
            services.AddSingleton(x => x.GetService<IMemDistCacheFactory<IEnumerable<JobSummary>>>().GetCache(new TimeSpan(1, 0, 0), ResetTimeFactory.OnMinute));
            services.AddSingleton(x => x.GetService<IMemDistCacheFactory<List<UserGroup>>>().GetCache(new TimeSpan(1, 0, 0), ResetTimeFactory.OnMinute));
            services.AddSingleton(x => x.GetService<IMemDistCacheFactory<int>>().GetCache(new TimeSpan(30, 0, 0, 0), ResetTimeFactory.OnMidday));
            services.AddSingleton(x => x.GetService<IMemDistCacheFactory<User>>().GetCache(new TimeSpan(2, 0, 0), ResetTimeFactory.OnHour));
            services.AddSingleton(x => x.GetService<IMemDistCacheFactory<Group>>().GetCache(new TimeSpan(2, 0, 0), ResetTimeFactory.OnHour));

            services.AddControllers();
            services.AddRazorPages()
            .AddRazorRuntimeCompilation()
            .AddRazorOptions(opt =>
            {
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
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider,
                OnPrepareResponse = ctx =>
                {
                    int durationInHours = Configuration.GetValue<int?>("StaticFileCacheInHours") ?? 24;
                    int durationInSeconds = 60 * 60 * durationInHours;
                    ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] =
                        "public,max-age=" + durationInSeconds;
                }
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
                    name: "request-help",
                    pattern: "request-help",
                    defaults: new { controller = "RequestHelp", action = "RequestHelp", referringGroup = "", source = "" });
                endpoints.MapControllerRoute(
                    name: "request-help/group",
                    pattern: "request-help/{referringGroup}",
                    defaults: new { controller = "RequestHelp", action = "RequestHelp", source = "" });
                endpoints.MapControllerRoute(
                    name: "request-help/group/source",
                    pattern: "request-help/{referringGroup}/{source}",
                    defaults: new { controller = "RequestHelp", action = "RequestHelp" });
                endpoints.MapControllerRoute(
                    name: "request-help/success",
                    pattern: "request-help/success",
                    defaults: new { controller = "RequestHelp", action = "Success" });
                endpoints.MapControllerRoute(
                    name: "login",
                    pattern: "login",
                    defaults: new { controller = "Account", action = "Login" });
                endpoints.MapControllerRoute(
                    name: "ForgottenPassword",
                    pattern: "forgotten-password",
                    defaults: new { controller = "Home", action = "ForgottenPassword" });

                endpoints.MapControllerRoute(
                    name: "account/g/group",
                    pattern: "account/g/{groupKey}",
                    defaults: new { controller = "Account", action = "Group" });
                endpoints.MapControllerRoute(
                    name: "account/g/group/requests",
                    pattern: "account/g/{groupKey}/requests",
                    defaults: new { controller = "Account", action = "GroupRequests" });
                endpoints.MapControllerRoute(
                    name: "account/g/group/volunteers",
                    pattern: "account/g/{groupKey}/volunteers",
                    defaults: new { controller = "Account", action = "GroupVolunteers" });

                // Community placeholders
                endpoints.MapControllerRoute(
                    name: "Kimberley",
                    pattern: "kimberley",
                    defaults: new { controller = "Home", action = "Index", });

                endpoints.MapControllerRoute(
                    name: "Tankersley",
                    pattern: "tankersley",
                    defaults: new { controller = "Community", action = "Index", communityName = "tankersley" });


                endpoints.MapControllerRoute(
                    name: "healthylondonpartnership",
                    pattern: "healthylondonpartnership",
                    defaults: new { controller = "Community", action = "Index", communityName = "hlp" });

                endpoints.MapControllerRoute(
                    name: "ageuklsl",
                    pattern: "ageuklsl",
                    defaults: new { controller = "Community", action = "Index", communityName = "ageuklsl" });

                endpoints.MapControllerRoute(
                   name: "face-coverings",
                   pattern: "face-coverings",
                   defaults: new { controller = "Community", action = "FaceCoverings" });

                endpoints.MapControllerRoute(
                   name: "face-masks",
                   pattern: "face-masks",
                   defaults: new { controller = "Community", action = "FaceMasks" });

                endpoints.MapControllerRoute(
                    name: "fortheloveofscrubs",
                    pattern: "for-the-love-of-scrubs",
                    defaults: new { controller = "Community", action = "Index", communityName = "ftlos" });

                endpoints.MapControllerRoute(
                    name: "OpenRequests",
                    pattern: "account/open-requests",
                    defaults: new { controller = "Account", action = "OpenRequests" });
                endpoints.MapControllerRoute(
                   name: "AcceptedRequests",
                   pattern: "account/accepted-requests",
                   defaults: new { controller = "Account", action = "AcceptedRequests" });
                endpoints.MapControllerRoute(
                   name: "CompletedRequests",
                   pattern: "account/completed-requests",
                   defaults: new { controller = "Account", action = "CompletedRequests" });

                endpoints.MapControllerRoute(
                   name: "registration/step-one",
                   pattern: "registration/step-one",
                   defaults: new { controller = "Registration", action = "StepOne" });

                endpoints.MapControllerRoute(
                   name: "registration/step-one/group",
                   pattern: "registration/step-one/{referringGroup}",
                   defaults: new { controller = "Registration", action = "StepOne" });

                endpoints.MapControllerRoute(
                   name: "registration/step-one/group/source",
                   pattern: "registration/step-one/{referringGroup}/{source}",
                   defaults: new { controller = "Registration", action = "StepOne" });

                endpoints.MapControllerRoute(
                   name: "registration/step-two",
                   pattern: "registration/step-two",
                   defaults: new { controller = "Registration", action = "StepTwo" });

                endpoints.MapControllerRoute(
                   name: "registration/step-three",
                   pattern: "registration/step-three",
                   defaults: new { controller = "Registration", action = "StepThree" });

                endpoints.MapControllerRoute(
                   name: "registration/step-four",
                   pattern: "registration/step-four",
                   defaults: new { controller = "Registration", action = "StepFour" });
            });
        }
    }
}
