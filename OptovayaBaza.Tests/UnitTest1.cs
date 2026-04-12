using NUnit.Framework;
using OptovayaBaza.Models;

namespace OptovayaBaza.Tests
{
    [TestFixture]
    
    public class ModelsTests
    {
        /// <summary>
        ///ТЕСТ MATERIAL
        /// </summary>
        [Test]
        public void Material_ValidData_PropertiesSetCorrectly()
        {
            var material = new Material
            {
                Id = 1,
                Name = "Цемент",
                Unit = "кг",
                Price = 500.50m,
                Stock = 100
            };

            Assert.That(material.Id, Is.EqualTo(1));
            Assert.That(material.Name, Is.EqualTo("Цемент"));
            Assert.That(material.Unit, Is.EqualTo("кг"));
            Assert.That(material.Price, Is.EqualTo(500.50m));
            Assert.That(material.Stock, Is.EqualTo(100));
        }

        [Test]
        public void Material_PriceCanBeZero()
        {
            var material = new Material { Price = 0 };
            Assert.That(material.Price, Is.EqualTo(0));
        }

        [Test]
        public void Material_StockCanBeZero()
        {
            var material = new Material { Stock = 0 };
            Assert.That(material.Stock, Is.EqualTo(0));
        }

        /// <summary>
        ///ТЕСТЫ USER
        /// </summary>
        [Test]
        public void User_ValidData_PropertiesSetCorrectly()
        {
            var user = new User
            {
                Id = 1,
                Login = "admin",
                PasswordHash = "admin123",
                Role = "Admin",
                OrganizationName = "ООО Рома",
                ContactPerson = "Иванов Иван",
                Phone = "+7(999)123-45-67"
            };

            Assert.That(user.Id, Is.EqualTo(1));
            Assert.That(user.Login, Is.EqualTo("admin"));
            Assert.That(user.PasswordHash, Is.EqualTo("admin123"));
            Assert.That(user.Role, Is.EqualTo("Admin"));
            Assert.That(user.OrganizationName, Is.EqualTo("ООО Рома"));
            Assert.That(user.ContactPerson, Is.EqualTo("Иванов Иван"));
            Assert.That(user.Phone, Is.EqualTo("+7(999)123-45-67"));
        }

        [Test]
        public void User_RoleCanBeWholesaler()
        {
            var user = new User { Role = "Wholesaler" };
            Assert.That(user.Role, Is.EqualTo("Wholesaler"));
        }

        [Test]
        public void User_RoleCanBeAdmin()
        {
            var user = new User { Role = "Admin" };
            Assert.That(user.Role, Is.EqualTo("Admin"));
        }


        /// <summary>
        ///ТЕСТЫ WHOLESALER 
        /// </summary>
        [Test]
        public void Wholesaler_ValidData_PropertiesSetCorrectly()
        {
            var wholesaler = new Wholesaler
            {
                Id = 1,
                OrganizationName = "ООО Рома",
                INN = "1234567890",
                Phone = "+7(999)111-22-33",
                LegalAddress = "г. Владимир, ул. Ленина, д.1",
                UserId = 5
            };

            Assert.That(wholesaler.Id, Is.EqualTo(1));
            Assert.That(wholesaler.OrganizationName, Is.EqualTo("ООО Рома"));
            Assert.That(wholesaler.INN, Is.EqualTo("1234567890"));
            Assert.That(wholesaler.Phone, Is.EqualTo("+7(999)111-22-33"));
            Assert.That(wholesaler.LegalAddress, Is.EqualTo("г. Владимир, ул. Ленина, д.1"));
            Assert.That(wholesaler.UserId, Is.EqualTo(5));
        }

        [Test]
        public void Wholesaler_INNCanBeEmpty()
        {
            var wholesaler = new Wholesaler { INN = "" };
            Assert.That(wholesaler.INN, Is.EqualTo(""));
        }

        [Test]
        public void Wholesaler_PhoneCanBeEmpty()
        {
            var wholesaler = new Wholesaler { Phone = "" };
            Assert.That(wholesaler.Phone, Is.EqualTo(""));
        }
    }
}