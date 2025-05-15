using System.ComponentModel.DataAnnotations;

namespace Tutorial9.Model
{
    public class AddProductRequest
    {
        [Required]
        public int IdProduct { get; set; }
        [Required]
        public int IdWarehouse { get; set; }
        [Required]
        public int Amount { get; set; }
    }
}