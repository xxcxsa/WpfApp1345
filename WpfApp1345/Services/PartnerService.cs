
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using WpfApp1345.Data;
using WpfApp1345.Models.MasterFloor;

namespace WpfApp1345.Services
{
    public static class PartnerService
    {
        // Получение списка партнеров и расчет скидок.
        public static List<PartnerListItem> GetPartnersForView()
        {
            using (var db = new MasterFloorDbContext())
            {
                List<PartnerListItem> partners = db.Partners
                    .AsNoTracking()
                    .Select(partner => new PartnerListItem
                    {
                        Id = partner.Id,
                        PartnerTypeName = partner.PartnerType.Name,
                        Name = partner.Name,
                        Director = partner.Director,
                        Phone = partner.Phone,
                        Rating = partner.Rating,

                        TotalSales = db.PartnerSales
                            .Where(sale => sale.PartnerId == partner.Id)
                            .Select(sale => (int?)sale.Quantity)
                            .Sum() ?? 0
                    })
                    .OrderBy(partner => partner.Name)
                    .ToList();

                foreach (PartnerListItem partner in partners)
                {
                    partner.DiscountPercent =
                        CalculateDiscount(partner.TotalSales);
                }

                return partners;
            }
        }

        // Получение типов партнеров для выпадающего списка.
        public static List<PartnerType> GetPartnerTypes()
        {
            using (var db = new MasterFloorDbContext())
            {
                return db.PartnerTypes
                    .AsNoTracking()
                    .OrderBy(type => type.Name)
                    .ToList();
            }
        }

        // Получение одного партнера по идентификатору.
        public static Partner GetPartnerById(int partnerId)
        {
            using (var db = new MasterFloorDbContext())
            {
                Partner? partner = db.Partners
                    .AsNoTracking()
                    .FirstOrDefault(p => p.Id == partnerId);

                if (partner == null)
                {
                    throw new InvalidOperationException(
                        "Выбранный партнёр не найден. " +
                        "Обновите список и повторите попытку.");
                }

                return partner;
            }
        }

        // Добавление нового партнера.
        public static void AddPartner(Partner partner)
        {
            using (var db = new MasterFloorDbContext())
            {
                bool innExists = db.Partners
                    .Any(p => p.INN == partner.INN);

                if (innExists)
                {
                    throw new InvalidOperationException(
                        "Партнёр с таким ИНН уже существует. " +
                        "Проверьте введённый ИНН.");
                }

                db.Partners.Add(partner);
                db.SaveChanges();
            }
        }

        // Редактирование существующего партнера.
        public static void UpdatePartner(Partner partner)
        {
            using (var db = new MasterFloorDbContext())
            {
                Partner? existingPartner = db.Partners
                    .FirstOrDefault(p => p.Id == partner.Id);

                if (existingPartner == null)
                {
                    throw new InvalidOperationException(
                        "Изменяемый партнёр не найден в базе данных.");
                }

                bool innExists = db.Partners.Any(
                    p => p.INN == partner.INN &&
                         p.Id != partner.Id);

                if (innExists)
                {
                    throw new InvalidOperationException(
                        "Другой партнёр уже зарегистрирован с таким ИНН.");
                }

                existingPartner.PartnerTypeId = partner.PartnerTypeId;
                existingPartner.Name = partner.Name;
                existingPartner.Director = partner.Director;
                existingPartner.Email = partner.Email;
                existingPartner.Phone = partner.Phone;
                existingPartner.LegalAddress = partner.LegalAddress;
                existingPartner.INN = partner.INN;
                existingPartner.Rating = partner.Rating;

                db.SaveChanges();
            }
        }

        // Расчет индивидуальной скидки.
        public static int CalculateDiscount(int totalSales)
        {
            if (totalSales < 10000)
                return 0;

            if (totalSales < 50000)
                return 5;

            if (totalSales <= 300000)
                return 10;

            return 15;
        }
    }
}
