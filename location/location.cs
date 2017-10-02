using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.OS;
using Android.Util;
using Android.Widget;
using Android.App;
using location;
using Android.Locations;
using Android.Runtime;
using Android.Content;
using Android.Net;
using System.Net;
using System.IO;

namespace location
{
    [Activity(Label = "location")]
    public class location : Activity , ILocationListener
    {
        Location currentLocation;
        LocationManager locationManager;
        string locationProvider;
        TextView latitude, longitude, test;
        string sendText1;
        string sendText2, sendText3;
        string sendText4;
        EditText edittext1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.layout1);
            TextView _locationProvider = FindViewById<TextView>(Resource.Id.Provider);
            latitude = FindViewById<TextView>(Resource.Id.latitude);
            longitude = FindViewById<TextView>(Resource.Id.longitude);
            Button click = FindViewById<Button>(Resource.Id.click);
            edittext1 = FindViewById<EditText>(Resource.Id.name);
            // test = FindViewById<TextView>(Resource.Id.fullname);



            if (Checknetwork())
            {
                locationManager = (LocationManager)(this).GetSystemService(Context.LocationService);
                Criteria c = new Criteria();
                locationProvider = locationManager.GetBestProvider(c, true);
                OnLocationChanged(locationManager.GetLastKnownLocation(locationProvider));
                _locationProvider.Text = "Provider : " + locationProvider;
            }
            else
            {
                _locationProvider.Text = "";

            }
            click.Click += Click_Click;
        }

        private void Click_Click(object sender, EventArgs e)
        {
            
            //double test = Haversine.calculate(13.775445, 100.509833, currentLocation.Latitude, currentLocation.Longitude);
            //sendText4 = test.ToString();
            string result1 = Intent.GetStringExtra("MyData") ?? "Data not available";
            sendText1 = result1;
            Toast.MakeText(this, result1, ToastLength.Long).Show();
            SendToPhp();
        }

        //

        public static class Haversine
        {
            public static double calculate(double lat1, double lon1, double lat2, double lon2)
            {
                var R = 6372.8; // In kilometers
                var dLat = toRadians(lat2 - lat1);
                var dLon = toRadians(lon2 - lon1);
                lat1 = toRadians(lat1);
                lat2 = toRadians(lat2);

                var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
                var c = 2 * Math.Asin(Math.Sqrt(a));
                return R * 2 * Math.Asin(Math.Sqrt(a));
            }

            public static double toRadians(double angle)
            {
                return Math.PI * angle / 180.0;
            }
        }

        //

        private bool Checknetwork()
        {
            ConnectivityManager connmanager = (ConnectivityManager)GetSystemService(ConnectivityService);
            if (connmanager != null)
            {
                NetworkInfo currentnetwork = connmanager.ActiveNetworkInfo;
                bool Isok = (currentnetwork != null) && currentnetwork.IsConnected;
                return Isok;

            }
            return false;

        }

        protected override void OnResume()
        {
            base.OnResume();
            locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);


        }
        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);

        }
        public void OnLocationChanged(Location location)
        {
            currentLocation = location;
            if (currentLocation != null)
            {
                latitude.Text = "Latitude : " + currentLocation.Latitude.ToString();
                longitude.Text = "Longgitude : " + currentLocation.Longitude.ToString();
                sendText2 = currentLocation.Latitude.ToString();
                sendText3 = currentLocation.Longitude.ToString();

                double test = Haversine.calculate(13.775445, 100.509833, currentLocation.Latitude, currentLocation.Longitude);
                sendText4 = test.ToString();
                //Toast.MakeText(this, sendText2, ToastLength.Long).Show();
                SendToPhp();
                //Toast.MakeText(this, "sucess", ToastLength.Long).Show();

            }
            else
            {
                latitude.Text = "";
                longitude.Text = "";

            }
        }

        public void OnProviderDisabled(string provider)
        {

        }

        public void OnProviderEnabled(string provider)
        {

        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {

        }

        //
        public class Data
        {
            // Add e.g. strings, int, DateTime,... for each datafield in your database.
            public string name { get; set; } // These strings should get the same name as your databasefields.
            public string lati { get; set; }
            public string longi { get; set; }
            public string distance { get; set; }

        }
        // 
        //Toast.MakeText(this, sendText1, ToastLength.Short).Show();
        private void SendToPhp()
        {
            try
            {
                // Create a new data object
                Data DataObj = new Data();
                DataObj.name = sendText1;
                DataObj.lati = sendText2;
                DataObj.longi = sendText3;
                DataObj.distance = sendText4;


                // Serialize your data object.
                string JSONString = jsonclass.JSONSerialize<Data>(DataObj);

                // Set your Url for your php-file on your webserver.
                string url = "http://192.168.1.8/test/update.php";

                // Create your WebRequest
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);

                myRequest.Method = "POST";

                string postData = JSONString;

                byte[] pdata = Encoding.UTF8.GetBytes(postData);

                myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.ContentLength = pdata.Length;

                Stream myStream = myRequest.GetRequestStream();
                myStream.Write(pdata, 0, pdata.Length);


                // Get response from your php file.
                WebResponse myResponse = myRequest.GetResponse();

                Stream responseStream = myResponse.GetResponseStream();

                StreamReader streamReader = new StreamReader(responseStream);

                // Pass the response to a string and display it in a toast message.
                string result = streamReader.ReadToEnd();

                Toast.MakeText(this, result, ToastLength.Long).Show();

                // Close your streams.
                streamReader.Close();
                responseStream.Close();
                myResponse.Close();
                myStream.Close();
            }
            catch (WebException ex)
            {
                string _exception = ex.ToString();
                Toast.MakeText(this, _exception, ToastLength.Long).Show();
                System.Console.WriteLine("--->" + _exception);
            }
        }

    }
 }
