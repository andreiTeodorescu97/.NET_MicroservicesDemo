using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.HTTP;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repo, IMapper mapper,
            ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
        {
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
            _repo = repo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
        {
            Console.WriteLine("--> Getting platforms...");

            var platformItems = _repo.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            Console.WriteLine($"--> Getting platform with id: {id}");

            var platform = _repo.GetPlatfromById(id);
            if (platform == null)
                return NotFound("Platform doest not exist! Try again!");

            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }


        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatformAsync(PlatformCreateDto platformDto)
        {
            var platformModel = _mapper.Map<Platform>(platformDto);
            _repo.CreatePlatform(platformModel);
            _repo.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            //Send Synchronous Message
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously. \n {ex.Message} \n {ex.InnerException.Message}");
            }

            //Send Asynchronous Message
            try
            {
                var platformToPublish = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformToPublish.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(platformToPublish);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send asynchronously. \n {ex.Message} \n {ex.InnerException.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById),
                new { Id = platformReadDto.Id }, platformReadDto);
        }


    }
}