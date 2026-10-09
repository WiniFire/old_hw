using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StationeryShop
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public Product Copy()
        {
            return (Product)MemberwiseClone();
        }
    }

    public class OrderItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [XmlIgnore]
        public decimal Total { get { return Price * Quantity; } }
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Customer { get; set; }
        public List<OrderItem> Items { get; set; }
        [XmlIgnore]
        public decimal Total { get { return Items.Sum(item => item.Total); } }

        public Order()
        {
            Items = new List<OrderItem>();
        }
    }

    public class ShopData
    {
        public int NextProductId { get; set; }
        public int NextOrderId { get; set; }
        public List<Product> Products { get; set; }
        public List<Order> Orders { get; set; }

        public ShopData()
        {
            NextProductId = 1;
            NextOrderId = 1;
            Products = new List<Product>();
            Orders = new List<Order>();
        }
    }
}
