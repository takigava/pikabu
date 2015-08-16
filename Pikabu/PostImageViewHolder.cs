using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace Pikabu
{
	public class PostImageViewHolder:Android.Support.V7.Widget.RecyclerView.ViewHolder
	{
		public TextView _AuthorName{ get; private set; }
		public TextView _HeaderRating{ get; private set; }
		public TextView _BottomRating{ get; private set; }
		public TextView _PostTime{ get; private set; }
		public TextView _Title{ get; private set; }
		public TextView _Description{ get; private set; }
		public TextView _Tags{ get; private set; }
		public TextView _Text{ get; private set; }
		//public WebView _Url{ get; private set; }
		public TextView _Comments{ get; private set; }
		public ImageView _Image { get; private set; }

		public PostImageViewHolder (View itemView):base(itemView)
		{
			_AuthorName = itemView.FindViewById<TextView> (Resource.Id.postUserName);
			_HeaderRating = itemView.FindViewById<TextView> (Resource.Id.postHeaderRating);
			_BottomRating = itemView.FindViewById<TextView> (Resource.Id.postBottomRating);
			_PostTime = itemView.FindViewById<TextView> (Resource.Id.postTime);
			_Title = itemView.FindViewById<TextView> (Resource.Id.postHeader);
			_Description = itemView.FindViewById<TextView> (Resource.Id.postDescription);
			_Image = itemView.FindViewById<ImageView> (Resource.Id.postImage);
			_Comments = itemView.FindViewById<TextView> (Resource.Id.postComments);
			_Tags = itemView.FindViewById<TextView> (Resource.Id.postTags);
		}
	}
}

