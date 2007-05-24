using System;
using System.Collections;
using System.Collections.Generic;
using System.Expressions;
using System.Query;
using SemWeb;

namespace RdfSerialisation
{
public class RdfContext : IRdfContext
{
    public Store Store
    {
        get { return store; }
        set { store = value; }
    }

    protected Store store;

    public RdfContext(Store store)
    {
        this.store = store;
    }

    public void AcceptChanges()
    {
        throw new NotImplementedException();
    }

	public IRdfQuery<T> ForType<T>()
    {
        return new RdfN3Query<T>(store);
    }
}
}