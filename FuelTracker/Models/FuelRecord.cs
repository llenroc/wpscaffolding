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
using System.Data.Linq.Mapping;
using System.ComponentModel;

namespace FuelTracker.Models
{
	[Table(Name = "FuelRecords")]
	public class FuelRecord : INotifyPropertyChanged
	{
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

		private int _fuelRecordId;
		[Column(IsPrimaryKey = true,
		IsDbGenerated = true)]
		public int FuelRecordId
		{
			get
			{
				return _fuelRecordId;
			}

			set
			{
				if (_fuelRecordId == value)
				{
					return;
				}
				_fuelRecordId = value;
				NotifyPropertyChanged("FuelRecordId");
			}
		}

		private double _quantity;
		[Column(CanBeNull = false)]
		public double Quantity
		{
			get
			{
				return _quantity;
			}

			set
			{
				if (_quantity == value)
				{
					return;
				}
				_quantity = value;
				NotifyPropertyChanged("Quantity");
			}
		}

		private double _amount;
		[Column]
		public double Amount
		{
			get
			{
				return _amount;
			}

			set
			{
				if (_amount == value)
				{
					return;
				}
				_amount = value;
				NotifyPropertyChanged("Amount");
			}
		}

		private DateTime? _refueldate;
		[Column(CanBeNull=true)]
		public DateTime? RefuelDate
		{
			get
			{
				return _refueldate;
			}

			set
			{
				if (_refueldate == value)
				{
					return;
				}
				_refueldate = value;
				NotifyPropertyChanged("RefuelDate");
			}
		}

		private string _notes;
		[Column]
		public string Notes
		{
			get
			{
				return _notes;
			}

			set
			{
				if (_notes == value)
				{
					return;
				}
				_notes = value;
				NotifyPropertyChanged("Notes");
			}
		}

	}
}
