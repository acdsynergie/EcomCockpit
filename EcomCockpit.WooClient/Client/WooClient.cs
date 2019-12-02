using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;
using WooCommerceNET.WooCommerce.v3.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using RestSharp.Authenticators;
using WordPressPCL;
using System.Net;
using WordPressPCL.Models;

namespace EcomCockpit.WooClient.Client
{
    public class WooClient
    {
        #region endpoints
        #endregion

        #region data
        private static string consumerKey = "ck_44583ada25d0ccdab28f476cafa62ad52b6f9b33";
        private static string consumerSecret = "cs_202b943c2337c9462db543ab82f69cb313f75636";
        #endregion
              
        public static async System.Threading.Tasks.Task connectAsync()
        {
            var client = new WordPressClient("http://localhost:81/wordpress/","wp-josn/wc/v3/");

            // Posts
            var posts = await client.Posts.GetAll();
            //var postbyid = await client.Posts.GetById(id);

            // Comments
            var comments = await client.Comments.GetAll();
            //var commentbyid = await client.Comments.GetById(id);
            //var commentsbypost = await client.Comments.GetCommentsForPost(postid, true, false);

            // Users
            // JWT authentication
            client.AuthMethod = AuthMethod.JWT;
            await client.RequestJWToken("user", "changeme");

            // check if authentication has been successful
            var isValidToken = await client.IsValidJWToken();

            // now you can send requests that require authentication
            //var response = client.Posts.Delete(postid);
            Console.WriteLine(posts);
        }
    }

    public class ProductClient : WooClient
    {
        public IEnumerable<Product> GetAllProducts()
        {

            connectAsync();

            return null;
        }
    }
}
