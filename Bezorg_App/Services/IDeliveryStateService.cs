using System.Collections.Generic;
using System.Threading.Tasks;
using Bezorg_App.Models;

namespace Bezorg_App.Services
{
    /// <summary>
    /// Definieert het ophalen van bezorgstatussen.
    /// </summary>
    public interface IDeliveryStateService
    {
        Task<IList<DeliveryState>> GetAllAsync();
    }
}
