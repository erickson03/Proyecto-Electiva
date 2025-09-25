using ETLProyecto.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETLProyecto.Services
{
    public interface IOrderDetailService
    {
        Task<int> InsertOrderDetailsAsync(IEnumerable<OrderDetail> details);
    }
}
