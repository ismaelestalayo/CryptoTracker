using CryptoTracker.Dialogs;
using CryptoTracker.Helpers;
using Microsoft.AppCenter.Analytics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;
using UWP.Services;
using UWP.Shared.Constants;
using UWP.Shared.Helpers;
using UWP.Shared.Interfaces;
using UWP.Shared.Models;
using UWP.UserControls;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace UWP.Views {

    public partial class Portfolio : Page, UpdatablePage {
        /// Variables to get historic
        private static int limit = 168;
        private static int aggregate = 1;
        private static string timeSpan = "1w";
        private static string timeUnit = "hour";
        private static string sortedBy = "";

        private ObservableCollection<PurchaseModel> LocalPurchases;

        public Portfolio() {
            InitializeComponent();   
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            /// Get timespan before updating
            timeSpan = App._LocalSettings.Get<string>(UserSettings.Timespan);
            TimeRangeRadioButtons.TimeSpan = timeSpan;
            (timeUnit, limit, aggregate) = GraphHelper.TimeSpanParser[timeSpan];

            /// Get the portfolio and group it
            LocalPurchases = await RetrievePortfolio();
            vm.Portfolio = await PortfolioHelper.GroupPortfolio(LocalPurchases);
            await UpdatePage();
        }

        public async Task UpdatePage() {
            vm.Chart.IsLoading = true;

            vm.CurrencySymbol = App.currencySymbol;

            /// Empty diversification chart and reset the Total amounts
            PortfolioChartGrid.ColumnDefinitions.Clear();
            PortfolioChartGrid.Children.Clear();

            /// Update the purchase details first
            for (int i = 0; i < vm.Portfolio.Count; i++)
                await PortfolioHelper.UpdatePurchase(vm.Portfolio[i]);

            vm.TotalInvested = vm.Portfolio.Sum(x => x.InvestedQty);
            vm.TotalWorth = vm.Portfolio.Sum(x => x.Worth);
            vm.TotalDelta = vm.Portfolio.Sum(x => x.Profit);
            vm.ROI = Math.Round(100 * (vm.TotalWorth - vm.TotalInvested) / vm.TotalInvested, 1);

            /// Create the diversification grid
            var grouped = vm.Portfolio.GroupBy(x => x.Crypto).OrderByDescending(x => x.Sum(item => item.Worth));
            foreach (var purchases in grouped) {
                var crypto = purchases.Key.ToUpperInvariant();
                var worth = purchases.ToList().Sum(x => x.Worth);

                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(worth, GridUnitType.Star);
                PortfolioChartGrid.ColumnDefinitions.Add(col);

                // Use a grid for Vertical alignment
                var g = new Grid();
                g.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 255));
                g.BorderThickness = new Thickness(0);
                g.VerticalAlignment = VerticalAlignment.Stretch;

                var val = Math.Round((worth / vm.TotalWorth) * 100, 1);
                ToolTipService.SetToolTip(g, $"{crypto} {val}% \n{worth}{vm.CurrencySymbol}");
                ToolTipService.SetPlacement(g, PlacementMode.Right);
                var t = new TextBlock() {
                    Text = crypto + "\n" + $"{val}%",
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                g.Children.Add(t);
                g.Background = ColorConstants.GetCoinBrush(crypto, 100);

                PortfolioChartGrid.Children.Add(g);
                Grid.SetColumn(g, PortfolioChartGrid.Children.Count - 1);
            }

            /// Finally, update the chart of the portfolio's worth
            await UpdatePortfolioChart();
        }

        /// ###############################################################################################
        /// Get portfolio from LocalStorage
        internal async Task<ObservableCollection<PurchaseModel>> RetrievePortfolio() {
            ObservableCollection<PurchaseModel> portfolio;

            var portfolioV2 = await LocalStorageHelper.ReadObject<List<PurchaseModel>>(UserStorage.Portfolio6);

            if (portfolioV2.Count > 0) {
                portfolioV2 = portfolioV2.OrderByDescending(x => x.Date).ToList();
                return new ObservableCollection<PurchaseModel>(portfolioV2);
            }

            /// If it is empty, there might be an old portfolio in the old format and key
            var portfolioV1 = await LocalStorageHelper.ReadObject<ObservableCollection<PurchaseClass>>("portfolio");
            if (portfolioV1.Count <= 0)
                return new ObservableCollection<PurchaseModel>();

            /// For retrocompatibility with old portfolios
            portfolio = new ObservableCollection<PurchaseModel>();
            foreach (var p in portfolioV1) {
                portfolio.Add(new PurchaseModel() {
                    Crypto = p.Crypto,
                    CryptoName = p.Crypto,
                    CryptoLogo = p.CryptoLogo,
                    CryptoQty = p.CryptoQty,
                    Currency = p.c,
                    Date = p.Date,
                    Exchange = p.Exchange,
                    InvestedQty = p.InvestedQty
                });
            }
            await LocalStorageHelper.SaveObject(UserStorage.Portfolio6, portfolio);
            return portfolio;
        }


        /// ###############################################################################################
        /// Update the chart of the historic values of the portfolio
        private async Task UpdatePortfolioChart() {
            var nPurchases = vm.Portfolio.Count;
            if (nPurchases == 0)
                return;

            /// Optimize by only getting historic for different cryptos
            var uniqueCryptos = new HashSet<string>(vm.Portfolio.Select(x => x.Crypto)).ToList();

            /// Get historic for each unique crypto, get invested qty, and multiply
            /// to get the worth of each crypto to the user's wallet
            var cryptoQties = new List<double>();
            var cryptoWorth = new List<List<double>>();
            var histos = new List<List<HistoricPrice>>(nPurchases);
            foreach (var crypto in uniqueCryptos) {
                var histo = await Ioc.Default.GetService<ICryptoCompare>().GetHistoric_(crypto, timeUnit, limit, aggregate);
                var cryptoQty = vm.Portfolio.Where(x => x.Crypto == crypto).Sum(x => x.CryptoQty);
                cryptoWorth.Add(histo.Select(x => x.Average * cryptoQty).ToList());
                histos.Add(histo);
            }

            /// There might be young cryptos that didnt exist in the past, so take the common minimum 
            var minCommon = histos.Min(x => x.Count);
            if (minCommon == 1)
                return;

            /// Check if all arrays are equal length, if not, remove the leading values
            var sameLength = cryptoWorth.All(x => x.Count == cryptoWorth[0].Count);
            if (!sameLength)
                for (int i = 0; i < histos.Count; i++)
                    histos[i] = histos[i].Skip(Math.Max(0, histos[i].Count() - minCommon)).ToList();


            var worth_arr = new List<double>();
            var dates_arr = histos[0].Select(kk => kk.DateTime).ToList();
            for (int i = 0; i < minCommon; i++)
                worth_arr.Add(cryptoWorth.Select(x => x[i]).Sum());

            /// Create List for chart
            var chartData = new List<ChartPoint>();
            for (int i = 0; i < minCommon; i++) {
                chartData.Add(new ChartPoint {
                    Date = dates_arr.ElementAt(i),
                    Value = (float)worth_arr[i],
                    Volume = 0
                });
            }
            vm.Chart.ChartData = chartData;
            var temp = GraphHelper.AdjustLinearAxis(new ChartStyling(), timeSpan);
            vm.Chart.LabelFormat = temp.LabelFormat;
            vm.Chart.Minimum = temp.Minimum;
            vm.Chart.MajorStepUnit = temp.MajorStepUnit;
            vm.Chart.MajorStep = temp.MajorStep;
            vm.Chart.TickInterval = temp.TickInterval;

            vm.Chart.ChartStroke = (vm.TotalDelta >= 0) ?
                ColorConstants.GetColorBrush("pastel_green") : ColorConstants.GetColorBrush("pastel_red");

            /// Calculate min-max to adjust axis
            var MinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
            vm.Chart.PricesMinMax = GraphHelper.OffsetMinMaxForChart(MinMax.Min, MinMax.Max);
            vm.Chart.VolumeMax = GraphHelper.GetMaxOfVolume(chartData);
            vm.Chart.VolumeMax = (vm.Chart.VolumeMax == 0) ? 10 : vm.Chart.VolumeMax;

            vm.Chart.IsLoading = false;
        }

        /// ###############################################################################################
        private void ToggleDetails_click(object sender, RoutedEventArgs e) {
            vm.ShowDetails = !vm.ShowDetails;
            if (vm.ShowDetails)
                MainGrid.RowDefinitions[3].Height = new GridLength(0, GridUnitType.Pixel);
            else
                MainGrid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
        }

        /// ###############################################################################################
        /// Add purchase dialog
        private async void AddPurchase_click(object sender, RoutedEventArgs e) {
            var dialog = new PortfolioEntryDialog() {
                NewPurchase = new PurchaseModel(),
                PrimaryButtonText = "Add",
                Title = "💵 New transaction"
            };
            var response = await dialog.ShowAsync();
            if (response == ContentDialogResult.Primary) {
                dialog.NewPurchase.CryptoName = App.coinListPaprika.FirstOrDefault(
                    x => x.symbol == dialog.NewPurchase.Crypto).name;
                vm.Portfolio.Add(dialog.NewPurchase);

                await PortfolioHelper.AddPurchase(dialog.NewPurchase);
                
                // Update everything
                await UpdatePage();
            }
        }

        private void TimeRangeButtons_Tapped(object sender, TappedRoutedEventArgs e) {
            if (sender != null)
                timeSpan = ((TimeRangeRadioButtons)sender).TimeSpan;

            (timeUnit, limit, aggregate) = GraphHelper.TimeSpanParser[timeSpan];
            UpdatePage();
        }


        /// #######################################################################################
        /// Sorting
        private void SortPortfolio_click(object sender, RoutedEventArgs e) {
            var sortBy = ((Button)sender).Content.ToString();
            switch (sortBy) {
                case "Coin":
                    if (sortedBy != "Coin")
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(
                            from item in vm.Portfolio orderby item.CryptoName ascending, item.Date select item);
                    else {
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(
                            from item in vm.Portfolio orderby item.CryptoName descending, item.Date select item);
                    }
                    break;
                case "Invested":
                    if (sortedBy != "Invested")
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(
                            from item in vm.Portfolio orderby item.InvestedQty descending select item);
                    else
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(
                            from item in vm.Portfolio orderby item.InvestedQty ascending select item);
                    break;
                case "Worth":
                    if (sortedBy != "Worth")
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(
                            from item in vm.Portfolio orderby item.Worth descending select item);
                    else
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(
                            from item in vm.Portfolio orderby item.Worth ascending select item);
                    break;
                case "Delta":
                    if (sortedBy != "Delta")
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(
                            from item in vm.Portfolio orderby item.Profit descending select item);
                    else
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(
                            from item in vm.Portfolio orderby item.Profit ascending select item);
                    break;
            }
            sortedBy = (sortedBy != sortBy) ? sortBy : "";
        }


        /// ###############################################################################################
        /// A purchase's right click ContextFlyout
        private void PortfolioList_ClickGoTo(object sender, EventArgs e) {
            var crypto = sender.ToString();
            this.Frame.Navigate(typeof(CoinDetails), crypto);
        }

        /// <summary>
        /// Update the page from the UserControl by routing the event here
        /// </summary>
        private async void PortfolioList_UpdateParent(object sender, EventArgs e)
            => await UpdatePage();


        /// ###############################################################################################
        /// Import/Export functionality
        private async void ImportPortfolio_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("portfolio-import");
            var picker = new FileOpenPicker() {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(".ct");

            StorageFile importedFile = await picker.PickSingleFileAsync();
            // Operation cancelled
            if (importedFile == null) 
                return;
            
            try {
                var stream = await importedFile.OpenStreamForReadAsync();
                DataContractSerializer stuffSerializer = new DataContractSerializer(typeof(List<PurchaseModel>));
                var setResult = (List<PurchaseModel>)stuffSerializer.ReadObject(stream);
                await stream.FlushAsync();
                stream.Dispose();


                var importedText = await FileIO.ReadTextAsync(importedFile);
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = await storageFolder.CreateFileAsync(
                    UserStorage.Portfolio6, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile, importedText);

                // Now update the page
                Page_Loaded(null, null);
            }
            catch (Exception ex) {
                var z = ex.Message;
                vm.InAppNotification("Error importing portfolio.", ex.Message);
                Analytics.TrackEvent("portfolio-importError");
            }            
        }

        private async void ExportPortfolio_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("portfolio-export");
            var allFiles = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            var fileNames = allFiles.Select(x => x.Name).ToList();

            if (!fileNames.Contains(UserStorage.Portfolio6)) {
                vm.InAppNotification("Portfolio not found.");
                return;
            }

            var portfolioFile = await ApplicationData.Current.LocalFolder.GetFileAsync(UserStorage.Portfolio6);
            var portfolioText = await FileIO.ReadTextAsync(portfolioFile);
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".ct" });
            savePicker.SuggestedFileName = "CryptoTracker-Portfolio";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null) {
                // Prevent updates to the remote version of the file until we finish making changes
                CachedFileManager.DeferUpdates(file);

                await FileIO.WriteTextAsync(file, portfolioText);
                
                // Let Windows know that we're finished changing the file
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                    vm.InAppNotification("Portfolio exported successfully.");
                else
                    vm.InAppNotification("Portfolio could not be saved.");
            }
            // else: Operation cancelled
        }
    }
}
