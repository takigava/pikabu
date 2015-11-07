using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using System;
using System.Net;
using System.Threading.Tasks;
using PCLStorage;
using Android.Widget;
using System.Linq;
using System.IO;

namespace Pikabu
{
	[Activity (Label = "",Theme="@style/ImageViewTheme")]			
	public class ImageViewer : AppCompatActivity
	{
		string imageUrl = String.Empty;
		private ProgressDialog progress;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.ImageViewer);
			var mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            
			SetSupportActionBar(mToolbar);
		    SupportActionBar.Title = "";
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);


            
            var view = FindViewById<WebView>(Resource.Id.webViewImage);

		    if (view == null) return;
		    //String imageUrl = "file:///android_asset/anime.jpeg";
		    var text = Intent.GetStringExtra ("url") ?? string.Empty;

			using(var client = new UrlCheckWebClient())
			{
				// code as before
				client.HeadOnly = true;
				// fine, no content downloaded
				try{
					string s1 = client.DownloadString(text);
					imageUrl = text;
				}catch(Exception ex){
					imageUrl = text.Replace("post_img/big/","post_img/");
				}


			}


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
		    var loadString =
				String.Format("<!DOCTYPE html><html><head><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, user-scalable=1\"><style>html, body, #wrapper {{height:100%; width: 100%; margin: 0; padding: 0; border: 0; background-color: #000000;}} #wrapper td {{ vertical-align: middle; text-align: center; }}</style></head><body><table id=\"wrapper\"><tr><td><img src=\"{0}\" style=\"width:100%\"/></td></tr></table></body></html>",imageUrl);
		    //view.LoadUrl("http://cs6.pikabu.ru/post_img/2014/12/29/10/1419870935_2035225649.png");
		    view.LoadData(loadString,"text/html","utf-8");

		    //view.LoadDataWithBaseURL("","<img src='https://hashtagnerdswag.files.wordpress.com/2014/04/anime_wallpaper_v1_by_jontewftnd4ye097.jpg'/>","text/html", "UTF-8", "");

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
		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.action_menu, menu);
			return base.OnCreateOptionsMenu (menu);
		}

	    public override bool OnOptionsItemSelected(IMenuItem item)
	    {
	        switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    break;
			case Resource.Id.save_image:
				progress = new ProgressDialog (this);
				progress.SetMessage ("Загрузка...");
				progress.Show();
				Task.Factory.StartNew (async () => {
					try{
						
						await WebClient.CreateRealFileAsync(imageUrl);
						RunOnUiThread(()=>{
							progress.Dismiss();
						});
					}
					catch (Exception ex)
					{
						RunOnUiThread(()=>{
							progress.Dismiss();
							Toast.MakeText(Application.Context,"Ошибка сохранения",ToastLength.Short).Show();
						});
					}
				});
				break;
			case Resource.Id.copy_link:
				ClipboardManager clipManager = (ClipboardManager)GetSystemService (ClipboardService);
				ClipData clip = ClipData.NewPlainText ("url", imageUrl);
				clipManager.PrimaryClip = clip;
				break;
                default:
                    return base.OnOptionsItemSelected(item);
            }
            return true;
        }


	}
}

