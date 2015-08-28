using System;
using System.Collections.Generic;

namespace Pikabu
{
	public enum PostType
	{
		
		Image =0,
		Text =1,
		Gif =2,
		Video =3,
		Coub =4
	}
	public class Post
	{
		public int Id{ get; set; }
		public string AuthorName{ get; set; }
		public int Rating{ get; set; }
		public string PostTime{ get; set; }
		public string Title{ get; set; }
		public string Description{ get; set; }
		public List<string> Tags{ get; set; }
		public PostType PostType{ get; set; }
		public string Text{ get; set; }
		public string Url{ get; set; }
		public int Comments{ get; set; }
		public Android.Graphics.Bitmap Bitmap{ get; set; }
		public List<Tuple<string,string>> FormattedDescription{ get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
		public string GifUrl { get; set; }
		public string VideoUrl{ get; set; }
	}
}

