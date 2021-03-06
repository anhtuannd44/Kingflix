using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kingflix.Domain.DomainModel
{
    [Table("RefundSetting")]
    public partial class RefundSetting
    {
        public RefundSetting()
        {
        }
        [Key]
        public string ID { get; set; }
        //Cài đặt số Max hoàn tiền
        public double? Value { get; set; }
    }
}