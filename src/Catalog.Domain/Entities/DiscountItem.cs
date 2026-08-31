namespace Catalog.Domain.Entities;

/// <summary>Discount applied to the catalog over a date range.</summary>
public class DiscountItem
{
    public int Id { get; set; }

    public double Size { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }
}
