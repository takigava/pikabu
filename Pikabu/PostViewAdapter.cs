using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Square.Picasso;
using Android.Graphics;
using Android.Text.Style;
using Xamarin;
using Android.Graphics.Drawables;
using Android.Preferences;


namespace Pikabu
{
	public class PostViewAdapter : Android.Support.V7.Widget.RecyclerView.Adapter
	{
		private static ProgressDialog progress;
		public List<Post> _Posts{ get; set; }
		public RecyclerView _RecyclerView { get; set; }
		private static Context _Context;


		public void SetRecyclerView(RecyclerView recyclerView)
		{
			_RecyclerView = recyclerView;
		}

		public PostViewAdapter(List<Post> posts, RecyclerView recyclerView, Context context)
		{
			_Posts = posts;
			_RecyclerView = recyclerView;
			_Context = context;


		}



		public override int GetItemViewType(int position)
		{
			
			switch (_Posts [position].PostType) {
			case PostType.Text:
				return Resource.Layout.PostTextCard;
			case PostType.Image:
				return Resource.Layout.PostImageCard;
			case PostType.Gif:
				return Resource.Layout.PostImageCard;
			case PostType.Video:
				return Resource.Layout.PostImageCard;
			case PostType.MultiImage:
				return Resource.Layout.PostMultiImageCard;
			default:
				return Resource.Layout.PostTextCard;
			}
		}
		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{	
			if (viewType == Resource.Layout.PostTextCard) {
				View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.PostTextCard, parent, false);
				PostTextViewHolder vh = new PostTextViewHolder(row);
				return vh;
			}
			if (viewType == Resource.Layout.PostImageCard) {
				View row = LayoutInflater.From (parent.Context).Inflate (Resource.Layout.PostImageCard, parent, false);
				PostImageViewHolder vh = new PostImageViewHolder (row);
				return vh;
			} else {
				View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.PostTextCard, parent, false);
				PostTextViewHolder vh = new PostTextViewHolder(row);
				return vh;
			}
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			if (holder is PostTextViewHolder)
			{
				PostTextViewHolder vh = holder as PostTextViewHolder;
				FillTextPost (vh, position);
			}
			if (holder is PostImageViewHolder) {
				PostImageViewHolder vh = holder as PostImageViewHolder;
				FillImagePost (vh, position);



			}
		}

		public void ShowImage(object sender, EventArgs e){
		}
		public override int ItemCount
		{
			get { return _Posts.Count; }
		}
		public override long GetItemId (int position)
		{
			return _Posts [position].Id;
		}
		/*public override long GetItemId (int position)
		{
			return _Posts [position].Id;
		}*/
		public static SpannableString BuildBackgroundColorSpan(SpannableString spannableString,
			String text, String searchString, Color color) {

			int indexOf = text.ToUpperInvariant().IndexOf(searchString.ToUpperInvariant());

			try {
				spannableString.SetSpan(new BackgroundColorSpan(color), indexOf,
					(indexOf + searchString.Length),SpanTypes.ExclusiveExclusive);
			} catch (Exception e) {

			}


			return spannableString;
		}
		public void FillTextPost(PostTextViewHolder vh,int position){
			try{
				vh.AuthorName.Text = _Posts[position].AuthorName;
				var context = Android.App.Application.Context;
				vh.HeaderRating.Text = _Posts [position].Rating.ToString ();
				vh.BottomRating.Text = _Posts [position].Rating.ToString ();
				if (_Posts [position].Rating > 0) {
					vh.HeaderRating.SetTextColor (context.Resources.GetColor(Resource.Color.mainGreen));
					vh.BottomRating.SetTextColor (context.Resources.GetColor(Resource.Color.mainGreen));
					var text = " + "+_Posts[position].Rating;
					vh.HeaderRating.Text = text;
					vh.BottomRating.Text = text;
				}
				if (_Posts [position].Rating < 0) {
					vh.HeaderRating.SetTextColor (context.Resources.GetColor(Resource.Color.red));
					vh.BottomRating.SetTextColor (context.Resources.GetColor(Resource.Color.red));
					var text = ""+_Posts[position].Rating;
					vh.HeaderRating.Text = text;
					vh.BottomRating.Text = text;
				}
				vh.PostTime.Text = _Posts[position].PostTime;
				vh.Title.Text = _Posts[position].Title;
				if(_Posts[position].Description!=null && !String.IsNullOrEmpty(_Posts[position].Description.ToString())){
					vh.Description.Text = _Posts[position].Description.ToString();
					vh.Description.Visibility = ViewStates.Visible;
				}else{
					vh.Description.Text = String.Empty;
					vh.Description.Visibility = ViewStates.Gone;
				}
//				if(_Posts[position].FormattedDescription!=null && _Posts[position].FormattedDescription.Count>0){
//					SpannableStringBuilder descriptionBulder = new SpannableStringBuilder();
//					Color descTextColorDarkGray = context.Resources.GetColor(Resource.Color.darkGray);
//					Color descTextColorGray = context.Resources.GetColor(Resource.Color.gray);
//
//					foreach(var des in _Posts[position].FormattedDescription){
//						if(des.Item1 == "text"){
//							SpannableString tag1 = new SpannableString(des.Item2);
//
//							tag1.SetSpan(new ForegroundColorSpan(descTextColorGray), 0, tag1.Length(), SpanTypes.ExclusiveExclusive);
//							descriptionBulder.Append(tag1);
//						}
//						if(des.Item1 == "textLink"){
//							SpannableString tag2 = new SpannableString(des.Item2);
//
//							tag2.SetSpan(new ForegroundColorSpan(descTextColorDarkGray), 0, tag2.Length(), SpanTypes.ExclusiveExclusive);
//							descriptionBulder.Append(tag2);
//						}
//					}
//					if(descriptionBulder.Count()>0){
//						vh.Description.SetText(descriptionBulder,TextView.BufferType.Spannable);
//					}
//				}else{
//					vh.Description.Visibility = ViewStates.Gone;
//
//				}
				//vh._Description.Text = _Posts[position].Description;
				SpannableStringBuilder stringBuilder = new SpannableStringBuilder();
				Color textColor = context.Resources.GetColor(Resource.Color.mainGreen);

				foreach (var tag in _Posts[position].Tags) {
					SpannableString tag1 = new SpannableString("#"+tag);

					tag1.SetSpan(new ForegroundColorSpan(textColor), 0, tag1.Length(), SpanTypes.ExclusiveExclusive);
					stringBuilder.Append(tag1);
					stringBuilder.Append("  ");
					//SpannableString tag2 = new SpannableString(" ");
					//stringBuilder.Append(tag2);
					//stringBuilder.SetSpan(new TagSpan(backgroundColor, backgroundColor), stringBuilder.Length() - tag2.Length(), stringBuilder.Length(), SpanTypes.ExclusiveExclusive);
				}
				vh.Tags.SetText(stringBuilder,TextView.BufferType.Spannable);
				vh.Comments.Text = _Posts[position].Comments.ToString();
				vh.Text.Text = _Posts [position].Text;
			}catch(Exception ex){
				Insights.Initialize("0637c26a1b2e27693e05298f7c3c3a04c102a3c7", Application.Context);
				Insights.Report(ex,new Dictionary<string,string>{{"Message",ex.Message}},Insights.Severity.Error);
				var text = ex.Message;
			}
		}

		public void FillImagePost(PostImageViewHolder vh,int position){
			try{
				vh.AuthorName.Text = _Posts[position].AuthorName;
				var context = Android.App.Application.Context;
				vh.HeaderRating.Text = _Posts [position].Rating.ToString ();
				vh.BottomRating.Text = _Posts [position].Rating.ToString ();
				if (_Posts [position].Rating > 0) {
					vh.HeaderRating.SetTextColor (context.Resources.GetColor(Resource.Color.mainGreen));
					vh.BottomRating.SetTextColor (context.Resources.GetColor(Resource.Color.mainGreen));
					var text = " + "+_Posts[position].Rating;
					vh.HeaderRating.Text = text;
					vh.BottomRating.Text = text;
				}
				if (_Posts [position].Rating < 0) {
					vh.HeaderRating.SetTextColor (context.Resources.GetColor(Resource.Color.red));
					vh.BottomRating.SetTextColor (context.Resources.GetColor(Resource.Color.red));
					var text = ""+_Posts[position].Rating;
					vh.HeaderRating.Text = text;
					vh.BottomRating.Text = text;
				}
				vh.PostTime.Text = _Posts[position].PostTime;
				vh.Title.Text = _Posts[position].Title;
				if(_Posts[position].Description!=null && !String.IsNullOrEmpty(_Posts[position].Description.ToString())){
					vh.Description.Text = _Posts[position].Description.ToString();
					vh.Description.Visibility = ViewStates.Visible;
				}else{
					vh.Description.Text = String.Empty;
					vh.Description.Visibility = ViewStates.Gone;
				}
//				if(_Posts[position].FormattedDescription!=null && _Posts[position].FormattedDescription.Count>0){
//					SpannableStringBuilder descriptionBulder = new SpannableStringBuilder();
//					Color descTextColorDarkGray = context.Resources.GetColor(Resource.Color.darkGray);
//					Color descTextColorGray = context.Resources.GetColor(Resource.Color.gray);
//
//					foreach(var des in _Posts[position].FormattedDescription){
//						if(des.Item1 == "text"){
//							SpannableString tag1 = new SpannableString(des.Item2);
//
//							tag1.SetSpan(new ForegroundColorSpan(descTextColorGray), 0, tag1.Length(), SpanTypes.ExclusiveExclusive);
//							descriptionBulder.Append(tag1);
//						}
//						if(des.Item1 == "textLink"){
//							SpannableString tag2 = new SpannableString(des.Item2);
//
//							tag2.SetSpan(new ForegroundColorSpan(descTextColorDarkGray), 0, tag2.Length(), SpanTypes.ExclusiveExclusive);
//							descriptionBulder.Append(tag2);
//						}
//					}
//					if(descriptionBulder.Count()>0){
//						//vh.Description.TextFormatted = descriptionBulder;
//						vh.Description.SetText(descriptionBulder,TextView.BufferType.Spannable);
//					}
//				}else{
//					vh.Description.Visibility = ViewStates.Invisible;
//					vh.Description.Text = null;
//				}
				//vh._Description.Text = _Posts[position].Description;
				//vh._Description.Text = _Posts[position].Description;
				SpannableStringBuilder stringBuilder = new SpannableStringBuilder();
				Color textColor = context.Resources.GetColor(Resource.Color.mainGreen);
				var tagsBuilder = new StringBuilder();

				foreach (var tag in _Posts[position].Tags) {
					SpannableString tag1 = new SpannableString("#"+tag);

					tag1.SetSpan(new ForegroundColorSpan(textColor), 0, tag1.Length(), SpanTypes.ExclusiveInclusive);
					stringBuilder.Append(tag1);
					stringBuilder.Append("  ");
					//SpannableString tag2 = new SpannableString(" ");
					//stringBuilder.Append(tag2);
					//stringBuilder.SetSpan(new TagSpan(backgroundColor, backgroundColor), stringBuilder.Length() - tag2.Length(), stringBuilder.Length(), SpanTypes.ExclusiveExclusive);
					//tagsBuilder.Append("#"+tag+" ");
				}

				vh.Tags.SetText(stringBuilder,TextView.BufferType.Spannable);
				//vh.Tags.Text = tagsBuilder.ToString();
				//vh.Tags.TextFormatted = stringBuilder;
				vh.Comments.Text = _Posts[position].Comments.ToString();
				vh.Image.Click -= videoHandler;
				vh.Image.Click -= imageHandler;
				vh.Image.Click -= gifHandler;
				if(_Posts[position].PostType==PostType.Video){
					if (vh.Image != null) {
						vh.Image.SetTag (Resource.String.currentPosition, vh.AdapterPosition.ToString ());
						vh.Image.SetTag (Resource.String.imageUrl, _Posts [vh.AdapterPosition].VideoUrl);

						vh.Image.Click -= imageHandler;
						vh.Image.Click -= gifHandler;
						vh.Image.Click -= videoHandler;
						vh.Image.Click += videoHandler;
						Picasso.With(Android.App.Application.Context)
							.Load(_Posts[position].Url)
							.Transform(new CropSquareTransformation())
							.Into(vh.Image);
					}
				}
				if(_Posts[position].PostType==PostType.Image){
				if (vh.Image != null) {
					vh.Image.SetTag (Resource.String.currentPosition, vh.AdapterPosition.ToString ());
					vh.Image.SetTag (Resource.String.imageUrl, _Posts [vh.AdapterPosition].Url);
					vh.Image.SetTag (Resource.String.isbiggeravailable,_Posts [vh.AdapterPosition].IsBiggerAvailable);

						vh.Image.Click -= videoHandler;
						vh.Image.Click -= gifHandler;
						vh.Image.Click -= imageHandler;
						vh.Image.Click += imageHandler;
					Picasso.With(Android.App.Application.Context)
							.Load(_Posts[position].Url)
						.Transform(new CropSquareTransformation())
						.Into(vh.Image);
				}
				}
				if(_Posts[position].PostType==PostType.Gif){
					if (vh.Image != null) {
						vh.Image.SetTag (Resource.String.currentPosition, vh.AdapterPosition.ToString ());
						vh.Image.SetTag (Resource.String.imageUrl, _Posts [vh.AdapterPosition].GifUrl);

						vh.Image.Click -= videoHandler;
						vh.Image.Click -= imageHandler;
						vh.Image.Click -= gifHandler;
						vh.Image.Click += gifHandler;
						Picasso.With(Android.App.Application.Context)
							.Load(_Posts[position].Url)
							.Transform(new CropSquareTransformation())
							.Into(vh.Image);
					}
				}



			}catch(Exception ex){
				Insights.Initialize("0637c26a1b2e27693e05298f7c3c3a04c102a3c7", Application.Context);
				Insights.Report(ex,new Dictionary<string,string>{{"Message",ex.Message}},Insights.Severity.Error);
				var text = ex.Message;
			}
		}
		EventHandler videoHandler = (object sender, EventArgs e) => {
			var image = sender as ImageView;
			//Rivets.AppLinks.Navigator.Navigate("http://static.kremlin.ru/media/events/video/ru/video_high/QVrS7FkL9R89xxB9frxsAtnvu9ANx6Od.mp4");

			var videoUrl = image.GetTag (Resource.String.imageUrl).ToString ();
			if(videoUrl.Contains("youtube")){
				videoUrl = videoUrl.Replace("http://www.youtube.com/embed/","http://www.youtube.com/watch?v=");

				Task.Factory.StartNew(async()=>{
					Application.SynchronizationContext.Post (_ => {
					progress = new ProgressDialog (_Context);
					progress.SetMessage ("Загрузка...");
					progress.Show();
					}, null);
					Rivets.AppLinks.Navigator.WillNavigateToWebUrl+=(object sender2, Rivets.WebNavigateEventArgs e2) => {
						e2.Handled = true;
					};
					var result = await Rivets.AppLinks.Navigator.Navigate(videoUrl);
					Application.SynchronizationContext.Post (_ => {
					progress.Dismiss();
					}, null);
				});
			}else{
				Application.SynchronizationContext.Post (_ => {
					Toast.MakeText(Application.Context,videoUrl,ToastLength.Short).Show();
				}, null);
			}

			//Intent intent = new Intent (_Context, typeof(VideoViewer));
			//intent.PutExtra ("url", videoUrl);
			//_Context.StartActivity (intent);

		};
		EventHandler gifHandler = (object sender, EventArgs e) => {
			var image = sender as ImageView;
			//Rivets.AppLinks.Navigator.Navigate("http://static.kremlin.ru/media/events/video/ru/video_high/QVrS7FkL9R89xxB9frxsAtnvu9ANx6Od.mp4");
			Intent intent = new Intent (_Context, typeof(GifViewer));
			intent.PutExtra ("url", image.GetTag (Resource.String.imageUrl).ToString ());
			_Context.StartActivity (intent);
		};
		EventHandler imageHandler = (object sender, EventArgs e) => {
			var image = sender as ImageView;
			//Rivets.AppLinks.Navigator.Navigate("http://static.kremlin.ru/media/events/video/ru/video_high/QVrS7FkL9R89xxB9frxsAtnvu9ANx6Od.mp4");
			Intent intent = new Intent (_Context, typeof(ImageViewer));
			if(bool.Parse(image.GetTag (Resource.String.isbiggeravailable).ToString())){
				var url = image.GetTag (Resource.String.imageUrl).ToString ().Replace("post_img/","post_img/big/");
				intent.PutExtra ("url", url);
			}else{
				intent.PutExtra ("url", image.GetTag (Resource.String.imageUrl).ToString ());
			}

			_Context.StartActivity (intent);




		};
	}

	public class CropSquareTransformation : Java.Lang.Object, ITransformation
	{
		public Bitmap Transform(Bitmap source)
		{
			Bitmap result = source;
			if(source.Height>3000 || source.Width>3000)
			{
				int size = Math.Min(source.Width, source.Height);
				int x = (source.Width - size) / 3;
				int y = (source.Height - size) / 3;
				result = Bitmap.CreateBitmap(source, x, y, size, size);
				if (result != source) {
					source.Recycle();
				}
			}
			return result;
		}

		public string Key
		{
			get { return "square()"; } 
		}
	}
	
	public class MyScrollListener 
		: RecyclerView.OnScrollListener
	{
		private int visibleThreshold = 6;
		private int lastVisibleItem, totalItemCount;
		private bool IsLoading = false;
		private int currentPage=1;
		private bool _messageShown = false;
		private ISharedPreferences pref;
		public override void OnScrolled (RecyclerView recyclerView, int dx, int dy)
		{
			pref = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
			LinearLayoutManager linearLayoutManager = (LinearLayoutManager)recyclerView.GetLayoutManager ();
			int lastVisibleItem = linearLayoutManager.FindLastVisibleItemPosition();
			int totalItemCount = recyclerView.GetAdapter().ItemCount;

			if (!IsLoading && totalItemCount <= (lastVisibleItem + visibleThreshold)) {
				// End has been reached
				// Do something
				var currPrefPage = pref.GetString ("CurrentPage", string.Empty);
				if (!String.IsNullOrEmpty (currPrefPage)) {
					if (Int32.Parse (currPrefPage) > 0) {
						currentPage++;
					} else {
						currentPage = 2;
					}
				} else {
					currentPage++;

				}
				var editor = pref.Edit();
				editor.PutString("CurrentPage",currentPage.ToString());
				editor.Apply();

				IsLoading = true;
				Task.Factory.StartNew (async () => {
					
					try{
						var newPostList = new List<Post>();
						await WebClient.LoadPosts(newPostList,currentPage);


						(recyclerView.GetAdapter()as PostViewAdapter)._Posts.AddRange(newPostList);
						//recyclerView.GetAdapter().HasStableIds = true;

						_messageShown = false;
						Application.SynchronizationContext.Post (_ => {recyclerView.GetAdapter().NotifyDataSetChanged();}, null);
						//recyclerView.GetAdapter().NotifyItemRangeInserted(recyclerView.GetAdapter().ItemCount,newPostList.Count);
					}catch(Exception ex){
						//Insights.Report(ex,new Dictionary<string,string>{{"Message",ex.Message}},Insights.Severity.Error);
						var text = ex.Message;
						if(!_messageShown){
							Application.SynchronizationContext.Post (_ => {
								Toast.MakeText(Application.Context,"При загрузке данных произошла ошибка",ToastLength.Short).Show();
							}, null);
							_messageShown = true;
						}

					}


					IsLoading = false;
				});

			}
		}
		public override void OnScrollStateChanged (RecyclerView recyclerView, int newState)
		{
			Picasso picasso = Picasso.With(Android.App.Application.Context);
			//if ((Android.Widget.ScrollState)recyclerView.ScrollState != ScrollState.Idle || (Android.Widget.ScrollState)recyclerView.ScrollState != ScrollState.TouchScroll) {
			//	picasso.ResumeTag(Android.App.Application.Context);
			//} else {
			//	picasso.PauseTag(Android.App.Application.Context);
			//}
			switch (newState) {
			case RecyclerView.ScrollStateIdle:
				picasso.ResumeTag(Android.App.Application.Context);
				break;
			case RecyclerView.ScrollStateDragging:
				picasso.PauseTag(Android.App.Application.Context);
				break;
			case RecyclerView.ScrollStateSettling:
				picasso.PauseTag(Android.App.Application.Context);
				break;
			}

		}


	}

}