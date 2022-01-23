using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo repo;
        private readonly IMapper mapper;

        public GrpcPlatformService(IPlatformRepo repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var reponse = new PlatformResponse();
            var platforms = repo.GetAllPlatforms();

            foreach(var plat in platforms)
            {
                reponse.Platform.Add(mapper.Map<GrpcPlatformModel>(plat));
            }

            return Task.FromResult(reponse);
        }
    }
}