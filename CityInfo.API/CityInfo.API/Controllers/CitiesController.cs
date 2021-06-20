using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CityInfo.API.Controllers
{
	[ApiController]
	[Route("api/cities")]
	public class CitiesController : ControllerBase
	{
		private ICityInfoRepository repository;

		public CitiesController(ICityInfoRepository repository)
		{
			this.repository = repository;
		}

		[HttpGet()]
		public IActionResult GetCities()
		{
			return Ok(repository.GetCities().Select(c =>
				new CityWithoutPointsOfInterestDto { Id = c.Id, Name = c.Name, Description = c.Description }));
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
				var result = new CityDto { Id = cityToReturn.Id, Name = cityToReturn.Name, Description = cityToReturn.Description };
				foreach (var pi in cityToReturn.PointsOfInterest)
					result.PointsOfInterest.Add(new PointsOfInterestDto
					{
						Id = pi.Id,
						Name = pi.Name,
						Description = pi.Description
					});
				return Ok(result);
			}
			return Ok(new CityWithoutPointsOfInterestDto
			{
				Id = cityToReturn.Id,
				Name = cityToReturn.Name,
				Description = cityToReturn.Description
			});
		}
	}
}
