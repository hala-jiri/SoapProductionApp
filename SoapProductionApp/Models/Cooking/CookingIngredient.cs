using SoapProductionApp.Models.Cooking;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class CookingIngredient
{
    public int Id { get; set; }

    [ForeignKey("Cooking")]
    public int CookingId { get; set; }

    [JsonIgnore] // Zabrání serializaci cyklické reference
    public Cooking Cooking { get; set; }

    public string IngredientName { get; set; } // Název ingredience

    public decimal QuantityUsed { get; set; } // Kolik bylo použito

    public string Unit { get; set; } // Jednotka (L, g, ml, ...)

    public decimal Cost { get; set; }

    public DateTime ExpirationDate { get; set; } // Expirace suroviny
}

