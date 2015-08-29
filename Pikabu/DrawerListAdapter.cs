using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using Android.Content.Res;

namespace Pikabu
{
	public class DrawerListAdapter:BaseAdapter
	{
		private static Context _Context;
		private DrawerListBadges _badges{ get; set; }
		private static LayoutInflater _inflater=null;
		private String[] _drawerMenuList;
		private TypedArray _drawerMenuImages;
		public DrawerListAdapter (Context context,int drawerMenuList, int drawerMenuImages,DrawerListBadges badges)
		{
			_Context = context;
			_badges = badges;
			//_drawerMenuList = drawerMenuList;
			//_drawerMenuImages = drawerMenuImages;
			_inflater = ( LayoutInflater )context.
				GetSystemService(Context.LayoutInflaterService);
			_drawerMenuList = _Context.Resources.GetStringArray (drawerMenuList);
			_drawerMenuImages = _Context.Resources.ObtainTypedArray (drawerMenuImages);
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var row = _inflater.Inflate (Resource.Layout.DrawerListItemView,parent,false);
			var vh = new DrawerListViewHolder (row);
			vh.Icon.SetImageDrawable (_drawerMenuImages.GetDrawable(position));
			vh.Text.Text = _drawerMenuList [position];
			row.Click+= (object sender, EventArgs e) => {
				Toast.MakeText(_Context,"23523",ToastLength.Short).Show();
			};
			if (position == 2) {
				vh.Badge.Visibility = ViewStates.Invisible;
			}
			return row;
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override int Count {
			get{ return _drawerMenuList.Length; }
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return position;
		}
	}
}

