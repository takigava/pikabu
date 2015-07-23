
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
using System.Threading.Tasks;
using Android.Preferences;

namespace Pikabu
{
	[Activity (Label = "Pikabu",MainLauncher = true, Icon = "@mipmap/ic_launcher",Theme="@style/Theme.NoActionBar")]			
	public class LoginSplash : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.LoginSplash);

			// Create your application here
			//ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo",FileCreationMode.Private);
			ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(this);
			var userName = pref.GetString ("UserName", String.Empty);
			var password = pref.GetString ("Password", String.Empty);

			if (String.IsNullOrEmpty (userName) || String.IsNullOrEmpty (password)) {
				Intent intent = new Intent (this, typeof(Login));
				this.StartActivity (intent);
			} 
			else 
			{
				LoginResponse result = new LoginResponse();
				Task.Factory.StartNew (async() => {
					try
					{
						WebClient.Initialize();
						result = await WebClient.Authorize(new LoginInfo(){
							mode = "login",
							password = password,
							username = userName,
							remember = "0"
						});
						if (result.logined == 1) 
						{
							//успешно
							Intent intent = new Intent (this, typeof(MainView));
							this.StartActivity (intent);
							this.Finish ();
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
					catch(Exception ex)
					{
						Intent intent = new Intent (this, typeof(Login));
						this.StartActivity (intent);
						this.Finish ();
					}

				});



			}
		}
	}
}

