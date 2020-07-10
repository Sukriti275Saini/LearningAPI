using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningAPI.Data;
using Microsoft.AspNetCore.Http;
using LearningAPI.Models;
using System.Reflection.Metadata.Ecma335;
using AutoMapper;
using LearningAPI.Data.Entities;
using Microsoft.AspNetCore.Routing;

namespace LearningAPI.Controllers
{
    //[ApiController]
    //[Route("api/camps")]

    [ApiController]
    [Route("api/[controller]")]

    public class CampsController : ControllerBase
    {

        private readonly ICampRepository repository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public CampsController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet]
        //public async Task<IActionResult> Get()
        //public async Task<ActionResult<CampModel[]>> Get()
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks=false)
        {
            try
            {
                //var results = await repository.GetAllCampsAsync();
                var results = await repository.GetAllCampsAsync(true);

                //CampModel[] models = mapper.Map<CampModel[]>(results);

                //return Ok(models);
                return mapper.Map<CampModel[]>(results);
            }
            catch(Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var result = await repository.GetCampAsync(moniker);

                if (result == null) return NotFound();

                return mapper.Map<CampModel>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var results = await repository.GetAllCampsByEventDate(theDate, includeTalks);

                if (!results.Any()) return NotFound();

                return mapper.Map<CampModel[]>(results);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }


        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                var ecamp = await repository.GetCampAsync(model.Moniker);
                if(ecamp != null)
                {
                    return BadRequest("Moniker in use");
                }

                var location = linkGenerator.GetPathByAction("Get",
                    "Camps",
                    new { moniker = model.Moniker });

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use current moniker");
                }

                //Create a new Camp 
                //return Ok();

                var camp = mapper.Map<Camp>(model);
                repository.Add(camp);
                if(await repository.SaveChangesAsync())
                {
                    return Created($"/api/camps/{camp.Moniker}", mapper.Map<CampModel>(camp));
                }

            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest();
        }



        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var oldCamp = await repository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound($"Could not find camp with moniker of {moniker}");

                //oldCamp.Name = model.Name;
                mapper.Map(model, oldCamp);

                if (await repository.SaveChangesAsync())
                {
                    return mapper.Map<CampModel>(oldCamp);
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest();
        }


        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete (string moniker)
        {
            try
            {
                var oldCamp = await repository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound();

                repository.Delete(oldCamp);

                if(await repository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }


        //[HttpGet]
        //public string[] Get()
        //{
          //  return new[] { "Hello", "from", "Pluralsight" };
        //}

        //[HttpGet]
        //public object Get()
        //{
          //  return new { Moniker = "ALT2018", Name = "Atlanta Code Camp" };
        //}

        //[HttpGet]
        //public IActionResult GetCamps()
        //{
          //  //if(false) return BadRequest("Bad stuff happens");
          //  //if(false) return NotFound("Did not found");
            //return Ok(new { Moniker = "ALT2018", Name = "Atlanta Code Camp" });
        //}
    }
}
