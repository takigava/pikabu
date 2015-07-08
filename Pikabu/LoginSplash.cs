
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
	[Activity (Label = "LoginSplash",MainLauncher = true, Icon = "@drawable/icon",Theme="@style/Theme.NoActionBar")]			
	public class LoginSplash : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LoginSplash);
			// Create your application here
			ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo",FileCreationMode.Private);
			var userName = pref.GetString ("UserName", String.Empty);
			var password = pref.GetString ("Password", String.Empty);

			if (String.IsNullOrEmpty (userName) || String.IsNullOrEmpty (password)) {
				Intent intent = new Intent (this, typeof(Login));
				this.StartActivity (intent);
			} 
			else 
			{
				var result = WebClient.Authorize(new LoginInfo(){
					mode = "login",
					password = password,
					username = userName,
					remember = "0"
				}).Result;

				if (result.logined == 1) 
				{
					//успешно
				} 
				else 
				{
					ISharedPreferencesEditor editor = pref.Edit();
					editor.Clear ();
					editor.Apply ();
					Intent intent = new Intent (this, typeof(Login));
					this.StartActivity (intent);
					this.Finish ();
				}
			}
		}
	}
}

