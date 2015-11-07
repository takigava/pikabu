using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using System;
using PL.Droidsonroids.Gif;
using System.ComponentModel;
using System.Net;

using System.IO;
using System.Threading.Tasks;
using Android.Widget;

namespace Pikabu
{
	[Activity (Label = "",Theme="@style/ImageViewTheme")]			
	public class GifViewer : AppCompatActivity
	{
		private string _fileUrl = String.Empty;
		private byte[] _rawGifBytes;
		GifImageView _gifTextureView;
		private ProgressDialog progress;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			SetContentView (Resource.Layout.GifViewer);
			var mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);

			SetSupportActionBar(mToolbar);
			SupportActionBar.Title = "";
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);

			_gifTextureView = FindViewById<GifImageView>(Resource.Id.gifViewImage);

			if (_gifTextureView == null) return;
			//String imageUrl = "file:///android_asset/anime.jpeg";
			_fileUrl = Intent.GetStringExtra ("url") ?? string.Empty;
			progress = new ProgressDialog (this);
			progress.SetMessage ("Загрузка...");
			progress.Show();
			startDownload ();
				
		}
		protected override void OnStart ()
		{
			base.OnStart ();
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

						await WebClient.CreateRealFileAsync(_fileUrl);
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
				ClipData clip = ClipData.NewPlainText ("url", _fileUrl);
				clipManager.PrimaryClip = clip;
				break;
			default:
				return base.OnOptionsItemSelected(item);
			}
			return true;
		}

		private void startDownload()
		{
			
			System.Net.WebClient client = new System.Net.WebClient();
			client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
			client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(client_DownloadFileCompleted);
			client.DownloadDataAsync (new Uri (_fileUrl));
		}
		void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			double bytesIn = double.Parse(e.BytesReceived.ToString());
			double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
			double percentage = bytesIn / totalBytes * 100;
			var pers = int.Parse(Math.Truncate(percentage).ToString());
			//AndHUD.Shared.Show(this, String.Format("Загрузка...{0}",pers), pers);
			RunOnUiThread (() => {
				//progress.(pers);	
			});
		}
		void client_DownloadFileCompleted(object sender, DownloadDataCompletedEventArgs e)
		{
			_rawGifBytes = e.Result;
			//AndHUD.Shared.Dismiss(this);
			GifDrawable gifFromBytes = new GifDrawable( _rawGifBytes );
			RunOnUiThread(()=>{
				progress.Dismiss();
			});
			_gifTextureView.SetImageDrawable (gifFromBytes);


		}
	}
}


