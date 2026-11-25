
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using WpfApp1345.Data;
using WpfApp1345.Models.MasterFloor;

namespace WpfApp1345.Services
{
    public static class MaterialCalculatorService
    {
        // Получение типов продукции.
        public static List<ProductType> GetProductTypes()
        {
            using (var db = new MasterFloorDbContext())
            {
                return db.ProductTypes
                    .AsNoTracking()
                    .OrderBy(type => type.Name)
                    .ToList();
            }
        }

        // Получение типов материалов.
        public static List<MaterialType> GetMaterialTypes()
        {
            using (var db = new MasterFloorDbContext())
            {
                return db.MaterialTypes
                    .AsNoTracking()
                    .OrderBy(type => type.Name)
                    .ToList();
            }
        }

        // Расчёт необходимого количества материала.
        // При некорректных данных возвращает -1.
        public static int CalculateRequiredMaterial(
            int productTypeId,
            int materialTypeId,
            int quantity,
            double width,
            double length)
        {
            if (productTypeId <= 0 ||
                materialTypeId <= 0 ||
                quantity <= 0 ||
                double.IsNaN(width) ||
                double.IsInfinity(width) ||
                double.IsNaN(length) ||
                double.IsInfinity(length) ||
                width <= 0 ||
                length <= 0)
            {
                return -1;
            }

            try
            {
                using (var db = new MasterFloorDbContext())
                {
                    ProductType? productType =
                        db.ProductTypes
                            .AsNoTracking()
                            .FirstOrDefault(
                                type => type.Id == productTypeId);

                    MaterialType? materialType =
                        db.MaterialTypes
                            .AsNoTracking()
                            .FirstOrDefault(
                                type => type.Id == materialTypeId);

                    // Проверка существования типов.
                    if (productType == null ||
                        materialType == null)
                    {
                        return -1;
                    }

                    // Проверка коэффициента и процента брака.
                    if (productType.Coefficient <= 0 ||
                        materialType.ScrapRate < 0 ||
                        materialType.ScrapRate >= 1)
                    {
                        return -1;
                    }

                    decimal productWidth = (decimal)width;
                    decimal productLength = (decimal)length;

                    // Расход материала на всю продукцию без брака.
                    decimal baseMaterial =
                        productWidth *
                        productLength *
                        productType.Coefficient *
                        quantity;

                    // Увеличение количества с учётом брака.
                    decimal materialWithScrap =
                        baseMaterial *
                        (1 + materialType.ScrapRate);

                    // Округление вверх до целой единицы.
                    decimal result =
                        decimal.Ceiling(materialWithScrap);

                    if (result > int.MaxValue)
                    {
                        return -1;
                    }

                    return (int)result;
                }
            }
            catch
            {
                // Некорректные или слишком большие значения.
                return -1;
            }
        }
    }
}
