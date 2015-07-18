using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using System.Collections.Generic;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.Animation;
//using RecyclerViewTutorial;

namespace Pikabu
{
	[Activity (Label = "Hot", Theme="@style/MyTheme")]
	public class MainView : ActionBarActivity
	{
		private SupportToolbar mToolbar;
		private DrawerToggle mDrawerToggle;
		private DrawerLayout mDrawerLayout;
		private ListView mLeftDrawer;
		private ListView mRightDrawer;
		private ArrayAdapter mLeftAdapter;
		private ArrayAdapter mRightAdapter;
		private List<string> mLeftDataSet;
		private List<string> mRightDataSet;
		private FrameLayout mFragmentContainer;
		private SupportFragment mCurrentFragment;
		private RecyclerViewFragment mFragment1;
		//private Fragment2 mFragment2;
		//private Fragment3 mFragment3;
		private Stack<SupportFragment> mStackFragments;

		private List<Post> mEmails;
		private PostViewAdapter mAdapter;


		//private RecyclerView mRecyclerView;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.MainView);

			mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
			mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);

			mFragmentContainer = FindViewById<FrameLayout>(Resource.Id.fragmentContainer);

			//mEmails = new List<Email>();
			//var recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
			//mAdapter = new RecyclerViewAdapter(mEmails, recyclerView, this);

			mFragment1 = new RecyclerViewFragment(mEmails,mAdapter);
			//mFragment2 = new Fragment2();
			// = new Fragment3();



			mStackFragments = new Stack<SupportFragment>();

			var trans = SupportFragmentManager.BeginTransaction();
			//trans.Add(Resource.Id.fragmentContainer, mFragment3, "Fragment3");
			//trans.Hide(mFragment3);

			//trans.Add(Resource.Id.fragmentContainer, mFragment2, "Fragment2");
			//trans.Hide(mFragment2);

			trans.Add(Resource.Id.fragmentContainer, mFragment1, "Fragment1");
			trans.Commit();

			mCurrentFragment = mFragment1;



			mLeftDrawer.Tag = 0;
			mRightDrawer.Tag = 1;

			SetSupportActionBar(mToolbar);

			mLeftDataSet = new List<string>();
			mLeftDataSet.Add ("Left Item 1");
			mLeftDataSet.Add ("Left Item 2");
			mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
			mLeftDrawer.Adapter = mLeftAdapter;

			mRightDataSet = new List<string>();
			mRightDataSet.Add ("Right Item 1");
			mRightDataSet.Add ("Right Item 2");
			mRightAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mRightDataSet);
			mRightDrawer.Adapter = mRightAdapter;

			mDrawerToggle = new DrawerToggle(
				this,							//Host Activity
				mDrawerLayout,					//DrawerLayout
				Resource.String.app_name,		//Opened Message
				Resource.String.host		//Closed Message
			);

			mDrawerLayout.SetDrawerListener(mDrawerToggle);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayShowTitleEnabled(true);
			mDrawerToggle.SyncState();

			if (bundle != null)
			{
				if (bundle.GetString("DrawerState") == "Opened")
				{
					SupportActionBar.SetTitle(Resource.String.app_name);
				}

				else
				{
					SupportActionBar.SetTitle(Resource.String.host);
				}
			}

			else
			{
				//This is the first the time the activity is ran
				SupportActionBar.SetTitle(Resource.String.host);
			}


		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{		
			switch (item.ItemId)
			{

			case Android.Resource.Id.Home:
				//The hamburger icon was clicked which means the drawer toggle will handle the event
				//all we need to do is ensure the right drawer is closed so the don't overlap
				mDrawerLayout.CloseDrawer (mRightDrawer);
				mDrawerToggle.OnOptionsItemSelected(item);
				return true;

			
			

			

			default:
				return base.OnOptionsItemSelected (item);
			}
		}

		private void ShowFragment (SupportFragment fragment)
		{
			if (fragment.IsVisible){
				return;
			}

			var trans = SupportFragmentManager.BeginTransaction();

			//trans.SetCustomAnimations(Resource.Animation.slide_in, Resource.Animation.slide_out, Resource.Animation.slide_in, Resource.Animation.slide_out);

			fragment.View.BringToFront();
			mCurrentFragment.View.BringToFront();

			trans.Hide(mCurrentFragment);
			trans.Show(fragment);

			trans.AddToBackStack(null);
			mStackFragments.Push(mCurrentFragment);
			trans.Commit();

			mCurrentFragment = fragment;

		}

		public override void OnBackPressed ()
		{

			if (SupportFragmentManager.BackStackEntryCount > 0)
			{
				SupportFragmentManager.PopBackStack();
				mCurrentFragment = mStackFragments.Pop();
			}

			else
			{
				base.OnBackPressed();
			}				
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			//MenuInflater.Inflate (Resource.Menu.action_menu, menu);
			return base.OnCreateOptionsMenu (menu);
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
			{
				outState.PutString("DrawerState", "Opened");
			}

			else
			{
				outState.PutString("DrawerState", "Closed");
			}

			base.OnSaveInstanceState (outState);
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			mDrawerToggle.SyncState();
		}

		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			mDrawerToggle.OnConfigurationChanged(newConfig);
		}
	}

	public class RecyclerViewFragment : Android.Support.V4.App.Fragment
	{
		private List<Post> mEmails;
		private PostViewAdapter mAdapter;
		private RecyclerView mRecyclerView;

		public RecyclerViewFragment(List<Post> emails,PostViewAdapter adapter)
		{
			mEmails = emails;
			mAdapter = adapter;
		}
		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			//var root = inflater.Inflate(Resource.Layout.fragment_recyclerview, container, false);

			//var recyclerView = root.FindViewById<RecyclerView>(Resource.Id.recycler_view);
			//mRecyclerView = recyclerView;
			//recyclerView.HasFixedSize = true;
			//recyclerView.SetItemAnimator(new DefaultItemAnimator());
			//recyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
			//recyclerView.AddItemDecoration(new DividerItemDecoration(Activity, DividerItemDecoration.HorizontalList));


			//mEmails.Add(new Email() { Name = "tom", Subject = "Wanna hang out?", Message = "I'll be around tomorrow!!" });

			//var adapter = new RecyclerViewAdapter(Activity, Resources.GetStringArray(Resource.Array.countries));
			//mAdapter = new RecyclerViewAdapter(mEmails, recyclerView, Activity);
			//recyclerView.SetAdapter(adapter);
			//mAdapter.SetRecyclerView(recyclerView);
			//recyclerView.SetAdapter(mAdapter);

			//var fab = root.FindViewById<FloatingActionButton>(Resource.Id.fab);
			//fab.AttachToRecyclerView(recyclerView, this);
			//fab.Size = FabSize.Mini;

			//fab.Click += (sender, args) =>
			//{
			//	Toast.MakeText(Activity, "FAB Clicked!", ToastLength.Short).Show();
			//};
			//var client = new RestClient ("http://zerx.co/engine/ajax/");
			//var request = new RestRequest("admin_series_list.php?{id}", Method.GET);
			//request.AddUrlSegment("id", "52186");
			//request.RequestFormat = DataFormat.Json;
			//var response = client.Execute(request);
			//var content = response.Content;
			//return root;
		}


	}


}



