using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	[ApiController]
	[Route("api/list-folder")]
	public class ListFolderController : ControllerBase
	{
		private readonly FolderListerService folderListerService;

		public ListFolderController(FolderListerService listerService)
		{
			folderListerService = listerService;
		}

		[HttpGet()]
		public IActionResult Get()
		{
			return Ok(folderListerService.GetFilesList());
		}
	}
}
