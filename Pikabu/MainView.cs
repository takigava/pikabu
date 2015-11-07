using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Xamarin;
using Android.Content;
using Android.Util;
using Android.Preferences;

namespace Pikabu
{
    [Activity (Label = "Горячее", Theme="@style/MyTheme",ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainView : AppCompatActivity
	{
		private SupportToolbar _mToolbar;
		private MyDrawerToggle _mDrawerToggle;
		private DrawerLayout _mDrawerLayout;
		private ListView _mLeftDrawer;

		private ArrayAdapter _mLeftAdapter;

		private List<string> _mLeftDataSet;
		private RecyclerView _recyclerView;

		private List<Post> _posts;
		private PostViewAdapter _adapter;
		private LinearLayout _container;


		//private RecyclerView mRecyclerView;
		private ISharedPreferences pref;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			//ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			// Set our view from the "main" layout resource

			SetContentView (Resource.Layout.MainView);

			pref = PreferenceManager.GetDefaultSharedPreferences(this);

			_mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);

			// Inflate the "decor.xml"
			//LayoutInflater inflater = (LayoutInflater) GetSystemService(Context.LayoutInflaterService);
			//_mDrawerLayout = (DrawerLayout) inflater.Inflate(Resource.Layout.DrawerView, null); // "null" is important.

			var view1 = LayoutInflater.From (this).Inflate (Resource.Layout.DrawerView, null, false);
			// HACK: "steal" the first child of decor view
			_mDrawerLayout = view1.FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			ViewGroup decor = (ViewGroup) Window.DecorView;
			View child = decor.GetChildAt(0);
			decor.RemoveView(child);
			_container = _mDrawerLayout.FindViewById<LinearLayout>(Resource.Id.fragmentContainer);
			var container = (FrameLayout) _mDrawerLayout.FindViewById(Resource.Id.container); // This is the container we defined just now.
			container.AddView(child);

			// Make the drawer replace the first child
			decor.AddView(_mDrawerLayout);

			//_mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			_mLeftDrawer = _mDrawerLayout.FindViewById<ListView>(Resource.Id.left_drawer);
			DisplayMetrics displayMetrics = Resources.DisplayMetrics;
			Android.Views.ViewGroup.LayoutParams lp = (Android.Views.ViewGroup.LayoutParams) _mLeftDrawer.LayoutParameters;
			lp.Height = displayMetrics.HeightPixels;
			_mLeftDrawer.LayoutParameters = lp;
			//_mLeftDrawer.SetMinimumHeight (displayMetrics.HeightPixels+GetStatusBarHeight()+(int)(48*displayMetrics.Density));
			//mToolbar.SystemUiVisibility =
			//	(StatusBarVisibility)Android.Views.SystemUiFlags.LowProfile;

			_mToolbar.SetPadding(0,GetStatusBarHeight (),0,0);
			_mToolbar.LayoutParameters.Height = (int)(52*displayMetrics.Density)+GetStatusBarHeight ();
			SetSupportActionBar(_mToolbar);




			_mDrawerToggle = new MyDrawerToggle(
				this,							//Host Activity
				_mDrawerLayout,					//DrawerLayout
				Resource.String.profile,		//Opened Message
				Resource.String.hot_title		//Closed Message
			);

			_mDrawerLayout.SetDrawerListener(_mDrawerToggle);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayShowTitleEnabled(true);
			SupportActionBar.SetDisplayHomeAsUpEnabled (true);

			//SupportActionBar.NavigationMode = (int)ActionBarNavigationMode.Standard;

			_mDrawerToggle.SyncState();

			//_container = FindViewById<LinearLayout>(Resource.Id.fragmentContainer);


			_posts = new List<Post>();


			var root2 = LayoutInflater.Inflate(Resource.Layout.PostFragmentRecycleView,_container, false);

			var recyclerView = root2.FindViewById<RecyclerView>(Resource.Id.postRecycleView);
			_container.AddView (root2);
			_recyclerView = recyclerView;
			//recyclerView.HasFixedSize = true;
			//recyclerView.SetItemAnimator(new DefaultItemAnimator());
			_recyclerView.SetLayoutManager(new LinearLayoutManager(this));
			//recyclerView.AddItemDecoration(new DividerItemDecoration(Activity, DividerItemDecoration.HorizontalList));


			//mEmails.Add(new Email() { Name = "tom", Subject = "Wanna hang out?", Message = "I'll be around tomorrow!!" });

			var adapter = new PostViewAdapter(_posts,_recyclerView,this);
			_adapter = adapter;
			_adapter.HasStableIds = true;

			_recyclerView.SetAdapter (_adapter);
			_recyclerView.AddOnScrollListener (new MyScrollListener());	

			Task.Factory.StartNew (async () => {
				try{
					var newPostList = new List<Post>();
					var editor = pref.Edit();
					editor.PutString("CurrentPage","0");
					editor.Apply();
					await WebClient.LoadPosts(newPostList,1);
					_posts.AddRange(newPostList);


					//(_RecyclerView.GetAdapter()as PostViewAdapter)._Posts.AddRange(newPostList);

					//_RecyclerView.GetAdapter().NotifyItemRangeInserted(_RecyclerView.GetAdapter().ItemCount,newPostList.Count);
					//recyclerView.GetAdapter().HasStableIds = true;
					//recyclerView.GetAdapter().NotifyDataSetChanged();
					//_RecyclerView.GetAdapter().NotifyDataSetChanged();
					RunOnUiThread(()=>{
						_adapter.NotifyDataSetChanged();
					});
				}
				catch (Exception ex)
				{
				    // ignored
					Insights.Report(ex,new Dictionary<string,string>{{"Message",ex.Message}},Insights.Severity.Error);
					Toast.MakeText(this,ex.Message,ToastLength.Short).Show();
				}
			});





			_mLeftDrawer.Tag = 0;
			//mRightDrawer.Tag = 1;



			int [] prgmImages={Resource.Drawable.ic_camera_64,Resource.Drawable.ic_star_64,Resource.Drawable.ic_comments_64,Resource.Drawable.ic_menu_64};
			String [] prgmNameList={"Сообщения","Избранное","Моя лента","Мои коммнтарии"};
			var drawerAdapter = new DrawerListAdapter (this, Resource.Array.drawerListItems, Resource.Array.drawerListIcons, new DrawerListBadges{ Feed = 0, Messages = 1 });
			_mLeftDrawer.Adapter = drawerAdapter;
			_mLeftDrawer.DividerHeight = 0;
			_mLeftDrawer.AddHeaderView (LayoutInflater.Inflate (Resource.Layout.DrawerListHeader, null),null,false);







		}


		public override bool OnOptionsItemSelected (IMenuItem item)
		{		
			switch (item.ItemId) {
			case Resource.Id.reload:
				Task.Factory.StartNew (async () => {
					try{
						var newPostList = new List<Post>();
						var editor = pref.Edit();
						editor.PutString("CurrentPage","0");
						editor.Apply();
						await WebClient.LoadPosts(newPostList,1);
						_posts.Clear();
						_posts.AddRange(newPostList);


						//(_RecyclerView.GetAdapter()as PostViewAdapter)._Posts.AddRange(newPostList);

						//_RecyclerView.GetAdapter().NotifyItemRangeInserted(_RecyclerView.GetAdapter().ItemCount,newPostList.Count);
						//recyclerView.GetAdapter().HasStableIds = true;
						//recyclerView.GetAdapter().NotifyDataSetChanged();
						//_RecyclerView.GetAdapter().NotifyDataSetChanged();
						RunOnUiThread(()=>{
							_adapter.NotifyDataSetChanged();
						});
					}
					catch (Exception ex)
					{
						// ignored
						Insights.Report(ex,new Dictionary<string,string>{{"Message",ex.Message}},Insights.Severity.Error);
						Toast.MakeText(this,ex.Message,ToastLength.Short).Show();
					}
				});
				break;
			default:
				_mDrawerToggle.OnOptionsItemSelected (item);
				break;
			}
				
				return base.OnOptionsItemSelected (item);

		}





		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.main_view_menu, menu);
			return base.OnCreateOptionsMenu (menu);
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
		    outState.PutString("DrawerState", _mDrawerLayout.IsDrawerOpen((int) GravityFlags.Left) ? "Opened" : "Closed");

		    base.OnSaveInstanceState (outState);
		}

        protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			_mDrawerToggle.SyncState();
		}

		public override void OnConfigurationChanged (Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			_mDrawerToggle.OnConfigurationChanged(newConfig);
			/*if (newConfig.Orientation == Android.Content.Res.Orientation.Landscape) {
				Toast.makeText(this, "landscape", Toast.LENGTH_SHORT).show();
			} else if (newConfig.Orientation == Android.Content.Res.Orientation.Portrait){
				Toast.makeText(this, "portrait", Toast.LENGTH_SHORT).show();
			}*/
			DisplayMetrics displayMetrics = Resources.DisplayMetrics;
			if (_mLeftDrawer != null) {
				
				Android.Views.ViewGroup.LayoutParams lp = (Android.Views.ViewGroup.LayoutParams)_mLeftDrawer.LayoutParameters;
				lp.Height = displayMetrics.HeightPixels;
				_mLeftDrawer.LayoutParameters = lp;
			}
			if (_mToolbar != null) {
				_mToolbar.SetPadding(0,GetStatusBarHeight (),0,0);
				_mToolbar.LayoutParameters.Height = (int)(52*displayMetrics.Density)+GetStatusBarHeight ();
			}
			if (newConfig.Orientation == Android.Content.Res.Orientation.Landscape) {
				Android.Views.ViewGroup.MarginLayoutParams lp = (Android.Views.ViewGroup.MarginLayoutParams)_container.LayoutParameters;
				lp.RightMargin = GetNavBarWidth();
				_container.LayoutParameters = lp;
			} else if (newConfig.Orientation == Android.Content.Res.Orientation.Portrait){
				Android.Views.ViewGroup.MarginLayoutParams lp = (Android.Views.ViewGroup.MarginLayoutParams)_container.LayoutParameters;
				lp.RightMargin = 0;
				_container.LayoutParameters = lp;
			}
		}
		public override void OnBackPressed ()
		{
			if (_mDrawerLayout.IsDrawerOpen ((int)GravityFlags.Left)) {
				_mDrawerLayout.CloseDrawers ();
			} else {
				base.OnBackPressed ();
			}

		}




		public int GetStatusBarHeight() {
			int result = 0;
			int resourceId = Resources.GetIdentifier("status_bar_height", "dimen", "android");
			if (resourceId > 0) {
				result = Resources.GetDimensionPixelSize(resourceId);
			}
			return result;
		}
		public int GetNavBarWidth(){
			int result = 0;
			int resourceId = Resources.GetIdentifier("navigation_bar_width", "dimen", "android");
			if (resourceId > 0) {
				return Resources.GetDimensionPixelSize(resourceId);
			}
			return result;
		}
	}




}



