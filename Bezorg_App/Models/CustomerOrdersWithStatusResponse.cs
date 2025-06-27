using System.Collections.Generic;

namespace Bezorg_App.Models
{
    // Response model voor customer orders met status
    public class CustomerOrdersWithStatusResponse
    {
        public Customer Customer { get; set; } = new();
        public List<OrderWithDeliveryStatus>? OrdersWithStatus { get; set; }
    }
} 