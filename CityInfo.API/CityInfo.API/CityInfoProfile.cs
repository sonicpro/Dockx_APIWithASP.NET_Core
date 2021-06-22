using AutoMapper;
using CityInfo.API.Entities;

namespace CityInfo.API.Models
{
	public class CityInfoProfile : Profile
	{
		public CityInfoProfile()
		{
			CreateMap<City, CityWithoutPointsOfInterestDto>();
			CreateMap<City, CityDto>();
			CreateMap<PointOfInterest, PointOfInterestDto>();
			CreateMap<PointOfInterestForCreationDto, PointOfInterest>();
			CreateMap<PointOfInterestForUpdateDto, PointOfInterest>();
			CreateMap<PointOfInterest, PointOfInterestForUpdateDto>();
		}
	}
}
