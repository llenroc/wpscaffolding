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
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace FuelTracker.Models
{
	public class FuelRecordRepository : IFuelRecordRepository
	{
		FuelTrackerContext _context;

		public FuelRecordRepository(string connection)
		{
			_context = new FuelTrackerContext(connection);
			if (!_context.DatabaseExists())
			{
				_context.CreateDatabase();
			}
		}

		public IQueryable<FuelRecord> All
		{
			get { return _context.FuelRecords; }
		}

		public IQueryable<FuelRecord> AllIncluding(params Expression<Func<FuelRecord, object>>[] includeProperties)
		{
			var dataOptions = new System.Data.Linq.DataLoadOptions();
			foreach (var expression in includeProperties)
			{
				dataOptions.LoadWith<FuelRecord>(expression);
			}
			_context.LoadOptions = dataOptions;
			IQueryable<FuelRecord> query = _context.FuelRecords;
			return query;
		}

		public FuelRecord Find(int id)
		{
			return _context.FuelRecords.Single(x => x.FuelRecordId == id);
		}

		public void InsertOrUpdate(FuelRecord fuelRecord)
		{
			if (fuelRecord.FuelRecordId == default(int))
			{
				_context.FuelRecords.InsertOnSubmit(fuelRecord);
			}
			else
				if (_context.FuelRecords.Contains(fuelRecord))
				{
					return;
				}
				else
				{
					_context.FuelRecords.Attach(fuelRecord, true);
				}
		}

		public void Delete(int id)
		{
			var fuelRecord = _context.FuelRecords.Single(x => x.FuelRecordId == id);
			_context.FuelRecords.DeleteOnSubmit(fuelRecord);
		}

		public void Save()
		{
			_context.SubmitChanges();
		}

	}

	public interface IFuelRecordRepository
	{
		IQueryable<FuelRecord> All { get; }
		IQueryable<FuelRecord> AllIncluding(params Expression<Func<FuelRecord, object>>[] includeProperties);
		FuelRecord Find(int id);
		void InsertOrUpdate(FuelRecord fuelRecord);
		void Delete(int id);
		void Save();
	}
}
