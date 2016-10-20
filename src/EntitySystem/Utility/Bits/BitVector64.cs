using System;
using System.Collections;

namespace EntitySystem.Utility.Bits
{
	public struct BitVector64 : IBitArray, IEquatable<BitVector64>
	{
		ulong _bits;
		
		public int Length => 64;
		
		public bool this[int index] {
			get {
				if ((index < 0) || (index >= Length))
					throw new ArgumentOutOfRangeException();
				return (((_bits >> index) & 1) != 0);
			}
			set {
				if ((index < 0) || (index >= Length))
					throw new ArgumentOutOfRangeException();
				if (value) _bits |= (1UL << index);
				else _bits &= ~(1UL << index);
			}
		}
		
		
		public BitVector64(ulong bits = 0) { _bits = bits; }
		
		public BitVector64(BitVector64 bits) : this(bits._bits) {  }
		public BitVector64(IBitArray bits)
			: this(bits.GetUInt(0) | ((ulong)bits.GetUInt(1) << 32)) {  }
		
		uint IBitArray.GetUInt(int index) {
			switch (index) {
				case 0: return unchecked((uint)_bits);
				case 1: return unchecked((uint)_bits >> 32);
				default: throw new ArgumentOutOfRangeException("index");
			}
		}
		
		
		public bool Equals(BitVector64 other) => (other._bits == _bits);
		
		public override bool Equals(object obj) =>
			((obj is BitVector64) && Equals((BitVector64)obj));
		
		public override int GetHashCode() => _bits.GetHashCode();
		
		public override string ToString() => string.Format("[BitVector64 {0:X16}]", _bits);
		
		
		public System.Collections.Generic.IEnumerator<bool> GetEnumerator()
		{
			var bits = _bits;
			for (var i = 0; i < Length; i++, bits >>= 1)
				yield return ((bits & 1) != 0);
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		
		
		public static BitVector64 operator ~(BitVector64 bits) =>
			new BitVector64((ushort)~bits._bits);
		
		public static BitVector64 operator &(BitVector64 left, BitVector64 right) =>
			new BitVector64((byte)(left._bits & right._bits));
		
		public static BitVector64 operator |(BitVector64 left, BitVector64 right) =>
			new BitVector64((byte)(left._bits | right._bits));
		
		public static BitVector64 operator ^(BitVector64 left, BitVector64 right) =>
			new BitVector64((byte)(left._bits ^ right._bits));
		
		
		public static BitVector64 operator >>(BitVector64 bits, int shift) =>
			new BitVector64((byte)(bits._bits >> shift));
		
		public static BitVector64 operator <<(BitVector64 bits, int shift) =>
			new BitVector64((byte)(bits._bits << shift));
		
		public static bool operator ==(BitVector64 left, BitVector64 right) =>
			(left._bits == right._bits);
		
		public static bool operator !=(BitVector64 left, BitVector64 right) =>
			(left._bits != right._bits);
	}
}
