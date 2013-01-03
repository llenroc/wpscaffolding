using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using FuelTracker.Helpers;
using FuelTracker.Models;
using FuelTracker.Services;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Text;
using System.ComponentModel;

namespace FuelTracker.ViewModels
{
	public class FuelRecordDetailsViewModel : INotifyPropertyChanged
	{
		public const string connectionString = "isostore:/FuelTracker.sdf";

		IFuelRecordRepository _repository;

		public FuelRecordDetailsViewModel(int id)
			:this(new FuelRecordRepository(connectionString), id)
		{

		}

		public FuelRecordDetailsViewModel(IFuelRecordRepository repository, int id)
			: this(repository)
		{
			if (id <= 0)
			{
				throw new ArgumentException("id must be greater than 0!");
			}
			this.Load(id);
		}

		public FuelRecordDetailsViewModel(IFuelRecordRepository repository)
		{
			if (repository == null)
			{
				throw new ArgumentNullException("repository must not be null");
			}

			this._repository = repository;

			this.FuelRecord = new FuelRecord();
		}

		/// <summary>
		/// Loads entity with specified id
		/// </summary>
		/// <param name="id"></param>
		private void Load(int id)
		{
			var fuelRecord = _repository.Find(id);
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

		/// <summary>
		/// Navigates back to previous location ui
		/// </summary>
		private void GoBack()
		{
			NavigationController.Instance.GoBack();
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
			NavigationController.Instance.Navigate(new Uri(uriAddress, UriKind.Relative));
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
			if (FuelRecord == null)
			{
				throw new NullReferenceException("FuelRecord must not be null!");
			}

			if (FuelRecord.FuelRecordId == 0)
			{
				return;
			}

			if (MessageBox.Show("Do you realy want to delete that FuelRecord?", "Confirm", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
			{
				return;
			}

			int toDoId = FuelRecord.FuelRecordId;
			_repository.Delete(toDoId);
			_repository.Save();

			this.GoBack();
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
			NavigationController.Instance.Navigate(new Uri(uriAddress, UriKind.Relative));
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
	}
}
