using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Pikabu
{
	public class PostTextViewHolder:RecyclerView.ViewHolder
	{
		public TextView AuthorName{ get; private set; }
		public TextView HeaderRating{ get; private set; }
		public TextView BottomRating{ get; private set; }
		public TextView PostTime{ get; private set; }
		public TextView Title{ get; private set; }
		public TextView Description{ get; private set; }
		public TextView Tags{ get; private set; }
		public TextView Text{ get; private set; }

		public TextView Comments{ get; private set; }


		public PostTextViewHolder (View itemView):base(itemView)
		{
			AuthorName = itemView.FindViewById<TextView> (Resource.Id.postUserName);
			HeaderRating = itemView.FindViewById<TextView> (Resource.Id.postHeaderRating);
			BottomRating = itemView.FindViewById<TextView> (Resource.Id.postBottomRating);
			PostTime = itemView.FindViewById<TextView> (Resource.Id.postTime);
			Title = itemView.FindViewById<TextView> (Resource.Id.postHeader);
			Description = itemView.FindViewById<TextView> (Resource.Id.postDescription);
			Text = itemView.FindViewById<TextView> (Resource.Id.postText);
			Comments = itemView.FindViewById<TextView> (Resource.Id.postComments);
			Tags = itemView.FindViewById<TextView> (Resource.Id.postTags);
		}
	}
}

