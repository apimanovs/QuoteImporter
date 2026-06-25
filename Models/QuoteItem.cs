using System;

namespace MolportQuoteImporter.Models;

public class QuoteItem
{
    public int RowNumber { get; set; }

    public string MolportId { get; set; } = "";
    public string ProductId { get; set; } = "";
    public string Supplier { get; set; } = "";
    public string CatalogueNumber { get; set; } = "";

    public int? DeliveryTimeBusinessDays { get; set; }

    public string SearchCriteria { get; set; } = "";
    public string MatchType { get; set; } = "";
    public string Smiles { get; set; } = "";

    public decimal? MolecularWeight { get; set; }

    public string Unit { get; set; } = "";
    public decimal? UnitPriceUsd { get; set; }
    public int? Quantity { get; set; }
    public decimal? DiscountUsd { get; set; }
    public decimal? NetPriceUsd { get; set; }

    public string Purity { get; set; } = "";
    public string Iupac { get; set; } = "";
    public string Compliance { get; set; } = "";

    public decimal? CalculatedLineTotalUsd =>
        UnitPriceUsd.HasValue && Quantity.HasValue
            ? UnitPriceUsd.Value * Quantity.Value - (DiscountUsd ?? 0)
            : null;

    public bool IsValid { get; set; } = true;
    public string ErrorMessage { get; set; } = "";
}