using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace FuelTracker.Models
{
	public class FuelTrackerContext : DataContext
	{
		// You can add custom code to this file. Changes will not be overwritten.

		public FuelTrackerContext(string connection)
			: base(connection){ }

		public Table<FuelTracker.Models.Car> Cars;
		public Table<FuelTracker.Models.FuelRecord> FuelRecords;

	}
}