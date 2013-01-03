
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using Microsoft.Phone.Controls;
using WpScaffolding.Helpers;//using FuelTracker.Helpers;
using FuelTracker.Models;

namespace FuelTracker.ViewModels
{
	public class CarDetailsViewModel : INotifyPropertyChanged
	{
		public const string connectionString = "isostore:/FuelTracker.sdf";

		FuelTrackerContext _context;

		public CarDetailsViewModel(System.Int32 id)
			:this(new FuelTrackerContext(connectionString), id)
		{

		}

		public CarDetailsViewModel(FuelTrackerContext context, System.Int32 id)
			: this(context)
		{
			if (id <= 0)
			{
				throw new ArgumentException("id must be greater than 0!");
			}
			this.Load(id);
		}

		private CarDetailsViewModel(FuelTrackerContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context must not be null");
			}

			this._context = context;
			//create database if not exists
			if (!_context.DatabaseExists())
			{
				_context.CreateDatabase();
			}

			this.Car = new Car();
		}

		/// <summary>
		/// Loads entity with specified id
		/// </summary>
		/// <param name="id"></param>
		public void Load(System.Int32 id)
		{
			var car = _context.Cars.Single(x => x.CarId == id);
			if (car == null)
			{
				throw new InvalidOperationException(string.Format("Car with id {0} could not be found!", id));
			}

			Car = car;
		}

		private Car _car;
		public Car Car
		{
			get
			{
				return _car;
			}

			set
			{
				if (_car == value)
				{
					return;
				}
				_car = value;
				NotifyPropertyChanged("Car");
			}
		}

		#region EditCommand
		private RelayCommand _editCommand;
		public RelayCommand EditCommand
		{
			get
			{
				if (_editCommand == null)
				{
					_editCommand =
						new RelayCommand(
							() =>
							{
								EditExecute();
							},
							() => CanEdit
						);
				}
				return _editCommand;
			}
			set
			{
				_editCommand = value;
			}
		}

		public void EditExecute()
		{
			if (Car == null)
			{
				throw new Exception("Fatal error:Car is null!");
			}

			int carId = Car.CarId;
			NavigateToCarEdit(carId);
		}

		/// <summary>
		/// Navigates to edit view
		/// </summary>
		/// <param name="carId"></param>
		private static void NavigateToCarEdit(int carId)
		{
			string uriAddress = string.Format("/Views/CarEditView.xaml?id={0}", carId);
			Navigate(new Uri(uriAddress, UriKind.Relative));
		}

		private bool _canEdit = false;
		public bool CanEdit
		{
			get
			{
				return _canEdit;
			}

			set
			{
				if (_canEdit == value)
				{
					return;
				}
				_canEdit = value;

				NotifyPropertyChanged("CanEdit");
				EditCommand.RaiseCanExecuteChanged();
			}
		}
		#endregion

		#region DeleteCommand
		private RelayCommand _deleteCommand;
		public RelayCommand DeleteCommand
		{
			get
			{
				if (_deleteCommand == null)
				{
					_deleteCommand = new RelayCommand(
							() =>
							{
								DeleteExecute();
							},
							() => CanDelete
						);
				}
				return _deleteCommand;
			}
			set
			{
				_deleteCommand = value;
			}
		}


		/// <summary>
		/// Deletes entity
		/// </summary>
		public void DeleteExecute()
		{
			var car = this.Car;
			if (car == null)
			{
				throw new NullReferenceException("Car must not be null!");
			}

			if (car.CarId == 0)
			{
				return;
			}

			if (MessageBox.Show("Do you realy want to delete that Car?", "Confirm", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
			{
				return;
			}

			//deleting fuelrecord item
			_context.Cars.DeleteOnSubmit(car);
			_context.SubmitChanges();

			GoBack();
		}

		private bool _canDelete = true;
		public bool CanDelete
		{
			get
			{
				return _canDelete;
			}

			set
			{
				if (_canDelete == value)
				{
					return;
				}
				_canDelete = value;

				NotifyPropertyChanged("CanDelete");
				DeleteCommand.RaiseCanExecuteChanged();
			}
		}
		#endregion

		#region GoToListCommand

		private RelayCommand _goToListCommand;
		public RelayCommand GoToListCommand
		{
			get
			{
				if (_goToListCommand == null)
				{
					_goToListCommand =
						new RelayCommand(
							() =>
							{
								GoToListExecute();
							});
				}
				return _goToListCommand;
			}
			set
			{
				_goToListCommand = value;
			}
		}

		/// <summary>
		/// Navigates to list page
		/// </summary>
		public void GoToListExecute()
		{
			this.NavigateToList();
		}

		/// <summary>
		/// Navigates to list page
		/// </summary>
		private void NavigateToList()
		{
			string uriAddress = "/Views/CarsListView.xaml";
			Navigate(new Uri(uriAddress, UriKind.Relative));
		}

		#endregion

		/// <summary>
		/// Clean up resources here
		/// </summary>
		public void Cleanup()
		{
			this.Car = new Car();
		}

		#region INotifyPropertyChanged
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
