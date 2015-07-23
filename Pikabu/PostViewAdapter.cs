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
using Android.Support.V7.Widget;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Square.Picasso;
using Android.Graphics;

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
				vh._AuthorName.Text = _Posts[position].AuthorName;
				vh._HeaderRating.Text = _Posts[position].Rating.ToString();
				vh._BottomRating.Text = _Posts[position].Rating.ToString();
				vh._PostTime.Text = _Posts[position].PostTime;
				vh._Title.Text = _Posts[position].Title;
				vh._Description.Text = _Posts[position].Description;

					vh._Text.Text = _Posts [position].Text;


				vh._Comments.Text = _Posts[position].Comments.ToString();
			}
			if (holder is PostImageViewHolder) {
				PostImageViewHolder vh = holder as PostImageViewHolder;
				vh._AuthorName.Text = _Posts[position].AuthorName;
				vh._HeaderRating.Text = _Posts[position].Rating.ToString();
				vh._BottomRating.Text = _Posts[position].Rating.ToString();
				vh._PostTime.Text = _Posts[position].PostTime;
				vh._Title.Text = _Posts[position].Title;
				vh._Description.Text = _Posts[position].Description;

				vh._Comments.Text = _Posts[position].Comments.ToString();
				Picasso.With(Android.App.Application.Context)
					.Load(_Posts[position].Url)
					.Transform(new CropSquareTransformation())
					.Into(vh._Image);
			}
		}


		public override int ItemCount
		{
			get { return _Posts.Count; }
		}
		public override long GetItemId (int position)
		{
			return _Posts [position].Id;
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

							newPost.Title = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("b-story__header-info story_head")).ChildNodes[1].InnerHtml.Replace("\n","").Replace("\t","");
							var desc = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("short"));
							newPost.Description = desc!=null?desc.InnerHtml:String.Empty;
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
						recyclerView.GetAdapter().NotifyDataSetChanged();

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