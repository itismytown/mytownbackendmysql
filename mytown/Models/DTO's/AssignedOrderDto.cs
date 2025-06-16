namespace mytown.Models.DTO_s
{
   
        public class AssignedOrderDto
        {
            public int ShippingDetailId { get; set; }
            public int OrderId { get; set; }

            // Shopper Info
            public string CustomerName { get; set; }
            public string CustomerPhoneNumber { get; set; }
            public string ShippingAddress { get; set; }

            // Store Info
            public string StoreName { get; set; }

            // Product Info
            public string ProductName { get; set; }
            public decimal ProductWeight { get; set; }
            public int Quantity { get; set; }

            // Shipping Info
            public string ShippingType { get; set; }
            public string ShippingStatus { get; set; }
            public decimal Cost { get; set; }
            public string TrackingId { get; set; }
            public DateTime EstimatedDeliveryDate { get; set; }
        }


    
}
