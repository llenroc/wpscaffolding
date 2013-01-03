
using System;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using FuelTracker.ViewModels;

namespace FuelTracker.Views
{
	public partial class CarsListView : PhoneApplicationPage
	{
		public CarsListView()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var viewModel = new CarsListViewModel();
			this.DataContext = viewModel;
			viewModel.Load();
		}

		private void AddButton_Click(object sender, EventArgs e)
		{
			var viewModel = (CarsListViewModel)this.DataContext;
			if (viewModel != null)
			{
				viewModel.CreateCarExecute();
			}
		}

		private void RefreshButton_Click(object sender, EventArgs e)
		{
			var viewModel = (CarsListViewModel)this.DataContext;
			if (viewModel != null)
			{
				viewModel.Load();
			}
		}

		private void CarsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var senderListBox = sender as ListBox;
			
			var selectedItem = senderListBox.SelectedItem as FuelTracker.Models.Car;
			if (selectedItem == null)
			{
				return;
			}

			var viewModel = senderListBox.DataContext as CarsListViewModel;
			if (viewModel != null)
			{
				viewModel.ViewDetailsCommand.Execute(selectedItem);
			}
		}
	}
}

