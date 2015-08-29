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


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			//ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			// Set our view from the "main" layout resource

			SetContentView (Resource.Layout.MainView);



			_mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
			_mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			_mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);

			//mToolbar.SystemUiVisibility =
			//	(StatusBarVisibility)Android.Views.SystemUiFlags.LowProfile;

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

			_container = FindViewById<LinearLayout>(Resource.Id.fragmentContainer);


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
			var drawerAdapter = new DrawerListAdapter (this, Resource.Array.drawerListItems, Resource.Array.drawerListIcons, new DrawerListBadges{ Feed = 0, Messages = 0 });
			_mLeftDrawer.Adapter = drawerAdapter;
			_mLeftDrawer.DividerHeight = 0;
			_mLeftDrawer.AddHeaderView (LayoutInflater.Inflate (Resource.Layout.DrawerListHeader, null),null,false);







		}


		public override bool OnOptionsItemSelected (IMenuItem item)
		{		

				_mDrawerToggle.OnOptionsItemSelected(item);
				return base.OnOptionsItemSelected (item);

		}





		//public override bool OnCreateOptionsMenu (IMenu menu)
		//{
		//	//MenuInflater.Inflate (Resource.Menu.action_menu, menu);
		//	return base.OnCreateOptionsMenu (menu);
		//}

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
		}
	}




}



