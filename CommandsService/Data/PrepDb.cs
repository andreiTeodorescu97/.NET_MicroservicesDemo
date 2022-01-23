using System;
using System.Collections.Generic;
using CommandsService.Models;
using CommandsService.SyncDataServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.Data
{
    public static class PrepDb
    {
        public static void PrePopulationOfDatabase(IApplicationBuilder applicationBuilder)
        {
            using(var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

                var platforms = grpClient.GetAllExistingPlatformsFromPlatformService();

                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
            }
        }

        private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("--> Seeding platforms from Platform Service!");

            foreach (var plat in platforms)
            {
                if(!repo.ExternalPlatformExists(plat.ExternalId))
                {
                    repo.CreatePlatform(plat);
                }

                repo.SaveChanges();                
            }
        }
    }
}