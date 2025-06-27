using System;
using System.Collections.Generic;
using Bezorg_App.Models.Enums;

namespace Bezorg_App.Models
{
    public class OrderWithDeliveryStatus
    {
        public Order Order { get; set; } = new();
        public DeliveryStateEnum CurrentStatus { get; set; }
        public List<DeliveryState>? DeliveryHistory { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
} 