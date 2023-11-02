using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.BizService;
using WebApplication1.Models;
using WebApplication1.Models.Products;
using WebApplication5;

namespace WebApplication1.Controllers
{
	public class ProductsController : Controller
	{
		private NorthwindEntities db = new NorthwindEntities();
		ProductsService productService = new ProductsService();

		/// <summary> 產品目錄列表
		/// </summary>
		/// <param name="pageInex">頁次</param>
		/// <returns></returns>
		public ActionResult Index()
		{
			try
			{
				int totalpages = 0;

				IEnumerable<Products> products = productService.GetProductsInfo(1, out totalpages);
				
				ProductIndexViewModel indexmodel = new ProductIndexViewModel();

				List<ProductsIndexModel> ViewModelLists = new List<ProductsIndexModel>();
				foreach (Products iten in products)
				{
					ViewModelLists.Add(
						new ProductsIndexModel()
						{
							ProductID = iten.ProductID,
							ProductName = iten.ProductName,
							QuantityPerUnit = iten.QuantityPerUnit,
							UnitPrice = iten.UnitPrice,
							UnitsInStock = iten.UnitsInStock,
							UnitsOnOrder = iten.UnitsOnOrder,
							ReorderLevel = iten.ReorderLevel,
							Discontinued = (iten.Discontinued) ? "是" : "否",
							CategoryID = iten.CategoryID
						});
				}
				if (products == null || products.Count() <= 0)
				{
					ViewModelLists.Add(
						new ProductsIndexModel()
						{
							ProductName = "無相關資料"
						});
				}

				indexmodel.Products = ViewModelLists;

				ViewBag.IndexPage = 1;
				ViewBag.totalpages = totalpages;
				ViewBag.CategoryID = new SelectList(GetCategoriesList(), "CategoryID", "CategoryName");
				ViewBag.SupplierID = new SelectList(GetSuppliersList(), "SupplierID", "CompanyName");

				return View(indexmodel);
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Home");
			}
		}

		/// <summary> 處理[Index]產品表單
		/// </summary>
		/// <param name="keyWord">搜尋關鍵字</param>
		/// <param name="pageIndex">頁次</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Index(ProductIndexViewModel model)
		{
			try
			{
				int totalpages = 0;
				IEnumerable<Products> products = productService.QueryProductsInfo(model.KeyWord,model.CategoryID.GetValueOrDefault(),model.SupplierID.GetValueOrDefault(), model.PageInex, out totalpages);
				
				ProductIndexViewModel indexmodel = new ProductIndexViewModel();

				List<ProductsIndexModel> ViewModelLists = new List<ProductsIndexModel>();
				foreach (Products iten in products)
				{
					ViewModelLists.Add(
						new ProductsIndexModel()
						{
							ProductID = iten.ProductID,
							ProductName = iten.ProductName,
							QuantityPerUnit = iten.QuantityPerUnit,
							UnitPrice = iten.UnitPrice,
							UnitsInStock = iten.UnitsInStock,
							UnitsOnOrder = iten.UnitsOnOrder,
							ReorderLevel = iten.ReorderLevel,
							Discontinued = (iten.Discontinued) ? "是" : "否",
							CategoryID = iten.CategoryID
						});
				}
				if (products == null || products.Count() <= 0)
				{
					ViewModelLists.Add(
						new ProductsIndexModel()
						{
							ProductName = "無相關資料"
						});
				}

				indexmodel.Products = ViewModelLists;
				indexmodel.KeyWord = model.KeyWord;
				ViewBag.IndexPage = model.PageInex;
				ViewBag.totalpages = totalpages;
				ViewBag.CategoryID = new SelectList(GetCategoriesList(), "CategoryID", "CategoryName");
				ViewBag.SupplierID = new SelectList(GetSuppliersList(), "SupplierID", "CompanyName");

				return View(indexmodel);
			}
			catch (Exception ex)
			{
				return View(model);
			}
		}

		/// <summary> 顯示[新增]產品表單
		/// </summary>
		/// <returns></returns>
		public ActionResult Create()
		{
			ViewBag.CategoryID = new SelectList(GetCategoriesList(), "CategoryID", "CategoryName");
			ViewBag.SupplierID = new SelectList(GetSuppliersList(), "SupplierID", "CompanyName");
			return View();
		}

		/// <summary> 處理[新增]產品表單
		/// </summary>
		/// <param name="vm">新增產品資料</param>
		/// <returns></returns>
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult Create(ProductsCreatViewModel vm)
		{
			bool isValid = true;

			if (!ModelState.IsValid)
				isValid = false;

			// 如果檢核出產品名稱相同告知使用者
			if (VaildProductSameName(vm.ProductName))
			{
				ModelState.AddModelError("ProductName", "已存在相同的產品名稱!");
				isValid = false;
			}

			// 如果產品啟用但是沒有庫存的話提醒使用者
			if (vm.Discontinued && vm.UnitsInStock <= 0)
			{
				ModelState.AddModelError("UnitsInStock", "該產品沒有庫存無法啟用!");
				isValid = false;
			}

			try
			{
				if (isValid)
				{
					Products iten = new Products()
					{
						ProductName = vm.ProductName,
						SupplierID = vm.SupplierID,
						CategoryID = vm.CategoryID,
						QuantityPerUnit = vm.QuantityPerUnit,
						UnitPrice = vm.UnitPrice,
						UnitsInStock = vm.UnitsInStock,
						UnitsOnOrder = vm.UnitsOnOrder,
						ReorderLevel = vm.ReorderLevel,
						Discontinued = vm.Discontinued
					};

					if (productService.InsertProducts(iten))
						return RedirectToAction("Index");
				}

				ViewBag.CategoryID = new SelectList(GetCategoriesList(), "CategoryID", "CategoryName", vm.CategoryID);
				ViewBag.SupplierID = new SelectList(GetSuppliersList(), "SupplierID", "CompanyName", vm.SupplierID);
				return View(vm);
			}
			catch (Exception ex)
			{
				return View(vm);
			}
		}

		/// <summary> 顯示[編輯]資料
		/// </summary>
		/// <param name="id">產品編號</param>
		/// <returns></returns>
		public ActionResult Edit(int? id)
		{
			if (id == 0 || ProductId == 0)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			id = (id == null || id == 0) ? ProductId : id;
			ProductId = id;

			var products = productService.FindProductsInfo(id);

			if (products == null)
				return HttpNotFound();

			// 欄位模型對應轉換
			ProductsEditViewModel model = new ProductsEditViewModel()
			{
				ProductID = products.ProductID,
				ProductName = products.ProductName,
				QuantityPerUnit = products.QuantityPerUnit,
				UnitPrice = products.UnitPrice,
				UnitsInStock = products.UnitsInStock,
				UnitsOnOrder = products.UnitsOnOrder,
				ReorderLevel = products.ReorderLevel,
				Discontinued = products.Discontinued,
				CategoryID = products.CategoryID
			};

			if (TempData["delErr"] != null)
				ModelState.AddModelError("", "該產品還有訂單或其他項目使用請先確認沒有使用才能刪除 !");

			//model.ProductID = products.ProductID;
			if (products == null)
				return HttpNotFound();


			ViewBag.CategoryID = new SelectList(GetCategoriesList(), "CategoryID", "CategoryName", products.CategoryID);
			ViewBag.SupplierID = new SelectList(GetSuppliersList(), "SupplierID", "CompanyName", products.SupplierID);
			return View(model);
		}

		/// <summary> 處理[編輯]輸入表單
		/// </summary>
		/// <param name="model">編輯產品資料</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(ProductsEditViewModel model)
		{
			bool isValid = true;

			if (!ModelState.IsValid)
				isValid = false;

			// 如果產品啟用但是沒有庫存的話提醒使用者
			if (isValid)
			{
				if (model.Discontinued && model.UnitsInStock <= 0)
				{
					ModelState.AddModelError("UnitsInStock", "該產品沒有庫存無法啟用!");
					isValid = false;
				}
			}
			try
			{
				if (isValid)
				{
					// 欄位模型對應轉換
					Products iten = new Products()
					{
						ProductName = model.ProductName,
						SupplierID = model.SupplierID,
						CategoryID = model.CategoryID,
						QuantityPerUnit = model.QuantityPerUnit,
						UnitPrice = model.UnitPrice,
						UnitsInStock = model.UnitsInStock,
						UnitsOnOrder = model.UnitsOnOrder,
						ReorderLevel = model.ReorderLevel,
						Discontinued = model.Discontinued
					};

					iten.ProductID = ProductId.Value;

					if (productService.UpdateProducts(iten))
						return RedirectToAction("Index");

				}

				ViewBag.CategoryID = new SelectList(GetCategoriesList(), "CategoryID", "CategoryName", model.CategoryID);
				ViewBag.SupplierID = new SelectList(GetSuppliersList(), "SupplierID", "CompanyName", model.SupplierID);

				return View(model);
			}
			catch (Exception ex)
			{
				return View(model);

			}
		}

		/// <summary> 產品細項
		/// </summary>
		/// <param name="id">產品編號</param>
		/// <returns></returns>
		public ActionResult Details(int? id)
		{
			if (id == null || id == 0)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			Products products = productService.FindProductsInfo(id);

			ProductsDetailsViewModel model = new ProductsDetailsViewModel()
			{
				//ProductID = products.ProductID,
				ProductName = products.ProductName,
				QuantityPerUnit = products.QuantityPerUnit,
				UnitPrice = products.UnitPrice,
				UnitsInStock = products.UnitsInStock,
				UnitsOnOrder = products.UnitsOnOrder,
				ReorderLevel = products.ReorderLevel,
				Discontinued = (products.Discontinued) ? "是" : "否",
				Categories = products.Categories,
				Suppliers = products.Suppliers,
			};

			if (products == null)
				return HttpNotFound();
			else
				ProductId = products.ProductID;

			return View(model);
		}

		// 處理[刪除]產品表單
		// GET: Products/Delete/5
		public ActionResult Delete()
		{
			try
			{
				if (!productService.DeleteProducts(ProductId.Value))
					return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
			{
				TempData["delErr"] = true;
				return RedirectToAction("Edit");
			}

			return RedirectToAction("Index");
		}

		#region --------private---------
		/// <summary> 取得類別所有資料
		/// </summary>
		/// <returns></returns>
		private IEnumerable<Categories> GetCategoriesList()
		{
			return db.Categories.AsEnumerable();
		}

		/// <summary> 取得供應商所有資料
		/// </summary>
		/// <returns></returns>
		private IEnumerable<Suppliers> GetSuppliersList()
		{
			return db.Suppliers.AsEnumerable();
		}

		/// <summary> 驗證產品名稱是否存在
		/// </summary>
		/// <param name="productName">欲檢核產品名稱</param>
		/// <returns></returns>
		private bool VaildProductSameName(string productName)
		{
			return productService.FindProductsInfo(productName);
		}

		private int? ProductId
		{
			get { return Session["ProductId"] as int?; }
			set { Session["ProductId"] = value; }
		}
		#endregion --------private---------

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
