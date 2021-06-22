using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CityInfo.API.Controllers
{
	[ApiController]
	[Route("api/cities")]
	public class CitiesController : ControllerBase
	{
		private ICityInfoRepository repository;
		private IMapper mapper;

		public CitiesController(ICityInfoRepository repository, IMapper mapper)
		{
			this.repository = repository;
			this.mapper = mapper;
		}

		[HttpGet()]
		public IActionResult GetCities()
		{
			var cityEntities = repository.GetCities();
			return Ok(mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
		}

		[HttpGet("{id}")]
		public IActionResult GetCity(int id, bool includePoitsOfInterest = false)
		{
			var cityToReturn = repository.GetCity(id, includePoitsOfInterest);
			if (cityToReturn == null)
			{
				return NotFound();
			}
			if (includePoitsOfInterest)
			{
				var cityInfoResult = mapper.Map<CityDto>(cityToReturn);
				return Ok(cityInfoResult);
			}
			var cityWithoutPointsOfInterestResult = mapper.Map<CityWithoutPointsOfInterestDto>(cityToReturn);
			return Ok(cityWithoutPointsOfInterestResult);
		}
	}
}
