using MolportQuoteImporter.Models;
using System.Collections.Generic;

namespace MolportQuoteImporter.Services;

public class QuoteValidationService
{
    public void Validate(QuoteItem item)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(item.MolportId))
        { 
            errors.Add("Molport ID is missing");
        }

        if (string.IsNullOrWhiteSpace(item.Supplier))
        { 
            errors.Add("Supplier is missing");
        }

        if (string.IsNullOrWhiteSpace(item.CatalogueNumber))
        { 
            errors.Add("Catalogue number is missing");
        }

        if (!item.UnitPriceUsd.HasValue || item.UnitPriceUsd < 0)
        { 
            errors.Add("Unit price is invalid");
        }

        if (!item.Quantity.HasValue || item.Quantity <= 0)
        { 
            errors.Add("Quantity is invalid");
        }

        if (!item.NetPriceUsd.HasValue || item.NetPriceUsd < 0)
        { 
            errors.Add("Net price is invalid");
        }

        if (string.IsNullOrWhiteSpace(item.Unit))
        {
            errors.Add("Unit is missing");
        }

        if (string.IsNullOrWhiteSpace(item.Purity))
        {
            errors.Add("Purity is missing");
        }

        if (item.CalculatedLineTotalUsd.HasValue &&
            item.NetPriceUsd.HasValue &&
            item.NetPriceUsd > item.CalculatedLineTotalUsd)
        {
            errors.Add("Net price exceeds calculated line total");
        }

        item.IsValid = errors.Count == 0;
        item.ErrorMessage = string.Join("; ", errors);
    }
}