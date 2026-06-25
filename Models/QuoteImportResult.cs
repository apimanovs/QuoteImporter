using System.Collections.Generic;
using MolportQuoteImporter.Models;

public class QuoteImportResult
{
	public List<QuoteItem> Items { get; set; } = new();
	public QuoteSummary Summary { get; set; } = new();
	public List<ShippingLimitation> ShippingLimitations { get; set; } = new();
}