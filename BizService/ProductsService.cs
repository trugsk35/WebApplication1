using System;
using System.Collections.Generic;
using WebApplication5;
using System.Data.Entity;
using System.Linq;

namespace WebApplication1.BizService
{
	public class ProductsService : IDisposable
	{
		private NorthwindEntities db;

		/// <summary> 建構式
		/// </summary>
		public ProductsService()
		{
			db = new NorthwindEntities();
		}

		/// <summary> Release resources
		/// </summary>
		public void Dispose()
		{
			db.Dispose();
		}

		/// <summary> 取得產品資訊列表
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Products> GetProductsInfo(int pageInex, out int totalpages)
		{
			if (pageInex < 1)
				pageInex = 1;

			totalpages = db.Products.Count() / 10;

			return db.Products.Include(p => p.Categories).Include(p => p.Suppliers).OrderBy(x => x.ProductID).Skip((pageInex - 1) * 10).Take(10);
		}

		/// <summary> 取得產品資訊列表
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Products> QueryProductsInfo(string keyWord, int CategoryID, int SupplierID, int pageInex, out int totalpages)
		{
			if (pageInex < 1)
				pageInex = 1;

			totalpages = db.Products.Count() / 10;

			IEnumerable<Products> list = db.Products;

			if (!string.IsNullOrWhiteSpace(keyWord))
				list = list.Where(x => x.ProductName.Contains(keyWord.Trim()));

			if (CategoryID != 0)
				list = list.Where(x => x.CategoryID == CategoryID);

			if (CategoryID != 0)
				list = list.Where(x => x.SupplierID == SupplierID);


			return list.OrderBy(x => x.ProductID).Skip((pageInex - 1) * 10).Take(10);
		}

		/// <summary> 依照ID取得產品資訊
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Products FindProductsInfo(int? id)
		{
			return db.Products.Find(id);
		}

		/// <summary> 存入產品
		/// </summary>
		/// <param name="products"></param>
		/// <returns></returns>
		public bool InsertProducts(Products products)
		{
			db.Products.Add(products);

			return db.SaveChanges() > 0;
		}

		/// <summary> 更新產品資訊
		/// </summary>
		/// <param name="products"></param>
		/// <returns></returns>
		public bool UpdateProducts(Products products)
		{
			db.Entry(products).State = EntityState.Modified;

			return db.SaveChanges() > 0;
		}

		/// <summary> 刪除產品
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool DeleteProducts(int id)
		{
			Products products = db.Products.Find(id);
			db.Products.Remove(products);

			return db.SaveChanges() > 0;
		}

		/// <summary> 差看是否有相同的產品名稱
		/// </summary>
		/// <param name="productName"></param>
		/// <returns></returns>
		public bool FindProductsInfo(string productName)
		{
			return db.Products.Where(x => x.ProductName == productName).Any();
		}
	}
}