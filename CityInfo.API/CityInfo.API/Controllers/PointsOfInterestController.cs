using System;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using CityInfo.API.Models;
using CityInfo.API.Services;
using System.Collections.Generic;
using AutoMapper;
using CityInfo.API.Entities;

namespace CityInfo.API.Controllers
{
	[Route("api/cities")]
	[ApiController]
	public class PointsOfInterestController : ControllerBase
	{
		// ILogger<T> uses T class Name as a logging message Category.
		private readonly ILogger<PointsOfInterestController> logger;
		private readonly IMailService mailService;
		private readonly ICityInfoRepository repository;
		private IMapper mapper;

		public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository repository, IMapper mapper)
		{
			this.logger = logger;
			this.mailService = mailService;
			this.repository = repository;
			this.mapper = mapper;
		}

		[HttpGet("{cityId}/pointsofinterest")]
		public IActionResult GetPointsOfInterest(int cityId)
		{
			try
			{
				//throw new Exception("Exception sample");
				if (!repository.DoesCityExist(cityId))
				{
					logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interests.");
					return NotFound();
				}

				var pointsOfInterest = repository.GetPointsOfInterestForCity(cityId);
				var pointsOfIntersetForCityResult = mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest);
				return Ok(pointsOfIntersetForCityResult);
			}
			catch(Exception ex)
			{
				logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);
				return StatusCode(500, "A problem happened while handling your request.");
			}
		}

		[HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
		public IActionResult GetPointOfInterest(int cityId, int id)
		{
			if (!repository.DoesCityExist(cityId))
			{
				logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interests.");
				return NotFound();
			}

			var pointOfInterest = repository.GetPointOfInterestForCity(cityId, id);

			if (pointOfInterest == null)
			{
				return NotFound();
			}

			var pointOfInterestResult = mapper.Map<PointOfInterestDto>(pointOfInterest);

			return Ok(pointOfInterestResult);
		}

		[HttpPost("{cityId}/pointsofinterest")]
		public IActionResult CreatePointOfInterest(int cityId,
			[FromBody] PointOfInterestForCreationDto pointOfInterest)
		{
			if (pointOfInterest == null)
			{
				return BadRequest();
			}

			if (!repository.DoesCityExist(cityId))
			{
				return NotFound();
			}

			var finalPointOfInterest = mapper.Map<PointOfInterest>(pointOfInterest);

			repository.CreatePointOfInterest(cityId, finalPointOfInterest);
			if (!repository.Save())
			{
				return StatusCode(500, "A problem happened while handling your request.");
			}

			return CreatedAtRoute("GetPointOfInterest",
				new { cityId, id = finalPointOfInterest.Id },
				finalPointOfInterest);
		}

		[HttpPut("{cityId}/pointsofinterest/{id}")]
		public IActionResult UpdatePointOfInterest(int cityId,
			int id,
			[FromBody] PointOfInterestForUpdateDto pointOfInterest)
		{
			if (pointOfInterest == null)
			{
				return BadRequest();
			}

			if (pointOfInterest.Name == pointOfInterest.Description)
			{
				ModelState.AddModelError("Description", "The provided description cannot be the same as the name.");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (!repository.DoesCityExist(cityId))
			{
				return NotFound();
			}

			var pointOfInterestEntity = repository.GetPointOfInterestForCity(cityId, id);
			if (pointOfInterestEntity == null)
			{
				return NotFound();
			}

			mapper.Map(pointOfInterest, pointOfInterestEntity);
			if (!repository.Save())
			{
				return StatusCode(500, "A problem happened while handling your request.");
			}

			return NoContent();
		}

		[HttpPatch("{cityId}/pointsofinterest/{id}")]
		public IActionResult PartiallyUpdatePointOfInterest(int cityId,
			int id,
			[FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> updateOperations)
		{
			if (updateOperations == null)
			{
				return BadRequest();
			}

			if (!repository.DoesCityExist(cityId))
			{
				return NotFound();
			}

			var pointOfInterestEntity = repository.GetPointOfInterestForCity(cityId, id);
			if (pointOfInterestEntity == null)
			{
				return NotFound();
			}

			var pointOfInterestToPatch = mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

			// The following call only validates JsonPatchDocument validity against the PointOfInterestForUpdateDto model.
			// PointOfInterestForUpdateDto own validation rules do not fire.
			updateOperations.ApplyTo(pointOfInterestToPatch, ModelState);
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Hand-code the validation rules for PointOfInterestForUpdateDto, like mandatory "Name" etc.
			TryValidateModel(pointOfInterestToPatch);

			if (!string.IsNullOrEmpty(pointOfInterestToPatch.Name) && !string.IsNullOrEmpty(pointOfInterestToPatch.Description)
				&& pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
			{
				ModelState.AddModelError("Description", "The provided description cannot be the same as the name.");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Map the DTO object back to entity to cause the entity state change.
			mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
			if (!repository.Save())
			{
				return StatusCode(500, "A problem happened while handling your request.");
			}

			return NoContent();
		}

		[HttpDelete("{cityId}/pointsofinterest/{id}")]
		public IActionResult DeletePointOfInterest(int cityId, int id)
		{
			if (!repository.DoesCityExist(cityId))
			{
				return NotFound();
			}

			var pointOfInterestEntity = repository.GetPointOfInterestForCity(cityId, id);
			if (pointOfInterestEntity == null)
			{
				return NotFound();
			}

			repository.DeletePointOfInterest(pointOfInterestEntity);
			if (!repository.Save())
			{
				return StatusCode(500, "A problem happened while handling your request.");
			}
			else
			{
				mailService.Send("Point of interest deleted.",
					$"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");
			}

			return NoContent();
		}
	}
}