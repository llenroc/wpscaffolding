using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using FuelTracker.ViewModels;

namespace FuelTracker.Views
{
	public partial class CarCreateView : PhoneApplicationPage
	{
		public CarCreateView()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			CarCreateOrEditViewModel viewModel = new CarCreateOrEditViewModel(0);
			this.DataContext = viewModel;
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			//Workaround to update bindings
			UpdateBindingOnFocussedControl();

			var viewModel = DataContext as CarCreateOrEditViewModel;
			if (viewModel != null)
			{
				viewModel.SaveCommand.Execute(null);
			}
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			var viewModel = DataContext as CarCreateOrEditViewModel;
			if (viewModel != null)
			{
				viewModel.CancelCommand.Execute(null);
			}
		}

		/// <summary>
		/// Updates bindings on clicking on AppBarButton
		/// </summary>
		public static void UpdateBindingOnFocussedControl()
		{
			object focusedElement = FocusManager.GetFocusedElement();
			if (focusedElement != null && focusedElement is TextBox)
			{
				var binding = (focusedElement as TextBox).GetBindingExpression(TextBox.TextProperty);
				if (binding != null)
					binding.UpdateSource();
			}
		}
	}
}
