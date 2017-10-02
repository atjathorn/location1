using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace location
{
    class data
    {

        public class LoginEvent
        {
            public string user_id { get; set; }
            public string username { get; set; }
            public string userpass { get; set; }
            public string name { get; set; }
            public string status { get; set; }
            public string ID { get; set; }
            public string lati { get; set; }
            public string longi { get; set; }
            public string distance { get; set; }
        }

        public class RootObject
        {
            public List<LoginEvent> login_event { get; set; }
        }
    }
}