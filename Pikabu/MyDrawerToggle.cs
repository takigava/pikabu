using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;

namespace Pikabu
{
	public class MyDrawerToggle : SupportActionBarDrawerToggle
	{
		private readonly AppCompatActivity _mHostActivity;
		private int _mOpenedResource;
		private readonly int _mClosedResource;

		public MyDrawerToggle (AppCompatActivity host, DrawerLayout drawerLayout, int openedResource, int closedResource) 
			: base(host, drawerLayout, openedResource, closedResource)
		{
			_mHostActivity = host;
			_mOpenedResource = openedResource;
			_mClosedResource = closedResource;
		}
		public void SetOpenedMessage(int openedResource)
		{
			_mOpenedResource = openedResource;
		}

		public override void OnDrawerOpened (View drawerView)
		{	
			var drawerType = (int)drawerView.Tag;

		    if (drawerType != 0) return;
		    base.OnDrawerOpened (drawerView);
		    _mHostActivity.SupportActionBar.SetTitle(_mOpenedResource);
		}

		public override void OnDrawerClosed (View drawerView)
		{
			var drawerType = (int)drawerView.Tag;

		    if (drawerType != 0) return;
		    base.OnDrawerClosed (drawerView);
		    _mHostActivity.SupportActionBar.SetTitle(_mClosedResource);
		}

		public override void OnDrawerSlide (View drawerView, float slideOffset)
		{
			var drawerType = (int)drawerView.Tag;

			if (drawerType == 0)
			{
				base.OnDrawerSlide (drawerView, slideOffset);
			}
		}
	}
}

