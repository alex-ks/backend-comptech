using Comptech.Backend.Service.Models;
using Comptech.Backend.Service.Analytics;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Comptech.Backend.Service.Test
{
    public class AnalyticsClientTest
    {
        private IAnalyticsClient client = new AnalyticsClient("94.180.119.78");
    }
}