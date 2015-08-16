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
						Color descTextColor = context.Resources.GetColor(Resource.Color.mainGreen);
						Color descTextColorGray = context.Resources.GetColor(Resource.Color.gray);
						Color descBackgroundColor = new Color(255,255,255,0);
						foreach(var des in _Posts[position].FormattedDescription){
							if(des.Item1 == "text"){
								SpannableString tag1 = new SpannableString(des.Item2);
								descriptionBulder.Append(tag1);
								descriptionBulder.SetSpan(new TagSpan(descBackgroundColor, descTextColor), descriptionBulder.Length() - tag1.Length(), descriptionBulder.Length(), SpanTypes.ExclusiveExclusive);
							}
							if(des.Item1 == "textLink"){
								SpannableString tag2 = new SpannableString(des.Item2);
								descriptionBulder.Append(tag2);
								descriptionBulder.SetSpan(new TagSpan(descBackgroundColor, descTextColor), descriptionBulder.Length() - tag2.Length(), descriptionBulder.Length(), SpanTypes.ExclusiveExclusive);
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
			if (holder is PostImageViewHolder) {
				PostImageViewHolder vh = holder as PostImageViewHolder;
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
						Color descTextColor = context.Resources.GetColor(Resource.Color.mainGreen);
						Color descTextColorGray = context.Resources.GetColor(Resource.Color.gray);
						Color descBackgroundColor = new Color(255,255,255,0);
						foreach(var des in _Posts[position].FormattedDescription){
							if(des.Item1 == "text"){
								SpannableString tag1 = new SpannableString(des.Item2);
								descriptionBulder.Append(tag1);
								descriptionBulder.SetSpan(new TagSpan(descBackgroundColor, descTextColor), descriptionBulder.Length() - tag1.Length(), descriptionBulder.Length(), SpanTypes.ExclusiveExclusive);
							}
							if(des.Item1 == "textLink"){
								SpannableString tag2 = new SpannableString(des.Item2);
								descriptionBulder.Append(tag2);
								descriptionBulder.SetSpan(new TagSpan(descBackgroundColor, descTextColor), descriptionBulder.Length() - tag2.Length(), descriptionBulder.Length(), SpanTypes.ExclusiveExclusive);
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
					var htmlPage = String.Empty;
					try{
						htmlPage = await WebClient.GetHot (currentPage);
						HtmlDocument htmlDoc = new HtmlDocument ();
						htmlDoc.OptionFixNestedTags = true;
						htmlDoc.LoadHtml (htmlPage);
						var root = htmlDoc.DocumentNode;
						var commonPosts = root.Descendants().Where(n => n.GetAttributeValue("class", "").Equals("b-story inner_wrap"));

						//_Adapter.NotifyDataSetChanged();
						var newPostList = new List<Post>();
						foreach(var post in commonPosts)
						{
							var newPost = new Post();
							newPost.Id = Int32.Parse(post.Attributes.FirstOrDefault(s=>s.Name=="data-story-id").Value);
							var rating=0;
							var result = Int32.TryParse(post.Descendants().Where(s=>s.GetAttributeValue("class","").Equals("b-rating__count curs")).FirstOrDefault().InnerHtml,out rating);
							newPost.Rating = result==true?rating:0;
							var header = post.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("b-story__main-header"));
							newPost.AuthorName = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("href","").Contains("profile")).InnerHtml;

							newPost.Comments = Int32.Parse(header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("b-link b-link_type_open-story")).InnerHtml.Split(' ')[0]);

							newPost.Title = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("b-story__header-info story_head")).ChildNodes[1].InnerText.Replace("\n","").Replace("\t","").Replace("&quot;","\"");
							var desc = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("short"));
							if(desc!=null){
								//newPost.Description = String.Empty;
								newPost.FormattedDescription = new List<Tuple<string, string>>();
								if(desc.ChildNodes.Count>1){
									
									foreach(var d in desc.ChildNodes){
										if(d.Name=="a"){
											//newPost.FormattedDescription.Add("textLink",d.InnerText);
											//newPost.FormattedDescription.Add("url",
											newPost.FormattedDescription.Add(new Tuple<string, string>("textLink",d.InnerText));
											newPost.FormattedDescription.Add(new Tuple<string, string>("url",d.Attributes["href"].Value));
										}else{
											//newPost.FormattedDescription.Add("text",d.InnerHtml);
											newPost.FormattedDescription.Add(new Tuple<string, string>("text",d.InnerHtml));
										}
										if(d.Name=="#text"){
											newPost.FormattedDescription.Add(new Tuple<string, string>("text",d.InnerHtml));
										}
									}
								}else{
									newPost.FormattedDescription.Add(new Tuple<string, string>("text",desc.InnerHtml));
								}
							}

							newPost.Tags = header.Descendants().Where(s=>s.GetAttributeValue("class","").Equals("tag no_ch")).Select(s=>s.InnerHtml).ToList();
							if(newPost.Tags.Count<=0){
								var test123 = String.Empty;
							}
							//var test = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("detailDate"));
							var textType = post.Descendants().Where(s=>s.GetAttributeValue("id","").Equals("textDiv"+newPost.Id)).FirstOrDefault();
							if(textType!=null){
								newPost.PostType = PostType.Text;
								newPost.Text = textType.InnerText;
							}
							var imageType = post.Descendants().Where(s=>s.GetAttributeValue("id","").Equals("picDiv"+newPost.Id)).FirstOrDefault();
							if(imageType!=null){
								newPost.PostType = PostType.Image;
								var attr = imageType.Descendants().Where(s=>s.GetAttributeValue("src","").Contains("http://")).FirstOrDefault();
								//var image = Picasso.With (Android.App.Application.Context).Load (attr.Attributes["src"].Value).Get ();
								//Bitmap resultImage;
								//if(image.Height>3000 || image.Width>3000)
								//{
								//	int size = Math.Min(image.Width, image.Height);
								//	int x = (image.Width - size) / 3;
								//	int y = (image.Height - size) / 3;
								//	resultImage = Bitmap.CreateBitmap(image, x, y, size, size);
								//	image = resultImage;
								//}
								//newPost.Bitmap = image;
								newPost.Url = attr.Attributes["src"].Value;
							}
							newPost.PostTime = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("detailDate")).InnerHtml;
							newPostList.Add(newPost);
							//(recyclerView.GetAdapter()as PostViewAdapter)._Posts.Add(newPost);
							//(recyclerView.GetAdapter()as PostViewAdapter).NotifyItemInserted((recyclerView.GetAdapter()as PostViewAdapter)._Posts.Count);
						}



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