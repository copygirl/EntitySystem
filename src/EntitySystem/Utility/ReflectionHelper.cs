using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EntitySystem.Utility
{
	public static class ReflectionHelper
	{
		static readonly Dictionary<Type, string> _friendlyNameLookup =
			new Dictionary<Type, string>() {
				{ typeof(void),    "void"    }, { typeof(object), "object" },
				{ typeof(sbyte),   "sbyte"   }, { typeof(byte),   "byte"   },
				{ typeof(int),     "int"     }, { typeof(uint),   "uint"   },
				{ typeof(short),   "short"   }, { typeof(ushort), "ushort" },
				{ typeof(long),    "long"    }, { typeof(ulong),  "ulong"  },
				{ typeof(float),   "float"   }, { typeof(double), "double" },
				{ typeof(decimal), "decimal" }, { typeof(bool),   "bool"   },
				{ typeof(char),    "char"    }, { typeof(string), "string" },
			};
		
		public static string GetFriendlyName(this Type type)
		{
			ThrowIf.Argument.IsNull(type, nameof(type));
			var typeInfo = type.GetTypeInfo();
			
			string name;
			if (_friendlyNameLookup.TryGetValue(type, out name))
				return name;
			
			if (type.IsArray)
				return new StringBuilder()
					.Append(type.GetElementType().GetFriendlyName())
					.Append('[')
					.Append(new string(',', type.GetArrayRank() - 1))
					.Append(']')
					.ToString();
			
			if (typeInfo.IsGenericType)
				return new StringBuilder()
					.Append(type.Name.Substring(0, type.Name.LastIndexOf('`')))
					.Append('<')
					.AppendAll(typeInfo.GenericTypeArguments.Select(GetFriendlyName), ",")
					.Append('>')
					.ToString();
			
			return type.Name;
		}
	}
}
