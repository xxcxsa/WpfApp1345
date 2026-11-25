
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using WpfApp1345.Data;
using WpfApp1345.Models;

namespace WpfApp1345.Services
{
    public static class DatabaseControl
    {
        // =========================================
        // РАБОТА С ТЕЛЕФОНАМИ
        // =========================================

        public static List<Phone> GetPhonesForView()
        {
            using (DbAppContext ctx = new DbAppContext())
            {
                return ctx.Phones
                    .Include(p => p.CompanyEntity)
                    .OrderBy(p => p.Id)
                    .ToList();
            }
        }

        public static List<Company> GetCompanies()
        {
            using (DbAppContext ctx = new DbAppContext())
            {
                return ctx.Companies
                    .OrderBy(c => c.Title)
                    .ToList();
            }
        }

        public static void AddPhone(Phone phone)
        {
            using (DbAppContext ctx = new DbAppContext())
            {
                ctx.Phones.Add(phone);
                ctx.SaveChanges();
            }
        }

        public static void UpdatePhone(Phone phone)
        {
            using (DbAppContext ctx = new DbAppContext())
            {
                Phone? existingPhone =
                    ctx.Phones.FirstOrDefault(p => p.Id == phone.Id);

                if (existingPhone == null)
                    return;

                existingPhone.Title = phone.Title;
                existingPhone.Price = phone.Price;
                existingPhone.CompanyId = phone.CompanyId;
                existingPhone.Definition = phone.Definition;
                existingPhone.ImageSource = phone.ImageSource;

                ctx.SaveChanges();
            }
        }

        public static void DeletePhone(Phone phone)
        {
            using (DbAppContext ctx = new DbAppContext())
            {
                Phone? existingPhone =
                    ctx.Phones.FirstOrDefault(p => p.Id == phone.Id);

                if (existingPhone == null)
                    return;

                ctx.Phones.Remove(existingPhone);
                ctx.SaveChanges();
            }
        }

        // =========================================
        // РАБОТА С СОТРУДНИКАМИ
        // База данных EmpDB
        // =========================================

        public static List<Employee> GetEmployeesList()
        {
            using (EmployeeDbContext ctx = new EmployeeDbContext())
            {
                return ctx.Employee
                    .OrderBy(e => e.Id)
                    .ToList();
            }
        }
    }
}
