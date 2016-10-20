using System.Collections.Generic;

namespace EntitySystem.Utility.Bits
{
	public interface IBitArray : IEnumerable<bool>
	{
		/// <summary> Gets the length of this BitArray in bits. </summary>
		int Length { get; }
		
		/// <summary> Gets or sets the bit at the specified index. </summary>
		bool this[int index] { get; set; }
		
		
		/// <summary> Returns the raw integer at the specified index. </summary>
		uint GetUInt(int index);
	}
}

