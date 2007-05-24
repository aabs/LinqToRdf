namespace RdfSerialisation
{
    internal interface IRdfContext
    {
        void AcceptChanges();
    	IRdfQuery<T> ForType<T>();
    }
}