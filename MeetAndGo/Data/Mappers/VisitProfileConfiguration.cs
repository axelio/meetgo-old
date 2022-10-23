using AutoMapper;
using MeetAndGo.Data.Dto;
using MeetAndGo.Data.Models;

namespace MeetAndGo.Data.Mappers
{
    public class VisitProfileConfiguration : Profile
    {
        public VisitProfileConfiguration()
        {
            CreateMap<Visit, VisitWithEventDto>()
                .ForMember(v => v.StartDate, opts => opts.MapFrom(v => v.StartDate.DateTime));

            CreateMap<VisitWithEventDto, VisitDisplayDto>();

            CreateMap<Visit, CompanyVisitDisplayDto>()
                .ForMember(dto => dto.EventName, opts => opts.MapFrom(v => v.Event.Name))
                .ForMember(dto => dto.EventKind, opts => opts.MapFrom(v => v.Event.Kind))
                .ForMember(dto => dto.RequiresConfirmation, opts => opts.MapFrom(v => v.Event.RequiresConfirmation))
                .ForMember(dto => dto.StartDate, opts => opts.MapFrom(v => v.StartDate.DateTime));

            CreateMap<Visit, DeletedVisitDto>();
        }
    }
}
