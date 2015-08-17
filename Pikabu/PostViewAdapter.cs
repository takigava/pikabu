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


namespace Pikabu
{
	public class PostViewAdapter : Android.Support.V7.Widget.RecyclerView.Adapter
	{
		
		public List<Post> _Posts{ get; set; }
		public RecyclerView _RecyclerView { get; set; }
		private Context _Context;


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
				vh._AuthorName.Text = _Posts[position].AuthorName;
				var context = Android.App.Application.Context;
				vh._HeaderRating.Text = _Posts [position].Rating.ToString ();
				vh._BottomRating.Text = _Posts [position].Rating.ToString ();
				if (_Posts [position].Rating > 0) {
					vh._HeaderRating.SetTextColor (context.Resources.GetColor(Resource.Color.mainGreen));
					vh._BottomRating.SetTextColor (context.Resources.GetColor(Resource.Color.mainGreen));
					var text = " + "+_Posts[position].Rating;
					vh._HeaderRating.Text = text;
					vh._BottomRating.Text = text;
				}
				if (_Posts [position].Rating < 0) {
					vh._HeaderRating.SetTextColor (context.Resources.GetColor(Resource.Color.red));
					vh._BottomRating.SetTextColor (context.Resources.GetColor(Resource.Color.red));
					var text = " - "+_Posts[position].Rating;
					vh._HeaderRating.Text = text;
					vh._BottomRating.Text = text;
				}
				vh._PostTime.Text = _Posts[position].PostTime;
				vh._Title.Text = _Posts[position].Title;
				if(_Posts[position].FormattedDescription!=null && _Posts[position].FormattedDescription.Count>0){
					SpannableStringBuilder descriptionBulder = new SpannableStringBuilder();
					Color descTextColorDarkGray = context.Resources.GetColor(Resource.Color.darkGray);
					Color descTextColorGray = context.Resources.GetColor(Resource.Color.gray);
					Color descBackgroundColor = new Color(255,255,255,0);
					foreach(var des in _Posts[position].FormattedDescription){
						if(des.Item1 == "text"){
							SpannableString tag1 = new SpannableString(des.Item2);
							descriptionBulder.Append(tag1);
							descriptionBulder.SetSpan(new TagSpan(descBackgroundColor, descTextColorGray), descriptionBulder.Length() - tag1.Length(), descriptionBulder.Length(), SpanTypes.ExclusiveExclusive);
						}
						if(des.Item1 == "textLink"){
							SpannableString tag2 = new SpannableString(des.Item2);
							descriptionBulder.Append(tag2);
							descriptionBulder.SetSpan(new TagSpan(descBackgroundColor, descTextColorDarkGray), descriptionBulder.Length() - tag2.Length(), descriptionBulder.Length(), SpanTypes.ExclusiveExclusive);
						}
					}
					vh._Description.TextFormatted = descriptionBulder;
				}else{
					//vh._Description.Visibility = ViewStates.Invisible;
				}
				//vh._Description.Text = _Posts[position].Description;
				SpannableStringBuilder stringBuilder = new SpannableStringBuilder();
				Color textColor = context.Resources.GetColor(Resource.Color.mainGreen);
				Color backgroundColor = new Color(255,255,255,0);
				foreach (var tag in _Posts[position].Tags) {
					SpannableString tag1 = new SpannableString("#"+tag);
					stringBuilder.Append(tag1);
					stringBuilder.SetSpan(new TagSpan(backgroundColor, textColor), stringBuilder.Length() - tag1.Length(), stringBuilder.Length(), SpanTypes.ExclusiveExclusive);

					SpannableString tag2 = new SpannableString("");
					stringBuilder.Append(tag2);
					stringBuilder.SetSpan(new TagSpan(backgroundColor, backgroundColor), stringBuilder.Length() - tag2.Length(), stringBuilder.Length(), SpanTypes.ExclusiveExclusive);
				}
				vh._Tags.TextFormatted = stringBuilder;
				vh._Comments.Text = _Posts[position].Comments.ToString();
				vh._Text.Text = _Posts [position].Text;
			}catch(Exception ex){
				var text = ex.Message;
			}
		}

		public void FillImagePost(PostImageViewHolder vh,int position){
			try{
				vh._AuthorName.Text = _Posts[position].AuthorName;
				var context = Android.App.Application.Context;
				vh._HeaderRating.Text = _Posts [position].Rating.ToString ();
				vh._BottomRating.Text = _Posts [position].Rating.ToString ();
				if (_Posts [position].Rating > 0) {
					vh._HeaderRating.SetTextColor (context.Resources.GetColor(Resource.Color.mainGreen));
					vh._BottomRating.SetTextColor (context.Resources.GetColor(Resource.Color.mainGreen));
					var text = " + "+_Posts[position].Rating;
					vh._HeaderRating.Text = text;
					vh._BottomRating.Text = text;
				}
				if (_Posts [position].Rating < 0) {
					vh._HeaderRating.SetTextColor (context.Resources.GetColor(Resource.Color.red));
					vh._BottomRating.SetTextColor (context.Resources.GetColor(Resource.Color.red));
					var text = " - "+_Posts[position].Rating;
					vh._HeaderRating.Text = text;
					vh._BottomRating.Text = text;
				}
				vh._PostTime.Text = _Posts[position].PostTime;
				vh._Title.Text = _Posts[position].Title;
				if(_Posts[position].FormattedDescription!=null && _Posts[position].FormattedDescription.Count>0){
					SpannableStringBuilder descriptionBulder = new SpannableStringBuilder();
					Color descTextColorDarkGray = context.Resources.GetColor(Resource.Color.darkGray);
					Color descTextColorGray = context.Resources.GetColor(Resource.Color.gray);
					Color descBackgroundColor = new Color(255,255,255,0);
					foreach(var des in _Posts[position].FormattedDescription){
						if(des.Item1 == "text"){
							SpannableString tag1 = new SpannableString(des.Item2);
							descriptionBulder.Append(tag1);
							descriptionBulder.SetSpan(new TagSpan(descBackgroundColor, descTextColorGray), descriptionBulder.Length() - tag1.Length(), descriptionBulder.Length(), SpanTypes.ExclusiveExclusive);
						}
						if(des.Item1 == "textLink"){
							SpannableString tag2 = new SpannableString(des.Item2);
							descriptionBulder.Append(tag2);
							descriptionBulder.SetSpan(new TagSpan(descBackgroundColor, descTextColorDarkGray), descriptionBulder.Length() - tag2.Length(), descriptionBulder.Length(), SpanTypes.ExclusiveExclusive);
						}
					}
					vh._Description.TextFormatted = descriptionBulder;
				}else{
					//vh._Description.Visibility = ViewStates.Invisible;
				}
				//vh._Description.Text = _Posts[position].Description;
				//vh._Description.Text = _Posts[position].Description;
				SpannableStringBuilder stringBuilder = new SpannableStringBuilder();
				Color textColor = context.Resources.GetColor(Resource.Color.mainGreen);
				Color backgroundColor = new Color(255,255,255,0);

				foreach (var tag in _Posts[position].Tags) {
					SpannableString tag1 = new SpannableString("#"+tag);
					stringBuilder.Append(tag1);
					stringBuilder.SetSpan(new TagSpan(backgroundColor, textColor), stringBuilder.Length() - tag1.Length(), stringBuilder.Length(), SpanTypes.ExclusiveExclusive);

					SpannableString tag2 = new SpannableString("");
					stringBuilder.Append(tag2);
					stringBuilder.SetSpan(new TagSpan(backgroundColor, backgroundColor), stringBuilder.Length() - tag2.Length(), stringBuilder.Length(), SpanTypes.ExclusiveExclusive);
				}
				vh._Tags.TextFormatted = stringBuilder;
				vh._Comments.Text = _Posts[position].Comments.ToString();
				if (vh._Image != null) {
					vh._Image.SetTag (Resource.String.currentPosition, vh.AdapterPosition.ToString ());
					vh._Image.SetTag (Resource.String.imageUrl, _Posts [vh.AdapterPosition].Url);
					EventHandler handler = (object sender, EventArgs e) => {
						var image = sender as ImageView;
						//Rivets.AppLinks.Navigator.Navigate("http://static.kremlin.ru/media/events/video/ru/video_high/QVrS7FkL9R89xxB9frxsAtnvu9ANx6Od.mp4");
						Intent intent = new Intent (_Context, typeof(ImageViewer));
						intent.PutExtra ("url", image.GetTag (Resource.String.imageUrl).ToString ());
						_Context.StartActivity (intent);




					};
					vh._Image.Click -= handler;
					vh._Image.Click += handler;
					Picasso.With(Android.App.Application.Context)
						.Load(_Posts[position].Url)
						.Transform(new CropSquareTransformation())
						.Into(vh._Image);
				}


			}catch(Exception ex){
				var text = ex.Message;
			}
		}
	}
	public class TagSpan: ReplacementSpan {
		private static float PADDING = 25.0f;
		private RectF mRect;
		private Color mBackgroundColor;
		private Color mForegroundColor;

		public TagSpan(Color backgroundColor, Color foregroundColor) {
			this.mRect = new RectF();
			this.mBackgroundColor = backgroundColor;
			this.mForegroundColor = foregroundColor;
		}
		public override void Draw (Canvas canvas, Java.Lang.ICharSequence text, int start, int end, float x, int top, int y, int bottom, Paint paint)
		{
			mRect.Set(x, top, x + paint.MeasureText(text, start, end) + PADDING, bottom);
			paint.Color = mBackgroundColor;
			canvas.DrawRect(mRect, paint);

			// Text
			paint.Color = mForegroundColor;
			int xPos = (int)Math.Round(x + (PADDING / 2));
			int yPos = (int) ((canvas.Height / 2) - ((paint.Descent() + paint.Ascent()) / 2)) ;
			canvas.DrawText(text, start, end, xPos, yPos, paint);
		}

		public override int GetSize (Paint paint, Java.Lang.ICharSequence text, int start, int end, Paint.FontMetricsInt fm)
		{
			return (int)Math.Round(paint.MeasureText(text, start, end) + PADDING);
		}



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
		public override void OnScrolled (RecyclerView recyclerView, int dx, int dy)
		{
			LinearLayoutManager linearLayoutManager = (LinearLayoutManager)recyclerView.GetLayoutManager ();
			int lastVisibleItem = linearLayoutManager.FindLastVisibleItemPosition();
			int totalItemCount = recyclerView.GetAdapter().ItemCount;

			if (!IsLoading && totalItemCount <= (lastVisibleItem + visibleThreshold)) {
				// End has been reached
				// Do something
				currentPage++;
				IsLoading = true;
				Task.Factory.StartNew (async () => {
					
					try{
						var newPostList = new List<Post>();
						await WebClient.LoadPosts(newPostList,currentPage);


						(recyclerView.GetAdapter()as PostViewAdapter)._Posts.AddRange(newPostList);
						//recyclerView.GetAdapter().HasStableIds = true;


						Application.SynchronizationContext.Post (_ => {recyclerView.GetAdapter().NotifyDataSetChanged();}, null);
						//recyclerView.GetAdapter().NotifyItemRangeInserted(recyclerView.GetAdapter().ItemCount,newPostList.Count);
					}catch(Exception ex){
						var text = ex.Message;
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