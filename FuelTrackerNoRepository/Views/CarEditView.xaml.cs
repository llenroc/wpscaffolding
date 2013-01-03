using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using FuelTracker.ViewModels;

namespace FuelTracker.Views
{
	public partial class CarEditView : PhoneApplicationPage
	{
		public CarEditView()
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

				CarCreateOrEditViewModel viewModel = new CarCreateOrEditViewModel(id);
				this.DataContext = viewModel;
			}
			else
			{
				throw new ArgumentNullException("id must be provided to a EditView");
			}
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
