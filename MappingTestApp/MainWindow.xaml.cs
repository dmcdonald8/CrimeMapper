using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.SpatialToolbox.Bing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Diagnostics;
namespace MappingTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Displays the MainWindow and Map
    /// Handles the UI thread
    /// Makes async API calls
    /// </summary>
    public partial class MainWindow : Window
    {
        private LocationApi locationApi;
        private PoliceApi policeApi;
        private HeatMapLayer layer;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.locationApi = new LocationApi();
            this.policeApi = new PoliceApi();
            this.Title = "Crime Mapper";
            
            SetBlackOutDates();
        }

        // Blacks out dates that are unavailable in the Police API
        private void SetBlackOutDates()
        {
            /* Dates that are too far in the past */
            StartDatePicker.BlackoutDates.Add(new CalendarDateRange(
                new DateTime(1990, 1, 1),
                new DateTime(2015, 2, 28)
                ));

            EndDatePicker.BlackoutDates.Add(new CalendarDateRange(
                new DateTime(1990, 1, 1),
                new DateTime(2015, 2, 28)
                ));

            /* Dates too far in the future */
            DateTime currentDate = DateTime.Today;
            DateTime month = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime endOfPreviousMonth = month.AddDays(-1).AddMonths(-1);

            StartDatePicker.BlackoutDates.Add(new CalendarDateRange(
                endOfPreviousMonth,
                new DateTime(2028, 1, 1)
                ));

            EndDatePicker.BlackoutDates.Add(new CalendarDateRange(
                endOfPreviousMonth,
                new DateTime(2028,1,1)
                ));

        }

        #region Event Handlers

        /// <summary>
        /// Clears placeholder text when location textbox is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Location_Box_Click(object sender, RoutedEventArgs e)
        {
            TextBox locationBox = (TextBox)LocationBox;
            locationBox.Clear();
        }

        /// <summary>
        /// Handles click event on search button and handles search functionality
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            if (EndDatePicker.SelectedDate != null && StartDatePicker.SelectedDate != null)
            {
                //Clear prior search
                myMap.Visibility = Visibility.Hidden;
                myMapLabel.Visibility = Visibility.Collapsed;
                myMap.Children.Clear();
                SearchResults.Visibility = Visibility.Collapsed;
                AddressList.Children.Clear();
                ErrorMessage.Visibility = Visibility.Collapsed;
                LoadingLabel.Visibility = Visibility.Visible;

                LocationData locationData = locationApi.getLocationData(LocationBox.Text);
                myMap.Center = locationData.CentrePoint;

                ComboBoxItem crimetype = (ComboBoxItem)CrimeType.SelectedItem;

                string crimeTag = (String)crimetype.Tag;

                DateTime startDate = (DateTime)StartDatePicker.SelectedDate;
                DateTime endDate = (DateTime)EndDatePicker.SelectedDate;

                List<Location> locations = new List<Location>();

                try
                {

                    locations = await policeApi.getCrimeLocations(locationData, startDate, endDate, crimeTag);

                    LocationCollection locationCollection = new LocationCollection();

                    // create List of locations to add to heat map layer
                    foreach (Location location in locations)
                    {
                        locationCollection.Add(location);
                    }

                    // Create new HeatMapLayer setting view properties
                    layer = new HeatMapLayer()
                    {
                        ParentMap = myMap,
                        Locations = locationCollection,
                        Radius = 500,
                        Opacity = 0.5
                    };

                    // Add to map view as child component layer
                    myMap.Children.Add(layer);

                    LocationRect box = new LocationRect(locations);
                    myMap.SetView(box);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("--Error retrieving data from server--\n" +
                        "Please try one or more of the following: \n" +
                        "* Ensuring your are entering a valid geolocation in England and Wales (e.g avoid any special characters)\n" +
                        "* Narrowing the search area (e.g South London/Chelsea instead of London \n" +
                        "* Reducing the date range \n" +
                        "* Searching for less common crimes (e.g bicycle crime instead of public order offences)\n\n"
                        ,
                        "Error", MessageBoxButton.OK);
                    Debug.WriteLine(ex.StackTrace);
                    InitializeComponent();
                }
                LoadingLabel.Visibility = Visibility.Hidden;
                myMap.Visibility = Visibility.Visible;
                myMap.Focus(); //allows '+' and '-' to zoom the map 
            }
            else
            {
                MessageBox.Show("Please enter a start and end date", "Enter date", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Change opacity of HeatLayer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Opacity_Changed(object sender, object e)
        {
            if (layer != null)
            {
                var slider = sender as Slider;
                layer.Opacity = slider.Value;
            }
        }

        /// <summary>
        /// Change intensity of HeatLayer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Intensity_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (layer != null)
            {
                var cbx = sender as ComboBox;

                layer.Intensity = double.Parse((cbx.SelectedItem as ComboBoxItem).Content as string);
            }
        }

        /// <summary>
        /// Change gradient of HeatLayer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gradient_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (layer != null)
            {
                var cbx = sender as ComboBox;
                var item = cbx.SelectedItem as ComboBoxItem;

                var bg = item.Background as LinearGradientBrush;
                layer.HeatGradient = bg.GradientStops;
            }
        }

        /// <summary>
        /// Change Radius of HeatLayer grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Radius_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (layer != null)
            {
                var cbx = sender as ComboBox;
                var rKM = int.Parse((cbx.SelectedItem as ComboBoxItem).Content as string);
                layer.Radius = rKM * 100;
            }
        }

        #endregion

        #region Async Methods

        /// <summary>
        /// Makes asynchronous API call to PoliceApi object
        /// </summary>
        /// <param name="locationData"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="crimeTag"></param>
        /// <returns></returns>
        async Task<LocationCollection> queryApi(LocationData locationData, DateTime startDate, DateTime endDate, string crimeTag)
        {
            List<Location> locations = new List<Location>();
            locations.AddRange(await policeApi.getCrimeLocations(locationData, startDate, endDate, crimeTag));

            LocationCollection locationCollection = new LocationCollection();
            foreach(Location location in locations)
            {
                locationCollection.Add(location);
            }
            return locationCollection;
        }

        #endregion
    }
}

