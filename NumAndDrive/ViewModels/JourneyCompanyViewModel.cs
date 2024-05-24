using System;
using NumAndDrive.Models;

namespace NumAndDrive.ViewModels
{
	public class JourneyCompanyViewModel
	{
		public Company Company { get; set; }
		public Journey Journey { get; set; }
		public Address Address { get; set; }
		public string AddressToTrim { get; set; }

	}
}

