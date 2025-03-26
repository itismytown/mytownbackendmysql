//using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace mytown.Models
{
    public class shopdetails
    {
        public int shopId { get; set; }
        public int BusRegId { get; set; }
        public string shopName { get; set; }
        public string productName { get; set; }
        public int productId { get; set; }
        public decimal productCost { get; set; }
        public string productdesc { get; set; }

        public string shopimage { get; set; }



    }
}
