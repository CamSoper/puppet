using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.Extensions.Configuration;
using Puppet.Common.Automation;
using Puppet.Common.Events;

namespace Puppet.Common.Telemetry
{
    public static class AppInsights
    {
        public static DependencyTrackingTelemetryModule InitializeDependencyTracking(TelemetryConfiguration configuration)
        {
            configuration.TelemetryProcessorChainBuilder
                .Use((next) => new HubitatAccessTokenDependencyFilter(next))
                .Build();

            DependencyTrackingTelemetryModule module = new DependencyTrackingTelemetryModule();
            module.Initialize(configuration);
            return module;
        }

        public static TelemetryConfiguration GetTelemetryConfiguration(IConfiguration configuration)
        {
            TelemetryConfiguration telemetryConfig = TelemetryConfiguration.CreateDefault();
            telemetryConfig.InstrumentationKey = configuration["InstrumentationKey"];
            telemetryConfig.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());
            telemetryConfig.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
            return telemetryConfig;
        }

        public static QuickPulseTelemetryModule InitializePerformanceTracking(TelemetryConfiguration configuration)
        {
            QuickPulseTelemetryProcessor processor = null;

            configuration.TelemetryProcessorChainBuilder
                .Use((next) =>
                {
                    processor = new QuickPulseTelemetryProcessor(next);
                    return processor;
                })
                .Build();

            QuickPulseTelemetryModule QuickPulse = new QuickPulseTelemetryModule();
            QuickPulse.Initialize(configuration);
            QuickPulse.RegisterTelemetryProcessor(processor);

            return QuickPulse;
        }

        public static TelemetryClient GetTelemetryClient(IConfiguration configuration)
        {
            return new TelemetryClient(GetTelemetryConfiguration(configuration));
        }
    }
}
