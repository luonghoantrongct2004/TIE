using TIE_Decor.Entities;

namespace TIE_Decor.Entities
{
    public class Orderdetail
    {
        public int OrderdetailId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public DateTime? ShipDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
