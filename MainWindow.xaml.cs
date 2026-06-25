using Microsoft.Win32;
using MolportQuoteImporter.Models;
using MolportQuoteImporter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MolportQuoteImporter;

public partial class MainWindow : Window
{
    private readonly QuoteImportService _quoteImportService = new();

    private List<QuoteItem> _items = new();
    private List<ShippingLimitation> _shippingLimitations = new();
    private QuoteSummary _summary = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void SelectFileButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            SelectFileButton.IsEnabled = false;
            ImportQuoteButton.IsEnabled = false;
            StatusTextBlock.Text = "Loading and processing file...";

            var filePath = dialog.FileName;

            var importResult = await Task.Run(() => _quoteImportService.Import(filePath));

            _items = importResult.Items;
            _summary = importResult.Summary;
            _shippingLimitations = importResult.ShippingLimitations;

            QuoteItemsGrid.ItemsSource = _items;
            ShippingLimitationsGrid.ItemsSource = _shippingLimitations;

            UpdateSummary();
            UpdateShippingLimitationsSummary(importResult);

            StatusTextBlock.Text = $"Loaded: {System.IO.Path.GetFileName(filePath)}";
            ImportQuoteButton.IsEnabled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Import error", MessageBoxButton.OK, MessageBoxImage.Error);
            StatusTextBlock.Text = "Failed to load file";
        }
        finally
        {
            SelectFileButton.IsEnabled = true;
        }
    }

    private void ImportQuoteButton_Click(object sender, RoutedEventArgs e)
    {
        if (_items.Count == 0)
        {
            MessageBox.Show("No quote items loaded.");
            return;
        }

        var validRows = _items.Count(x => x.IsValid);
        var invalidRows = _items.Count(x => !x.IsValid);

        MessageBox.Show(
            $"Import Quote action completed visually.\n\nValid rows: {validRows}\nInvalid rows: {invalidRows}\nTotal order value: {FormatUsd(_summary.TotalOrderValueWithShippingUsd)}",
            "Import Quote",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void UpdateSummary()
    {
        TotalRowsTextBlock.Text = $"Total rows: {_items.Count}";
        ValidRowsTextBlock.Text = $"Valid rows: {_items.Count(x => x.IsValid)}";
        InvalidRowsTextBlock.Text = $"Invalid rows: {_items.Count(x => !x.IsValid)}";

        var totalNetPrice = _items
            .Where(x => x.IsValid && x.NetPriceUsd.HasValue)
            .Sum(x => x.NetPriceUsd!.Value);

        TotalNetPriceTextBlock.Text = $"Total net price: {totalNetPrice:0.00} USD";

        DiscountSavingsTextBlock.Text = $"Discount savings: {FormatUsd(_summary.TotalAppliedDiscountSavingsUsd)}";
        CompoundsTextBlock.Text = $"Compounds: {FormatUsd(_summary.CompoundsUsd)}";
        TariffSurchargeTextBlock.Text = $"Tariff surcharge: {FormatUsd(_summary.TariffSurchargeUsd)}";
        ShippingTextBlock.Text = $"Shipping: {FormatUsd(_summary.ConsolidatedMolportShippingUsd)}";
        TotalOrderValueTextBlock.Text = $"Total order value: {FormatUsd(_summary.TotalOrderValueWithShippingUsd)}";
    }

    private void UpdateShippingLimitationsSummary(QuoteImportResult result)
    {
        TotalLimitationsTextBlock.Text =
            $"Total limitations: {result.ShippingLimitations.Count}";

        ValidLimitationsTextBlock.Text =
            $"Valid limitations: {result.ShippingLimitations.Count(x => x.IsValid)}";

        InvalidLimitationsTextBlock.Text =
            $"Invalid limitations: {result.ShippingLimitations.Count(x => !x.IsValid)}";
    }

    private static string FormatUsd(decimal? value)
    {
        return value.HasValue
            ? $"{value.Value:0.00} USD"
            : "-";
    }
}