using CsvHelper;
using CsvHelper.Configuration;
using ETLProyecto.Models;
using ETLProyecto.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ETLProyecto.Services.Implementations
{
    public class CsvReaderService : ICsvReaderService
    {
        private readonly IConfiguration _configuration;

        public CsvReaderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<Producto>> ReadProductsAsync()
        {
            var productos = new List<Producto>();
            string filePath = _configuration["CsvFiles:Products"] ?? "CsvFiles/products.csv";

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"CSV file not found: {filePath}");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                BadDataFound = null
            };

            using var reader = new StreamReader(filePath, Encoding.UTF8);
            using var csv = new CsvReader(reader, config);

            await foreach (var record in csv.GetRecordsAsync<Producto>())
            {
                if (record.ProductID <= 0 || string.IsNullOrWhiteSpace(record.ProductName)) continue;
                if (record.Price <= 0 || record.Stock < 0) continue;
                productos.Add(record);
            }

            return productos;
        }

        public async Task<List<Cliente>> ReadClientesAsync()
        {
            var clientes = new List<Cliente>();
            string filePath = _configuration["CsvFiles:Clientes"] ?? "CsvFiles/customers.csv";

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"CSV file not found: {filePath}");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                BadDataFound = null
            };

            using var reader = new StreamReader(filePath, Encoding.UTF8);
            using var csv = new CsvReader(reader, config);

            await foreach (var record in csv.GetRecordsAsync<Cliente>())
            {
                if (record.CustomerID <= 0 || string.IsNullOrWhiteSpace(record.FirstName)) continue;
                clientes.Add(record);
            }

            return clientes;
        }

        public async Task<List<Order>> ReadOrdersAsync()
        {
            var orders = new List<Order>();
            string filePath = _configuration["CsvFiles:Orders"] ?? "CsvFiles/orders.csv";

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"CSV file not found: {filePath}");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                BadDataFound = null
            };

            using var reader = new StreamReader(filePath, Encoding.UTF8);
            using var csv = new CsvReader(reader, config);

            await foreach (var record in csv.GetRecordsAsync<Order>())
            {
                if (record.OrderID <= 0 || record.CustomerID <= 0) continue;
                orders.Add(record);
            }

            return orders;
        }

        public async Task<List<OrderDetail>> ReadOrderDetailsAsync()
        {
            var details = new List<OrderDetail>();
            string filePath = _configuration["CsvFiles:OrderDetails"] ?? "CsvFiles/order_details.csv";

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"CSV file not found: {filePath}");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                BadDataFound = null
            };

            using var reader = new StreamReader(filePath, Encoding.UTF8);
            using var csv = new CsvReader(reader, config);

            await foreach (var record in csv.GetRecordsAsync<OrderDetail>())
            {
                if (record.OrderID <= 0 || record.ProductID <= 0) continue;
                if (record.Quantity <= 0 || record.TotalPrice <= 0) continue;
                details.Add(record);
            }

            return details;
        }
    }
}
