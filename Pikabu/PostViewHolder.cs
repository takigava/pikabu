using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using System.Collections.Generic;

namespace Pikabu
{
	public class PostViewHolder:RecyclerView.ViewHolder
	{
		public TextView AuthorName{ get; private set; }
		public TextView Rating{ get; private set; }
		public TextView PostTime{ get; private set; }
		public TextView Title{ get; private set; }
		public TextView Description{ get; private set; }
		public List<string> Tags{ get; private set; }
		public TextView Text{ get; private set; }
		public WebView Url{ get; private set; }
		public TextView Comments{ get; private set; }
		public ImageView Image { get; private set; }

		public PostViewHolder (View itemView):base(itemView)
		{
		}
	}
}

