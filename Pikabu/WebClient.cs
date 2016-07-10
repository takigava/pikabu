using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Android.Widget;
using Android.Content;
using Android.Preferences;

namespace Pikabu
{
    public static class WebClient
    {
        public static CookieContainer CookieContainer;
		private static ISharedPreferences pref;

        public static void Initialize()
        {
            CookieContainer = new CookieContainer();
			pref = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
        }

        public static async Task<LoginResponse> Authorize(LoginInfo loginInfo)
        {
            LoginResponse result;
            var context = Application.Context;
            var uri = new Uri(context.GetString(Resource.String.origin));
            var request = (HttpWebRequest)WebRequest.Create(uri);
			request.Headers.Add("Accept-Language", "en-US,en;q=0.8");
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
			request.ContentType = "text/html; charset=windows-1251";
            request.Method = "GET";
            request.Timeout = 3000;


			//request.Headers.Add("Origin", context.GetString(Resource.String.origin));
			//request.Host = context.GetString(Resource.String.host);
			//request.Referer = context.GetString(Resource.String.origin);
			//request.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36";
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.CookieContainer = CookieContainer;

			using (var response = await request.GetResponseAsync())
            {
				var temp = (HttpWebResponse)response;
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var responseResult = reader.ReadToEnd();

				}
                CookieContainer = request.CookieContainer;
            }


            var loginUri = new Uri(context.GetString(Resource.String.origin) + context.GetString(Resource.String.loginPath));
            var loginRequest = (HttpWebRequest)WebRequest.Create(loginUri);
            loginRequest.CookieContainer = CookieContainer;
            loginRequest.Method = "POST";
            loginRequest.Timeout = 3000;

            var sb = new StringBuilder();
            sb.Append("mode=" + loginInfo.Mode);
            sb.Append("&");
            sb.Append("username=" + WebUtility.UrlEncode(loginInfo.Username));
            sb.Append("&");
            sb.Append("password=" + loginInfo.Password);
            sb.Append("&");
            sb.Append("remember=" + loginInfo.Remember);
            //var loginString = WebUtility.UrlEncode(sb.ToString());
            var loginBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var phpSesion = CookieContainer.GetCookies(new Uri(context.GetString(Resource.String.origin)))["PHPSESS"].Value;

            loginRequest.Headers.Add("Accept-Language", "en-US,en;q=0.8");
            loginRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");
            loginRequest.Headers.Add("X-Csrf-Token", phpSesion);
            loginRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            loginRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            loginRequest.Headers.Add("Origin", context.GetString(Resource.String.origin));
            loginRequest.Host = context.GetString(Resource.String.host);
            loginRequest.Referer = context.GetString(Resource.String.origin);
            loginRequest.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36";
            loginRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            var writeStream = loginRequest.GetRequestStream();

            writeStream.Write(loginBytes, 0, loginBytes.Length);
            writeStream.Flush();
            writeStream.Close();

            using (var loginResponse = await loginRequest.GetResponseAsync())
            {
                CookieContainer = loginRequest.CookieContainer;
                using (var reader = new StreamReader(loginResponse.GetResponseStream()))
                {
                    var responseResult = reader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<LoginResponse>(responseResult);
                }
            }

            return result;
        }

        public static async Task<string> GetPosts(int page)
        {
            string result;
            var context = Application.Context;
			var chanelUrl = String.Empty;
			var currentChanel = pref.GetString ("CurrentChanel", string.Empty);
			if (!String.IsNullOrEmpty (currentChanel)) {
				switch (Int32.Parse (currentChanel)) {
				case Resource.Id.hotRowLayout:
					chanelUrl = context.GetString (Resource.String.hot_url);
					break;
				case Resource.Id.bestRowLayout:
					chanelUrl = context.GetString (Resource.String.best_url);
					break;
				case Resource.Id.newRowLayout:
					chanelUrl = context.GetString (Resource.String.new_url);
					break;
				default:
					break;
				}
			} else {
				chanelUrl = context.GetString (Resource.String.hot_url);
			}

			var uri = new Uri (chanelUrl + page);

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Headers.Add("Accept-Language", "en-US,en;q=0.8");
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.ContentType = "text/html; charset=windows-1251";
            request.Method = "GET";
            request.Timeout = 3000;
            request.CookieContainer = CookieContainer;
            request.Host = context.GetString(Resource.String.host);
            //request.Referer = context.GetString(Resource.String.origin) + "/new";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (var response = await request.GetResponseAsync())
            {
                CookieContainer = request.CookieContainer;

                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("windows-1251")))
                {
                    var responseResult = reader.ReadToEnd();

                    //result = utf8.GetString(utf8Bytes);
                    result = responseResult;
                }
            }
            return result;
        }

        public static async Task LoadPosts(List<Post> newPostList, int currentPage)
        {
            var htmlPage = await GetPosts(currentPage);
            var htmlDoc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };
            htmlDoc.LoadHtml(htmlPage);
            var root = htmlDoc.DocumentNode;
            var commonPosts = root.Descendants().Where(n => n.GetAttributeValue("class", "").Equals("story"));

            //_Adapter.NotifyDataSetChanged();
			if(commonPosts.Count()<=0)return;
			try
			{
				foreach (var post in commonPosts)
				{
					var pinedPost = post.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Equals("story__pin"));
					if (pinedPost != null)
					{
						continue;
					}
					var pinedOPost = post.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Equals("story__pin-o"));
					if (pinedOPost != null)
					{
						continue;
					}
					var postId = post.Attributes.FirstOrDefault(s => s.Name == "data-story-id");

					if (postId == null) continue;
					if (newPostList.Where(s => s.Id == int.Parse(postId.Value)).ToList().Count > 1)
					{
						Application.SynchronizationContext.Post(_ =>
						{
							Toast.MakeText(Application.Context, "Повтор", ToastLength.Short).Show();
						}, null);
						continue;
					}
					var newPost = new Post
					{
						Id = int.Parse(postId.Value)
					};
					var rating = 0;
					var ratingNode = post.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Equals("story__rating-count"));
					var result = ratingNode != null && int.TryParse(ratingNode.InnerHtml, out rating);
					newPost.Rating = result ? rating : 0;
					var header = post.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Equals("story__header"));
					if (header != null)
					{
						var authorNameNode = header.Descendants().FirstOrDefault(s => s.GetAttributeValue("href", "").Contains("profile"));
						if (authorNameNode != null)
							newPost.AuthorName = authorNameNode.InnerHtml;

						var commentsNode = header.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Equals("story__comments-count story__to-comments"));
						if (
							commentsNode !=
							null)
							newPost.Comments = int.Parse(commentsNode.InnerHtml.Split(' ')[0]);

						var titleNode = header.Descendants().FirstOrDefault(s => s.GetAttributeValue("href", "").Contains("story"));
						if (
							titleNode !=
							null)
							//newPost.Title = titleNode.ChildNodes[1].InnerText.Replace("\n", "").Replace("\t", "").Replace("&quot;", "\"");
							newPost.Title = titleNode.InnerHtml;
						var desc = header.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Equals("short"));
						if (desc != null)
						{
							//newPost.Description = String.Empty;
							newPost.Description = desc.InnerHtml;
							//                        newPost.FormattedDescription = new List<Tuple<string, string>>();
							//                        if (desc.ChildNodes.Count > 1)
							//                        {
							//
							//                            foreach (var d in desc.ChildNodes)
							//                            {
							//                                if (d.Name == "a")
							//                                {
							//                                    //newPost.FormattedDescription.Add("textLink",d.InnerText);
							//                                    //newPost.FormattedDescription.Add("url",
							//                                    newPost.FormattedDescription.Add(new Tuple<string, string>("textLink", d.InnerText));
							//                                    newPost.FormattedDescription.Add(new Tuple<string, string>("url", d.Attributes["href"].Value));
							//                                }
							//                                else
							//                                {
							//                                    //newPost.FormattedDescription.Add("text",d.InnerHtml);
							//                                    newPost.FormattedDescription.Add(new Tuple<string, string>("text", d.InnerText));
							//                                }
							//                                if (d.Name == "#text")
							//                                {
							//                                    newPost.FormattedDescription.Add(new Tuple<string, string>("text", d.InnerText));
							//                                }
							//                            }
							//                        }
							//                        else
							//                        {
							//                            newPost.FormattedDescription.Add(new Tuple<string, string>("text", desc.InnerText));
							//                        }
						}
						var tagsListHtml = header.Descendants().Where(s => s.GetAttributeValue("class", "").Equals("story__tags")).FirstOrDefault();
						var tagsList = tagsListHtml.Descendants().Where(s => s.GetAttributeValue("href", "").Contains("tag"));
						newPost.Tags = new List<string>();
						foreach (var tag in tagsList)
						{
							newPost.Tags.Add(tag.InnerText.Replace("\n", "").Replace("\t", ""));
						}
						var time = header.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Equals("story__date"));
						if (time != null)
							newPost.PostTime = time.InnerHtml;
						//newPost.Tags = header.Descendants().Where(s => s.GetAttributeValue("class", "").Equals("story__tags")).Select(s => s.InnerHtml).ToList();

						//var test = header.Descendants().FirstOrDefault(s=>s.GetAttributeValue("class","").Equals("detailDate"));

						//
						// Text Post
						//
						var textType = header.Descendants().FirstOrDefault(s => s.GetAttributeValue("data-story-type", "").Equals("text"));
						if (textType != null)
						{
							newPost.PostType = PostType.Text;
							var shortText = post.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Equals("b-story__content b-story__content_type_text"));
							if (shortText != null)
							{
								newPost.Text = shortText.InnerText; //.Replace("\n\t\t\t", "");
							}
							else
							{
								newPost.Text = post.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Contains("b-story-block__content")).InnerText; //.Replace("\n\t\t\t\t\t\t\t\t", "");
							}
							newPostList.Add(newPost);
						}


						var imageType = header.Descendants().FirstOrDefault(s => s.GetAttributeValue("data-story-type", "").Equals("image"));
						var gifNode = post.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Equals("b-gifx__player"));
						if (imageType != null)
						{
							var isLongPost = bool.Parse(imageType.Attributes["data-story-long"].Value);
							if (!isLongPost)
							{
								
								if (gifNode != null)
								{
									var attr = post.Descendants().FirstOrDefault(s => s.GetAttributeValue("src", "").Contains("http://"));
									var gifSource = gifNode.Attributes["data-src"].Value;
									newPost.PostType = PostType.Gif;
									newPost.GifUrl = gifSource;
									newPost.Url = post.Descendants().Where(s => s.GetAttributeValue("src", "").Contains("http://")).ToList()[1].Attributes["src"].Value;
									newPostList.Add(newPost);
								}
								else
								{
									var isLong = post.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Equals("b-story__content b-story__content_type_media "));
									if (isLong != null)
									{
										var attr = post.Descendants().Where(s => s.GetAttributeValue("src", "").Contains("http://")).ToList()[1];
										if (attr != null)
										{
											newPost.PostType = PostType.Image;
											newPost.Url = attr.Attributes["src"].Value;
											newPost.Width = int.Parse(attr.Attributes["width"].Value);
											newPost.Height = int.Parse(attr.Attributes["height"].Value);
											newPost.IsBiggerAvailable = bool.Parse(attr.Attributes["data-bigger-available"].Value);
										}
										newPostList.Add(newPost);
									}
									else
									{
										var block = post.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Equals("b-story__content b-story-blocks"));
										var images = block.Descendants().Where(s => s.GetAttributeValue("src", "").Contains("http://")).ToList();
										newPost.Images = new List<string>();
										foreach (var image in images)
										{
											newPost.PostType = PostType.MultiImage;
											newPost.Images.Add(image.Attributes["src"].Value);
										}
										newPostList.Add(newPost);
									}

								}
							}
							else 
							{
								if (gifNode == null)
								{
									var block = post.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Equals("b-story__content b-story-blocks"));
									var images = block.Descendants().Where(s => s.GetAttributeValue("src", "").Contains("http://")).ToList();
									newPost.Images = new List<string>();
									foreach (var image in images)
									{
										newPost.PostType = PostType.MultiImage;
										newPost.Images.Add(image.Attributes["src"].Value);
									}
									newPostList.Add(newPost);
								}

							}



						}

						var videoType = header.Descendants().FirstOrDefault(s => s.GetAttributeValue("data-story-type", "").Equals("video"));
						if (videoType != null)
						{
							var videoNode = post.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Equals("b-video"));
							if (videoNode != null)
							{
								newPost.PostType = PostType.Video;
								newPost.VideoUrl = videoNode.Attributes["data-url"].Value;
								var imagePreviewNode = videoNode.Descendants().FirstOrDefault(s => s.GetAttributeValue("class", "").Contains("b-video__preview"));
								if (imagePreviewNode != null)
								{
									var style = imagePreviewNode.Attributes["style"].Value;
									newPost.Url = style.Split('(')[1].Split(')')[0].Replace("'", "");
								}
							}
							newPostList.Add(newPost);
						}

					}

					//(recyclerView.GetAdapter()as PostViewAdapter)._Posts.Add(newPost);
					//(recyclerView.GetAdapter()as PostViewAdapter).NotifyItemInserted((recyclerView.GetAdapter()as PostViewAdapter)._Posts.Count);
				}
			}
			catch (Exception ex)
			{
				var testException = ex.Message;
			}
        }

		public static async Task<byte[]> DownloadFile(string path){
			var result = new byte[0];
			try{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
				request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				request.Method = "GET";
				request.Timeout = 3000;
				if (request != null){
					var response = request.GetResponse();
					if (response != null){
						var remoteStream = response.GetResponseStream();
						result = new byte[remoteStream.Length];
						await remoteStream.ReadAsync(result,0,(int)remoteStream.Length);
						remoteStream.Close();
						response.Close();
					}
				}
			}
			catch(Exception ex){
			}
			return result;
		}

		public static async Task CreateRealFileAsync(string path)
		{
			// get hold of the file system

			//IFolder rootFolder = FileSystem.Current.LocalStorage;

			// create a folder, if one does not exist already
			//IFolder folder = await rootFolder.CreateFolderAsync("Pikabu", CreationCollisionOption.OpenIfExists);

			// create a file, overwriting any existing file
			var fileName = path.Split('/').Last();
			//IFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
			string path1 = Android.OS.Environment.ExternalStorageDirectory.Path;
			path1 = Path.Combine(path1, Android.OS.Environment.DirectoryDownloads);
			string filename = Path.Combine(path1, fileName);
			// populate the file with some text
			//await file.WriteAllTextAsync("Sample Text...");
			//var test = await file.OpenAsync(FileAccess.ReadAndWrite);
			var webClient = new System.Net.WebClient();
			var bytes = webClient.DownloadData(new Uri(path));
			if (bytes != null && bytes.Length > 0)
			{
				// do something with bytes (save etc)
				System.IO.File.WriteAllBytes (filename,bytes);
			}
			//var fileContent = await WebClient.DownloadFile (path);
			//if (fileContent.Length > 0) {
			//test.Write (fileContent, 0, fileContent.Length);
			//using (var streamWriter = new FileStream(filename,FileMode.CreateNew))
			//{
			//	streamWriter.Write(fileContent, 0, fileContent.Length);
			//}
			//}
			//test.Close ();

		}
    }
	class UrlCheckWebClient : System.Net.WebClient
	{
		public bool HeadOnly { get; set; }
		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest req = base.GetWebRequest(address);
			if (HeadOnly && req.Method == "GET")
			{
				req.Method = "HEAD";
			}
			return req;
		}
	}

    public class LoginInfo
    {
        public string Mode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Remember { get; set; }
    }

    public class LoginResponse
    {
        public int Logined { get; set; }
        public string Error { get; set; }
    }

    //public class HeaderData
    //{
    //	public string debugLocation{ get; set; }
    //	public bool developmentMode { get; set; }
    //	public int environment { get; set; }
    //	public string imgURL { get; set; }
    //	public bool isAjaxLoadMode { get; set; }
    //	public bool isCommAnimPreview { get; set; }
    //	public bool isCommentsBranchesHidden { get; set; }
    //	public bool isExpandedMode { get; set; }
    //	public bool isScrollTopBtn { get; set; }
    //	public bool isStoryAnimPreview { get; set; }
    //	public bool isUserBanned { get; set;}
    //	public int pageNumber { get; set; }
    //	public string pickedDate { get; set; }
    //	public string privateKey { get; set; }
    //	public string registrationKey { get; set; }
    //	public string scriptName { get; set; }
    //	public string sessionID { get; set; }
    //	public string siteURL { get; set; }
    //	public string uniqueBufferID { get; set; }
    //	public string userBanTime { get; set; }
    //	public int userID { get; set; }
    //	public decimal userKarma { get; set; }
    //	public string userName { get; set; }
    //}
}

