﻿using JobIntechCHAT.API.Models;
using Microsoft.EntityFrameworkCore;


namespace JobIntechCHAT.API.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }
        public DbSet<AppUser> Users { get; set; }
    }
}
