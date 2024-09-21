using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TIE_Decor.Entities
{
    public class CheckoutViewModel
    {
        public User? User { get; set; }
        public List<Cart>? CartItems { get; set; }
        public decimal TotalPrice { get; set; }

        [Required(ErrorMessage = "Please enter the shipping address.")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Please enter your contact phone.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string ContactPhone { get; set; }
    }
}
