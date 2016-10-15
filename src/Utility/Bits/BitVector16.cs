using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace EntitySystem.Utility.Bits
{
	public struct BitVector16 : IBitArray, IEquatable<BitVector16>
	{
		ushort _bits;
		
		public int Length => 16;
		
		public bool this[int index] {
			get {
				if ((index < 0) || (index >= Length))
					throw new ArgumentOutOfRangeException();
				return (((_bits >> index) & 1) != 0);
			}
			set {
				if ((index < 0) || (index >= Length))
					throw new ArgumentOutOfRangeException();
				if (value) _bits |= (ushort)(1 << index);
				else _bits &= (ushort)~(1 << index);
			}
		}
		
		
		public BitVector16(ushort bits = 0) { _bits = bits; }
		public BitVector16(uint bits) : this(unchecked((byte)bits)) {  }
		
		public BitVector16(BitVector16 bits) : this(bits._bits) {  }
		public BitVector16(IBitArray bits) : this(bits.GetUInt(0)) {  }
		
		uint IBitArray.GetUInt(int index) {
			if (index == 0) return _bits;
			throw new ArgumentOutOfRangeException("index");
		}
		
		
		public bool Equals(BitVector16 other) => (other._bits == _bits);
		
		public override bool Equals(object obj) =>
			((obj is BitVector16) && Equals((BitVector16)obj));
		
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
		
		
		public static BitVector16 operator ~(BitVector16 bits) =>
			new BitVector16((ushort)~bits._bits);
		
		public static BitVector16 operator &(BitVector16 left, BitVector16 right) =>
			new BitVector16((byte)(left._bits & right._bits));
		
		public static BitVector16 operator |(BitVector16 left, BitVector16 right) =>
			new BitVector16((byte)(left._bits | right._bits));
		
		public static BitVector16 operator ^(BitVector16 left, BitVector16 right) =>
			new BitVector16((byte)(left._bits ^ right._bits));
		
		
		public static BitVector16 operator >>(BitVector16 bits, int shift) =>
			new BitVector16((byte)(bits._bits >> shift));
		
		public static BitVector16 operator <<(BitVector16 bits, int shift) =>
			new BitVector16((byte)(bits._bits << shift));
		
		public static bool operator ==(BitVector16 left, BitVector16 right) =>
			(left._bits == right._bits);
		
		public static bool operator !=(BitVector16 left, BitVector16 right) =>
			(left._bits != right._bits);
	}
}
