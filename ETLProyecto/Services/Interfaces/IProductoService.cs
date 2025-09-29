using ETLProyecto.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETLProyecto.Services.Interfaces
{
    public interface IProductoService
    {
        Task<int> InsertProductosAsync(IEnumerable<Producto> productos);
    }
}
