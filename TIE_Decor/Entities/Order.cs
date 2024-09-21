using System.ComponentModel.DataAnnotations;
using TIE_Decor.Entities;

namespace TIE_Decor.Entities
{
    public class Order
    {
        public int OrderId { get; set; }

        public Guid UserId { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? ShipDate { get; set; }

        public DateTime PaymentDate { get; set; }

        public int PaymentId { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? ShipperId { get; set; }
        public User User { get; set; }
        public string Status { get; set; }
        [Required(ErrorMessage = "Please choise your payment method")]
        public string PaymentMethod { get; set; }
        public string ShippingAddress { get; set; }
        public string ContactPhone { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal TotalPrice { get; set; }
        public ICollection<Orderdetail> OrderDetails { get; set; }
    }
}
