﻿using AI.Test.Support;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using ML.Net.ImageClassification.Tests.Configuration;

namespace ML.Net.ImageClassification.Tests.Fixtures;

public class TensorFlowImageClassificationFixture : TestFixtureBase
{
    public TensorFlowImageClassificationOptions TensorFlowImageClassificationOptions { get; private set; }

    public TensorFlowImageClassificationFixture()
    {
        Factory = HostApplicationFactory.Build(
            environment: () => "IntegrationTests",
            serviceContext: (services, configuration) =>
            {
                services
                    .AddTensorFlowImageClassification(configuration)
                    ;
            },
            fixedDateTime: () => DateTimeOffset.UtcNow
        );
        TensorFlowImageClassificationOptions = Factory.Services.GetRequiredService<IOptions<TensorFlowImageClassificationOptions>>().Value;
    }
}
