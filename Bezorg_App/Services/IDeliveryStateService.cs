using System.Collections.Generic;
using System.Threading.Tasks;
using Bezorg_App.Models;

namespace Bezorg_App.Services
{
    public interface IDeliveryStateService
    {
        Task<IList<DeliveryState>> GetAllAsync();
        Task<DeliveryState> UpdateAsync(DeliveryState state);
    }
}
