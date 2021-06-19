using CityInfo.API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	public class DummyController : Controller
	{
		private CityInfoContext ctx;

		public DummyController(CityInfoContext ctx)
		{
			this.ctx = ctx;
		}

		[HttpGet]
		[Route("api/testdatabase")]
		public IActionResult TestDatabase()
		{
			return Ok();
		}
	}
}
