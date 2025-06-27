using Bezorg_App.Models.Enums;

namespace Bezorg_App.Models
{
    public class UpdateDeliveryStateRequest
    {
        public DeliveryStateEnum State { get; set; }
        public int? DeliveryServiceId { get; set; }
    }
} 