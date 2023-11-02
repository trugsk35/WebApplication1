using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.Products
{
	public class ProductsIndexModel
	{
		public int ProductID { get; set; }

		[Display(Name = "商品名稱")]
		public string ProductName { get; set; }

		[Display(Name = "供應商序號")]
		public Nullable<int> SupplierID { get; set; }

		[Display(Name = "類別序號")]
		public Nullable<int> CategoryID { get; set; }

		[Display(Name = "數量")]
		public string QuantityPerUnit { get; set; }

		[Display(Name = "單價")]
		[DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
		public Nullable<decimal> UnitPrice { get; set; }

		[Display(Name = "庫存量")]
		public Nullable<short> UnitsInStock { get; set; }

		[Display(Name = "單位")]
		public Nullable<short> UnitsOnOrder { get; set; }

		[Display(Name = "最低存貨量")]
		public Nullable<short> ReorderLevel { get; set; }

		[Display(Name = "是否停售")]
		public string Discontinued { get; set; }
	}
}