using System;
using System.Net;
using Android.App;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;


namespace Pikabu
{
	public static class WebClient
	{
		public static CookieContainer CookieContainer = null;

		public static void Initialize()
		{
			CookieContainer = new CookieContainer ();
		}

		public static async Task<LoginResponse> Authorize(LoginInfo loginInfo)
		{
			LoginResponse result = null;
			var context = Android.App.Application.Context;
			var uri = new Uri (context.GetString (Resource.String.origin));
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (uri);
			request.ContentType = "text/html";
			request.Method = "GET";
			request.Timeout = 3000;
			request.CookieContainer = CookieContainer;

			using (var response = await request.GetResponseAsync ()) {
				CookieContainer = ((HttpWebRequest)request).CookieContainer;
			}


			var loginUri = new Uri (context.GetString (Resource.String.origin) + context.GetString (Resource.String.loginPath));
			HttpWebRequest loginRequest = (HttpWebRequest)HttpWebRequest.Create (loginUri);
			loginRequest.CookieContainer = CookieContainer;
			loginRequest.Method = "POST";
			loginRequest.Timeout = 3000;

			var sb = new StringBuilder();
			sb.Append ("mode="+loginInfo.mode);
			sb.Append ("&");
			sb.Append ("username=" + WebUtility.UrlEncode(loginInfo.username));
			sb.Append ("&");
			sb.Append ("password=" + loginInfo.password);
			sb.Append ("&");
			sb.Append ("remember=" + loginInfo.remember);
			//var loginString = WebUtility.UrlEncode(sb.ToString());
			var loginBytes = Encoding.UTF8.GetBytes(sb.ToString());
			var phpSesion = CookieContainer.GetCookies (new Uri (context.GetString (Resource.String.origin)))["PHPSESS"].Value;

			loginRequest.Headers.Add("Accept-Language","en-US,en;q=0.8");
			loginRequest.Headers.Add("X-Requested-With","XMLHttpRequest");
			loginRequest.Headers.Add("X-Csrf-Token",phpSesion);
			loginRequest.Accept = "application/json, text/javascript, */*; q=0.01";
			loginRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
			loginRequest.Headers.Add("Origin",context.GetString (Resource.String.origin));
			loginRequest.Host = context.GetString (Resource.String.host);
			loginRequest.Referer = context.GetString (Resource.String.origin);
			loginRequest.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36";
			loginRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

			Stream writeStream = loginRequest.GetRequestStream ();

			writeStream.Write(loginBytes,0,loginBytes.Length);
			writeStream.Flush();
			writeStream.Close();

			using (var loginResponse = await loginRequest.GetResponseAsync ()) {
				CookieContainer = ((HttpWebRequest)loginRequest).CookieContainer;
				using (var reader = new StreamReader(loginResponse.GetResponseStream())) {
					var responseResult = reader.ReadToEnd();
					result = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse>(responseResult);
				}
			}

			return result;
		}

		public static async Task<string> GetHot(int page)
		{
			var result = String.Empty;
			var context = Android.App.Application.Context;
			var uri = new Uri (context.GetString (Resource.String.hot_url)+page);

			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (uri);
			request.Headers.Add("Accept-Language","en-US,en;q=0.8");
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
			request.ContentType = "text/html; charset=windows-1251";
			request.Method = "GET";
			request.Timeout = 3000;
			request.CookieContainer = CookieContainer;
			request.Host = context.GetString (Resource.String.host);
			request.Referer = context.GetString (Resource.String.origin)+"/hot";
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

			using (var response = await request.GetResponseAsync ()) {
				CookieContainer = ((HttpWebRequest)request).CookieContainer;
				using (var reader = new StreamReader(response.GetResponseStream(),Encoding.GetEncoding(1251))) {
					var responseResult = reader.ReadToEnd();
					Encoding utf8 = Encoding.GetEncoding("UTF-8");
					Encoding win1251 = Encoding.GetEncoding("Windows-1251");

					byte[] win1251Bytes = win1251.GetBytes(responseResult);
					byte[] utf8Bytes = Encoding.Convert(win1251, utf8, win1251Bytes);
					//result = utf8.GetString(utf8Bytes);
					result = responseResult;
				}
			}
			return result;
		}

		public static async Task LoadPosts(List<Post> newPostList,int currentPage){
			var htmlPage = String.Empty;
			htmlPage = await WebClient.GetHot (currentPage);
			HtmlDocument htmlDoc = new HtmlDocument ();
			htmlDoc.OptionFixNestedTags = true;
			htmlDoc.LoadHtml (htmlPage);
			var root = htmlDoc.DocumentNode;
			var commonPosts = root.Descendants().Where(n => n.GetAttributeValue("class", "").Equals("b-story inner_wrap"));

			//_Adapter.NotifyDataSetChanged();

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

				newPost.Title = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("b-story__header-info story_head")).ChildNodes[1].InnerText.Replace("\n","").Replace("\t","").Replace("&quot;","\"");
				var desc = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("short"));
				if(desc!=null){
					//newPost.Description = String.Empty;
					newPost.Description = desc.InnerText;
					newPost.FormattedDescription = new List<Tuple<string, string>>();
					if(desc.ChildNodes.Count>1){

						foreach(var d in desc.ChildNodes){
							if(d.Name=="a"){
								//newPost.FormattedDescription.Add("textLink",d.InnerText);
								//newPost.FormattedDescription.Add("url",
								newPost.FormattedDescription.Add(new Tuple<string, string>("textLink",d.InnerText));
								newPost.FormattedDescription.Add(new Tuple<string, string>("url",d.Attributes["href"].Value));
							}else{
								//newPost.FormattedDescription.Add("text",d.InnerHtml);
								newPost.FormattedDescription.Add(new Tuple<string, string>("text",d.InnerText));
							}
							if(d.Name=="#text"){
								newPost.FormattedDescription.Add(new Tuple<string, string>("text",d.InnerText));
							}
						}
					}else{
						newPost.FormattedDescription.Add(new Tuple<string, string>("text",desc.InnerText));
					}
				}

				newPost.Tags = header.Descendants().Where(s=>s.GetAttributeValue("class","").Equals("tag no_ch")).Select(s=>s.InnerHtml).ToList();
				if(newPost.Tags.Count<=0){
					var test123 = String.Empty;
				}
				//var test = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("detailDate"));
				var textType = post.Descendants().Where(s=>s.GetAttributeValue("id","").Equals("textDiv"+newPost.Id)).FirstOrDefault();
				if(textType!=null){
					newPost.PostType = PostType.Text;
					newPost.Text = textType.InnerText.Replace("\n\t\t\t\t\t","");
				}
				var imageType = post.Descendants().Where(s=>s.GetAttributeValue("id","").Equals("picDiv"+newPost.Id)).FirstOrDefault();
				if(imageType!=null){
					newPost.PostType = PostType.Image;
					var attr = imageType.Descendants().Where(s=>s.GetAttributeValue("src","").Contains("http://")).FirstOrDefault();
					//var image = Picasso.With (Android.App.Application.Context).Load (attr.Attributes["src"].Value).Get ();
					//Bitmap resultImage;
					//if(image.Height>3000 || image.Width>3000)
					//{
					//	int size = Math.Min(image.Width, image.Height);
					//	int x = (image.Width - size) / 3;
					//	int y = (image.Height - size) / 3;
					//	resultImage = Bitmap.CreateBitmap(image, x, y, size, size);
					//	image = resultImage;
					//}
					//newPost.Bitmap = image;
					newPost.Url = attr.Attributes["src"].Value;
				}
				newPost.PostTime = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("detailDate")).InnerHtml;
				newPostList.Add(newPost);
				//(recyclerView.GetAdapter()as PostViewAdapter)._Posts.Add(newPost);
				//(recyclerView.GetAdapter()as PostViewAdapter).NotifyItemInserted((recyclerView.GetAdapter()as PostViewAdapter)._Posts.Count);
			}

		}
	}

	public class LoginInfo
	{
		public string mode{ get; set; }
		public string username{ get; set; }
		public string password{ get; set; }
		public string remember{ get; set; }
	}

	public class LoginResponse
	{
		public int logined { get; set; }
		public string error { get; set; }
	}

	public class HeaderData
	{
		public string debugLocation{ get; set; }
		public bool developmentMode { get; set; }
		public int environment { get; set; }
		public string imgURL { get; set; }
		public bool isAjaxLoadMode { get; set; }
		public bool isCommAnimPreview { get; set; }
		public bool isCommentsBranchesHidden { get; set; }
		public bool isExpandedMode { get; set; }
		public bool isScrollTopBtn { get; set; }
		public bool isStoryAnimPreview { get; set; }
		public bool isUserBanned { get; set;}
		public int pageNumber { get; set; }
		public string pickedDate { get; set; }
		public string privateKey { get; set; }
		public string registrationKey { get; set; }
		public string scriptName { get; set; }
		public string sessionID { get; set; }
		public string siteURL { get; set; }
		public string uniqueBufferID { get; set; }
		public string userBanTime { get; set; }
		public int userID { get; set; }
		public decimal userKarma { get; set; }
		public string userName { get; set; }
	}
}

