using CityInfo.API.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CityInfo.API.Services
{
	public class FolderListerService
	{
		// Used only for real lising, ignored for mock.
		private readonly string localFolderPath;

		public FolderListerService(IOptions<LocalFolderOptions> options)
		{
			localFolderPath = options.Value.Path;
		}

		public Dictionary<string, FileInfoModel> GetFilesList()
		{
			string json = @"{
				""Bulka"": {
					""LastWriteTimeUtc"": ""2019-06-06T10:31:01.4849966Z"",
					""Length"": ""2147965157""
					}
				}";

			return JsonConvert.DeserializeObject<Dictionary<string, FileInfoModel>>(json);
		}
	}
}
