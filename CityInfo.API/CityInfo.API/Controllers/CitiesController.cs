using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CityInfo.API.Controllers
{
	[ApiController]
	public class CitiesController : ControllerBase
	{
		[HttpGet("api/cities", Name = "CityInfoApi_CityList")]
		public JsonResult GetCities()
		{
			return new JsonResult(new List<object>
			{
				new { id=1, Name="New York City" },
				new { id=2, Name="Antwerp" }
			});
		}
	}
}
