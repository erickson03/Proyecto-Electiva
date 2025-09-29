using ETLProyecto.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETLProyecto.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> InsertOrdersAsync(IEnumerable<Order> orders);
        Task<int> InsertOrderDetailsAsync(IEnumerable<OrderDetail> details);
    }
}
