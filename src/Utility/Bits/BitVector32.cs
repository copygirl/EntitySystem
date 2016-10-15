using System;
using System.Collections;

namespace EntitySystem.Utility.Bits
{
	public struct BitVector32 : IBitArray, IEquatable<BitVector32>
	{
		uint _bits;
		
		public int Length => 32;
		
		public bool this[int index] {
			get {
				if ((index < 0) || (index >= Length))
					throw new ArgumentOutOfRangeException();
				return (((_bits >> index) & 1) != 0);
			}
			set {
				if ((index < 0) || (index >= Length))
					throw new ArgumentOutOfRangeException();
				if (value) _bits |= (1U << index);
				else _bits &= ~(1U << index);
			}
		}
		
		
		public BitVector32(uint bits = 0) { _bits = bits; }
		
		public BitVector32(BitVector32 bits) : this(bits._bits) {  }
		public BitVector32(IBitArray bits) : this(bits.GetUInt(0)) {  }
		
		uint IBitArray.GetUInt(int index) {
			if (index == 0) return _bits;
			throw new ArgumentOutOfRangeException("index");
		}
		
		
		public bool Equals(BitVector32 other) => (other._bits == _bits);
		
		public override bool Equals(object obj) =>
			((obj is BitVector32) && Equals((BitVector32)obj));
		
		public override int GetHashCode() => _bits.GetHashCode();
		
		public override string ToString() => string.Format("[BitVector32 {0:X16}]", _bits);
		
		
		public System.Collections.Generic.IEnumerator<bool> GetEnumerator()
		{
			var bits = _bits;
			for (var i = 0; i < Length; i++, bits >>= 1)
				yield return ((bits & 1) != 0);
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		
		
		public static BitVector32 operator ~(BitVector32 bits) =>
			new BitVector32((ushort)~bits._bits);
		
		public static BitVector32 operator &(BitVector32 left, BitVector32 right) =>
			new BitVector32((byte)(left._bits & right._bits));
		
		public static BitVector32 operator |(BitVector32 left, BitVector32 right) =>
			new BitVector32((byte)(left._bits | right._bits));
		
		public static BitVector32 operator ^(BitVector32 left, BitVector32 right) =>
			new BitVector32((byte)(left._bits ^ right._bits));
		
		
		public static BitVector32 operator >>(BitVector32 bits, int shift) =>
			new BitVector32((byte)(bits._bits >> shift));
		
		public static BitVector32 operator <<(BitVector32 bits, int shift) =>
			new BitVector32((byte)(bits._bits << shift));
		
		public static bool operator ==(BitVector32 left, BitVector32 right) =>
			(left._bits == right._bits);
		
		public static bool operator !=(BitVector32 left, BitVector32 right) =>
			(left._bits != right._bits);
	}
}
