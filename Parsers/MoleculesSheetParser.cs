using ClosedXML.Excel;
using MolportQuoteImporter.Constants;
using MolportQuoteImporter.Models;
using System.Collections.Generic;

namespace MolportQuoteImporter.Services;

public class MoleculesSheetParser
{
    private readonly QuoteValidationService _validationService = new();

    public List<QuoteItem> Parse(XLWorkbook workbook)
    {
        var items = new List<QuoteItem>();

        var worksheet = workbook.Worksheet("Molecules");
        var lastRow = worksheet.LastRowUsed().RowNumber();

        for (var row = QuoteConstants.FirstDataRow; row <= lastRow; row++)
        {
            var molportId = ExcelCellReader.GetString(worksheet, row, QuoteColumns.MolportId);

            if (string.IsNullOrWhiteSpace(molportId))
            {
                continue;
            }

            var item = new QuoteItem
            {
                RowNumber = row - QuoteConstants.FirstDataRow + 1,

                MolportId = molportId,
                ProductId = ExcelCellReader.GetString(worksheet, row, QuoteColumns.ProductId),
                Supplier = ExcelCellReader.GetString(worksheet, row, QuoteColumns.Supplier),
                CatalogueNumber = ExcelCellReader.GetString(worksheet, row, QuoteColumns.CatalogueNumber),
                DeliveryTimeBusinessDays = ExcelCellReader.GetInt(worksheet, row, QuoteColumns.DeliveryTimeBusinessDays),
                SearchCriteria = ExcelCellReader.GetString(worksheet, row, QuoteColumns.SearchCriteria),
                MatchType = ExcelCellReader.GetString(worksheet, row, QuoteColumns.MatchType),
                Smiles = ExcelCellReader.GetString(worksheet, row, QuoteColumns.Smiles),
                MolecularWeight = ExcelCellReader.GetDecimal(worksheet, row, QuoteColumns.MolecularWeight),
                Unit = ExcelCellReader.GetString(worksheet, row, QuoteColumns.Unit),
                UnitPriceUsd = ExcelCellReader.GetDecimal(worksheet, row, QuoteColumns.UnitPriceUsd),
                Quantity = ExcelCellReader.GetInt(worksheet, row, QuoteColumns.Quantity),
                DiscountUsd = ExcelCellReader.GetDecimal(worksheet, row, QuoteColumns.DiscountUsd),
                NetPriceUsd = ExcelCellReader.GetDecimal(worksheet, row, QuoteColumns.NetPriceUsd),
                Purity = ExcelCellReader.GetString(worksheet, row, QuoteColumns.Purity),
                Iupac = ExcelCellReader.GetString(worksheet, row, QuoteColumns.Iupac),
                Compliance = ExcelCellReader.GetString(worksheet, row, QuoteColumns.Compliance),
            };

            _validationService.Validate(item);
            items.Add(item);
        }

        return items;
    }
}