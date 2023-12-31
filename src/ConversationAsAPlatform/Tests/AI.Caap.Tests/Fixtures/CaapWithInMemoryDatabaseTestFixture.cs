﻿using System.Data.Common;

using AI.CaaP.Repository.DatabaseContexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Caap.Tests.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class CaapWithInMemoryDatabaseTestFixture : CaapWithDatabaseTestFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        base.AddServices(services, configuration);
        var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ConversationDbContext>));
        if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);
        var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
        if (dbConnectionDescriptor != null) services.Remove(dbConnectionDescriptor);
        services.AddDbContext<ConversationDbContext>(options =>
        {
            options.UseInMemoryDatabase("Conversations");
        });
    }
}

