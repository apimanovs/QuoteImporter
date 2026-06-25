using ClosedXML.Excel;
using System.Globalization;

namespace MolportQuoteImporter.Services;

public static class ExcelCellReader
{
    public static string GetString(IXLWorksheet worksheet, int row, int column)
    {
        return worksheet.Cell(row, column).GetString().Trim();
    }

    public static int? GetInt(IXLWorksheet worksheet, int row, int column)
    {
        var value = GetString(worksheet, row, column);

        if (string.IsNullOrWhiteSpace(value))
            return null;

        value = value.Replace("+", "").Trim();

        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }

    public static decimal? GetDecimal(IXLWorksheet worksheet, int row, int column)
    {
        var cell = worksheet.Cell(row, column);

        if (cell.IsEmpty())
            return null;

        if (cell.TryGetValue<decimal>(out var numericValue))
            return numericValue;

        var value = cell.GetString().Trim();

        if (string.IsNullOrWhiteSpace(value))
            return null;

        value = value.Replace(" ", "").Replace(",", ".");

        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }
}