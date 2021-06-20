﻿using CityInfo.API.Entities;
using System.Collections.Generic;

namespace CityInfo.API.Services
{
	public interface ICityInfoRepository
	{
		IEnumerable<City> GetCities();

		City GetCity(int cityId, bool includePointsOfInterest);

		IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId);

		PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId);

		bool DoesCityExist(int cityId);
	}
}
