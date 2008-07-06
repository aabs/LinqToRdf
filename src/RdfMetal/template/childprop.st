[OwlResource(OntologyName = "$handle$", RelativeUriReference = "$it.Name$")]
public string $it.Name$Uri { get; set; }

private EntityRef<$it.Range$> _$it.Name$ { get; set; }

[OwlResource(OntologyName = "$handle$", RelativeUriReference = "$it.Name$")]
public $it.Range$ $it.Name$
{
    get
    {
        if (_$it.Name$.HasLoadedOrAssignedValue)
            return _$it.Name$.Entity;
        if (DataContext != null)
        {
            var ctx = ($handle$DataContext)DataContext;
            _$it.Name$ = new EntityRef<$it.Range$>(from x in ctx.$it.Range$s where x.HasInstanceUri($it.Name$Uri) select x);
            return _$it.Name$.Entity;
        }
        return null;
    }
}
