private EntitySet<$it.HostClass.Name$> _$it.Name$s = new EntitySet<$it.HostClass.Name$>();
[OwlResource(OntologyName = "$handle$", RelativeUriReference = "$it.Name$")]
public EntitySet<$it.HostClass.Name$> $it.Name$
{
    get
    {
        if (_$it.Name$s.HasLoadedOrAssignedValues)
            return _$it.Name$s;
        if (DataContext != null)
        {
            var ctx = ($handle$DataContext)DataContext;
            _$it.Name$s.SetSource(from t in ctx.$it.HostClass.Name$s
								  where t.$it.Name$.HavingSubjectUri(this.InstanceUri)
								  select t);
        }
        return _$it.Name$s;
    }
}
