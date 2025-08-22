using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class ShopperProductRecentView
    {
        [Key]
        public int RecentViewId { get; set; }

        [ForeignKey("Shopper")]
        public int ShopperId { get; set; }   // which shopper viewed

        [ForeignKey("Product")]
        public int ProductId { get; set; }   // which product was viewed

        public DateTime LastViewedAt { get; set; } = DateTime.UtcNow; // last viewed time

        public int ViewCount { get; set; } = 1; // optional (track number of times viewed)

        // Navigation Properties
        public virtual ShopperRegister Shopper { get; set; }
        public virtual products Product { get; set; }
    }
}
