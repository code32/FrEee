﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using static FrEee.Utility.Extensions.CommonExtensions;

namespace FrEee.Utility
{
	public class SimpleDataObject<T> : MarshalByRefObject, IDataObject
		where T : IDataObject
	{
		public SimpleDataObject()
		{

		}

		public SimpleDataObject(T t, ObjectGraphContext ctx = null)
		{
			if (t != null)
				Data = t.Data;
			else
				Data = new SafeDictionary<string, object>();
			Context = ctx;
		}

		private ObjectGraphContext Context;

		public SafeDictionary<string, IData<object>> SimpleData
		{
			get; set;
		}

		public SafeDictionary<string, object> Data
		{
			get
			{
				var dict = new SafeDictionary<string, object>();
				foreach (var pname in SimpleData.Keys)
					dict[pname] = SimpleData[pname]?.Value;
				return dict;
			}
			set
			{
				SimpleData = new SafeDictionary<string, IData<object>>();
				if (Context == null)
					Context = new ObjectGraphContext();
				foreach (var pname in value.Keys)
				{
					var pval = value[pname];
					if (pval == null)
						SimpleData[pname] = null;
					else if (pval.GetType().IsScalar())
						SimpleData[pname] = new DataScalar(pval);
					else
						SimpleData[pname] = (IData<object>)typeof(DataReference<>).MakeGenericType(pval.GetType()).Instantiate(Context, pval);
				}
			}
		}

		public T Value
		{
			get
			{
				var t = Instantiate<T>();
				t.Data = Data;
				return t;
			}
			set
			{
				Data = value.Data;
			}
		}

		public static implicit operator T(SimpleDataObject<T> d)
		{
			return d.Value;
		}
	}
}