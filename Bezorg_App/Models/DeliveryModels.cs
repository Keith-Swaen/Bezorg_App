using System;
using System.Collections.Generic;

namespace Bezorg_App.Models
{
    public enum DeliveryStateEnum
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4
    }

    public class DeliveryState
    {
        public int Id { get; set; }
        public DeliveryStateEnum State { get; set; }
        public DateTime DateTime { get; set; }
        public int OrderId { get; set; }
        public int DeliveryServiceId { get; set; }
    }
    public class DeliveryStatesResponse : List<DeliveryState>
    {
    }
}
