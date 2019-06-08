using System;

namespace CityInfo.API.Models
{
	public class FileInfoModel
	{
		public long Length { get; set; }

		public DateTime LastWriteTimeUtc { get; set; }
	}
}
