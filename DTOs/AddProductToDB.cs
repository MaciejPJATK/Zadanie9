using System.ComponentModel.DataAnnotations;

namespace Tutorial9.Model;

public class AddProductToDB
{
    public int IdProduct { get; set; }
    public int IdWarehouse { get; set; }
    public int Amount { get; set; }
    public DateTime DateAdded { get; set; }
    
    public float Price { get; set; }
    public int IdOrder { get; set; }
}