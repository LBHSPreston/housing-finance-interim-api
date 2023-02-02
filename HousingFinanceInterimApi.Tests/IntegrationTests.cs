using System;
using System.Collections.Generic;
using System.Net.Http;
using HousingFinanceInterimApi.V1.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace HousingFinanceInterimApi.Tests
{
    //[Collection("AppTest collection")]
    public class IntegrationTests<_TStartup> where _TStartup : class
    {

        protected HttpClient Client { get; private set; }
        protected DatabaseContext DatabaseContext { get; private set; }

        private MockWebApplicationFactory<_TStartup> _factory;
        private SqlConnection _connection;
        private IDbContextTransaction _transaction;
        private DbContextOptionsBuilder _builder;

        private readonly List<Action> _cleanup = new List<Action>();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                foreach (var action in _cleanup)
                    action();

                _disposed = true;
            }
        }

        
        public IntegrationTests()
        {
            var connectionString = ConnectionString.TestDatabase();
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            SqlCommand sqlServerCommand = _connection.CreateCommand();
            sqlServerCommand.CommandText = "SET deadlock_timeout TO 30";
            sqlServerCommand.ExecuteNonQuery();

            _builder = new DbContextOptionsBuilder();
            _builder.UseSqlServer(_connection);

            _factory = new MockWebApplicationFactory<_TStartup>(_connection);
            Client = _factory.CreateClient();
            DatabaseContext = new DatabaseContext(_builder.Options);
            DatabaseContext.Database.EnsureCreated();
            _transaction = DatabaseContext.Database.BeginTransaction();

            
        }
    }

}
