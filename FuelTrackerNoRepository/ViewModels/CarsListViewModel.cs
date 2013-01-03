
using System;
using System.Windows;
using FuelTracker.Models;
using System.Collections.ObjectModel;
using WpScaffolding.Helpers;//using FuelTracker.Helpers;
using System.ComponentModel;
using Microsoft.Phone.Controls;

namespace FuelTracker.ViewModels
{
	public class CarsListViewModel : INotifyPropertyChanged
	{
		public const string connectionString = "isostore:/FuelTracker.sdf";

		FuelTrackerContext _context;

		public CarsListViewModel()
			: this(new FuelTrackerContext(connectionString))
		{ }

		public CarsListViewModel(FuelTrackerContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("repository must not be null");
			}
			this._context = context;
			//create database if not exists
			if (!_context.DatabaseExists())
			{
				_context.CreateDatabase();
			}
		}

		private ObservableCollection<Car> _cars;
		public ObservableCollection<Car> Cars
		{
			get
			{
				return _cars;
			}

			set
			{
				if (_cars == value)
				{
					return;
				}
				_cars = value;
				NotifyPropertyChanged("Cars");
			}
		}

		private Car _selectedCar;
		public Car SelectedCar
		{
			get
			{
				return _selectedCar;
			}

			set
			{
				if (_selectedCar == value)
				{
					return;
				}
				_selectedCar = value;
				NotifyPropertyChanged("SelectedCar");
			}
		}

		#region CreateCommand
		private RelayCommand _createCommand;
		public RelayCommand CreateCommand
		{
			get
			{
				if (_createCommand == null)
				{
					_createCommand =
						new RelayCommand(
							() =>
							{
								CreateCarExecute();
							}
						);
				}
				return _createCommand;
			}
			set
			{
				_createCommand = value;
			}
		}

		/// <summary>
		/// Navigates to Create view. Executes when CreateCommand is executed
		/// </summary>
		public void CreateCarExecute()
		{
			//TODO: Check if that is the CreateView url
			string uriAddress = "/Views/CarCreateView.xaml";
			Navigate(new Uri(uriAddress, UriKind.Relative));
		}

		#endregion

		#region EditCommand
		private RelayCommand<Car> _editCommand;
		public RelayCommand<Car> EditCommand
		{
			get
			{
				if (_editCommand == null)
				{
					_editCommand =
						new RelayCommand<Car>(
							(item) =>
							{
								EditCarExecute(item);
							},
							(item) => item != null
						);
				}
				return _editCommand;
			}
			set
			{
				_editCommand = value;
			}
		}

		/// <summary>
		/// Navigates to Edit view
		/// </summary>
		/// <param name="car"></param>
		public void EditCarExecute(Car car)
		{
			if (car == null)
			{
				return;
			}

			int carId = car.CarId;
			NavigateToCarEdit(carId);
		}

		/// <summary>
		/// Navigates to EditView passing the entity id to edit
		/// </summary>
		/// <param name="id"></param>
		private static void NavigateToCarEdit(int id)
		{
			//TODO: Check if that is the EditView url
			string uriAddress = string.Format("/Views/CarEditViewPage.xaml?id={0}", id);
			Navigate(new Uri(uriAddress, UriKind.Relative));
		}

		#endregion

		#region ViewDetailsCommand
		private RelayCommand<Car> _viewDetailsCommand;
		public RelayCommand<Car> ViewDetailsCommand
		{
			get
			{
				if (_viewDetailsCommand == null)
				{
					_viewDetailsCommand =
						new RelayCommand<Car>(
							(param) =>
							{
								ViewDetailsExecute(param);
							},
							(param) => param != null
						);
				}
				return _viewDetailsCommand;
			}
			set
			{
				_viewDetailsCommand = value;
			}
		}

		public void ViewDetailsExecute(Car car)
		{
			var selectedCar = car;
			//if (SelectedCar == null)
			//{
			//    return;
			//}

			int carId = selectedCar.CarId;
			NavigateToCarDetails(carId);
		}

		/// <summary>
		/// Navigates to Details view
		/// </summary>
		/// <param name="id"></param>
		private static void NavigateToCarDetails(int id)
		{
			//TODO: Check if that is the DetailsView url
			string uriAddress = string.Format("/Views/CarDetailsView.xaml?id={0}", id);
			Navigate(new Uri(uriAddress, UriKind.Relative));
		}

		#endregion

		/// <summary>
		/// Loads entities list
		/// </summary>
		public void Load()
		{
			var cars = _context.Cars;
			this.Cars = new ObservableCollection<Car>(cars);
		}

		/// <summary>
		/// Clean up resources here
		/// </summary>
		public void Cleanup()
		{
			Cars = new ObservableCollection<Car>();
			SelectedCar = null;
		}

		#region INotifyPropertyChanged
		//TODO: Extact INotifyPropertyChanged into a separate base class (ViewModelBase recommended)
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (null != handler)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion

		#region Navigation
		//TODO: Extract navigation into a separate class (NavigationController recommended)
		private static PhoneApplicationFrame GetRootPhoneApplicationFrame()
		{
			PhoneApplicationFrame applicationFrame = (Application.Current.RootVisual as PhoneApplicationFrame);
			return applicationFrame;
		}

		private static void Navigate(Uri address)
		{
			PhoneApplicationFrame applicationFrame = GetRootPhoneApplicationFrame();
			if (applicationFrame == null)
			{
				throw new NullReferenceException("applicationFrame must not be null!");
			}

			applicationFrame.Navigate(address);
		}

		private static void GoBack()
		{
			PhoneApplicationFrame applicationFrame = GetRootPhoneApplicationFrame();
			if (applicationFrame == null)
			{
				throw new NullReferenceException("applicationFrame must not be null!");
			}

			if (applicationFrame.CanGoBack)
			{
				applicationFrame.GoBack();
			}
		}
		#endregion
	}
}
