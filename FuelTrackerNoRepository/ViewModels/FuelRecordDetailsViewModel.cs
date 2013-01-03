using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
//using FuelTracker.Helpers;
using FuelTracker.Models;
using Microsoft.Phone.Controls;
using WpScaffolding.Helpers;

namespace FuelTracker.ViewModels
{
	public class FuelRecordDetailsViewModel : INotifyPropertyChanged
	{
		public const string connectionString = "isostore:/FuelTracker.sdf";

		FuelTrackerContext _context;

		public FuelRecordDetailsViewModel(int id)
			:this(new FuelTrackerContext(connectionString), id)
		{

		}

		public FuelRecordDetailsViewModel(FuelTrackerContext context, int id)
			: this(context)
		{
			if (id <= 0)
			{
				throw new ArgumentException("id must be greater than 0!");
			}
			this.Load(id);
		}

		private FuelRecordDetailsViewModel(FuelTrackerContext context)
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

		/// <summary>
		/// Loads entity with specified id
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
			if (FuelRecord == null)
			{
				throw new Exception("Fatal error:FuelRecord is null!");
			}

			int fuelRecordId = FuelRecord.FuelRecordId;
			NavigateToFuelRecordEdit(fuelRecordId);
		}

		/// <summary>
		/// Navigates to edit view
		/// </summary>
		/// <param name="fuelRecordId"></param>
		private static void NavigateToFuelRecordEdit(int fuelRecordId)
		{
			string uriAddress = string.Format("/Views/FuelRecordEditView.xaml?id={0}", fuelRecordId);
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
			var fuelRecord = this.FuelRecord;
			if (fuelRecord == null)
			{
				throw new NullReferenceException("FuelRecord must not be null!");
			}

			if (fuelRecord.FuelRecordId == 0)
			{
				return;
			}

			if (MessageBox.Show("Do you realy want to delete that FuelRecord?", "Confirm", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
			{
				return;
			}

			//deleting fuelrecord item
			_context.FuelRecords.DeleteOnSubmit(fuelRecord);
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
			string uriAddress = "/Views/FuelRecordsListView.xaml";
			Navigate(new Uri(uriAddress, UriKind.Relative));
		}

		#endregion

		/// <summary>
		/// Clean up resources here
		/// </summary>
		public void Cleanup()
		{
			this.FuelRecord = new FuelRecord();
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
