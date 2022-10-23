using AutoMapper;
using MeetAndGo.Data.Dto;
using MeetAndGo.Data.Models;

namespace MeetAndGo.Data.Mappers
{
    public class BookingProfileConfiguration : Profile
    {
        public BookingProfileConfiguration()
        {
            CreateMap<Booking, ClientBookingDto>()
                .ForMember(dto => dto.EventName, opts => opts.MapFrom(b => b.Visit.Event.Name))
                .ForMember(dto => dto.EventKind, opts => opts.MapFrom(b => b.Visit.Event.Kind))
                .ForMember(dto => dto.VisitStartDate, opts => opts.MapFrom(b => b.Visit.StartDate.DateTime))
                .ForMember(dto => dto.MaxPersons, opts => opts.MapFrom(b => b.Visit.MaxPersons))
                .ForMember(dto => dto.Price, opts => opts.MapFrom(b => b.Visit.Price))
                .ForMember(dto => dto.CompanyName, opts => opts.MapFrom(b => b.Visit.Event.Address.CompanyName))
                .ForMember(dto => dto.BookingsNumber, opts => opts.MapFrom(b => b.Visit.BookingsNumber));

            CreateMap<Booking, CompanyBookingDto>()
                .ForMember(dto => dto.CustomerMail, opts => opts.MapFrom(b => b.Customer.UserName))
                .ForMember(dto => dto.CustomerName, opts => opts.MapFrom(b => b.Customer.Name))
                .ForMember(dto => dto.CustomerPhoneNumber, opts => opts.MapFrom(b => b.Customer.PhoneNumber));

            CreateMap<Booking, DeletedBookingDto>()
                .ForMember(dto => dto.VisitId, opts => opts.MapFrom(b => b.Visit.Id));
        }
    }
}