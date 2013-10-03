using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoProxySwitcherWPF.View
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        // Force selection change impossible via UI
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // forbids changing active configuration from interface
            AutoProxySwitcherWPF.ViewModel.MainViewModel vm = (sender as ListBox).DataContext as AutoProxySwitcherWPF.ViewModel.MainViewModel;
            int index = vm.Configurations.IndexOf(vm.ActiveConfiguration);
            if (index != (sender as ListBox).SelectedIndex)
            {
                e.Handled = true;
                (sender as ListBox).SelectedIndex = index;
            }
        }

        // Force selection change impossible via UI
        private void ListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            // forbids changing active network from interface
            AutoProxySwitcherWPF.ViewModel.MainViewModel vm = (sender as ListBox).DataContext as AutoProxySwitcherWPF.ViewModel.MainViewModel;
            int index = vm.CurrentNetworks.IndexOf(vm.CurrentNetwork);
            if (index != (sender as ListBox).SelectedIndex)
            {
                e.Handled = true;
                (sender as ListBox).SelectedIndex = index;
            }
        }
    }
}
