using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace OptovayaBaza.Models
{
    public class OptovayaBazaContext : DbContext
    {
        public OptovayaBazaContext() : base("name=OptovayaBazaContext") 
        {
            SeedData();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Wholesaler> Wholesalers { get; set; }


        private void SeedData()
        {
            //Проверка, есть ли уже данные в базе
            if (Users.Any())
                return;

            //Добавление пользователей
            var adminPasswordHash = HashPassword("admin123");
            var optPasswordHash = HashPassword("opt123");

            var users = new List<User>
            {
                new User
                {
                    Login = "admin",
                    PasswordHash = adminPasswordHash,
                    Role = "Admin",
                    OrganizationName = "Администрация",
                    ContactPerson = "Иванов Иван Иванович",
                    Phone = "+7 (495) 123-45-67"
                },
                new User
                {
                    Login = "optovik1",
                    PasswordHash = optPasswordHash,
                    Role = "Wholesaler",
                    OrganizationName = "ОптТорг",
                    ContactPerson = "Петров Петр Петрович",
                    Phone = "+7 (495) 765-43-21"
                },
                new User
                {
                    Login = "optovik2",
                    PasswordHash = optPasswordHash,
                    Role = "Wholesaler",
                    OrganizationName = "СтройОпт",
                    ContactPerson = "Сидоров Сидор Сидорович",
                    Phone = "+7 (495) 987-65-43"
                },
                new User
                {
                    Login = "optovik3",
                    PasswordHash = optPasswordHash,
                    Role = "Wholesaler",
                    OrganizationName = "ЭлектроСнаб",
                    ContactPerson = "Кузнецова Анна Михайловна",
                    Phone = "+7 (495) 111-22-33"
                }
            };

            Users.AddRange(users);
            SaveChanges();

            //Добавление оптовиков
            var optovik1User = Users.First(u => u.Login == "optovik1");
            var optovik2User = Users.First(u => u.Login == "optovik2");
            var optovik3User = Users.First(u => u.Login == "optovik3");

            var wholesalers = new List<Wholesaler>
            {
                new Wholesaler
                {
                    OrganizationName = "ОптТорг",
                    INN = "7701234567",
                    Phone = "+7 (495) 765-43-21",
                    LegalAddress = "г. Москва, ул. Тверская, д. 10",
                    UserId = optovik1User.Id
                },
                new Wholesaler
                {
                    OrganizationName = "СтройОпт",
                    INN = "7702345678",
                    Phone = "+7 (495) 987-65-43",
                    LegalAddress = "г. Москва, ул. Ленина, д. 25",
                    UserId = optovik2User.Id
                },
                new Wholesaler
                {
                    OrganizationName = "ЭлектроСнаб",
                    INN = "7703456789",
                    Phone = "+7 (495) 111-22-33",
                    LegalAddress = "г. Санкт-Петербург, Невский пр-т, д. 5",
                    UserId = optovik3User.Id
                }
            };

            Wholesalers.AddRange(wholesalers);
            SaveChanges();

            //Добавление материалов
            var materials = new List<Material>
            {
                new Material { Name = "Цемент М500", Unit = "кг", Price = 35.50m, Stock = 10000 },
                new Material { Name = "Песок строительный", Unit = "м³", Price = 1200.00m, Stock = 500 },
                new Material { Name = "Кирпич красный", Unit = "шт", Price = 15.00m, Stock = 50000 },
                new Material { Name = "Арматура 12 мм", Unit = "м", Price = 85.00m, Stock = 3000 },
                new Material { Name = "Доска обрезная 50х150", Unit = "м³", Price = 14500.00m, Stock = 120 },
                new Material { Name = "Гвозди 100 мм", Unit = "кг", Price = 120.00m, Stock = 800 },
                new Material { Name = "Краска белая", Unit = "л", Price = 450.00m, Stock = 400 },
                new Material { Name = "Плитка керамическая", Unit = "м²", Price = 890.00m, Stock = 600 }
            };

            Materials.AddRange(materials);
            SaveChanges();

            //Добавление транзакций
            var adminId = Users.First(u => u.Login == "admin").Id;
            var optovik1 = Wholesalers.First(w => w.OrganizationName == "ОптТорг");
            var optovik2 = Wholesalers.First(w => w.OrganizationName == "СтройОпт");

            var cement = materials[0];
            var sand = materials[1];
            var brick = materials[2];
            var rebar = materials[3];
            var board = materials[4];
            var paint = materials[6];

            var transactions = new List<Transaction>
            {
                //Поставки
                new Transaction
                {
                    DateTime = DateTime.Now.AddDays(-30),
                    Type = "Income",
                    Quantity = 2000,
                    Comment = "Поставка цемента от завода",
                    MaterialId = cement.Id,
                    AdminId = adminId,
                    WholesalerId = null
                },
                new Transaction
                {
                    DateTime = DateTime.Now.AddDays(-25),
                    Type = "Income",
                    Quantity = 100,
                    Comment = "Поставка песка с карьера",
                    MaterialId = sand.Id,
                    AdminId = adminId,
                    WholesalerId = null
                },
                new Transaction
                {
                    DateTime = DateTime.Now.AddDays(-20),
                    Type = "Income",
                    Quantity = 10000,
                    Comment = "Поставка кирпича с завода",
                    MaterialId = brick.Id,
                    AdminId = adminId,
                    WholesalerId = null
                },
                new Transaction
                {
                    DateTime = DateTime.Now.AddDays(-18),
                    Type = "Income",
                    Quantity = 500,
                    Comment = "Поставка арматуры",
                    MaterialId = rebar.Id,
                    AdminId = adminId,
                    WholesalerId = null
                },
                
                //Расходы_материал
                new Transaction
                {
                    DateTime = DateTime.Now.AddDays(-15),
                    Type = "Expense",
                    Quantity = 500,
                    Comment = "Продажа цемента ОптТорг",
                    MaterialId = cement.Id,
                    AdminId = adminId,
                    WholesalerId = optovik1.Id
                },
                new Transaction
                {
                    DateTime = DateTime.Now.AddDays(-10),
                    Type = "Expense",
                    Quantity = 2000,
                    Comment = "Продажа кирпича СтройОпт",
                    MaterialId = brick.Id,
                    AdminId = adminId,
                    WholesalerId = optovik2.Id
                },
                new Transaction
                {
                    DateTime = DateTime.Now.AddDays(-5),
                    Type = "Expense",
                    Quantity = 300,
                    Comment = "Продажа арматуры ОптТорг",
                    MaterialId = rebar.Id,
                    AdminId = adminId,
                    WholesalerId = optovik1.Id
                },
                new Transaction
                {
                    DateTime = DateTime.Now.AddDays(-3),
                    Type = "Expense",
                    Quantity = 20,
                    Comment = "Продажа доски СтройОпт",
                    MaterialId = board.Id,
                    AdminId = adminId,
                    WholesalerId = optovik2.Id
                },
                new Transaction
                {
                    DateTime = DateTime.Now.AddDays(-1),
                    Type = "Expense",
                    Quantity = 100,
                    Comment = "Продажа краски ОптТорг",
                    MaterialId = paint.Id,
                    AdminId = adminId,
                    WholesalerId = optovik1.Id
                },
                new Transaction
                {
                    DateTime = DateTime.Now,
                    Type = "Expense",
                    Quantity = 50,
                    Comment = "Продажа цемента СтройОпт",
                    MaterialId = cement.Id,
                    AdminId = adminId,
                    WholesalerId = optovik2.Id
                }
            };

            Transactions.AddRange(transactions);
            SaveChanges();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}