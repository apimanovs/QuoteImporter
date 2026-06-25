using ClosedXML.Excel;
using MolportQuoteImporter.Models;

namespace MolportQuoteImporter.Services;

public class QuoteImportService
{
    private readonly MoleculesSheetParser _moleculesSheetParser = new();
    private readonly QuoteSummaryParser _quoteSummaryParser = new();
    private readonly ShippingLimitationsSheetParser _shippingLimitationsSheetParser = new();

    public QuoteImportResult Import(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);

        return new QuoteImportResult
        {
            Items = _moleculesSheetParser.Parse(workbook),
            Summary = _quoteSummaryParser.Parse(workbook),
            ShippingLimitations = _shippingLimitationsSheetParser.Parse(workbook)
        };
    }
}