using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;
        public PlatformsController(IPlatformRepo repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
        {
            Console.WriteLine("--> Getting platforms...");

            var platformItems = _repo.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id}", Name="GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            Console.WriteLine($"--> Getting platform with id: {id}");

            var platform = _repo.GetPlatfromById(id);
            if(platform == null)
                return NotFound("Platform doest not exist! Try again!");

            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }


        [HttpPost]
        public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platformDto)
        {
            var platformModel = _mapper.Map<Platform>(platformDto);
            _repo.CreatePlatform(platformModel);

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            if(_repo.SaveChanges())
                return CreatedAtRoute(nameof(GetPlatformById), 
                    new {Id = platformReadDto.Id}, platformReadDto);

            return BadRequest("Something went wrong!");
        }


    }
}