
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

namespace Pikabu
{
	[Activity (Label = "LoginSplash",MainLauncher = true)]			
	public class LoginSplash : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo",FileCreationMode.Private);
			var userName = pref.GetString ("UserName", String.Empty);
			var password = pref.GetString ("Password", String.Empty);

			if (String.IsNullOrEmpty (userName) || String.IsNullOrEmpty (password)) {
				Intent intent = new Intent (this, typeof(Login));
				this.StartActivity (intent);
			}
		}
	}
}

