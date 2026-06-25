using ClosedXML.Excel;
using MolportQuoteImporter.Constants;
using MolportQuoteImporter.Models;
using System;

namespace MolportQuoteImporter.Services;

public class QuoteSummaryParser
{
    public QuoteSummary Parse(XLWorkbook workbook)
    {
        var worksheet = workbook.Worksheet("Molecules");
        var lastRow = worksheet.LastRowUsed().RowNumber();

        var summary = new QuoteSummary();

        for (var row = 1; row <= lastRow; row++)
        {
            var label = ExcelCellReader.GetString(worksheet, row, QuoteColumns.SummaryLabelPrimary);

            if (string.IsNullOrWhiteSpace(label))
            {
                label = ExcelCellReader.GetString(worksheet, row, QuoteColumns.SummaryLabelSecondary);
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                continue;
            }

            var value = ExcelCellReader.GetDecimal(worksheet, row, QuoteColumns.NetPriceUsd);

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
}