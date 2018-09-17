using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCREngine.WebApi
{
    public class ApplicationInsightsHelper
    {
        private readonly TelemetryClient client = new TelemetryClient();
    
        public ApplicationInsightsHelper(IConfiguration configuration)
        {

            if (Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY") != null)
            {
                client.InstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
            }
            else
            {
                client.InstrumentationKey = configuration.GetValue<string>(key: "ApplicationInsights:InstrumentationKey");    
            }            
        }

        public void TrackEvent(string eventMessage)
        {
            client.TrackEvent(eventMessage);
        }

        public void TrackEvent(string eventMessage, IDictionary<string, string> keyValuePairs)
        {
            client.TrackEvent(eventMessage, keyValuePairs);
        }

        public void TrackEvent(string eventMessage, IDictionary<string, string> keyValuePairs, IDictionary<string, double> subKeyValuePairs)
        {
            client.TrackEvent(eventMessage, keyValuePairs, subKeyValuePairs);
        }

        public void TrackEvent(EventTelemetry eventTelemetry)
        {
            client.TrackEvent(eventTelemetry);
        }
    }
}
