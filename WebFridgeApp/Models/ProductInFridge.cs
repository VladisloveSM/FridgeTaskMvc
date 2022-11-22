using System;
using System.ComponentModel.DataAnnotations;

namespace WebFridgeApp.Models
{
    public class ProductInFridge
    {
        public Guid Id { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public string Name { get; set; }

        public Guid Fridge_id { get; set; }

        public Guid Product_id { get; set; }
    }
}
