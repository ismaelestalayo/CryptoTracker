using System.Collections.ObjectModel;
using UWP.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UWP.UserControls {
    public sealed partial class PortfolioList : UserControl {
        public PortfolioList() {
            InitializeComponent();
        }

        public static readonly DependencyProperty PurchasesProperty =
        DependencyProperty.Register(
            nameof(Purchases),
            typeof(ObservableCollection<PurchaseModel>),
            typeof(PortfolioList),
            null);

        public ObservableCollection<PurchaseModel> Purchases {
            get => (ObservableCollection<PurchaseModel>)GetValue(PurchasesProperty);
            set => SetValue(PurchasesProperty, value);
        }
    }
}
