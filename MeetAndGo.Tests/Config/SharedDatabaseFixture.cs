using System;
using System.Data.Common;
using MeetAndGo.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MeetAndGo.Tests.Config
{
    public class SharedDatabaseFixture : IDisposable
    {
        private static readonly object Lock = new();

        public SharedDatabaseFixture()
        {
            Connection = new SqlConnection(@"Server=(localdb)\mssqllocaldb;Database=UNIT_TESTS_MeetAndGo;Trusted_Connection=True");
            Seed();
            Connection.Open();
        }

        public DbConnection Connection { get; }

        public MeetGoDbContext CreateContext(DbTransaction transaction = null)
        {
            var context = new MeetGoDbContext(new DbContextOptionsBuilder<MeetGoDbContext>().UseSqlServer(Connection, x => x.UseNetTopologySuite()).Options);

            if (transaction != null) 
                context.Database.UseTransaction(transaction);

            return context;
        }

        private void Seed()
        {
            lock (Lock)
            {
                using var context = CreateContext();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                using var transaction = context.Database.BeginTransaction();

                TestData.AddClients(context);
                TestData.AddCompanies(context);
                context.SaveChanges();

                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT Addresses ON;");
                TestData.AddAddresses(context);
                context.SaveChanges();
                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT Addresses OFF;");

                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT CompanySettings ON;");
                TestData.AddCompanySettings(context);
                context.SaveChanges();
                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT CompanySettings OFF;");

                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT AspNetUserClaims ON;");
                TestData.AddCompanyClaims(context);
                context.SaveChanges();
                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT AspNetUserClaims OFF;");

                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT Events ON;");
                TestData.AddEvents(context);
                context.SaveChanges();
                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT Events OFF;");

                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT Visits ON;");
                TestData.AddVisits(context);
                context.SaveChanges();
                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT Visits OFF;");

                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT Bookings ON;");
                TestData.AddBookings(context);
                context.SaveChanges();
                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT Bookings OFF;");

                transaction.Commit();
            }
        }
        public void Dispose() => Connection.Dispose();
    }
}
