using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using diploma.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace diploma
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

            var connection =
               Configuration.GetConnectionString("DefaultConnection");
        services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<BookingContext>();

        services.AddDbContext<BookingContext>(options =>
        options.UseSqlServer(connection));

        services.AddMvc().AddJsonOptions(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        });

        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

        services.ConfigureApplicationCookie(options =>
        {//401 ошибка если недостаточно прав
            options.Cookie.Name = "SimpleWebApp";
            options.LoginPath = "/";
            options.AccessDeniedPath = "/";
            options.LogoutPath = "/";
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider
services)
    {
        CreateUserRoles(services).Wait();

        app.UseAuthentication();
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
        }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.UseMvc();
            app.UseCookiePolicy();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    private async Task CreateUserRoles(IServiceProvider serviceProvider)
    {
        var roleManager =
        serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager =
        serviceProvider.GetRequiredService<UserManager<User>>();
        // Создание ролей администратора и пользователя
        if (await roleManager.FindByNameAsync("admin") == null)
        {
            await roleManager.CreateAsync(new
            IdentityRole("admin"));
        }
        if (await roleManager.FindByNameAsync("user") == null)
        {
            await roleManager.CreateAsync(new IdentityRole("user"));
        }
        // Создание Администратора
        string adminEmail = "admin@mail.com";
        string adminPassword = "Aa123456!";
        if (await userManager.FindByNameAsync(adminEmail) == null)
        {
            User admin = new User
            {
                Email = adminEmail,
                UserName = "admin",
                Fio = "Администратор",
                Address = "Russia",
                PhoneNumber = "9871010101",
                IdCity = 1
            };
            IdentityResult result = await
            userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "admin");
            }
        }
        // Создание Пользователя
        string userEmail = "user@mail.com";
        string userPassword = "Aa123456!";
        if (await userManager.FindByNameAsync(userEmail) == null)
        {
            User user = new User
            {
                Email = userEmail,
                UserName = "user",
                Fio = "Пользователь",
                Address = "Russia",
                PhoneNumber = "9894343434",
                IdCity = 1
            };
            IdentityResult result = await
            userManager.CreateAsync(user, userPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "user");
            }
        }
    }
}
}
