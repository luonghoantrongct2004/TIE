using TIE_Decor.Entities;

namespace TIE_Decor.ViewModels
{
    public class ProductDetailReviewViewModel
    {
        public Product Product { get; set; } // Product ID for which the reviews are displayed
        public List<Review> Reviews { get; set; } // List of reviews for this product
        public double AverageRating { get; set; } // Average rating of the product
        public int TotalReviews { get; set; } // Total number of reviews
        public int[] RatingDistribution { get; set; } // Number of 1-star, 2-star, ..., 5-star reviews
    }
}
