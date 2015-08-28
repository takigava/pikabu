using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Media;

namespace Pikabu
{
	[Activity (Label = "",Theme="@style/Theme.NoActionBar")]			
	public class VideoViewer : Activity
	{
		

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.VideoViewer);
			//var mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);

			//SetSupportActionBar(mToolbar);
			//SupportActionBar.Title = string.Empty;
			//SupportActionBar.SetHomeButtonEnabled(true);
			//SupportActionBar.SetDisplayHomeAsUpEnabled(true);

			var text = Intent.GetStringExtra ("url") ?? string.Empty;

		}
	}
}

