using System;
using System.ComponentModel.DataAnnotations;

namespace WebFridgeApp.Models
{
    public class Product
    {
        public Guid Id  { get; set; }

        public string Name { get; set; }

        [Range(1, int.MaxValue)]
        public int? Default_Quantity { get; set; }
    }
}
