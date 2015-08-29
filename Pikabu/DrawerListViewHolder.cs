using System;
using Android.Widget;
using Android.Views;

namespace Pikabu
{
	public class DrawerListViewHolder
	{
		public ImageView Icon{ get; set;}
		public TextView Text{ get; set;}
		public TextView Badge{ get; set;}
		public DrawerListViewHolder (View itemView)
		{
			Icon = itemView.FindViewById<ImageView> (Resource.Id.drawerListIcon);
			Text = itemView.FindViewById<TextView> (Resource.Id.drawerListText);
			Badge = itemView.FindViewById<TextView> (Resource.Id.drawerListBadge);
		}
	}
}

