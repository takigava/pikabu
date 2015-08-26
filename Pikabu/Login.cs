using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin;
using Android.Accounts;
using Android.Preferences;



namespace Pikabu
{
    [Activity (Label = "Login",Theme="@style/Theme.NoActionBar",
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,ScreenOrientation=ScreenOrientation.Portrait)]
	public class Login : Activity
    {
        private string _userName;
        private string _password;
        private EditText _userNameTextView;
        private EditText _userPasswordTextView;

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
			var i18 = new I18N.Other.CP1251 ();

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Login);

			// Get our button from the layout resource,
			// and attach an event to it
			var button = FindViewById<Button> (Resource.Id.LoginButton);
			_userNameTextView = FindViewById<EditText>(Resource.Id.UserName);
			_userPasswordTextView = FindViewById<EditText>(Resource.Id.Password);

			//text.Click += delegate {
			//

			//};
			var pref = PreferenceManager.GetDefaultSharedPreferences(this);
			_userName = pref.GetString ("UserName", string.Empty);
			_password = pref.GetString ("Password", string.Empty);
			if (!string.IsNullOrEmpty(_userName))
			{
				_userNameTextView.Text = _userName;
			}
			if (!string.IsNullOrEmpty(_password))
			{
				_userPasswordTextView.Text = _password;
			}


			button.Click += delegate {
				var prog = ProgressDialog.Show(this,"Авторизация","Выполняем вход...",false,false);

				Task.Factory.StartNew(async() => {
					try
					{
						WebClient.Initialize();

						_userName = _userNameTextView.Text.Trim();


						_password = _userPasswordTextView.Text.Trim();


						var result = await WebClient.Authorize(new LoginInfo(){
							Mode = "login",
							Password = _password,
							Username = _userName,
							Remember = "0"
						});
						var message = string.Empty;
						if(result.Logined==1)
						{
							//ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo",FileCreationMode.Private);

							var editor = pref.Edit();
							editor.PutString("UserName",_userName);
							editor.PutString("Password",_password);
							editor.Apply();
							var intent = new Intent (this, typeof(MainView));
							StartActivity(intent);
							Finish();
						}
						else
						{
							message = "Неверное имя пользователя или пароль";
						}

						RunOnUiThread(()=>{

							prog.Dismiss();
							if(!string.IsNullOrEmpty(message))
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