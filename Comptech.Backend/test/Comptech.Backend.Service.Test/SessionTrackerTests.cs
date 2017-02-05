using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Comptech.Backend.Service.Test
{
    public class SessionTrackerTests
    {
        private AspApplicationMock app;
        public SessionTrackerTests()
        {
            var config = new Dictionary<string, string>()
            {
                ["SessionTimeout"] = "\"00:01:00\"",
                ["TimeoutCheckInterval"] = "\"00:00:01\""
            };
        }

        [Fact]
        public void CanStartSession()
        {
            //Arrange
            var sessionTracker = new SessionTracker()
    
            //Act

            //Assert

        }
    }
}
