using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using FuelTracker.Models;
using System.Collections.ObjectModel;
using FuelTracker.Helpers;
using FuelTracker.Services;
using System.ComponentModel;

namespace FuelTracker.ViewModels
{
	public class FuelRecordsListViewModel : INotifyPropertyChanged
	{
		public const string connectionString = "isostore:/FuelTracker.sdf";

		IFuelRecordRepository _fuelRecordRepository;

		public FuelRecordsListViewModel()
			: this(new FuelRecordRepository(connectionString))
		{ }

		public FuelRecordsListViewModel(IFuelRecordRepository fuelRecordRepository)
		{
			if (fuelRecordRepository == null)
			{
				throw new ArgumentNullException("repository must not be null");
			}
			this._fuelRecordRepository = fuelRecordRepository;
		}

		private ObservableCollection<FuelRecord> _fuelRecords;
		public ObservableCollection<FuelRecord> FuelRecords
		{
			get
			{
				return _fuelRecords;
			}

			set
			{
				if (_fuelRecords == value)
				{
					return;
				}
				_fuelRecords = value;
				NotifyPropertyChanged("FuelRecords");
			}
		}

		private FuelRecord _selectedFuelRecord;
		public FuelRecord SelectedFuelRecord
		{
			get
			{
				return _selectedFuelRecord;
			}

			set
			{
				if (_selectedFuelRecord == value)
				{
					return;
				}
				_selectedFuelRecord = value;
				NotifyPropertyChanged("SelectedFuelRecord");
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
								CreateFuelRecordExecute();
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
		public void CreateFuelRecordExecute()
		{
			string uriAddress = "/Views/FuelRecordCreateView.xaml";
			NavigationController.Instance.Navigate(new Uri(uriAddress, UriKind.Relative));
		}

		#endregion

		#region EditCommand
		private RelayCommand<FuelRecord> _editCommand;
		public RelayCommand<FuelRecord> EditCommand
		{
			get
			{
				if (_editCommand == null)
				{
					_editCommand =
						new RelayCommand<FuelRecord>(
							(item) =>
							{
								EditFuelRecordExecute(item);
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
		/// <param name="fuelRecord"></param>
		public void EditFuelRecordExecute(FuelRecord fuelRecord)
		{
			if (fuelRecord == null)
			{
				return;
			}

			int fuelRecordId = fuelRecord.FuelRecordId;
			NavigateToFuelRecordEdit(fuelRecordId);
		}

		/// <summary>
		/// Navigates to EditView passing the entity id to edit
		/// </summary>
		/// <param name="id"></param>
		private static void NavigateToFuelRecordEdit(int id)
		{
			string uriAddress = string.Format("/Views/FuelRecordEditViewPage.xaml?id={0}", id);
			NavigationController.Instance.Navigate(new Uri(uriAddress, UriKind.Relative));
		}

		#endregion

		#region ViewDetailsCommand
		private RelayCommand<FuelRecord> _viewDetailsCommand;
		public RelayCommand<FuelRecord> ViewDetailsCommand
		{
			get
			{
				if (_viewDetailsCommand == null)
				{
					_viewDetailsCommand =
						new RelayCommand<FuelRecord>(
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

		public void ViewDetailsExecute(FuelRecord fuelRecord)
		{
			var selectedFuelRecord = fuelRecord;
			//if (SelectedFuelRecord == null)
			//{
			//    return;
			//}

			int fuelRecordId = selectedFuelRecord.FuelRecordId;
			NavigateToFuelRecordDetails(fuelRecordId);
		}

		/// <summary>
		/// Navigates to Details view
		/// </summary>
		/// <param name="id"></param>
		private static void NavigateToFuelRecordDetails(int id)
		{
			string uriAddress = string.Format("/Views/FuelRecordDetailsView.xaml?id={0}", id);
			NavigationController.Instance.Navigate(new Uri(uriAddress, UriKind.Relative));
		}

		#endregion

		/// <summary>
		/// Loads entities list
		/// </summary>
		public void Load()
		{
			var fuelRecords = _fuelRecordRepository.All;
			this.FuelRecords = new ObservableCollection<FuelRecord>(fuelRecords);
		}


		/// <summary>
		/// Clean up resources here
		/// </summary>
		public void Cleanup()
		{
			FuelRecords = new ObservableCollection<FuelRecord>();
			SelectedFuelRecord = null;
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
