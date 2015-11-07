using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Android.Content;
using System;
using Android.App;

namespace Pikabu
{
	public class MyDrawerToggle : SupportActionBarDrawerToggle
	{
		private readonly AppCompatActivity _mHostActivity;
		private int _mOpenedResource;
		private readonly int _mClosedResource;
		private ISharedPreferences _pref;

		public MyDrawerToggle (AppCompatActivity host, DrawerLayout drawerLayout, int openedResource, int closedResource,ISharedPreferences pref) 
			: base(host, drawerLayout, openedResource, closedResource)
		{
			_mHostActivity = host;
			_mOpenedResource = openedResource;
			_mClosedResource = closedResource;
			_pref = pref;
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
			var currentChanel = _pref.GetString ("CurrentChanel", string.Empty);

			if (!String.IsNullOrEmpty (currentChanel)) {
				switch (Int32.Parse (currentChanel)) {
				case Resource.Id.hotRowLayout:
					//title = Application.Context.GetString (Resource.String.hot_title);
					_mHostActivity.SupportActionBar.SetTitle(Resource.String.hot_title);
					break;
				case Resource.Id.bestRowLayout:
					_mHostActivity.SupportActionBar.SetTitle(Resource.String.best_title);
					break;
				case Resource.Id.newRowLayout:
					_mHostActivity.SupportActionBar.SetTitle(Resource.String.new_title);
					break;
				default:
					break;
				}
			} else {
				_mHostActivity.SupportActionBar.SetTitle(Resource.String.hot_title);
			}

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

