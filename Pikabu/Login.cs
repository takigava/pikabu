using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;


using System.Json;
using System.Collections.Generic;
using System.Collections.Specialized;

using System.Text;
using System.IO;
using System.Collections;
using System.Net.Http.Headers;
using Xamarin;
using System.Threading;
using Android.Accounts;
using Android.Preferences;



namespace Pikabu
{
	[Activity (Label = "Login",Theme="@style/Theme.NoActionBar",
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,ScreenOrientation=ScreenOrientation.Portrait)]
	public class Login : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Insights.HasPendingCrashReport += (sender, isStartupCrash) => {
				if (isStartupCrash) {
					Insights.PurgePendingCrashReports ().Wait ();
				}
			};
			Insights.Initialize("0637c26a1b2e27693e05298f7c3c3a04c102a3c7", this);
			var acc = AccountManager.Get(this).GetAccountsByType("com.google");
			if (acc.Length > 0) {
				Insights.Identify (acc [0].Name, Insights.Traits.Email, acc [0].Name);
			}
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Login);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.LoginButton);
			TextView text = FindViewById<TextView> (Resource.Id.LoginAnonymus);
			//text.Click += delegate {
				//

			//};


			button.Click += delegate {
				var prog = ProgressDialog.Show(this,"Авторизация","Выполняем вход...",false,false);

				Task.Factory.StartNew(async() => {
					try
					{
						WebClient.Initialize();
						var userName = FindViewById<EditText> (Resource.Id.UserName).Text.Trim();
						var password = FindViewById<EditText> (Resource.Id.Password).Text.Trim();
						var result = await WebClient.Authorize(new LoginInfo(){
							mode = "login",
							password = password,
							username = userName,
							remember = "0"
						});
						var message = String.Empty;
						if(result.logined==1)
						{
							//ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo",FileCreationMode.Private);
							ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(this);
							ISharedPreferencesEditor editor = pref.Edit();
							editor.PutString("UserName",userName);
							editor.PutString("Password",password);
							editor.Apply();
							Intent intent = new Intent (this, typeof(MainView));
							this.StartActivity (intent);
							this.Finish();
						}
						else
						{
							message = "Неверное имя пользователя или пароль";
						}

						RunOnUiThread(()=>{
							
							prog.Dismiss();
							if(!String.IsNullOrEmpty(message))
							{
								Toast.MakeText(this,message,ToastLength.Long).Show();
							}
						});

					}
					catch(Exception ex)
					{
						Insights.Report(ex,new Dictionary<string,string>{{"Message",ex.Message}},Insights.Severity.Error);
						RunOnUiThread(()=>{
							prog.Dismiss();
							Toast.MakeText(this,"Ошибка подключения сервера",ToastLength.Long).Show();
						});
					}

				});
			};
		}
	}
}