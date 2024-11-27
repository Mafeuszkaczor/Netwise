using System;
using Microsoft.EntityFrameworkCore;
using Netwise.Models;
namespace Netwise.Database
{
	public class NetwiseDbContext : DbContext
    {
		public DbSet<CatFactsResponseModel> facts { get; set; }

        public NetwiseDbContext(DbContextOptions<NetwiseDbContext> options) : base (options)
        {
        }
    }
}

