using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace StationeryShop
{
    public class ShopStore
    {
        private readonly string path;
        private ShopData data;
        private readonly XmlSerializer serializer = new XmlSerializer(typeof(ShopData));

        public ShopStore(string filePath)
        {
            path = Path.GetFullPath(filePath);
            if (File.Exists(path))
            {
                using (var reader = XmlReader.Create(path, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }))
                    data = (ShopData)serializer.Deserialize(reader);
                if (data == null || data.Products == null || data.Orders == null)
                    throw new InvalidDataException("Некорректный файл данных магазина.");
            }
            else
            {
                data = new ShopData();
                data.Products.Add(new Product { Id = 1, Name = "Тетрадь 48 листов", Category = "Тетради", Price = 45m, Stock = 50 });
                data.Products.Add(new Product { Id = 2, Name = "Ручка шариковая", Category = "Письменные принадлежности", Price = 20m, Stock = 80 });
                data.Products.Add(new Product { Id = 3, Name = "Карандаш HB", Category = "Письменные принадлежности", Price = 15m, Stock = 60 });
                data.Products.Add(new Product { Id = 4, Name = "Папка А4", Category = "Папки", Price = 70m, Stock = 25 });
                data.Products.Add(new Product { Id = 5, Name = "Набор маркеров", Category = "Рисование", Price = 180m, Stock = 15 });
                data.NextProductId = 6;
                Save(data);
            }
        }

        public List<Product> GetProducts()
        {
            return data.Products.Select(p => p.Copy()).OrderBy(p => p.Id).ToList();
        }

        public List<Order> GetOrders()
        {
            return Clone().Orders.OrderByDescending(o => o.Id).ToList();
        }

        public void SaveProduct(Product product)
        {
            if (product == null || string.IsNullOrWhiteSpace(product.Name) || product.Name.Trim().Length > 100)
                throw new ArgumentException("Название должно содержать от 1 до 100 символов.");
            if (string.IsNullOrWhiteSpace(product.Category) || product.Category.Trim().Length > 60)
                throw new ArgumentException("Категория должна содержать от 1 до 60 символов.");
            if (product.Price < 0.01m || product.Price > 1000000m || product.Stock < 0 || product.Stock > 1000000)
                throw new ArgumentException("Цена или остаток вне допустимого диапазона.");

            var next = Clone();
            var copy = product.Copy();
            copy.Name = copy.Name.Trim();
            copy.Category = copy.Category.Trim();
            copy.Price = decimal.Round(copy.Price, 2);
            if (copy.Id == 0)
            {
                copy.Id = next.NextProductId++;
                next.Products.Add(copy);
            }
            else
            {
                int index = next.Products.FindIndex(p => p.Id == copy.Id);
                if (index < 0) throw new InvalidOperationException("Товар не найден.");
                next.Products[index] = copy;
            }
            Save(next);
            data = next;
        }

        public void DeleteProduct(int id)
        {
            var next = Clone();
            if (next.Products.RemoveAll(p => p.Id == id) == 0)
                throw new InvalidOperationException("Товар не найден.");
            Save(next);
            data = next;
        }

        public Order Checkout(string customer, IDictionary<int, int> cart)
        {
            if (string.IsNullOrWhiteSpace(customer) || customer.Trim().Length > 100)
                throw new ArgumentException("Введите имя покупателя (до 100 символов).");
            if (cart == null || cart.Count == 0)
                throw new InvalidOperationException("Корзина пуста.");

            var next = Clone();
            var order = new Order { Id = next.NextOrderId++, CreatedAt = DateTime.Now, Customer = customer.Trim() };
            foreach (var entry in cart)
            {
                Product product = next.Products.FirstOrDefault(p => p.Id == entry.Key);
                if (product == null) throw new InvalidOperationException("Один из товаров больше не доступен.");
                if (entry.Value <= 0 || entry.Value > product.Stock)
                    throw new InvalidOperationException("Недостаточно товара: " + product.Name);
                order.Items.Add(new OrderItem { ProductId = product.Id, Name = product.Name, Price = product.Price, Quantity = entry.Value });
                product.Stock -= entry.Value;
            }
            next.Orders.Add(order);
            Save(next);
            data = next;
            return GetOrders().First(o => o.Id == order.Id);
        }

        private ShopData Clone()
        {
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, data);
                stream.Position = 0;
                return (ShopData)serializer.Deserialize(stream);
            }
        }

        private void Save(ShopData next)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string temp = path + ".tmp";
            try
            {
                using (var stream = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None))
                    serializer.Serialize(stream, next);
                if (File.Exists(path)) File.Replace(temp, path, null);
                else File.Move(temp, path);
            }
            finally
            {
                if (File.Exists(temp)) File.Delete(temp);
            }
        }
    }
}
