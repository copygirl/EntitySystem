using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace EntitySystem.Utility.Bits
{
	public struct BitVector8 : IBitArray, IEquatable<BitVector8>
	{
		byte _bits;
		
		public int Length => 8;
		
		public bool this[int index] {
			get {
				if ((index < 0) || (index >= Length))
					throw new ArgumentOutOfRangeException();
				return (((_bits >> index) & 1) != 0);
			}
			set {
				if ((index < 0) || (index >= Length))
					throw new ArgumentOutOfRangeException();
				if (value) _bits |= (byte)(1 << index);
				else _bits &= (byte)~(1 << index);
			}
		}
		
		
		public BitVector8(byte bits = 0) { _bits = bits; }
		public BitVector8(uint bits) : this(unchecked((byte)bits)) {  }
		
		public BitVector8(BitVector8 bits) : this(bits._bits) {  }
		public BitVector8(IBitArray bits) : this(bits.GetUInt(0)) {  }
		
		uint IBitArray.GetUInt(int index) {
			if (index == 0) return _bits;
			throw new ArgumentOutOfRangeException("index");
		}
		
		
		public bool Equals(BitVector8 other) => (other._bits == _bits);
		
		public override bool Equals(object obj) =>
			((obj is BitVector8) && Equals((BitVector8)obj));
		
		public override int GetHashCode() => _bits.GetHashCode();
		
		public override string ToString() => new StringBuilder(Length + 2)
			.Append('[')
			.AppendAll(this.Select(b => (b ? '1' : '0')))
			.Append(']')
			.ToString();
		
		
		public System.Collections.Generic.IEnumerator<bool> GetEnumerator()
		{
			var bits = _bits;
			for (var i = 0; i < Length; i++, bits >>= 1)
				yield return ((bits & 1) != 0);
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		
		
		public static BitVector8 operator ~(BitVector8 bits) =>
			new BitVector8((byte)~bits._bits);
		
		public static BitVector8 operator &(BitVector8 left, BitVector8 right) =>
			new BitVector8((byte)(left._bits & right._bits));
		
		public static BitVector8 operator |(BitVector8 left, BitVector8 right) =>
			new BitVector8((byte)(left._bits | right._bits));
		
		public static BitVector8 operator ^(BitVector8 left, BitVector8 right) =>
			new BitVector8((byte)(left._bits ^ right._bits));
		
		
		public static BitVector8 operator >>(BitVector8 bits, int shift) =>
			new BitVector8((byte)(bits._bits >> shift));
		
		public static BitVector8 operator <<(BitVector8 bits, int shift) =>
			new BitVector8((byte)(bits._bits << shift));
		
		public static bool operator ==(BitVector8 left, BitVector8 right) =>
			(left._bits == right._bits);
		
		public static bool operator !=(BitVector8 left, BitVector8 right) =>
			(left._bits != right._bits);
	}
}
