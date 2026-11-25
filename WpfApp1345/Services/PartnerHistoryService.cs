
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using WpfApp1345.Data;
using WpfApp1345.Models.MasterFloor;

namespace WpfApp1345.Services
{
    public static class PartnerHistoryService
    {
        // Получение истории продаж конкретного партнёра.
        public static List<PartnerSaleHistoryItem> GetHistory(
            int partnerId)
        {
            using (var db = new MasterFloorDbContext())
            {
                return db.PartnerSales
                    .AsNoTracking()
                    .Where(sale => sale.PartnerId == partnerId)
                    .Select(sale => new PartnerSaleHistoryItem
                    {
                        ProductName = sale.Product.Name,
                        Quantity = sale.Quantity,
                        SaleDate = sale.SaleDate
                    })
                    .OrderByDescending(sale => sale.SaleDate)
                    .ToList();
            }
        }
    }
}
