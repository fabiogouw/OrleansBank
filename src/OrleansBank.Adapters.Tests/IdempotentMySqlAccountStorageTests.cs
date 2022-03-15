using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Orleans.Hosting;
using Orleans.Storage;
using Orleans.TestingHost;
using OrleansBank.Adapters.Grain;
using OrleansBank.Adapters.Storage;
using System;
using System.Threading.Tasks;
using Xunit;

namespace OrleansBank.Adapters.Tests
{
    [Collection("Idempotency Account Storage Tests")]
    public class IdempotentMySqlAccountStorageTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public async void Should_GetBalanceFromDatabase_When_GrainIsActivated()
        {
            await using var testContainer = await CreateDatabaseInstance();
            await testContainer.ExecScriptAsync("INSERT IGNORE INTO tb_account (account_id, balance, etag) VALUES (1999, 101.00, 1);");

            var cluster = CreateDeployedCluster();
            try
            {
                var account = cluster.GrainFactory.GetGrain<IAccountActor>("1999");
                var balance = await account.GetBalance();
                balance.Should().Be(101);
            }
            finally
            {
                cluster.StopAllSilos();
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void Should_WriteNewBalance_When_ACreditIsSaved()
        {
            await using var testContainer = await CreateDatabaseInstance();
            await testContainer.ExecScriptAsync("INSERT IGNORE INTO tb_account (account_id, balance, etag) VALUES (1999, 101.00, 1);");

            var cluster = CreateDeployedCluster();
            try
            {
                var account = cluster.GrainFactory.GetGrain<IAccountActor>("1999");
                await account.MakeCredit("unique", 10);
                var balance = await account.GetBalance();
                balance.Should().Be(111);
            }
            finally
            {
                cluster.StopAllSilos();
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void Should_ReturnIdempotentially_When_InsertingAnExistingIdempotencyKeyInDatabase()
        {
            await using var testContainer = await CreateDatabaseInstance();
            await testContainer.ExecScriptAsync("INSERT IGNORE INTO tb_account (account_id, balance, etag) VALUES (1999, 101.00, 1);");

            var cluster = CreateDeployedCluster();
            try
            {
                var account = cluster.GrainFactory.GetGrain<IAccountActor>("1999");
                await account.GetBalance(); // to activate the grain
                // simulate when the idempotency already exists, but is not load in the grain due memory soft limit
                await testContainer.ExecScriptAsync("INSERT IGNORE INTO tb_idempotency_keys (idempotency_key, account_id) VALUES ('unique', 1999);");
                var creditResult = await account.MakeCredit("unique", 3);
                creditResult.Should().BeTrue();
            }
            finally
            {
                cluster.StopAllSilos();
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void Should_KeepTheOldState_When_WritingNewStateFails()
        {
            await using var testContainer = await CreateDatabaseInstance();
            await testContainer.ExecScriptAsync("INSERT IGNORE INTO tb_account (account_id, balance, etag) VALUES (1999, 101.00, 1);");

            var cluster = CreateDeployedCluster();
            try
            {
                var account = cluster.GrainFactory.GetGrain<IAccountActor>("1999");
                double oldBalance = await account.GetBalance(); // to activate the grain
                // simulate an write error increasing the etag value
                await testContainer.ExecScriptAsync("UPDATE tb_account SET etag = 2 WHERE account_id = 1999;");
                try
                {
                    await account.MakeCredit("unique", 3);
                }
                catch (Exception ex)
                {
                    ex.Should().BeOfType<InconsistentStateException>();
                }
                double currentBalance = await account.GetBalance();
                currentBalance.Should().Be(oldBalance);

            }
            finally
            {
                cluster.StopAllSilos();
            }
        }

        private async Task<MySqlTestcontainer> CreateDatabaseInstance()
        {
            var testcontainersBuilder = new TestcontainersBuilder<MySqlTestcontainer>()
              .WithDatabase(new MySqlTestcontainerConfiguration
              {
                  Database = "orleansbank",
                  Username = "davidbowie",
                  Password = "secret",
                  Port = 51999
              });

            var testcontainer = testcontainersBuilder.Build();
            await testcontainer.StartAsync();
            await testcontainer.ExecScriptAsync(Properties.Resources.database_creation);
            return testcontainer;
        }

        private TestCluster CreateDeployedCluster()
        {
            var builder = new TestClusterBuilder();
            builder.AddSiloBuilderConfigurator<TestSiloConfigurations>();

            var cluster = builder.Build();
            cluster.Deploy();
            return cluster;
        }

        public class TestSiloConfigurations : ISiloConfigurator
        {
            public void Configure(ISiloBuilder siloBuilder)
            {
                siloBuilder.AddIdempotentySqlServerGrainStorage("Accounts", options =>
                {
                    // TODO: how can we pass the connection string from the test container?
                    options.ConnectionString = "Server=localhost;Port=51999;Database=orleansbank;Uid=davidbowie;Pwd=secret;";
                });
            }
        }
    }
}