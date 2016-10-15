namespace EntitySystem.Utility
{
	public static class HashHelper
	{
		public static int For<T>(T arg)
		{
			unchecked {
				int hash = (int)2166136261;
				hash = (hash * 16777619) ^ arg.GetHashCode();
				return hash;
			}
		}
		
		public static int For<T1, T2>(T1 arg1, T2 arg2)
		{
			unchecked {
				int hash = (int)2166136261;
				hash = (hash * 16777619) ^ arg1.GetHashCode();
				hash = (hash * 16777619) ^ arg2.GetHashCode();
				return hash;
			}
		}
		
		public static int For<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
		{
			unchecked {
				int hash = (int)2166136261;
				hash = (hash * 16777619) ^ arg1.GetHashCode();
				hash = (hash * 16777619) ^ arg2.GetHashCode();
				hash = (hash * 16777619) ^ arg3.GetHashCode();
				return hash;
			}
		}
		
		public static int For<T1, T2, T3, T4>
			(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			unchecked {
				int hash = (int)2166136261;
				hash = (hash * 16777619) ^ arg1.GetHashCode();
				hash = (hash * 16777619) ^ arg2.GetHashCode();
				hash = (hash * 16777619) ^ arg3.GetHashCode();
				hash = (hash * 16777619) ^ arg4.GetHashCode();
				return hash;
			}
		}
		
		public static int For<T1, T2, T3, T4, T5>
			(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			unchecked {
				int hash = (int)2166136261;
				hash = (hash * 16777619) ^ arg1.GetHashCode();
				hash = (hash * 16777619) ^ arg2.GetHashCode();
				hash = (hash * 16777619) ^ arg3.GetHashCode();
				hash = (hash * 16777619) ^ arg4.GetHashCode();
				hash = (hash * 16777619) ^ arg5.GetHashCode();
				return hash;
			}
		}
		
		public static int For<T1, T2, T3, T4, T5, T6>
			(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			unchecked {
				int hash = (int)2166136261;
				hash = (hash * 16777619) ^ arg1.GetHashCode();
				hash = (hash * 16777619) ^ arg2.GetHashCode();
				hash = (hash * 16777619) ^ arg3.GetHashCode();
				hash = (hash * 16777619) ^ arg4.GetHashCode();
				hash = (hash * 16777619) ^ arg5.GetHashCode();
				hash = (hash * 16777619) ^ arg6.GetHashCode();
				return hash;
			}
		}
		
		public static int For<T1, T2, T3, T4, T5, T6, T7>
			(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			unchecked {
				int hash = (int)2166136261;
				hash = (hash * 16777619) ^ arg1.GetHashCode();
				hash = (hash * 16777619) ^ arg2.GetHashCode();
				hash = (hash * 16777619) ^ arg3.GetHashCode();
				hash = (hash * 16777619) ^ arg4.GetHashCode();
				hash = (hash * 16777619) ^ arg5.GetHashCode();
				hash = (hash * 16777619) ^ arg6.GetHashCode();
				hash = (hash * 16777619) ^ arg7.GetHashCode();
				return hash;
			}
		}
		
		public static int For<T1, T2, T3, T4, T5, T6, T7, T8>
			(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			unchecked {
				int hash = (int)2166136261;
				hash = (hash * 16777619) ^ arg1.GetHashCode();
				hash = (hash * 16777619) ^ arg2.GetHashCode();
				hash = (hash * 16777619) ^ arg3.GetHashCode();
				hash = (hash * 16777619) ^ arg4.GetHashCode();
				hash = (hash * 16777619) ^ arg5.GetHashCode();
				hash = (hash * 16777619) ^ arg6.GetHashCode();
				hash = (hash * 16777619) ^ arg7.GetHashCode();
				hash = (hash * 16777619) ^ arg8.GetHashCode();
				return hash;
			}
		}
		
		public static int ForAny(params object[] args)
		{
			unchecked {
				var hash = (int)2166136261;
				foreach (var arg in args)
					hash = (hash * 16777619) ^ (arg?.GetHashCode() ?? 0);
				return hash;
			}
		}
	}
}
