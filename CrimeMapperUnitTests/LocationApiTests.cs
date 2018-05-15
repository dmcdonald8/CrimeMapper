using Microsoft.VisualStudio.TestTools.UnitTesting;
using MappingTestApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MappingTestApp.Tests
{
    [TestClass()]
    public class LocationApiTests
    {
        [TestMethod]
        public void getLocationDataTest()
        {
            LocationApi locationApi = new LocationApi();

            // established correct values from observed Bing Api output
            double expectedLat = 51.506420135498;
            double expectedLng = -0.127210006117821;

            string testLocation = "london";

            // actual output from LocationApi class
            LocationData actual = locationApi.getLocationData(testLocation);

            // Assert expected outputs match actual output
            Assert.AreEqual(expectedLat, actual.CentrePoint.Latitude);
            Assert.AreEqual(expectedLng, actual.CentrePoint.Longitude);
        }
    }
}