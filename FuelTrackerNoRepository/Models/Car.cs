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
	[Table(Name="Cars")]
	public class Car : INotifyPropertyChanged
	{
		private int _carId;
		[Column(IsPrimaryKey = true, IsDbGenerated = true)]
		public int CarId
		{
			get
			{
				return _carId;
			}

			set
			{
				if (_carId == value)
				{
					return;
				}
				_carId = value;
				RaisePropertyChanged("CarId");
			}
		}

		private string _name;
		[Column(CanBeNull=false)]
		public string Name
		{
			get
			{
				return _name;
			}

			set
			{
				if (_name == value)
				{
					return;
				}
				_name = value;
				RaisePropertyChanged("Name");
			}
		}

		private string _mark;
		[Column()]
		public string Mark
		{
			get
			{
				return _mark;
			}

			set
			{
				if (_mark == value)
				{
					return;
				}
				_mark = value;
				RaisePropertyChanged("Mark");
			}
		}

		private string _model;
		[Column]
		public string Model
		{
			get
			{
				return _model;
			}

			set
			{
				if (_model == value)
				{
					return;
				}
				_model = value;
				RaisePropertyChanged("Model");
			}
		}

		private DateTime? _manufactureDate;
		[Column]
		public DateTime? ManufactureDate
		{
			get
			{
				return _manufactureDate;
			}

			set
			{
				if (_manufactureDate == value)
				{
					return;
				}
				_manufactureDate = value;
				RaisePropertyChanged("ManufactureDate");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
