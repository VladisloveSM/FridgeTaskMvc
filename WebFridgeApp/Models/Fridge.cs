using System;
using System.ComponentModel.DataAnnotations;

namespace WebFridgeApp.Models
{
    public class Fridge
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Owner_Name { get; set; }

        public Guid Model_id { get; set; }

        public string Model { get; set; }
    }
}
