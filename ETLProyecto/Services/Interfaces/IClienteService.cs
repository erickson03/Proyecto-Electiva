using ETLProyecto.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETLProyecto.Services.Interfaces
{
    public interface IClienteService
    {
        Task<int> InsertClientesAsync(IEnumerable<Cliente> clientes);
    }
}
