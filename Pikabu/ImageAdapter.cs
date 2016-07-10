using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Square.Picasso;

namespace Pikabu
{
	public class ImageAdapter : PagerAdapter
	{
		private Context context;
		private List<string> images;

		public ImageAdapter(Context context,List<string> images)
		{
			this.context = context;
			this.images = images;
		}

		public override int Count
		{
			get
			{
				return images.Count;
			}
		}

		public override bool IsViewFromObject(View view, Java.Lang.Object objectValue)
		{
			return view == ((ImageView)objectValue);
		}

		public override Java.Lang.Object InstantiateItem(View container, int position)
		{
			ImageView imageView = new ImageView(context);
			imageView.SetScaleType(ImageView.ScaleType.CenterInside);
			Picasso.With(Android.App.Application.Context)
			       .Load(images[position])
						.Transform(new CropSquareTransformation())
						.Into(imageView);
			((ViewPager)container).AddView(imageView, 0);
			return imageView;
		}

		public override void DestroyItem(View container, int position, Java.Lang.Object objectValue)
		{
			((ViewPager)container).RemoveView((ImageView)objectValue);
		}
	}
}

