using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace Pikabu
{
	public class PostViewAdapter : RecyclerView.Adapter
	{
		private List<Post> _Posts;
		public RecyclerView _RecyclerView { get; set; }
		private Context _Context;
		private int _CurrentPosition = -1;

		public void SetRecyclerView(RecyclerView recyclerView)
		{
			_RecyclerView = recyclerView;
		}

		public PostViewAdapter(List<Post> posts, RecyclerView recyclerView, Context context)
		{
			_Posts = posts;
			_RecyclerView = recyclerView;
			_Context = context;
		}

		public class PostView : RecyclerView.ViewHolder
		{
			public View _PostView { get; set; }
			public TextView mName { get; set; }
			public TextView mSubject { get; set; }
			public TextView mMessage { get; set; }

			public PostView (View view) : base(view)
			{
				_PostView = view;
			}
		}

		

		public override int GetItemViewType(int position)
		{
			return Resource.Layout.PostCard;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			if (viewType == Resource.Layout.PostCard)
			{
				//First card view
				View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.PostCard, parent, false);

				TextView txtName = row.FindViewById<TextView>(Resource.Id.txtName);
				TextView txtSubject = row.FindViewById<TextView>(Resource.Id.txtSubject);
				TextView txtMessage = row.FindViewById<TextView>(Resource.Id.txtMessage);

				PostView view = new PostView(row) { mName = txtName, mSubject = txtSubject, mMessage = txtMessage };
				return view;
			}
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			if (holder is PostView)
			{
				//First view
				PostView myHolder = holder as PostView;
				myHolder.mMainView.Click += mMainView_Click;
				myHolder.mName.Text = mEmails[position].Name;
				myHolder.mSubject.Text = mEmails[position].Subject;
				myHolder.mMessage.Text = mEmails[position].Message;

				if (position > _CurrentPosition)
				{
					
					_CurrentPosition = position;
				}
			}

		}

		void mMainView_Click(object sender, EventArgs e)
		{
			int position = _RecyclerView.GetChildPosition((View)sender);
		}

		public override int ItemCount
		{
			get { return _Posts.Count; }
		}
	}
}