
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
using Android.Webkit;

namespace Pikabu
{
	[Activity (Label = "ImageView",Theme="@style/Theme.NoActionBar")]			
	public class ImageViewer : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.ImageViewer);

			WebView view = FindViewById<WebView>(Resource.Id.webViewImage);

			if (view != null)
			{
				//String imageUrl = "file:///android_asset/anime.jpeg";
				string text = Intent.GetStringExtra ("url") ?? String.Empty;
				view.Settings.DefaultZoom = WebSettings.ZoomDensity.Far;
				view.Settings.SetSupportZoom(true);
				view.Settings.DisplayZoomControls = false;
				view.Settings.BuiltInZoomControls = true;
				view.ScaleX = 1;
				view.ScaleY = 1;
				//view.Settings.LoadWithOverviewMode = true;
				//view.Settings.UseWideViewPort = true;
				//view.Settings.SetLayoutAlgorithm (WebSettings.LayoutAlgorithm.SingleColumn);
				//view.LoadUrl(imageUrl);
				var loadString = String.Format("<!DOCTYPE html><html><head><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, user-scalable=1\"><style>html, body, #wrapper {{height:100%; width: 100%; margin: 0; padding: 0; border: 0; background-color: #000000;}} #wrapper td {{ vertical-align: middle; text-align: center; }}</style></head><body><table id=\"wrapper\"><tr><td><img src=\"{0}\" style=\"width:100%\"/></td></tr></table></body></html>",text);
				//view.LoadUrl("http://cs6.pikabu.ru/post_img/2014/12/29/10/1419870935_2035225649.png");
				view.LoadData(loadString,"text/html","utf-8");
				//view.LoadDataWithBaseURL("","<img src='https://hashtagnerdswag.files.wordpress.com/2014/04/anime_wallpaper_v1_by_jontewftnd4ye097.jpg'/>","text/html", "UTF-8", "");
			}

			/*ScaleImageView siView = FindViewById<ScaleImageView>(Resource.Id.AnimateImage);
			//siView.SetImageResource (Resource.Drawable.Anime);
			Task.Factory.StartNew(()=>{
				var image = Picasso.With (this).Load ("http://cs6.pikabu.ru/post_img/2014/12/29/10/1419870935_2035225649.png").Get ();
				RunOnUiThread(()=>{
					//siView.SetImageBitmap (image);
					//siView.SetScaleType(ImageView.ScaleType.FitCenter);
					//siView.SetAdjustViewBounds(false);
					//siView.SetImageDrawable(new BitmapDrawable(image));
					//siView.SetFitsSystemWindows(true);
				});
			});
			*/
		}
	}
}

