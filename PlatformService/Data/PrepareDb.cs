using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepareDb
    {
        public static void PreparePopulation(IApplicationBuilder app, bool isProd)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
        }

        private static void SeedData(AppDbContext context, bool isProd)
        {
            if (isProd)
            {
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception e)
                {
                    Console.WriteLine("--> could not run migrations");
                    Console.WriteLine(e);
                    throw;
                }
            }
            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding data...");
                
                context.Platforms.AddRange(
                    new Platform() {Name = "Dot Net", Publisher = "Microsoft", Cost = "Free"},
                    new Platform() {Name = "Linux", Publisher = "Someone", Cost = "Free as fuck"},
                    new Platform() {Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Semi-Free"},
                    new Platform() {Name = "Kubernetes", Publisher = "CNCF", Cost = "Free"}
                );

                context.SaveChanges();
                return;
            }

            Console.WriteLine("--> We already have data");
        }
    }
}