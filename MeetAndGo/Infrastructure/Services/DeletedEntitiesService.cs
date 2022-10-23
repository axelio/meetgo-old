using System.Collections.Generic;
using System.Linq;
using MeetAndGo.Data;
using MeetAndGo.Data.Dto;
using MeetAndGo.Data.Models;
using Newtonsoft.Json;

namespace MeetAndGo.Infrastructure.Services
{
    public interface IDeletedEntitiesService
    {
        void StoreDeletedEntity(DeletedEntityDto dto);
        void StoreDeletedEntities(List<DeletedEntityDto> dtos);
    }

    public class DeletedEntitiesService : IDeletedEntitiesService
    {
        private readonly MeetGoDbContext _dbContext;

        public DeletedEntitiesService(MeetGoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void StoreDeletedEntity(DeletedEntityDto dto) => _dbContext.DeletedEntities.Add(MapDtoToEntity(dto));

        public void StoreDeletedEntities(List<DeletedEntityDto> dtos) => _dbContext.DeletedEntities.AddRange(dtos.Select(MapDtoToEntity));

        private static DeletedEntity MapDtoToEntity(DeletedEntityDto dto) =>
            new()
            {
                IdOrig = dto.IdOrig,
                Type = dto.Type,
                JsonEntity = JsonConvert.SerializeObject(dto.Obj, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })
            };
    }
}
