using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EcomCockpit.WooClient.Client;
using WooCommerceNET.WooCommerce.v1;

namespace EcomCockpit.WooClient.Controllers
{
    public class ProductsController : ApiController
    {
        public IEnumerable<Product> GetAllProducts()
        {
            return new ProductClient().GetAllProducts();
        }

        //public Product GetProductById(int id) { }
        //public HttpResponseMessage DeleteProduct(int id) { }

    }
}
