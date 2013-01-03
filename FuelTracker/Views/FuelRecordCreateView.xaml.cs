using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using FuelTracker.ViewModels;
using System.Windows.Navigation;
using FuelTracker.Helpers;

namespace FuelTracker.Views
{
	public partial class FuelRecordCreateView : PhoneApplicationPage
	{
		public FuelRecordCreateView()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			FuelRecordCreateOrEditViewModel viewModel = new FuelRecordCreateOrEditViewModel(0);
			this.DataContext = viewModel;
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			//Workaround to update bindings
			ApplicationBarHelper.UpdateBindingOnFocussedControl();

			var viewModel = DataContext as FuelRecordCreateOrEditViewModel;
			if (viewModel != null)
			{
				viewModel.SaveCommand.Execute(null);
			}
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			var viewModel = DataContext as FuelRecordCreateOrEditViewModel;
			if (viewModel != null)
			{
				viewModel.CancelCommand.Execute(null);
			}
		}
	}
}