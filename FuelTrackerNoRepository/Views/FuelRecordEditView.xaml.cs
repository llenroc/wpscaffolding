using System;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using FuelTracker.ViewModels;
using System.Windows.Navigation;

namespace FuelTracker.Views
{
	public partial class FuelRecordEditView : PhoneApplicationPage
	{
		public FuelRecordEditView()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (NavigationContext.QueryString.ContainsKey("id"))
			{
				string idQueryString = NavigationContext.QueryString["id"];
				int id = 0;
				if (!Int32.TryParse(idQueryString, out id))
				{
					throw new ArgumentException("id is not valid value!");
				}

				FuelRecordCreateOrEditViewModel viewModel = new FuelRecordCreateOrEditViewModel(id);
				this.DataContext = viewModel;
			}
			else
			{
				throw new ArgumentNullException("id must be provided to an EditView");
			}
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			//Workaround to update bindings
			UpdateBindingOnFocussedControl();

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