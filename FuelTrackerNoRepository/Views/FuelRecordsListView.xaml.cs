﻿using System;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using FuelTracker.ViewModels;

namespace FuelTracker.Views
{
	public partial class FuelRecordsListView : PhoneApplicationPage
	{
		public FuelRecordsListView()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var viewModel = new FuelRecordsListViewModel();
			this.DataContext = viewModel;
			viewModel.Load();
		}

		private void AddButton_Click(object sender, EventArgs e)
		{
			var viewModel = (FuelRecordsListViewModel)this.DataContext;
			if (viewModel != null)
			{
				viewModel.CreateFuelRecordExecute();
			}
		}

		private void RefreshButton_Click(object sender, EventArgs e)
		{
			var viewModel = (FuelRecordsListViewModel)this.DataContext;
			if (viewModel != null)
			{
				viewModel.Load();
			}
		}

		private void FuelRecordsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var senderListBox = sender as ListBox;
			
			var selectedItem = senderListBox.SelectedItem as FuelTracker.Models.FuelRecord;
			if (selectedItem == null)
			{
				return;
			}

			var viewModel = senderListBox.DataContext as FuelRecordsListViewModel;
			if (viewModel != null)
			{
				viewModel.ViewDetailsCommand.Execute(selectedItem);
			}
		}
	}
}