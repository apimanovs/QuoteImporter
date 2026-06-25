using ClosedXML.Excel;
using MolportQuoteImporter.Constants;
using MolportQuoteImporter.Models;
using System.Collections.Generic;

namespace MolportQuoteImporter.Services;

public class ShippingLimitationsSheetParser
{
    public List<ShippingLimitation> Parse(XLWorkbook workbook)
    {
        var limitations = new List<ShippingLimitation>();

        var worksheet = workbook.Worksheet("Shipping Limitations");
        var lastRow = worksheet.LastRowUsed().RowNumber();

        for (var row = QuoteConstants.ShippingLimitationsFirstDataRow; row <= lastRow; row++)
        {
            var molportId = ExcelCellReader.GetString(worksheet, row, ShippingLimitationColumns.MolportId);

            if (string.IsNullOrWhiteSpace(molportId))
            {
                continue;
            }

            var limitation = new ShippingLimitation
            {
                RowNumber = row - QuoteConstants.ShippingLimitationsFirstDataRow + 1,
                MolportId = molportId,
                Supplier = ExcelCellReader.GetString(worksheet, row, ShippingLimitationColumns.Supplier),
                CatalogueNumber = ExcelCellReader.GetString(worksheet, row, ShippingLimitationColumns.CatalogueNumber),
                CountryOfOrigin = ExcelCellReader.GetString(worksheet, row, ShippingLimitationColumns.CountryOfOrigin),
                Unit = ExcelCellReader.GetString(worksheet, row, ShippingLimitationColumns.Unit),
                UnNumber = ExcelCellReader.GetString(worksheet, row, ShippingLimitationColumns.UnNumber),
                HazardousClass = ExcelCellReader.GetString(worksheet, row, ShippingLimitationColumns.HazardousClass),
                PackingGroup = ExcelCellReader.GetString(worksheet, row, ShippingLimitationColumns.PackingGroup),
                ShippingLimitations = ExcelCellReader.GetString(worksheet, row, ShippingLimitationColumns.ShippingLimitations),
                CompoundState = ExcelCellReader.GetString(worksheet, row, ShippingLimitationColumns.CompoundState),
                Solubility = ExcelCellReader.GetString(worksheet, row, ShippingLimitationColumns.Solubility)
            };

            limitations.Add(limitation);
        }

        return limitations;
    }
}