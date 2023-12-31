﻿using Microsoft.Extensions.Logging;

namespace AI.Test.Support.Logging;

//TODO: Should look into using Serilog.Sinks.XUnit/serilog-sinks-xunittestoutput

/// <summary>
/// An <see cref="ILoggerFactory"/> used to create instance of XUnitTestLogger
/// </summary>
public class XUnitTestLoggerFactory : ILoggerFactory
{
    private readonly ILogger logger;

    /// <summary>
    /// Creates a new <see cref="XUnitTestLoggerFactory"/> instance.
    /// </summary>
    public XUnitTestLoggerFactory(ILogger logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc />
    /// <remarks>
    /// This returns a <see cref="XUnitTestLogger"/> instance which logs nothing.
    /// </remarks>
    public Microsoft.Extensions.Logging.ILogger CreateLogger(string name)
    {
        return new XUnitTestLogger(logger);
    }

    /// <inheritdoc />
    /// <remarks>
    /// This method ignores the parameter and does nothing.
    /// </remarks>
    public void AddProvider(ILoggerProvider provider)
    {
    }
    /// <inheritdoc />
    public void Dispose()
    {
    }
}