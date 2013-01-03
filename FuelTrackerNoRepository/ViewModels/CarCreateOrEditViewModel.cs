
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
	public class CarCreateOrEditViewModel : INotifyPropertyChanged
	{
		private static readonly string connectionString = "isostore:/FuelTracker.sdf";
		FuelTrackerContext _context;

		public CarCreateOrEditViewModel(FuelTrackerContext context)
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

		public CarCreateOrEditViewModel(FuelTrackerContext context, System.Int32 id)
			: this(context)
		{
			if (id < 0)
			{
				throw new ArgumentException("id must be greater than 0!");
			}

			if (id == 0)
			{
				this.CreateNew();
			}
			else
			{
				this.Load(id);
			}
		}

		/// <summary>
		/// Contructor
		/// </summary>
		/// <param name="id">id of editing entity.Use id different from 0 to edit object. Use 0 to create new object</param>
		public CarCreateOrEditViewModel(System.Int32 id)
			: this(new FuelTrackerContext(connectionString), id)
		{ }

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

		/// <summary>
		/// Creates new entity
		/// </summary>
		public void CreateNew()
		{
			this.Car = new Car();
		}

		/// <summary>
		/// Loads entity with key id
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

		/// <summary>
		/// Validates if entity data is valid.
		/// </summary>
		/// <param name="isDataValid">False if data is invalid. True if data is valid</param>
		private void ValidateData(out bool isDataValid)
		{
			string errorMessage = string.Empty;
			bool hasError = false;
			isDataValid = true;

			//TODO: Consider property validation
			////MyProperty validation
			//bool isMyPropertyValid = (!string.IsNullOrEmpty(Car.MyProperty));
			//if (!isMyPropertyValid)
			//{
			//	errorMessage += "MyProperty is invalid!\n";
			//	hasError = true;
			//}

			if (hasError)
			{
				DisplayMessage(errorMessage, "Data error");
			}

			isDataValid = !hasError;

			return;
		}

		/// <summary>
		/// Displays message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="caption"></param>
		private void DisplayMessage(string message, string caption)
		{
			MessageBox.Show(message, caption, MessageBoxButton.OK);
		}

		#region SaveCommand
		private RelayCommand _saveCommand;
		public RelayCommand SaveCommand
		{
			get
			{
				if (_saveCommand == null)
				{
					_saveCommand =
						new RelayCommand(
							() =>
							{
								SaveExecute();
							},
							() => CanSave
						);
				}
				return _saveCommand;
			}
			set
			{
				_saveCommand = value;
			}
		}

		/// <summary>
		/// Executes when SaveCommand is executed
		/// </summary>
		public void SaveExecute()
		{
			var car = this.Car;
			if (car == null)
			{
				throw new NullReferenceException("Car must not be null!");
			}

			bool isDataValid = false;
			ValidateData(out isDataValid);
			if (!isDataValid)
			{
				return;
			}

			//saving FuelTracker
			if (car.CarId == default(int))
			{
				_context.Cars.InsertOnSubmit(car);
			}
			else
				if (!_context.Cars.Contains(car))
				{
					_context.Cars.Attach(car, true);
				}

			_context.SubmitChanges();

			GoBack();
		}


		private bool _canSave = false;
		public bool CanSave
		{
			get
			{
				return _canSave;
			}

			set
			{
				if (_canSave == value)
				{
					return;
				}
				_canSave = value;

				NotifyPropertyChanged("CanSave");
				SaveCommand.RaiseCanExecuteChanged();
			}
		}
		#endregion

		#region CancelCommand
		private RelayCommand _cancelCommand;
		public RelayCommand CancelCommand
		{
			get
			{
				if (_cancelCommand == null)
				{
					_cancelCommand =
						new RelayCommand(
							() =>
							{
								CancelExecute();
							},
							() => CanCancel
						);
				}
				return _cancelCommand;
			}
			set
			{
				_cancelCommand = value;
			}
		}

		/// <summary>
		/// Cancel editing. Executes when CancelCommand is executed
		/// </summary>
		public void CancelExecute()
		{
			this.Cleanup();
			GoBack();
		}

		public const string CanCancelPropertyName = "CanCancel";
		private bool _canCancel = false;
		public bool CanCancel
		{
			get
			{
				return _canCancel;
			}
			set
			{
				if (_canCancel == value)
				{
					return;
				}
				_canCancel = value;

				NotifyPropertyChanged(CanCancelPropertyName);
				CancelCommand.RaiseCanExecuteChanged();
			}
		}
		#endregion


		/// <summary>
		/// Free special resources here
		/// </summary>
		public void Cleanup()
		{
			this.Car = null;
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
