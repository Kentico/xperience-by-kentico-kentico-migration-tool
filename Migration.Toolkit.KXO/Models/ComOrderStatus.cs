using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Migration.Toolkit.KXO.Models
{
    [Table("COM_OrderStatus")]
    [Index("StatusSiteId", "StatusOrder", Name = "IX_COM_OrderStatus_StatusSiteID_StatusOrder")]
    public partial class ComOrderStatus
    {
        public ComOrderStatus()
        {
            ComOrderStatusUserFromStatuses = new HashSet<ComOrderStatusUser>();
            ComOrderStatusUserToStatuses = new HashSet<ComOrderStatusUser>();
            ComOrders = new HashSet<ComOrder>();
            ComPaymentOptionPaymentOptionAuthorizedOrderStatuses = new HashSet<ComPaymentOption>();
            ComPaymentOptionPaymentOptionFailedOrderStatuses = new HashSet<ComPaymentOption>();
            ComPaymentOptionPaymentOptionSucceededOrderStatuses = new HashSet<ComPaymentOption>();
        }

        [Key]
        [Column("StatusID")]
        public int StatusId { get; set; }
        [StringLength(200)]
        public string StatusName { get; set; } = null!;
        [StringLength(200)]
        public string StatusDisplayName { get; set; } = null!;
        public int? StatusOrder { get; set; }
        [Required]
        public bool? StatusEnabled { get; set; }
        [StringLength(7)]
        public string? StatusColor { get; set; }
        [Column("StatusGUID")]
        public Guid StatusGuid { get; set; }
        public DateTime StatusLastModified { get; set; }
        public bool? StatusSendNotification { get; set; }
        [Column("StatusSiteID")]
        public int? StatusSiteId { get; set; }
        public bool? StatusOrderIsPaid { get; set; }

        [ForeignKey("StatusSiteId")]
        [InverseProperty("ComOrderStatuses")]
        public virtual CmsSite? StatusSite { get; set; }
        [InverseProperty("FromStatus")]
        public virtual ICollection<ComOrderStatusUser> ComOrderStatusUserFromStatuses { get; set; }
        [InverseProperty("ToStatus")]
        public virtual ICollection<ComOrderStatusUser> ComOrderStatusUserToStatuses { get; set; }
        [InverseProperty("OrderStatus")]
        public virtual ICollection<ComOrder> ComOrders { get; set; }
        [InverseProperty("PaymentOptionAuthorizedOrderStatus")]
        public virtual ICollection<ComPaymentOption> ComPaymentOptionPaymentOptionAuthorizedOrderStatuses { get; set; }
        [InverseProperty("PaymentOptionFailedOrderStatus")]
        public virtual ICollection<ComPaymentOption> ComPaymentOptionPaymentOptionFailedOrderStatuses { get; set; }
        [InverseProperty("PaymentOptionSucceededOrderStatus")]
        public virtual ICollection<ComPaymentOption> ComPaymentOptionPaymentOptionSucceededOrderStatuses { get; set; }
    }
}
