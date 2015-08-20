using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Square.Picasso;
using Exception = System.Exception;
using Math = System.Math;
using Object = Java.Lang.Object;

namespace Pikabu
{
	public class PostViewAdapter : RecyclerView.Adapter
	{
		
		public List<Post> Posts{ get; set; }
		public RecyclerView RecyclerView { get; set; }
		private readonly Context _context;


		public void SetRecyclerView(RecyclerView recyclerView)
		{
			RecyclerView = recyclerView;
		}

		public PostViewAdapter(List<Post> posts, RecyclerView recyclerView, Context context)
		{
			Posts = posts;
			RecyclerView = recyclerView;
			_context = context;


		}



		public override int GetItemViewType(int position)
		{
			
			switch (Posts [position].PostType) {
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
		    switch (viewType)
		    {
		        case Resource.Layout.PostTextCard:
		        {
		            var row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.PostTextCard, parent, false);
		            var vh = new PostTextViewHolder(row);
		            return vh;
		        }
		        case Resource.Layout.PostImageCard:
		        {
		            var row = LayoutInflater.From (parent.Context).Inflate (Resource.Layout.PostImageCard, parent, false);
		            var vh = new PostImageViewHolder (row);
		            return vh;
		        }
		        default:
		        {
		            var row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.PostTextCard, parent, false);
		            var vh = new PostTextViewHolder(row);
		            return vh;
		        }
		    }
		}

	    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			if (holder is PostTextViewHolder)
			{
				var vh = (PostTextViewHolder) holder;
				FillTextPost (vh, position);
			}
			if (holder is PostImageViewHolder) {
				var vh = (PostImageViewHolder) holder;
				FillImagePost (vh, position);



			}
		}

		public void ShowImage(object sender, EventArgs e){
		}
		public override int ItemCount => Posts.Count;

	    public override long GetItemId (int position)
		{
			return Posts [position].Id;
		}
		public static SpannableString BuildBackgroundColorSpan(SpannableString spannableString,
			string text, string searchString, Color color) {

			var indexOf = text.ToUpperInvariant().IndexOf(searchString.ToUpperInvariant(), StringComparison.Ordinal);

			try {
				spannableString.SetSpan(new BackgroundColorSpan(color), indexOf,
					(indexOf + searchString.Length),SpanTypes.ExclusiveExclusive);
			}
			catch (Exception)
			{
			    // ignored
			}


		    return spannableString;
		}
		public void FillTextPost(PostTextViewHolder vh,int position){
			try{
				vh.AuthorName.Text = Posts[position].AuthorName;
				var context = Application.Context;
				vh.HeaderRating.Text = Posts [position].Rating.ToString ();
				vh.BottomRating.Text = Posts [position].Rating.ToString ();
				if (Posts [position].Rating > 0) {
					vh.HeaderRating.SetTextColor (context.Resources.GetColor(Resource.Color.mainGreen));
					vh.BottomRating.SetTextColor (context.Resources.GetColor(Resource.Color.mainGreen));
					var text = " + "+Posts[position].Rating;
					vh.HeaderRating.Text = text;
					vh.BottomRating.Text = text;
				}
				if (Posts [position].Rating < 0) {
					vh.HeaderRating.SetTextColor (context.Resources.GetColor(Resource.Color.red));
					vh.BottomRating.SetTextColor (context.Resources.GetColor(Resource.Color.red));
					var text = " - "+Posts[position].Rating;
					vh.HeaderRating.Text = text;
					vh.BottomRating.Text = text;
				}
				vh.PostTime.Text = Posts[position].PostTime;
				vh.Title.Text = Posts[position].Title;
				if(Posts[position].FormattedDescription!=null && Posts[position].FormattedDescription.Count>0){
					var descriptionBulder = new SpannableStringBuilder();
					var descTextColorDarkGray = context.Resources.GetColor(Resource.Color.darkGray);
					var descTextColorGray = context.Resources.GetColor(Resource.Color.gray);
					var descBackgroundColor = new Color(255,255,255,0);
					foreach(var des in Posts[position].FormattedDescription){
						if(des.Item1 == "text"){
							SpannableString tag1 = new SpannableString(des.Item2);
							descriptionBulder.Append(tag1);
							descriptionBulder.SetSpan(new TagSpan(descBackgroundColor, descTextColorGray), descriptionBulder.Length() - tag1.Length(), descriptionBulder.Length(), SpanTypes.ExclusiveExclusive);
						}
					    if (des.Item1 != "textLink") continue;
					    var tag2 = new SpannableString(des.Item2);
					    descriptionBulder.Append(tag2);
					    descriptionBulder.SetSpan(new TagSpan(descBackgroundColor, descTextColorDarkGray), descriptionBulder.Length() - tag2.Length(), descriptionBulder.Length(), SpanTypes.ExclusiveExclusive);
					}
					vh.Description.TextFormatted = descriptionBulder;
				}else{
					//vh._Description.Visibility = ViewStates.Invisible;
				}
				//vh._Description.Text = _Posts[position].Description;
				var stringBuilder = new SpannableStringBuilder();
				var textColor = context.Resources.GetColor(Resource.Color.mainGreen);
				var backgroundColor = new Color(255,255,255,0);
				foreach (var tag in Posts[position].Tags) {
					var tag1 = new SpannableString("#"+tag);
					stringBuilder.Append(tag1);
					stringBuilder.SetSpan(new TagSpan(backgroundColor, textColor), stringBuilder.Length() - tag1.Length(), stringBuilder.Length(), SpanTypes.ExclusiveExclusive);

					var tag2 = new SpannableString("");
					stringBuilder.Append(tag2);
					stringBuilder.SetSpan(new TagSpan(backgroundColor, backgroundColor), stringBuilder.Length() - tag2.Length(), stringBuilder.Length(), SpanTypes.ExclusiveExclusive);
				}
				vh.Tags.TextFormatted = stringBuilder;
				vh.Comments.Text = Posts[position].Comments.ToString();
				vh.Text.Text = Posts [position].Text;
			}
			catch (Exception)
			{
			    // ignored
			}
		}

		public void FillImagePost(PostImageViewHolder vh,int position){
			try{
				vh.AuthorName.Text = Posts[position].AuthorName;
				var context = Application.Context;
				vh.HeaderRating.Text = Posts [position].Rating.ToString ();
				vh.BottomRating.Text = Posts [position].Rating.ToString ();
				if (Posts [position].Rating > 0) {
					vh.HeaderRating.SetTextColor (context.Resources.GetColor(Resource.Color.mainGreen));
					vh.BottomRating.SetTextColor (context.Resources.GetColor(Resource.Color.mainGreen));
					var text = " + "+Posts[position].Rating;
					vh.HeaderRating.Text = text;
					vh.BottomRating.Text = text;
				}
				if (Posts [position].Rating < 0) {
					vh.HeaderRating.SetTextColor (context.Resources.GetColor(Resource.Color.red));
					vh.BottomRating.SetTextColor (context.Resources.GetColor(Resource.Color.red));
					var text = " - "+Posts[position].Rating;
					vh.HeaderRating.Text = text;
					vh.BottomRating.Text = text;
				}
				vh.PostTime.Text = Posts[position].PostTime;
				vh.Title.Text = Posts[position].Title;
				if(Posts[position].FormattedDescription!=null && Posts[position].FormattedDescription.Count>0){
					var descriptionBulder = new SpannableStringBuilder();
					var descTextColorDarkGray = context.Resources.GetColor(Resource.Color.darkGray);
					var descTextColorGray = context.Resources.GetColor(Resource.Color.gray);
					var descBackgroundColor = new Color(255,255,255,0);
					foreach(var des in Posts[position].FormattedDescription){
						if(des.Item1 == "text"){
							var tag1 = new SpannableString(des.Item2);
							descriptionBulder.Append(tag1);
							descriptionBulder.SetSpan(new TagSpan(descBackgroundColor, descTextColorGray), descriptionBulder.Length() - tag1.Length(), descriptionBulder.Length(), SpanTypes.ExclusiveExclusive);
						}
					    if (des.Item1 != "textLink") continue;
					    var tag2 = new SpannableString(des.Item2);
					    descriptionBulder.Append(tag2);
					    descriptionBulder.SetSpan(new TagSpan(descBackgroundColor, descTextColorDarkGray), descriptionBulder.Length() - tag2.Length(), descriptionBulder.Length(), SpanTypes.ExclusiveExclusive);
					}
					vh.Description.TextFormatted = descriptionBulder;
				}else{
					//vh._Description.Visibility = ViewStates.Invisible;
				}
				//vh._Description.Text = _Posts[position].Description;
				//vh._Description.Text = _Posts[position].Description;
				var stringBuilder = new SpannableStringBuilder();
				var textColor = context.Resources.GetColor(Resource.Color.mainGreen);
				var backgroundColor = new Color(255,255,255,0);

				foreach (var tag in Posts[position].Tags) {
					var tag1 = new SpannableString("#"+tag);
					stringBuilder.Append(tag1);
					stringBuilder.SetSpan(new TagSpan(backgroundColor, textColor), stringBuilder.Length() - tag1.Length(), stringBuilder.Length(), SpanTypes.ExclusiveExclusive);

					var tag2 = new SpannableString("");
					stringBuilder.Append(tag2);
					stringBuilder.SetSpan(new TagSpan(backgroundColor, backgroundColor), stringBuilder.Length() - tag2.Length(), stringBuilder.Length(), SpanTypes.ExclusiveExclusive);
				}
				vh.Tags.TextFormatted = stringBuilder;
				vh.Comments.Text = Posts[position].Comments.ToString();
			    if (vh.Image == null) return;
                
			    vh.Image.SetTag (Resource.String.currentPosition, vh.AdapterPosition.ToString ());
			    vh.Image.SetTag (Resource.String.imageUrl, Posts [vh.AdapterPosition].Url);
			    EventHandler handler = (sender, e) => {
			                                              var image = sender as ImageView;
			                                              //Rivets.AppLinks.Navigator.Navigate("http://static.kremlin.ru/media/events/video/ru/video_high/QVrS7FkL9R89xxB9frxsAtnvu9ANx6Od.mp4");
			                                              var intent = new Intent (_context, typeof(ImageViewer));
			                                              if (image != null)
			                                                  intent.PutExtra ("url", image.GetTag (Resource.String.imageUrl).ToString ());
			                                              _context.StartActivity (intent);




			    };
			    vh.Image.Click -= handler;
			    vh.Image.Click += handler;

                Picasso.With(Application.Context)
			        .Load(Posts[position].Url)
			        .Transform(new CropSquareTransformation())
			        .Into(vh.Image);
			}
			catch (Exception)
			{
			    // ignored
			}
		}
	}
	public class TagSpan: ReplacementSpan {
		private static readonly float _padding = 25.0f;
		private readonly RectF _mRect;
		private readonly Color _mBackgroundColor;
		private readonly Color _mForegroundColor;

		public TagSpan(Color backgroundColor, Color foregroundColor) {
			_mRect = new RectF();
			_mBackgroundColor = backgroundColor;
			_mForegroundColor = foregroundColor;
		}
		public override void Draw (Canvas canvas, ICharSequence text, int start, int end, float x, int top, int y, int bottom, Paint paint)
		{
			_mRect.Set(x, top, x + paint.MeasureText(text, start, end) + _padding, bottom);
			paint.Color = _mBackgroundColor;
			canvas.DrawRect(_mRect, paint);

			// Text
			paint.Color = _mForegroundColor;
			var xPos = (int)Math.Round(x + (_padding / 2));
		    // ReSharper disable once PossibleLossOfFraction
			var yPos = (int) ((canvas.Height / 2) - ((paint.Descent() + paint.Ascent()) / 2)) ;
			canvas.DrawText(text, start, end, xPos, yPos, paint);
		}

		public override int GetSize (Paint paint, ICharSequence text, int start, int end, Paint.FontMetricsInt fm)
		{
			return (int)Math.Round(paint.MeasureText(text, start, end) + _padding);
		}



	}
	public class CropSquareTransformation : Object, ITransformation
	{
		public Bitmap Transform(Bitmap source)
		{
			var result = source;
		    if (source.Height <= 3000 && source.Width <= 3000) return result;
		    var size = Math.Min(source.Width, source.Height);
		    var x = (source.Width - size) / 3;
		    var y = (source.Height - size) / 3;
		    result = Bitmap.CreateBitmap(source, x, y, size, size);
		    if (result != source) {
		        source.Recycle();
		    }
		    return result;
		}

		public string Key => "square()";
	}
	
	public class MyScrollListener 
		: RecyclerView.OnScrollListener
	{
	    private const int VisibleThreshold = 6;
	    private int _lastVisibleItem, _totalItemCount;
		private bool _isLoading;
		private int _currentPage=1;
		public override void OnScrolled (RecyclerView recyclerView, int dx, int dy)
		{
			var linearLayoutManager = (LinearLayoutManager)recyclerView.GetLayoutManager ();
            _lastVisibleItem = linearLayoutManager.FindLastVisibleItemPosition();
            _totalItemCount = recyclerView.GetAdapter().ItemCount;

		    if (_isLoading || _totalItemCount > (_lastVisibleItem + VisibleThreshold)) return;
		    // End has been reached
		    // Do something
		    _currentPage++;
		    _isLoading = true;
		    Task.Factory.StartNew (async () => {
					
		                                           try{
		                                               var newPostList = new List<Post>();
		                                               await WebClient.LoadPosts(newPostList,_currentPage);


		                                               var postViewAdapter = recyclerView.GetAdapter()as PostViewAdapter;
		                                               if (postViewAdapter != null)
		                                                   postViewAdapter.Posts.AddRange(newPostList);
		                                               //recyclerView.GetAdapter().HasStableIds = true;


		                                               Application.SynchronizationContext.Post (_ => {recyclerView.GetAdapter().NotifyDataSetChanged();}, null);
		                                               //recyclerView.GetAdapter().NotifyItemRangeInserted(recyclerView.GetAdapter().ItemCount,newPostList.Count);
		                                           }
		                                           catch (Exception)
		                                           {
		                                               // ignored
		                                           }


		                                           _isLoading = false;
		    });
		}
		public override void OnScrollStateChanged (RecyclerView recyclerView, int newState)
		{
			var picasso = Picasso.With(Application.Context);
			//if ((Android.Widget.ScrollState)recyclerView.ScrollState != ScrollState.Idle || (Android.Widget.ScrollState)recyclerView.ScrollState != ScrollState.TouchScroll) {
			//	picasso.ResumeTag(Android.App.Application.Context);
			//} else {
			//	picasso.PauseTag(Android.App.Application.Context);
			//}
			switch (newState) {
			case RecyclerView.ScrollStateIdle:
				picasso.ResumeTag(Application.Context);
				break;
			case RecyclerView.ScrollStateDragging:
				picasso.PauseTag(Application.Context);
				break;
			case RecyclerView.ScrollStateSettling:
				picasso.PauseTag(Application.Context);
				break;
			}

		}


	}

}