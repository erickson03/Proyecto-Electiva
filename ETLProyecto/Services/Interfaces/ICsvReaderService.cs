using ETLProyecto.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETLProyecto.Services.Interfaces
{
    public interface ICsvReaderService
    {
        Task<List<Producto>> ReadProductsAsync();
        Task<List<Cliente>> ReadClientesAsync();
        Task<List<Order>> ReadOrdersAsync();
        Task<List<OrderDetail>> ReadOrderDetailsAsync();
    }
}
