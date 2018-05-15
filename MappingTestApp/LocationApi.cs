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

namespace MappingTestApp
{
    /// <summary>
    /// Class for calling the Bing Maps API to get location data to pass to Police API
    /// </summary>
    public class LocationApi
    {
        private string urlHeader, bingKey;

        /// <summary>
        /// Constructor
        /// </summary>
        public LocationApi()
        {
            this.urlHeader = @"http://dev.virtualearth.net/REST/v1/Locations/";
            this.bingKey = "AgG-RlgJtGrRdqAkyrfd8cOk5jKjbikZZOUoQAGLJUFOG9-wTiApx9SANGDSQMTf";
        }

        /// <summary>
        /// Queries Bing maps API to get geolocational data
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public LocationData getLocationData(string loc)
        {
            LocationData locationData = new LocationData();

            /* Format request string to localise to UK and build Uri request to API*/
            loc += "%20United%20Kingdom";
            string formattedLoc = loc.Replace(" ", "%20");
            string requestUri = urlHeader + formattedLoc + "?&key=" + bingKey;
            Debug.WriteLine(requestUri);
            string jsonData = new WebClient().DownloadString(requestUri);
            var jObject = JObject.Parse(jsonData);

            var bestResult = jObject["resourceSets"][0]["resources"][0];

            /* Get name of location and add to LocationData object */
            string name = bestResult["name"].ToString();
            locationData.Name = name;

            IList<double> bbox = bestResult["bbox"].ToObject<List<double>>();
            locationData.BoundingBox = bbox;

            IList<double> point = bestResult["point"]["coordinates"].ToObject<List<double>>();
            locationData.CentrePoint = new Location(point[0], point[1]);

            return locationData;
        }
    
    }
}
