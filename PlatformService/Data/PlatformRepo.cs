using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext _context;

        public PlatformRepo(AppDbContext appDbContext)
        {
            this._context = appDbContext;

        }
        public void CreatePlatform(Platform plat)
        {
            if (plat == null)
            {
                throw new ArgumentNullException(nameof(plat));
            }
            var lastId = GetAllPlatforms().OrderByDescending(c => c.Id).FirstOrDefault().Id;
            Console.WriteLine($"--> Last registered id is {lastId}");
            _context.Platforms.Add(plat);
            Console.WriteLine($"--> Platform added to DB: {JsonSerializer.Serialize(plat)}, Number of platforms: {GetNumberOfPlatforms()}");
        }

        public int GetNumberOfPlatforms()
        {
            return _context.Platforms.Count();
        }
        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Platform GetPlatfromById(int id)
        {
            return _context.Platforms.FirstOrDefault(c => c.Id == id);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}