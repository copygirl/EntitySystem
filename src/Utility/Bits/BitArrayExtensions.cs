namespace EntitySystem.Utility.Bits
{
	public static class BitArrayExtensions
	{
		// Note: Using generic constraints instead of the interface type should
		//       avoid boxing as long as the type is known at compile time.
		/// <summary> Returns if any of the bits in this IBitArray are set. </summary>
		public static bool Any<TBits>(this TBits bits) where TBits : IBitArray
		{
			// This assumes that no bits outside
			// the range of the BitArray are set.
			for (var i = 0; i < (bits.Length >> 5); i++)
				if (bits.GetUInt(i) > 0)
					return true;
			return false;
		}
		
		// Too lazy to implement "All" as well.
		// !Any(~bits) will work for now, though only for known types.
	}
}

