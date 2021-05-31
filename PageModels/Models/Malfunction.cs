using System;

namespace PageModels.Models
{
	public class Malfunction
	{
		public string Id { get; set; }

		public string Description { get; set; }

		public override bool Equals(object obj)
		{
			var otherMalfunction = obj as Malfunction;
			if (otherMalfunction == null)
				return false;
		
			return Id.Equals(otherMalfunction.Id) && Description.Equals(otherMalfunction.Description);
		}
		
		public override int GetHashCode()
		{
			return HashCode.Combine(Id.GetHashCode(), Description.GetHashCode());
		}
	}
}