using System;
using System.Linq;
using System.Windows;
using FuelTracker.Models;
//using FuelTracker.Helpers;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using WpScaffolding.Helpers;

namespace FuelTracker.ViewModels
{
	public class FuelRecordCreateOrEditViewModel : INotifyPropertyChanged
	{
		private static readonly string connectionString = "isostore:/FuelTracker.sdf";
		FuelTrackerContext _context;

		public FuelRecordCreateOrEditViewModel(FuelTrackerContext context)
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

			this.FuelRecord = new FuelRecord();
		}

		public FuelRecordCreateOrEditViewModel(FuelTrackerContext context, int id)
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
		public FuelRecordCreateOrEditViewModel(int id)
			: this(new FuelTrackerContext(connectionString), id)
		{ }

		private FuelRecord _fuelRecord;
		public FuelRecord FuelRecord
		{
			get
			{
				return _fuelRecord;
			}

			set
			{
				if (_fuelRecord == value)
				{
					return;
				}
				_fuelRecord = value;
				NotifyPropertyChanged("FuelRecord");
			}
		}

		/// <summary>
		/// Creates new entity
		/// </summary>
		public void CreateNew()
		{
			this.FuelRecord = new FuelRecord();
		}

		/// <summary>
		/// Loads entity with key id
		/// </summary>
		/// <param name="id"></param>
		public void Load(int id)
		{
			var fuelRecord = _context.FuelRecords.Single(x => x.FuelRecordId == id);
			if (fuelRecord == null)
			{
				throw new InvalidOperationException(string.Format("FuelRecord with id {0} could not be found!", id));
			}

			FuelRecord = fuelRecord;
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

			//Quantity validation
			bool isQuantityValid = (FuelRecord.Quantity > 0);
			if (!isQuantityValid)
			{
				errorMessage += "Quantity is zero or invalid!\n";
				hasError = true;
			}

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
			var fuelRecord = this.FuelRecord;
			if (fuelRecord == null)
			{
				throw new NullReferenceException("FuelRecord must not be null!");
			}

			bool isDataValid = false;
			ValidateData(out isDataValid);
			if (!isDataValid)
			{
				return;
			}

			//saving FuelTracker
			if (fuelRecord.FuelRecordId == default(int))
			{
				_context.FuelRecords.InsertOnSubmit(fuelRecord);
			}
			else
				if (!_context.FuelRecords.Contains(fuelRecord))
				{
					_context.FuelRecords.Attach(fuelRecord, true);
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
			this.FuelRecord = null;
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
