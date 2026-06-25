using ClosedXML.Excel;
using MolportQuoteImporter.Constants;
using MolportQuoteImporter.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MolportQuoteImporter.Services;

public class QuoteImportService
{
    private readonly QuoteValidationService _validationService = new();

    public QuoteImportResult Import(string filePath)
    {
        var result = new QuoteImportResult();

        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet("Molecules");

        var lastRow = worksheet.LastRowUsed().RowNumber();

        for (var row = QuoteConstants.FirstDataRow; row <= lastRow; row++)
        {
            var molportId = GetString(worksheet, row, QuoteColumns.MolportId);

            if (string.IsNullOrWhiteSpace(molportId))
            {
                continue;
            }

            var item = new QuoteItem
            {
                RowNumber = row,

                MolportId = molportId,
                ProductId = GetString(worksheet, row, QuoteColumns.ProductId),
                Supplier = GetString(worksheet, row, QuoteColumns.Supplier),

                CatalogueNumber = GetString(worksheet, row, QuoteColumns.CatalogueNumber),
                DeliveryTimeBusinessDays =GetInt(worksheet, row, QuoteColumns.DeliveryTimeBusinessDays),

                SearchCriteria = GetString(worksheet, row, QuoteColumns.SearchCriteria),

                MatchType = GetString(worksheet, row, QuoteColumns.MatchType),

                Smiles = GetString(worksheet, row, QuoteColumns.Smiles),

                MolecularWeight = GetDecimal(worksheet, row, QuoteColumns.MolecularWeight),

                Unit = GetString(worksheet, row, QuoteColumns.Unit),

                UnitPriceUsd = GetDecimal(worksheet, row, QuoteColumns.UnitPriceUsd),

                Quantity = GetInt(worksheet, row, QuoteColumns.Quantity),

                DiscountUsd = GetDecimal(worksheet, row, QuoteColumns.DiscountUsd),

                NetPriceUsd = GetDecimal(worksheet, row, QuoteColumns.NetPriceUsd),

                Purity = GetString(worksheet, row, QuoteColumns.Purity),

                Iupac = GetString(worksheet, row, QuoteColumns.Iupac),

                Compliance = GetString(worksheet, row, QuoteColumns.Compliance),
            };

            _validationService.Validate(item);
            result.Items.Add(item);
        }

        result.Summary = ParseSummary(worksheet, lastRow);

        return result;
    }

    private static QuoteSummary ParseSummary(IXLWorksheet worksheet, int lastRow)
    {
        var summary = new QuoteSummary();

        for (var row = 1; row <= lastRow; row++)
        {
            var label = GetString(worksheet, row, QuoteColumns.SummaryLabelPrimary);

            if (string.IsNullOrWhiteSpace(label))
            {
                label = GetString(worksheet, row, QuoteColumns.SummaryLabelSecondary);
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                continue;
            }

            var value = GetDecimal(worksheet, row, QuoteColumns.NetPriceUsd);

            if (label.Contains("Total Applied Discount Savings", StringComparison.OrdinalIgnoreCase))
            {
                summary.TotalAppliedDiscountSavingsUsd = value;
            }
            else if (label.Contains("Compounds", StringComparison.OrdinalIgnoreCase))
            {
                summary.CompoundsUsd = value;
            }
            else if (label.Contains("Tariff Surcharge", StringComparison.OrdinalIgnoreCase))
            {
                summary.TariffSurchargeUsd = value;
            }
            else if (label.Contains("Consolidated Molport Shipping", StringComparison.OrdinalIgnoreCase)
                     && !label.Contains("Total order value", StringComparison.OrdinalIgnoreCase))
            {
                summary.ConsolidatedMolportShippingUsd = value;
            }
            else if (label.Contains("Total order value", StringComparison.OrdinalIgnoreCase))
            {
                summary.TotalOrderValueWithShippingUsd = value;
            }
        }

        return summary;
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
        var cell = worksheet.Cell(row, column);

        if (cell.IsEmpty())
        {
            return null;
        }

        if (cell.TryGetValue<decimal>(out var numericValue))
        {
            return numericValue;
        }

        var value = cell.GetString().Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var normalizedResult))
        {
            return normalizedResult;
        }

        return null;
    }
}