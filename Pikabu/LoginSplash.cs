using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
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
			var userName = pref.GetString ("UserName", string.Empty);
			var password = pref.GetString ("Password", string.Empty);

			if (string.IsNullOrEmpty (userName) || string.IsNullOrEmpty (password)) {
				var intent = new Intent (this, typeof(Login));
				StartActivity (intent);
			} 
			else 
			{
				LoginResponse result;
				Task.Factory.StartNew (async() => {
					try
					{
						WebClient.Initialize();
						result = await WebClient.Authorize(new LoginInfo(){
							Mode = "login",
							Password = password,
							Username = userName,
							Remember = "0"
						});
						if (result.Logined == 1) 
						{
							//успешно
							var intent = new Intent (this, typeof(MainView));
							StartActivity (intent);
							Finish ();
						} 
						else 
						{
							var editor = pref.Edit();
							editor.Clear ();
							editor.Apply ();
							var intent = new Intent (this, typeof(Login));
							StartActivity (intent);
							Finish ();
						}
					}
					catch(Exception)
					{
						var intent = new Intent (this, typeof(Login));
						StartActivity (intent);
						Finish ();
					}

				});



			}
		}
	}
}

