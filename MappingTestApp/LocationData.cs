using Microsoft.Maps.MapControl.WPF;
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

namespace MappingTestApp
{
    public class LocationData
    {
        private string name;
        private IList<double> boundingBox;
        private Location centrePoint;

        public LocationData() { }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public IList<double> BoundingBox
        {
            get
            {
                return boundingBox;
            }

            set
            {
                boundingBox = value;
            }
        }

        public Location CentrePoint
        {
            get
            {
                return centrePoint;
            }

            set
            {
                centrePoint = value;
            }
        }
    }
}
