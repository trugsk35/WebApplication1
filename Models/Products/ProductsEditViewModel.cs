using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication5;

namespace WebApplication1.Models
{
    public class ProductsEditViewModel
    {
        public int ProductID { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "{0}不可以超過{1}個字元。")]
        [Display(Name = "產品名稱")]
        public string ProductName { get; set; }

        [Display(Name = "供應商")]
        public Nullable<int> SupplierID { get; set; }

        [Display(Name = "類別")]
        public Nullable<int> CategoryID { get; set; }

        [Required(ErrorMessage = "請輸入產品內含數量")]
        [Display(Name = "產品內含數量")]
        [DataType(DataType.Currency)]
        public string QuantityPerUnit { get; set; }

        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "請輸入產品售價")]
        [Display(Name = "售價")]
		[DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
		public Nullable<decimal> UnitPrice { get; set; }

        [Required(ErrorMessage = "請輸入現有庫存量")]
        [Display(Name = "庫存量")]
        public Nullable<short> UnitsInStock { get; set; }

        [Required(ErrorMessage = "請輸入產品單位")]
        [Display(Name = "產品單位")]
        public Nullable<short> UnitsOnOrder { get; set; }

        [Required(ErrorMessage = "請輸入{0}")]
        [Display(Name = "基本庫存存貨量")]
        public Nullable<short> ReorderLevel { get; set; }

        [Required(ErrorMessage = "請勾選是否停止販售")]
        [Display(Name = "是否停售")]
        public bool Discontinued { get; set; }


        public virtual Categories Categories { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Details> Order_Details { get; set; }

        public virtual Suppliers Suppliers { get; set; }
    }
}