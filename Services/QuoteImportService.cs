using ClosedXML.Excel;
using MolportQuoteImporter.Models;
using MolportQuoteImporter.Constants;
using System.Collections.Generic;
using System.Globalization;

namespace MolportQuoteImporter.Services;

public class QuoteImportService
{
    private readonly QuoteValidationService _validationService = new();

    public List<QuoteItem> Import(string filePath)
    {
        var items = new List<QuoteItem>();

        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet("Molecules");

        var lastRow = worksheet.LastRowUsed().RowNumber();

        for (var row = QuoteConstants.FirstDataRow; row <= lastRow; row++)
        {
            var molportId = GetString(worksheet, row, 2);

            if (string.IsNullOrWhiteSpace(molportId))
            { 
                continue;
            }

            var item = new QuoteItem
            {
                RowNumber = row,

                MolportId = molportId,
                ProductId = GetString(worksheet, row, 3),
                Supplier = GetString(worksheet, row, 4),
                CatalogueNumber = GetString(worksheet, row, 5),

                DeliveryTimeBusinessDays = GetInt(worksheet, row, 6),

                SearchCriteria = GetString(worksheet, row, 7),
                MatchType = GetString(worksheet, row, 8),
                Smiles = GetString(worksheet, row, 9),

                MolecularWeight = GetDecimal(worksheet, row, 10),

                Unit = GetString(worksheet, row, 11),
                UnitPriceUsd = GetDecimal(worksheet, row, 12),
                Quantity = GetString(worksheet, row, 13), // row saved as string
                DiscountUsd = GetDecimal(worksheet, row, 14),
                NetPriceUsd = GetDecimal(worksheet, row, 15),

                Purity = GetString(worksheet, row, 16),
                Iupac = GetString(worksheet, row, 17),
                Compliance = GetString(worksheet, row, 18),
            };

            _validationService.Validate(item);
            items.Add(item);
        }

        return items;
    }

    private static string GetString(IXLWorksheet worksheet, int row, int column)
    {
        return worksheet.Cell(row, column).GetString().Trim();
    }

    private static int? GetInt(IXLWorksheet worksheet, int row, int column)
    {
        var value = GetString(worksheet, row, column);

        if (string.IsNullOrWhiteSpace(value))
        { 
            return null;
        }

        value = value.Replace("+", "").Trim();

        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
        { 
            return result;
        }

        return null;
    }

    private static decimal? GetDecimal(IXLWorksheet worksheet, int row, int column)
    {
        var value = GetString(worksheet, row, column);

        if (string.IsNullOrWhiteSpace(value))
        { 
            return null;
        }

        value = value.Replace("$", "").Replace(",", "").Trim();

        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        { 
            return result;
        }

        return null;
    }
}