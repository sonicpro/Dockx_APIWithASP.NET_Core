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

		public ICollection<PointOfInterestDto> PointsOfInterest { get; set; } =
			new List<PointOfInterestDto>();
	}
}
