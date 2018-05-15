using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections;
using Microsoft.Maps.MapControl.WPF;
using System.Globalization;

namespace MappingTestApp
{
    /// <summary>
    /// Class for querying the Police UK API
    /// </summary>
    public class PoliceApi
    {
        string policeUrlHeader, dateStub, dateFormatPattern;

        WebClient client;

        /// <summary>
        /// Constructor for Api Class
        /// </summary>
        public PoliceApi()
        {
            this.policeUrlHeader = @"https://data.police.uk/api/crimes-street/";
            this.dateStub = "&date=";
            this.dateFormatPattern = "yyyy-MM";
            this.client = new WebClient();
        }

        #region Asnchronous API methods
        /// <summary>
        /// Asynchronous method for building request URI string and calling query method
        /// </summary>
        /// <param name="location"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task<List<Location>> getCrimeLocations (LocationData location, DateTime start, DateTime end, string category)
        {
            List<Location> locations = new List<Location>();

            string requestUri = policeUrlHeader + category + "?poly=";

            // Builds geolocational polyArea from Bing REST service result
            IList<double> locs = location.BoundingBox;
            string SW = locs[0].ToString("0.000") + "," + locs[1].ToString("0.000");
            string NE = locs[2].ToString("0.000") + "," + locs[3].ToString("0.000");
            string SE = locs[0].ToString("0.000") + "," + locs[3].ToString("0.000");
            string NW = locs[2].ToString("0.000") + "," + locs[1].ToString("0.000");
            string addressQuery = SW + ":" + NW + ":" + NE + ":" + SE;
            requestUri += addressQuery + dateStub;

            // Iterates through months in given date range and queries API for each month
            foreach (string month in EachMonth(start, end))
            {
                Debug.WriteLine(month);
                Debug.WriteLine(requestUri + month);
                locations.AddRange(await queryApiLocations(requestUri + month));
                
            }
            Debug.WriteLine(locations.Count);
            return locations;
        }

        /// <summary>
        /// Asynchronous method for querying Police UK API with given request URI
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        private async Task<List<Location>> queryApiLocations(string requestUri)
        {
            List<Location> crimeLocs = new List<Location>();

            // Calls Police UK API with asynchronous task
            string jsonData = await client.DownloadStringTaskAsync(requestUri);
            JArray rss = JArray.Parse(jsonData);

            // Builds Location objects from API response body and adds to list
            foreach (var crime in rss)
            {
                Location loc = new Location(
                    double.Parse(crime["location"]["latitude"].ToString(), CultureInfo.InvariantCulture),
                    double.Parse(crime["location"]["longitude"].ToString(), CultureInfo.InvariantCulture));

                crimeLocs.Add(loc);
            }
            return crimeLocs;
        }
        #endregion

        #region Helper methods
        // Iterates through months in range between given dates  
        private IEnumerable<String> EachMonth(DateTime from, DateTime end)
        {
            for (var month = from.Date; month.Date <= end; month = month.AddMonths(1))
                yield return month.ToString(dateFormatPattern);
        }
        #endregion
    }
}
