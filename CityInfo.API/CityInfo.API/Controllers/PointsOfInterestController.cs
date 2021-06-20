﻿using System;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using CityInfo.API.Models;
using CityInfo.API.Services;
using System.Collections.Generic;

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

		public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository repository)
		{
			this.logger = logger;
			this.mailService = mailService;
			this.repository = repository;
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
				var pointsOfIntersetForCityResult = new List<PointsOfInterestDto>();
				foreach (var pi in pointsOfInterest)
					pointsOfIntersetForCityResult.Add(new PointsOfInterestDto
					{
						Id = pi.Id,
						Name = pi.Name,
						Description = pi.Description
					});
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

			var pointOfInterestResult = new PointsOfInterestDto
			{
				Id = pointOfInterest.Id,
				Name = pointOfInterest.Name,
				Description = pointOfInterest.Description
			};

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

			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

			if (city == null)
			{
				return NotFound();
			}

			// demo purposes - to be improved.
			var maxPointOfInterest = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest)
				.Max(p => p.Id);

			var finalPointOfInterest = new PointsOfInterestDto
			{
				Id = ++maxPointOfInterest,
				Name = pointOfInterest.Name,
				Description = pointOfInterest.Description
			};

			city.PointsOfInterest.Add(finalPointOfInterest);

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

			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null)
			{
				return NotFound();
			}

			var pointOfInterestToUpdate = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
			if (pointOfInterestToUpdate == null)
			{
				return NotFound();
			}

			pointOfInterestToUpdate.Name = pointOfInterest.Name;
			pointOfInterestToUpdate.Description = pointOfInterest.Description;

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

			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null)
			{
				return NotFound();
			}

			var pointOfInterestToUpdate = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
			if (pointOfInterestToUpdate == null)
			{
				return NotFound();
			}

			var dtoToUpdate = new PointOfInterestForUpdateDto
			{
				Name = pointOfInterestToUpdate.Name,
				Description = pointOfInterestToUpdate.Description
			};

			// The following call only validates JsonPatchDocument validity against the PointOfInterestForUpdateDto model.
			// PointOfInterestForUpdateDto own validation rules do not fire.
			updateOperations.ApplyTo(dtoToUpdate, ModelState);
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Hand-code the validation rules for PointOfInterestForUpdateDto, like mandatory "Name" etc.
			TryValidateModel(dtoToUpdate);

			if (!string.IsNullOrEmpty(dtoToUpdate.Name) && !string.IsNullOrEmpty(dtoToUpdate.Description)
				&& dtoToUpdate.Name == dtoToUpdate.Description)
			{
				ModelState.AddModelError("Description", "The provided description cannot be the same as the name.");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			// Kind of "flush".
			pointOfInterestToUpdate.Name = dtoToUpdate.Name;
			pointOfInterestToUpdate.Description = dtoToUpdate.Description;

			return NoContent();
		}

		[HttpDelete("{cityId}/pointsofinterest/{id}")]
		public IActionResult DeletePointOfInterest(int cityId, int id)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null)
			{
				return NotFound();
			}

			var pointOfInterestToDelete = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
			if (pointOfInterestToDelete == null)
			{
				return NotFound();
			}

			city.PointsOfInterest.Remove(pointOfInterestToDelete);
			mailService.Send("Point of interest deleted.",
				$"Point of interest {pointOfInterestToDelete} with id {pointOfInterestToDelete.Id} was deleted.");

			return NoContent();
		}
	}
}