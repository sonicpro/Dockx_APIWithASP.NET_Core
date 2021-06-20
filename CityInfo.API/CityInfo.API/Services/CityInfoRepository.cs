using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CityInfo.API.Services
{
	public class CityInfoRepository : ICityInfoRepository
	{
		private readonly CityInfoContext context;

		public CityInfoRepository(CityInfoContext context)
		{
			this.context = context;
		}

		public IEnumerable<City> GetCities()
		{
			return context.Cities.OrderBy(c => c.Name).ToList();
		}

		public City GetCity(int cityId, bool includePointsOfInterest)
		{
			IQueryable<City> query = context.Cities;
			if (includePointsOfInterest)
				query = query.Include(c => c.PointsOfInterest);

			return query.Where(c => c.Id == cityId).FirstOrDefault();
		}

		public PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId)
		{
			return context.PointsOfInterest.Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
				.FirstOrDefault();
		}

		public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
		{
			return context.PointsOfInterest.Where(p => p.CityId == cityId).ToList();
		}

		public bool DoesCityExist(int cityId)
		{
			return context.Cities.Any(c => c.Id == cityId);
		}
	}
}
