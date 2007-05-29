using System;
using SemWeb;

namespace LinqToRdf
{
	public class RdfN3Context : IRdfContext
	{
		public Store Store
		{
			get { return store; }
			set { store = value; }
		}

		protected Store store;

		public RdfN3Context(Store store)
		{
			this.store = store;
		}

		public void AcceptChanges()
		{
			throw new NotImplementedException();
		}

		public IRdfQuery<T> ForType<T>()
		{
			QueryFactory<T> qf = new QueryFactory<T>(QueryType.LocalN3StoreInMemory);
			RdfN3Query<T> result = (RdfN3Query<T>)qf.CreateQuery<T>();
			result.Store = store;
			result.QueryFactory = qf;
			return result;
		}
	}
}