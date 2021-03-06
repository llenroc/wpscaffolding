using System;
using Microsoft.Phone.Controls;
using FuelTracker.ViewModels;
using System.Windows.Navigation;

namespace FuelTracker.Views
{
	public partial class CarDetailsView : PhoneApplicationPage
	{
		public CarDetailsView()
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

				CarDetailsViewModel viewModel = new CarDetailsViewModel(id);
				this.DataContext = viewModel;
			}
			else
			{
				throw new ArgumentNullException("id must be provided to a DetailsView");
			}
		}

		private void EditButton_Click(object sender, EventArgs e)
		{
			var viewModel = DataContext as CarDetailsViewModel;
			if (viewModel != null)
			{
				viewModel.EditCommand.Execute(null);
			}
		}

		private void DeleteButton_Click(object sender, EventArgs e)
		{
			var viewModel = DataContext as CarDetailsViewModel;
			if (viewModel != null)
			{
				viewModel.DeleteCommand.Execute(null);
			}
		}

		private void ListMenuItem_Click(object sender, EventArgs e)
		{
			var viewModel = DataContext as CarDetailsViewModel;
			if (viewModel != null)
			{
				viewModel.GoToListCommand.Execute(null);
			}
		}
	}
}

