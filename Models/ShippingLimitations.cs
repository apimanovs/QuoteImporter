namespace MolportQuoteImporter.Models;

public class ShippingLimitation
{
    public int RowNumber { get; set; }

    public string MolportId { get; set; } = "";
    public string Supplier { get; set; } = "";
    public string CatalogueNumber { get; set; } = "";

    public string CountryOfOrigin { get; set; } = "";
    public string Unit { get; set; } = "";
    public string UnNumber { get; set; } = "";

    public string HazardousClass { get; set; } = "";
    public string PackingGroup { get; set; } = "";

    public string ShippingLimitations { get; set; } = "";
    public string CompoundState { get; set; } = "";
    public string Solubility { get; set; } = "";
}