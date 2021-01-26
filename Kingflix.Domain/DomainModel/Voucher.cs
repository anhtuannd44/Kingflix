using Kingflix.Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace Kingflix.Domain.DomainModel
{
    [Table("Voucher")]
    public partial class Voucher
    {
        public Voucher()
        {
            VoucherChilds = new HashSet<VoucherChild>();
            Voucher_Category = new HashSet<Voucher_Category>();
        }

        [Key]
        [Display(Name = "Mã Voucher")]
        [Required(ErrorMessage = "Bạn chưa nhập mã Voucher")]
        public string VoucherId { get; set; }

        [Display(Name = "Tên Voucher")]
        [Required(ErrorMessage = "Bạn chưa nhập tên Voucher")]
        public string Name { get; set; }

        [Display(Name = "Điều kiện áp dụng (Nội dung)")]
        [Required(ErrorMessage = "Bạn chưa nhập mục này")]
        [AllowHtml]
        public string PolicyContent { get; set; }

        [Display(Name = "Giá trị Voucher")]
        [Required(ErrorMessage = "Bạn chưa nhập giá trị Voucher")]
        public double Value { get; set; }

        [Display(Name = "Số tiền giảm tối đa")]
        public double? MaxMoney { get; set; }

        [Display(Name = "Hạn sử dụng Voucher")]
        public DateTime DateEnd { get; set; }

        [Display(Name = "Ngày tạo")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Ngày chỉnh sửa")]
        public DateTime? DateModified { get; set; }

        [Display(Name = "Hình thức")]
        [Required(ErrorMessage = "Bạn chưa chọn hình thức")]
        public VoucherType Type { get; set; }

        [Display(Name = "Loại Voucher")]
        [Required(ErrorMessage = "Bạn chưa chọn Voucher loại này")]
        public VoucherFor VoucherFor { get; set; }

        public VoucherStatus Status { get; set; }
        public int UseCount = 0;
        public virtual ICollection<VoucherChild> VoucherChilds { get; set; }
        public virtual ICollection<Voucher_Category> Voucher_Category { get; set; }
    }
}