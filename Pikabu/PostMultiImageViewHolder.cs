using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Syncfusion.Rotator;
using Android.Support.V4.View;

namespace Pikabu
{
	public class PostMultiImageViewHolder : RecyclerView.ViewHolder
	{
		public TextView AuthorName { get; private set; }
		public TextView HeaderRating { get; private set; }
		public TextView BottomRating { get; private set; }
		public TextView PostTime { get; private set; }
		public TextView Title { get; private set; }
		public TextView Description { get; private set; }
		public TextView Tags { get; private set; }
		//public WebView _Url{ get; private set; }
		public TextView Comments { get; private set; }
		//public ImageView Image { get; private set; }
		public ViewPager Rotator { get; private set;}


		public PostMultiImageViewHolder(View itemView) : base(itemView)
		{
			AuthorName = itemView.FindViewById<TextView>(Resource.Id.postUserName);
			HeaderRating = itemView.FindViewById<TextView>(Resource.Id.postHeaderRating);
			BottomRating = itemView.FindViewById<TextView>(Resource.Id.postBottomRating);
			PostTime = itemView.FindViewById<TextView>(Resource.Id.postTime);
			Title = itemView.FindViewById<TextView>(Resource.Id.postHeader);
			Description = itemView.FindViewById<TextView>(Resource.Id.postDescription);
			//Image = itemView.FindViewById<ImageView>(Resource.Id.postImage);
			Comments = itemView.FindViewById<TextView>(Resource.Id.postComments);
			Tags = itemView.FindViewById<TextView>(Resource.Id.postTags);
			Rotator = itemView.FindViewById<ViewPager>(Resource.Id.postMultiImage);
		}
	}
}


