using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Kingflix.Domain.Enumerables
{
    public enum Status
    {
        [Display(Name = "Đã đăng")]
        Public = 0,
        [Display(Name = "Chưa đăng")]
        Private = 1,
        [Display(Name = "Thùng rác")]
        Trash = 2,
    }
    public enum ReviewStatus
    {
        [Display(Name = "Đã duyệt")]
        Accept = 0,
        [Display(Name = "Đang đợi")]
        Pending = 1,
        [Display(Name = "Spam")]
        Spam = 2,
        [Display(Name = "Thùng rác")]
        Trash = 3,
    }
    public enum PageStatus
    {
        [Display(Name = "Đã đăng")]
        Public = 0,
        [Display(Name = "Chưa đăng")]
        Private = 1,
    }
    public enum Gentle
    {
        [Display(Name = "Chọn giới tính")]
        Notset = 0,
        [Display(Name = "Nam")]
        Man = 1,
        [Display(Name = "Nữ")]
        Women = 2,
        [Display(Name = "Khác")]
        Other = 3,
    }
    public enum OrderStatus
    {
        [Display(Name = "Hoàn tất")]
        Done = 0,
        [Display(Name = "Chưa thanh toán")]
        WaitingForPay = 1,
        [Display(Name = "Đang xử lý")]
        Paid = 2,
        [Display(Name = "Đã hủy")]
        Cencelled = 3,
    }

    public enum OrderType
    {
        [Display(Name = "Order")]
        Order = 0,
        [Display(Name = "Extend")]
        Extend = 1,
        [Display(Name = "GiftCode")]
        GiftCode = 2,
    }

    public enum RenewStatus
    {
        [Display(Name = "Hoàn tất")]
        Done = 0,
        [Display(Name = "Đang xử lý")]
        Pending = 1,
        [Display(Name = "Đã hủy")]
        Cencelled = 2,
    }
    public enum CoinStatus
    {
        [Display(Name = "Hoàn tất")]
        Done = 0,
        [Display(Name = "Đang xử lý")]
        Pending = 1,
        [Display(Name = "Đã hủy")]
        Cencelled = 2,
    }
    public enum VoucherType
    {
        [Display(Name = "Giảm số tiền (VNĐ)")]
        Money = 0,
        [Display(Name = "Giảm phần trăm (%)")]
        Percent = 1,
        [Display(Name = "Cả 2")]
        Both = 2,
    }
    public enum VoucherResult
    {
        Voucher = 0,
        Referral = 1,
    }

    public enum VoucherFor
    {
        [Display(Name = "Bình thường")]
        Normal = 0,
        [Display(Name = "Hệ thống")]
        System = 1,
    }
    public enum PaymentType
    {
        [Display(Name = "Ví Bảo Kim API")]
        BaoKim = 0,
        [Display(Name = "ATM các thẻ ngân hàng API")]
        InternetBanking = 1,
        [Display(Name = "Visa/MasterCard API")]
        Visa = 128,
        [Display(Name = "QR Code API")]
        QRCode = 297,
        [Display(Name = "QR Ví điện tử API")]
        QRVidientu = 15,
        [Display(Name = "Card API")]
        Card = 3,
        [Display(Name = "Ví điện tử (Thủ công)")]
        EWallet = 4,
        [Display(Name = "Chuyển khoản (Thủ công)")]
        Bank = 5,
        [Display(Name = "Khác")]
        Orther = 6
    }
    public enum UserProductStatus
    {
        [Display(Name = "Đang sử dụng")]
        Using = 0,
        [Display(Name = "Tạm khóa")]
        Blocked = 1,
    }
    public enum UserProductStatusShow
    {
        [Display(Name = "Bình thường")]
        Using = 0,
        [Display(Name = "Khách báo lỗi")]
        Blocked = 1,
        [Display(Name = "Sắp hết hạn")]
        PreExpired = 2,
        [Display(Name = "Hết hạn")]
        Expired = 3,

    }

    public enum VoucherStatus
    {
        [Display(Name = "Đang hoạt động")]
        Active = 0,
        [Display(Name = "Chưa đăng")]
        Private = 1,
    }
    public enum ProductStatus
    {
        [Display(Name = "Đang hoạt động")]
        Active = 0,
        [Display(Name = "Chưa hoạt động")]
        Private = 1,
        [Display(Name = "Tài khoản thay thế")]
        Replace = 2
    }

    public enum ProductType
    {
        [Display(Name = "Tài khoản hệ thống cung cấp")]
        SystemAccount = 0,
        [Display(Name = "Tài khoản khách hàng cung cấp")]
        UserAccount = 1,
    }
    public enum ProductStatusShow
    {
        [Display(Name = "Đang hoạt động")]
        Active = 0,
        [Display(Name = "Chưa hoạt động")]
        Private = 1,
        [Display(Name = "Tài khoản thay thế")]
        Replace = 2,
        [Display(Name = "Báo lỗi")]
        Error = 3,
    }
    public enum Cycle
    {
        [Display(Name = "King (Chu kỳ 1)")]
        King = 0,
        [Display(Name = "Avenger (Chu kỳ 2)")]
        Avenger = 1,
        [Display(Name = "Mouse (Chu kỳ 3)")]
        Mouse = 2,
    }

    public enum MemberShip
    {
        [Display(Name = "Tiêu Chuẩn")]
        Standard = 0,
        [Display(Name = "Bạc")]
        Silver = 1,
        [Display(Name = "Titan")]
        Titanium = 2,
        [Display(Name = "Vàng")]
        Gold = 3,
        [Display(Name = "Bạch Kim")]
        Platinum = 4
    }
    public enum CategoryDetailsType
    {
        [Display(Name = "Không đổi tài khoản")]
        NoChangeAccount = 0,
        [Display(Name = "Bảo hành")]
        Support = 1,
        [Display(Name = "Thời gian giao hàng")]
        TimeAcceptOrder = 2,
        [Display(Name = "Profie riêng")]
        OwnProfile = 3,
        [Display(Name = "Số màn hình")]
        ScreenCount = 4,
        [Display(Name = "UltraHD, 5.1")]
        UltraHD = 5,
        [Display(Name = "Xem nhiều thiết bị")]
        MultiScreen = 6,
        [Display(Name = "Download Offline")]
        Download = 7,
    }
    public enum TypeOfCategory
    {
        [Display(Name = "NETFLIX")]
        Netflix = 0,
        [Display(Name = "Bán kèm Netflix")]
        Upsale = 1,
        [Display(Name = "Khác")]
        Orther = 2,
    }
    public enum TypeOfAccount
    {
        [Display(Name = "Cung cấp tài khoản")]
        KingflixAccount = 0,
        [Display(Name = "Tài khoản khách")]
        UserAccount = 1,
    }
    public enum OrderBy
    {
        Id = 1,
        Date = 2
    }

    public enum SupportStatus
    {
        [Display(Name = "Hoàn tất")]
        Success = 0,
        [Display(Name = "Đang xử lý")]
        Pending = 1,
        [Display(Name = "Đã hủy")]
        Cancel = 2
    }

    public enum NotificationType
    {
        Order = 0,
        Error = 1,
    }

    public enum EmailType
    {
        [Display(Name = "Sắp hết hạn (Nhắc gia hạn)")]
        PreExpired = 0,
        [Display(Name = "Hết hạn")]
        Exprired = 1,
        [Display(Name = "Tùy chỉnh")]
        Custom = 2
    }
    public enum EmailTypeHistory
    {
        [Display(Name = "Sắp hết hạn (Nhắc gia hạn)")]
        PreExpired = 0,
        [Display(Name = "Hết hạn")]
        Exprired = 1,
        [Display(Name = "Tùy chỉnh")]
        Custom = 2,
        [Display(Name = "Đặt hàng thành công")]
        OrderSuccess = 3,
        [Display(Name = "Duyệt đơn thành công")]
        OrderAccept = 4,
        [Display(Name = "Hủy đơn")]
        OrderCancel = 5,
        [Display(Name = "Gia hạn thành công")]
        ExtendSuccess = 6,
        [Display(Name = "Chấp nhận gia hạn")]
        ExtendAccept = 7,
        [Display(Name = "Hủy gia hạn")]
        ExtendCancel = 8,
        [Display(Name = "Nạp tiền")]
        KingCoinSuccess = 9,
        [Display(Name = "Chấp nhận nạp tiền")]
        KingCoinAccept = 10,
        [Display(Name = "Hủy nạp tiền")]
        KingCoinCancel = 11,
        [Display(Name = "Đăng ký tài khoản thành công")]
        AccountRegister = 12,
        [Display(Name = "Hủy tư cách thành viên")]
        AccouctRemove = 13,
        [Display(Name = "Hủy tư cách thành viên")]
        ChangeProfile = 14

    }

    public enum PictureHomeType
    {
        Slider = 0,
        SmallPicture = 1,
        SliderCategory = 2,
        BannerShop = 3,
        Social = 4,
        Orther = 5,
    }
    public enum BlogType
    {
        Blog = 0,
        Recruitment = 1
    }
    public static class EnumExtensionMethods
    {
        public static string GetEnumDisplayName(this Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>()
                           .Name;
        }


    }
}
