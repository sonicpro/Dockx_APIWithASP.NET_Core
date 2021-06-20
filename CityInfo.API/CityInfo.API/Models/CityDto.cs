using System.Collections.Generic;

namespace CityInfo.API.Models
{
	public class CityDto : CityWithoutPointsOfInterestDto
	{
		public int NumberOfPointsOfInterest { get
			{
				return PointsOfInterest.Count;
			}
		}

		public ICollection<PointsOfInterestDto> PointsOfInterest { get; set; } =
			new List<PointsOfInterestDto>();
	}
}
