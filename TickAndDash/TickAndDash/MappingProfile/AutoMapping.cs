using AutoMapper;
using System;
using TickAndDash.Controllers.V1.Requests;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.HttpClients.GeoClients.DTOs;
using TickAndDash.Services.ServicesDtos;
using TickAndDashDAL.DAL;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDash.MappingProfile
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<RidersSendPincodeRequest, Riders>()
                    .ForMember(dest => dest.User, opt => opt.MapFrom(src => new User() { }));
            CreateMap<RidersLoginRequest, Riders>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new User() { }));

            CreateMap<DeviceInfo, Riders>();


            CreateMap<Site, GetActiveSitesResponse>();
            CreateMap<Site, GetActiveSitesResponse>();

            CreateMap<Site, GetAllSitesCanVisitResponse>();


            CreateMap<ResentPincodeRequest, Riders>()
             .ForMember(dest => dest.User, opt => opt.MapFrom(src => new User() { }));

            CreateMap<PickupStations, PickupStationsResponse>()
            .ForMember(
            dest => dest.PikcupId, opt =>
            opt.MapFrom(src => src.Id))
            .ForMember(
            dest => dest.StationType, opt =>
            opt.MapFrom(src => src.PickupStationsLookup.Type))
            .ForMember(
            dest => dest.SiteId, opt =>
            opt.MapFrom(src => src.Sites.Id)).
            ForMember(
            dest => dest.SiteName, opt =>
            opt.MapFrom(src => src.Name))
            .ForMember(
            dest => dest.ItineraryId, opt =>
            opt.MapFrom(src => src.Transportation_Itineraries.Id))
            .ForMember(
            dest => dest.ItineraryName, opt =>
            opt.MapFrom(src => src.Transportation_Itineraries.Name));



            CreateMap<MajorsMinorStations, PickupStationsResponse>()
            .ForMember(
            dest => dest.PikcupId, opt =>
            opt.MapFrom(src => src.MinorPickupStations.Id))
            .ForMember(
            dest => dest.StationType, opt =>
            opt.MapFrom(src => "Minor"))
            .ForMember(
            dest => dest.SiteId, opt =>
            opt.MapFrom(src => src.MinorPickupStations.Sites.Id)).
            ForMember(
            dest => dest.SiteName, opt =>
            opt.MapFrom(src => src.MinorPickupStations.Name))
            .ForMember(
            dest => dest.ItineraryId, opt =>
            opt.MapFrom(src => src.MinorPickupStations.TransItineraryId))
            .ForMember(
            dest => dest.ItineraryName, opt =>
            opt.MapFrom(src => src.MinorPickupStations.Transportation_Itineraries.Name)
            )
            .ForMember(dest => dest.Descriptions, opt => opt.MapFrom(src => src.MinorPickupStations.Descriptions))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.MinorPickupStations.Longitude))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.MinorPickupStations.Latitude))
            .ForMember(dest => dest.Radius, opt => opt.MapFrom(src => src.MinorPickupStations.Radius));


            CreateMap<Riders, RiderInfoResponse>().ForMember(
            dest => dest.Name, opt =>
            opt.MapFrom(src => src.User.Name ?? ""))
                .ForMember(
            dest => dest.Gender, opt =>
            opt.MapFrom(src => src.Gender != null ? src.Gender : ' '));

            CreateMap<RidersQueue, RidersQResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name != null ? src.User.Name : $"{src.Id} الراكب"))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.RidersQStatusLookup.Name))
                .ForMember(dest => dest.PickupStation, opt => opt.MapFrom(src => src.PickupStations.Name))
                .ForMember(dest => dest.ReservationDate, opt => opt.MapFrom(src => src.ReservationDate.ToString("HH:mm:ss")));


            CreateMap<RidersTickets, RidersQResponse>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RidersQueue.Id))
              .ForMember(dest => dest.CountOfSeats, opt => opt.MapFrom(src => src.RidersQueue.CountOfSeats))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Ticket))
              .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.RidersQueue.RidersQStatusLookup.Name))
              .ForMember(dest => dest.PickupStation, opt => opt.MapFrom(src => src.RidersQueue.PickupStations.Name))
              .ForMember(dest => dest.IsPresent, opt => opt.MapFrom(src => src.RidersQueue.IsPresent))
              .ForMember(dest => dest.ReservationDate, opt => opt.MapFrom(src => src.RidersQueue.ReservationDate.ToString("HH:mm:ss")));


            CreateMap<RidersQueue, ActiveReservationResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.RidersQStatusLookup.Name))
                .ForMember(dest => dest.PickupStation, opt => opt.Ignore());


            CreateMap<Car, DriverCarInfo>();

            CreateMap<Transportation_Itineraries, DriverTransItineraryInfo>();

            CreateMap<Driver, DriverInfoResponse>()
                .ForMember(dest => dest.CarInfoResponse, opt => opt.MapFrom(src => src.Car))
                .ForMember(dest => dest.TransItineraryResponse, opt => opt.MapFrom(src => src.Car.Transportation_Itineraries))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForPath(dest => dest.CarInfoResponse.CarCode, opt => opt.MapFrom(src => src.Car.CarCode))
                .ForPath(dest => dest.CarInfoResponse.Model, opt => opt.MapFrom(src => src.Car.Model == null ? "" : src.Car.Model))
                .ForPath(dest => dest.CarInfoResponse.ModelYear, opt => opt.MapFrom(src => src.Car.ModelYear == null ? "" : src.Car.ModelYear))
                .ForPath(dest => dest.TransItineraryResponse.Description, opt => opt.MapFrom(src => src.Car.Transportation_Itineraries.Description == null ? "" : src.Car.Transportation_Itineraries.Description));
            CreateMap<DeviceInfo, UsersSessions>();//.ForMember(dest => dest.Id, src => src.Ignore());

            CreateMap<CarsQueue, GetPickupQCarsResponse>()
             .ForMember(dest => dest.QTurn, opt => opt.MapFrom(src => src.Turn))
             .ForMember(dest => dest.RegistrationPlate, opt => opt.MapFrom(src => src.Car.RegistrationPlate))
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Car.LoggedInDriver.User.Name == null ? "سائق" : src.Car.LoggedInDriver.User.Name))
             .ForMember(dest => dest.CarCode, opt => opt.MapFrom(src => src.Car.CarCode));

            CreateMap<AddComplaintsRequest, Complaint>()
                 .ForMember(dest => dest.User, opt => opt.Ignore())
                 .ForMember(dest => dest.OpenDate, opt => opt.MapFrom(src => DateTime.Now))
                 .ForMember(dest => dest.complaintStatus, opt => opt.MapFrom(src => ComplaintEnum.Open.ToString()));

            CreateMap<AddComplaintsReplyRequest, ComplaintsTickets>()
             .ForMember(dest => dest.User, opt => opt.Ignore())
             .ForMember(dest => dest.Complaint, opt => opt.Ignore())
             .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<DigitalCodexGetBalanceResponse, GetUserBalanceResponse>()
                .ForMember(dest => dest.UserTransactions, opt => opt.Ignore());
            CreateMap<UserTransactions, UserTransactionsData>()
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src =>
                src.CreationDate.ToString("yyyy-MM-dd HH:mm:ss")));

            CreateMap<AddCallbackRequest, Callback>()
                 .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                 .ForMember(dest => dest.BillingStatus, opt => opt.MapFrom(src => src.Status));

            CreateMap<ComplaintTypeTranslation, GetAllComplaintsType>();
            CreateMap<ComplaintSubTypeTranslation, GetAllComplaintsSubType>();

            CreateMap<PointOfSales, GetAllPointOfSales>()
                .ForMember(dest => dest.SiteName, opt => opt.MapFrom(src => src.Site.Name));


            CreateMap<Transportation_Itineraries, GetItineraryPrices>()
                .ForMember(dest => dest.ItineraryName, opt => opt.MapFrom(src => src.Name));

            CreateMap<Transportation_Itineraries, GetItinerariesRequest>()
                   .ForMember(dest => dest.ItineraryName, opt => opt.MapFrom(src => src.Name));


            CreateMap<RefillRequest, GetAllRefillingRequestsResponse>()
                   .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.Rider.MobileNumber));

        }
    }
}
