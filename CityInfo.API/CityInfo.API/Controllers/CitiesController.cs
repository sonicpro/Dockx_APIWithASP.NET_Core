using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CityInfo.API.Controllers
{
	[ApiController]
	[Route("api/cities")]
	public class CitiesController : ControllerBase
	{
		[HttpGet("api/cities", Name = "CityInfoApi_CityList")]
		public JsonResult GetCities()
		{
			return new JsonResult(CitiesDataStore.Current.Cities);
		}

		[HttpGet("{id}", Name = "CityInfoApi_City")]
		public JsonResult GetCity(int id)
		{
			return new JsonResult(
				CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id));
		}
	}
}
