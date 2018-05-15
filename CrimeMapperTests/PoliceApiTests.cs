using Microsoft.VisualStudio.TestTools.UnitTesting;
using MappingTestApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maps.

namespace MappingTestApp.Tests
{
    [TestClass()]
    public class PoliceApiTests
    {
        [TestMethod()]
        public async void getCrimeLocationsTest()
        {
            LocationApi locationApi = new LocationApi();
            PoliceApi policeApi = new PoliceApi();

            // start date first of february
            DateTime startDate = new DateTime(2018, 2, 1);

            // end date 28th of February
            DateTime endDate = new DateTime(2018, 2, 28);

            LocationData locData = locationApi.getLocationData("norwich");

            string category = "bicycle-theft";

            int expectedCount = 34;

            List<Location> locations = await policeApi.getCrimeLocations(locData, startDate, endDate, category);







        }
    }
}