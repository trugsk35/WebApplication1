using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication1.Models.Products;

namespace WebApplication1.Models
{
    public class ProductIndexViewModel
    {
		public IEnumerable<ProductsIndexModel> Products { get; set; }
		
        public int PageInex { get; set; }

		[Display(Name = "搜尋關鍵字")]
		public string KeyWord { get; set; }

		[Display(Name = "供應商")]
		public Nullable<int> SupplierID { get; set; }

		[Display(Name = "類別")]
		public Nullable<int> CategoryID { get; set; }
	}
}