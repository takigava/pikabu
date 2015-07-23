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
using Android.Content.PM;
using ActionBar =  Android.Support.V7.App.ActionBar;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Linq;

namespace Pikabu
{
	[Activity (Label = "Горячее", Theme="@style/MyTheme",ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainView : AppCompatActivity
	{
		private SupportToolbar mToolbar;
		private MyDrawerToggle mDrawerToggle;
		private DrawerLayout mDrawerLayout;
		private ListView mLeftDrawer;

		private ArrayAdapter mLeftAdapter;

		private List<string> mLeftDataSet;
		private RecyclerView _RecyclerView;

		private List<Post> _Posts;
		private PostViewAdapter _Adapter;
		private LinearLayout container;


		//private RecyclerView mRecyclerView;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			//ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
			// Set our view from the "main" layout resource

			SetContentView (Resource.Layout.MainView);



			mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
			mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);

			//mToolbar.SystemUiVisibility =
			//	(StatusBarVisibility)Android.Views.SystemUiFlags.LowProfile;

			SetSupportActionBar(mToolbar);




			mDrawerToggle = new MyDrawerToggle(
				this,							//Host Activity
				mDrawerLayout,					//DrawerLayout
				Resource.String.profile,		//Opened Message
				Resource.String.hot_title		//Closed Message
			);

			mDrawerLayout.SetDrawerListener(mDrawerToggle);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayShowTitleEnabled(true);
			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			//SupportActionBar.NavigationMode = (int)ActionBarNavigationMode.Standard;

			mDrawerToggle.SyncState();

			container = FindViewById<LinearLayout>(Resource.Id.fragmentContainer);


			_Posts = new List<Post>();


			var root2 = LayoutInflater.Inflate(Resource.Layout.PostFragmentRecycleView,container, false);

			var recyclerView = root2.FindViewById<Android.Support.V7.Widget.RecyclerView>(Resource.Id.postRecycleView);
			container.AddView (root2);
			_RecyclerView = recyclerView;
			//recyclerView.HasFixedSize = true;
			//recyclerView.SetItemAnimator(new DefaultItemAnimator());
			_RecyclerView.SetLayoutManager(new LinearLayoutManager(this));
			//recyclerView.AddItemDecoration(new DividerItemDecoration(Activity, DividerItemDecoration.HorizontalList));


			//mEmails.Add(new Email() { Name = "tom", Subject = "Wanna hang out?", Message = "I'll be around tomorrow!!" });

			var adapter = new PostViewAdapter(_Posts,_RecyclerView,this);
			_Adapter = adapter;
			_Adapter.HasStableIds = true;

			_RecyclerView.SetAdapter (_Adapter);
			_RecyclerView.AddOnScrollListener (new MyScrollListener());	

			Task.Factory.StartNew (async () => {
				try{
					var htmlPage = String.Empty;
					htmlPage = await WebClient.GetHot (1);
					HtmlDocument htmlDoc = new HtmlDocument ();
					htmlDoc.OptionFixNestedTags = true;
					htmlDoc.LoadHtml (htmlPage);
					var root = htmlDoc.DocumentNode;
					var commonPosts = root.Descendants().Where(n => n.GetAttributeValue("class", "").Equals("b-story inner_wrap"));

					//_Adapter.NotifyDataSetChanged();
					var newPostList = new List<Post>();
					foreach(var post in commonPosts)
					{
						var newPost = new Post();
						newPost.Id = Int32.Parse(post.Attributes.FirstOrDefault(s=>s.Name=="data-story-id").Value);
						var rating=0;
						var result = Int32.TryParse(post.Descendants().Where(s=>s.GetAttributeValue("class","").Equals("b-rating__count curs")).FirstOrDefault().InnerHtml,out rating);
						newPost.Rating = result==true?rating:0;
						var header = post.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("b-story__main-header"));
						newPost.AuthorName = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("href","").Contains("profile")).InnerHtml;

						newPost.Comments = Int32.Parse(header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("b-link b-link_type_open-story")).InnerHtml.Split(' ')[0]);

						newPost.Title = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("b-story__header-info story_head")).ChildNodes[1].InnerHtml.Replace("\n","").Replace("\t","");

						var desc = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("short"));
						newPost.Description = desc!=null?desc.InnerHtml:String.Empty;
						var textType = post.Descendants().Where(s=>s.GetAttributeValue("id","").Equals("textDiv"+newPost.Id)).FirstOrDefault();
						if(textType!=null){
							newPost.PostType = PostType.Text;
							newPost.Text = textType.InnerText;
						}
						var imageType = post.Descendants().Where(s=>s.GetAttributeValue("id","").Equals("picDiv"+newPost.Id)).FirstOrDefault();
						if(imageType!=null){
							newPost.PostType = PostType.Image;
							var attr = imageType.Descendants().Where(s=>s.GetAttributeValue("src","").Contains("http://")).FirstOrDefault();
							newPost.Url = attr.Attributes["src"].Value;
						}
						newPost.PostTime = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("detailDate")).InnerHtml;
						newPostList.Add(newPost);
						//_Posts.Add(newPost);
						//_RecyclerView.GetAdapter().NotifyItemInserted(_Posts.Count);
					}
					_Posts.AddRange(newPostList);


					//(_RecyclerView.GetAdapter()as PostViewAdapter)._Posts.AddRange(newPostList);

					//_RecyclerView.GetAdapter().NotifyItemRangeInserted(_RecyclerView.GetAdapter().ItemCount,newPostList.Count);
					//recyclerView.GetAdapter().HasStableIds = true;
					//recyclerView.GetAdapter().NotifyDataSetChanged();
					//_RecyclerView.GetAdapter().NotifyDataSetChanged();
					RunOnUiThread(()=>{
						_Adapter.NotifyDataSetChanged();
					});
				}
				catch(Exception ex)
				{
					var text = ex.Message;
				}

			});





			mLeftDrawer.Tag = 0;
			//mRightDrawer.Tag = 1;



			mLeftDataSet = new List<string>();
			mLeftDataSet.Add ("Left Item 1");
			mLeftDataSet.Add ("Left Item 2");
			mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
			mLeftDrawer.Adapter = mLeftAdapter;








		}


		public override bool OnOptionsItemSelected (IMenuItem item)
		{		

				mDrawerToggle.OnOptionsItemSelected(item);
				return base.OnOptionsItemSelected (item);

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




}



