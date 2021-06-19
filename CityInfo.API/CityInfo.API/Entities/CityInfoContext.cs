using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CityInfo.API.Entities
{
	public class CityInfoContext : DbContext
	{
		public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
		{
			Database.Migrate();
		}

		public DbSet<City> Cities { get; set; }

		public DbSet<PointOfInterest> PointsOfInterest { get; set; }

		/// <summary>
		/// OnConfiguring() override is not for injecting DbContext through DI, but for explicit new() DbContext initialization.
		/// </summary>
		/// <param name="optionsBuilder"></param>
		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	optionsBuilder.UseSqlServer("connectionstring");
		//	base.OnConfiguring(optionsBuilder);
		//}
	}
}
