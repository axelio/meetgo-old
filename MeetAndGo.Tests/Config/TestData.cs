using System;
using System.Collections.Generic;
using MeetAndGo.Data;
using MeetAndGo.Data.Models;
using MeetAndGo.Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;

namespace MeetAndGo.Tests.Config
{
    public static class TestData
    {
        #region add-data
        public static void AddCompanies(MeetGoDbContext dbContext)
        {
            dbContext.Users.AddRange(
                PrepareGdanskCompany(),
                PrepareCoolGdanskCompany(),
                PrepareGdyniaCompany()
            );
        }

        public static void AddClients(MeetGoDbContext dbContext) =>
            dbContext.Users.AddRange(
                PrepareFirstCustomer(),
                PrepareSecondCustomer(),
                PrepareThirdCustomer()
            );

        public static void AddCompanyClaims(MeetGoDbContext dbContext) =>
            dbContext.UserClaims.AddRange(
                PrepareCompanyClaim(1, "68d1b5ce-8c7e-4f2f-97db-16c9bfe92b57"),
                PrepareCompanyClaim(2, "e161c8fa-7430-4df2-9040-93704a5b40df"),
                PrepareCompanyClaim(3, "e256b87a-333d-49b7-aab6-2e149313b8cc")
            );

        public static void AddAddresses(MeetGoDbContext dbContext) =>
            dbContext.Addresses.AddRange(
                PrepareGdanskCompanyAddress(),
                PrepareGdyniaCompanyAddress(),
                PrepareCoolGdanskCompanyAddress()
            );

        public static void AddEvents(MeetGoDbContext dbContext) =>
            dbContext.Events.AddRange(
                PrepareGdanskCompanyEvent(),
                PrepareGdyniaCompanyEvent(),
                PrepareGdanskCoolCompanyEvent(),
                PrepareGdanskCompanyEnrollmentEvent()
            );

        public static void AddVisits(MeetGoDbContext dbContext) => dbContext.Visits.AddRange(PrepareVisits());

        public static void AddBookings(MeetGoDbContext dbContext) => dbContext.Bookings.AddRange(PrepareBookings());

        public static void AddCompanySettings(MeetGoDbContext dbContext) =>
            dbContext.CompanySettings.AddRange(
                PrepareCoolGdanskCompanySettings(),
                PrepareGdanskCompanySettings(),
                PrepareGdyniaCompanySettings());
        #endregion

        #region companies
        private static User PrepareGdyniaCompany() =>
            new()
            {
                Id = "e256b87a-333d-49b7-aab6-2e149313b8cc",
                EmailConfirmed = true,
                Email = "2@company.pl",
                UserName = "2@company.pl",
                LockoutEnabled = true,
                NormalizedEmail = "2@COMPANY.PL",
                NormalizedUserName = "2@COMPANY.PL"
            };

        private static User PrepareGdanskCompany() =>
            new()
            {
                Id = "68d1b5ce-8c7e-4f2f-97db-16c9bfe92b57",
                EmailConfirmed = true,
                Email = "gdansk@company.pl",
                UserName = "gdansk@company.pl",
                LockoutEnabled = true,
                NormalizedEmail = "GDANSK@COMPANY.PL",
                NormalizedUserName = "GDANSK@COMPANY.PL",
            };

        private static User PrepareCoolGdanskCompany() =>
            new()
            {
                Id = "e161c8fa-7430-4df2-9040-93704a5b40df",
                EmailConfirmed = true,
                Email = "3@company.pl",
                UserName = "3@company.pl",
                LockoutEnabled = true,
                NormalizedEmail = "3@COMPANY.PL",
                NormalizedUserName = "3@COMPANY.PL"
            };
        private static IdentityUserClaim<string> PrepareCompanyClaim(int claimId, string userId) =>
            new()
            {
                Id = claimId,
                ClaimType = "Company",
                ClaimValue = "true",
                UserId = userId
            };

        #endregion

        #region customers
        private static User PrepareSecondCustomer() =>
            new()
            {
                Id = "cd11d467-0ff5-4c73-83f7-646ea62b803b",
                Email = "second@customer.pl",
                UserName = "second@customer.pl",
                LockoutEnabled = true,
                NormalizedEmail = "SECOND@CUSTOMER.PL",
                NormalizedUserName = "SECOND@CUSTOMER.PL",
                PhoneNumber = "+48796111333",
                PhoneNumberConfirmed = true
            };

        private static User PrepareThirdCustomer() =>
            new()
            {
                Id = "third-customer",
                Email = "third@customer.pl",
                UserName = "third@customer.pl",
                LockoutEnabled = true,
                NormalizedEmail = "THIRD@CUSTOMER.PL",
                NormalizedUserName = "THIRD@CUSTOMER.PL",
                PhoneNumber = "+48796666006",
                PhoneNumberConfirmed = true
            };

        private static User PrepareFirstCustomer() =>
            new()
            {
                Id = "a52dbf80-2e31-4817-b701-2a4c96c2b8f4",
                Email = "first@customer.pl",
                UserName = "first@customer.pl",
                LockoutEnabled = true,
                NormalizedEmail = "FIRST@CUSTOMER.PL",
                NormalizedUserName = "FIRST@CUSTOMER.PL",
                PhoneNumber = "+48796111222",
                PhoneNumberConfirmed = true
            };
        #endregion

        #region addresses
        private static Address PrepareCoolGdanskCompanyAddress() =>
            new()
            {
                Id = 2,
                CityId = 1,
                CompanyName = "CoolFirma",
                Street = "Szczecińska",
                Number = "53",
                Website = "www.test.pl"
            };

        private static Address PrepareGdanskCompanyAddress() =>
            new()
            {
                Id = 3,
                CityId = 1,
                CompanyName = "GdanskFirma",
                Street = "Gdańsk, Gdańska",
                Number = "53",
                Website = "www.test.pl"
            };

        private static Address PrepareGdyniaCompanyAddress() =>
            new()
            {
                Id = 1,
                CityId = 2,
                CompanyName = "GdyniaFirma",
                Street = "Gdynia, Toruńska",
                Number = "62",
                Website = "www.test.pl"
            };
        #endregion

        #region company-settings
        private static CompanySettings PrepareCoolGdanskCompanySettings() =>
            new()
            {
                Id = 2,
                UserId = "e161c8fa-7430-4df2-9040-93704a5b40df",
                IsActive = true,
                MaxDailyVisits = 5

            };

        private static CompanySettings PrepareGdanskCompanySettings() =>
            new()
            {
                Id = 3,
                IsActive = true,
                MaxDailyVisits = 3,
                UserId = "68d1b5ce-8c7e-4f2f-97db-16c9bfe92b57"
            };

        private static CompanySettings PrepareGdyniaCompanySettings() =>
            new()
            {
                Id = 1,
                IsActive = true,
                MaxDailyVisits = 5,
                UserId = "e256b87a-333d-49b7-aab6-2e149313b8cc"
            };
        #endregion

        #region events
        private static Event PrepareGdyniaCompanyEvent() =>
            new()
            {
                Id = 1,
                Name = "Inny event",
                UserId = "e256b87a-333d-49b7-aab6-2e149313b8cc",
                Description = "Testowy opis wydarzenia",
                DurationInMinutes = 60,
                PictureUrl = "ohter-url-picture",
                RequiresConfirmation = true,
                CategoryId = 1,
                AddressId = 1,
                Kind = EventKind.Booking
            };

        private static Event PrepareGdanskCoolCompanyEvent() =>
            new()
            {
                Id = 2,
                Name = "Event firmy cool gdansk",
                Description = "Wydarzenie opis",
                DurationInMinutes = 120,
                PictureUrl = "url-picture",
                UserId = "e161c8fa-7430-4df2-9040-93704a5b40df",
                CategoryId = 2,
                AddressId = 2,
                Kind = EventKind.Booking
            };

        private static Event PrepareGdanskCompanyEvent() =>
            new()
            {
                Id = 3,
                UserId = "68d1b5ce-8c7e-4f2f-97db-16c9bfe92b57",
                Name = "Wydarzenie",
                Description = "Wydarzenie opis",
                DurationInMinutes = 60,
                PictureUrl = "url-picture",
                RequiresConfirmation = true,
                CategoryId = 2,
                AddressId = 3,
                Kind = EventKind.Booking
            };

        private static Event PrepareGdanskCompanyEnrollmentEvent() =>
            new()
            {
                Id = 4,
                UserId = "68d1b5ce-8c7e-4f2f-97db-16c9bfe92b57",
                Name = "Zapisy",
                Description = "Zapisy opis",
                DurationInMinutes = 60,
                PictureUrl = "url-picture",
                RequiresConfirmation = true,
                CategoryId = 3,
                AddressId = 3,
                Kind = EventKind.Enrollment
            };
        #endregion

        #region visits
        private static List<Visit> PrepareVisits() =>
            new()
            {
                new Visit //17-12-2030 Gdynia Company, not booked
                {
                    Id = 1,
                    MaxPersons = 2,
                    Price = 50,
                    StartDate = new DateTimeOffset(2030, 12, 17, 21, 00, 00, TimeSpan.FromHours(1)),
                    EventId = 1,
                    CityId = 2,
                    TimeOfDay = TimeOfDay.Morning
                },
                new Visit //17-12-2030, coolgdansk Company, not booked
                {
                    Id = 2,
                    MaxPersons = 6,
                    Price = 120,
                    StartDate = new DateTimeOffset(2030, 12, 17, 22, 00, 00, TimeSpan.FromHours(1)),
                    EventId = 2,
                    CityId = 1,
                    TimeOfDay = TimeOfDay.Morning
                },
                new Visit //17-12-2030, Gdansk Company, already booked
                {
                    Id = 3,
                    EventId = 3,
                    MaxPersons = 4,
                    Price = 30,
                    StartDate = new DateTimeOffset(2030, 12, 17, 21, 00, 00, TimeSpan.FromHours(1)),
                    CityId = 1,
                    TimeOfDay = TimeOfDay.Morning,
                    IsBooked = true,
                    BookingsNumber = 1
                },
                new Visit //17-12-2030, Gdansk company, not booked
                {
                    Id = 4,
                    EventId = 3,
                    MaxPersons = 4,
                    Price = 30,
                    StartDate = new DateTimeOffset(2030, 12, 17, 22, 00, 00, TimeSpan.FromHours(1)),
                    CityId = 1,
                    TimeOfDay = TimeOfDay.Morning
                },
                new Visit //17-12-2030, Gdansk company, not booked
                {
                    Id = 5,
                    EventId = 3,
                    MaxPersons = 2,
                    Price = 50,
                    StartDate = new DateTimeOffset(2030, 12, 17, 20, 00, 00, TimeSpan.FromHours(1)),
                    CityId = 1,
                    TimeOfDay = TimeOfDay.Evening
                },
                new Visit //18-12-2030, Gdansk company, booked
                {
                    Id = 6,
                    EventId = 3,
                    MaxPersons = 4,
                    Price = 30,
                    StartDate = new DateTimeOffset(2030, 12, 18, 22, 00, 00, TimeSpan.FromHours(1)),
                    CityId = 1,
                    TimeOfDay = TimeOfDay.Morning,
                    IsBooked = true,
                    BookingsNumber = 1
                },
                new Visit //18-12-2015, Gdansk company, booked
                {
                    Id = 7,
                    EventId = 3,
                    MaxPersons = 4,
                    Price = 30,
                    StartDate = new DateTimeOffset(2015, 12, 18, 22, 00, 00, TimeSpan.FromHours(1)),
                    CityId = 1,
                    TimeOfDay = TimeOfDay.Morning,
                    IsBooked = false
                },
                new Visit //18-12-2015, enrollment, Gdansk company, partly booked
                {
                    Id = 8,
                    EventId = 4,
                    MaxPersons = 10,
                    Price = 30,
                    StartDate = new DateTimeOffset(2030, 12, 18, 22, 00, 00, TimeSpan.FromHours(1)),
                    CityId = 1,
                    TimeOfDay = TimeOfDay.Morning,
                    IsBooked = false,
                    BookingsNumber = 1
                }
            };
        #endregion

        #region bookings
        private static List<Booking> PrepareBookings() =>
            new()
            {
                new Booking
                {
                    Id = 1,
                    UserId = "a52dbf80-2e31-4817-b701-2a4c96c2b8f4",
                    IsConfirmed = false,
                    VisitId = 3,
                    Code = "4124"
                },
                new Booking
                {
                    Id = 2,
                    UserId = "a52dbf80-2e31-4817-b701-2a4c96c2b8f4",
                    IsConfirmed = false,
                    VisitId = 6,
                    Code = "0224"
                },
                new Booking
                {
                    Id = 3,
                    UserId = "a52dbf80-2e31-4817-b701-2a4c96c2b8f4",
                    IsConfirmed = true,
                    VisitId = 7,
                    Code = "1234"
                },
                new Booking
                {
                    Id = 4,
                    UserId = "third-customer",
                    IsConfirmed = true,
                    VisitId = 8,
                    Code = "7982"
                }
            };
        #endregion
    }
}
