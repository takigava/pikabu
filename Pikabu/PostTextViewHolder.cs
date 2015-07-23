using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using System.Collections.Generic;

namespace Pikabu
{
	public class PostTextViewHolder:Android.Support.V7.Widget.RecyclerView.ViewHolder
	{
		public TextView _AuthorName{ get; private set; }
		public TextView _HeaderRating{ get; private set; }
		public TextView _BottomRating{ get; private set; }
		public TextView _PostTime{ get; private set; }
		public TextView _Title{ get; private set; }
		public TextView _Description{ get; private set; }
		public List<string> _Tags{ get; private set; }
		public TextView _Text{ get; private set; }

		public TextView _Comments{ get; private set; }


		public PostTextViewHolder (View itemView):base(itemView)
		{
			_AuthorName = itemView.FindViewById<TextView> (Resource.Id.postUserName);
			_HeaderRating = itemView.FindViewById<TextView> (Resource.Id.postHeaderRating);
			_BottomRating = itemView.FindViewById<TextView> (Resource.Id.postBottomRating);
			_PostTime = itemView.FindViewById<TextView> (Resource.Id.postTime);
			_Title = itemView.FindViewById<TextView> (Resource.Id.postHeader);
			_Description = itemView.FindViewById<TextView> (Resource.Id.postDescription);
			_Text = itemView.FindViewById<TextView> (Resource.Id.postText);
			_Comments = itemView.FindViewById<TextView> (Resource.Id.postComments);
		}
	}
}

