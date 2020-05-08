using BLL.Interfaces;
using BLL.Models;
using BLL.Operations;
using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Infrastracture
{
    public static class BllServiceCollectionExtensions
    {
        public static IServiceCollection AddBll(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<BookingContext>();
            services.AddDbContext<BookingContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IdbRepos, DBReposSQL>();
            services.AddTransient<IBookCrud, BookCrud>();
            return services;
        }
    }
}
