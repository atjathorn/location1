
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Org.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace location
{
    [Activity(Label = "Get Location", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        string name, pass;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
           // SetContentView(Resource.Layout.Main);
            EditText username = FindViewById<EditText>(Resource.Id.txtUsername);
            EditText password = FindViewById<EditText>(Resource.Id.txtPassword);
            Button btnlogin = FindViewById<Button>(Resource.Id.btnLogin);
            Button btnregister = FindViewById<Button>(Resource.Id.btnRegister);


            //Jalankan Aktivity Register Ketika Button Register di Klik
            btnregister.Click += (object sender, System.EventArgs e) =>
            {
               // StartActivity(typeof(RegisterActivity));
            };


            //Jalankan fungsi untuk login cek apakah username dan password benar?
            btnlogin.Click += delegate
            {
                name = username.Text;
                pass = password.Text;
                WebClient client = new WebClient();
                Uri uri = new Uri("http://192.168.1.8/googlemap/New%20folder/xamarinSignIn.php");
                NameValueCollection parameters = new NameValueCollection();
                parameters.Add("xusername", name);
                parameters.Add("xpassword", pass);
                var response = client.UploadValues(uri, parameters);
                var responseString = Encoding.Default.GetString(response);
                JSONObject o = new JSONObject(responseString);
                //JObject o = JObject.Parse(responseString);
                //var ab = o.GetValue("login_event").ToString();

                if (o.OptString("success").Equals("1"))
                {
                    Toast.MakeText(this, "Login Success" , ToastLength.Short).Show();
                    //Jika login berhasil arahkan ke halaman welcome
                    var activity2 = new Intent(this, typeof(location));
                    activity2.PutExtra("MyData", name);
                    StartActivity(activity2);
                    //StartActivity(typeof(location));
                }
                else
                {

                    Toast.MakeText(this, "Username and Password Fail", ToastLength.Short).Show();

                }
            };


        }

        
    }
}