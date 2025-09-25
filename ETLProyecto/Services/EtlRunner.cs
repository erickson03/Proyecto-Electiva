using ETLProyecto.Models;
using Microsoft.Extensions.Logging;

namespace ETLProyecto.Services
{
    public class EtlRunner
    {
        private readonly ICsvReaderService _csvService;
        private readonly IProductoService _productoService;
        private readonly IClienteService _clienteService;
        private readonly IOrderService _orderService;
        private readonly ILogger<EtlRunner> _logger;

        public EtlRunner(
            ICsvReaderService csvService,
            IProductoService productoService,
            IClienteService clienteService,
            IOrderService orderService,
            ILogger<EtlRunner> logger)
        {
            _csvService = csvService;
            _productoService = productoService;
            _clienteService = clienteService;
            _orderService = orderService;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            // -------------------------
            // Pipeline Productos
            // -------------------------
            _logger.LogInformation("Iniciando pipeline ETL para Productos...");
            var productos = await _csvService.ReadProductsAsync();
            if (productos.Any())
            {
                var inserted = await _productoService.InsertProductosAsync(productos);
                _logger.LogInformation("Productos insertados en BD: {Inserted}", inserted);
            }
            else
            {
                _logger.LogWarning("No hay Productos para insertar.");
            }

            // -------------------------
            // Pipeline Clientes
            // -------------------------
            _logger.LogInformation("Iniciando pipeline ETL para Clientes...");
            var clientes = await _csvService.ReadClientesAsync();
            if (clientes.Any())
            {
                var inserted = await _clienteService.InsertClientesAsync(clientes);
                _logger.LogInformation("Clientes insertados en BD: {Inserted}", inserted);
            }
            else
            {
                _logger.LogWarning("No hay Clientes para insertar.");
            }

            // -------------------------
            // Pipeline Orders
            // -------------------------
            _logger.LogInformation("Iniciando pipeline ETL para Orders...");
            var orders = await _csvService.ReadOrdersAsync();
            if (orders.Any())
            {
                var inserted = await _orderService.InsertOrdersAsync(orders);
                _logger.LogInformation("Orders insertadas en BD: {Inserted}", inserted);
            }
            else
            {
                _logger.LogWarning("No hay Orders para insertar.");
            }

            // -------------------------
            // Pipeline OrderDetails
            // -------------------------
            _logger.LogInformation("Iniciando pipeline ETL para OrderDetails...");
            var details = await _csvService.ReadOrderDetailsAsync();
            if (details.Any())
            {
                var inserted = await _orderService.InsertOrderDetailsAsync(details);
                _logger.LogInformation("OrderDetails insertados en BD: {Inserted}", inserted);
            }
            else
            {
                _logger.LogWarning("No hay OrderDetails para insertar.");
            }

            _logger.LogInformation("Pipeline ETL completo.");
        }
    }
}
