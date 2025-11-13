using System.Collections.Generic;

public class QueryOptions
{
    public string OrderBy { get; set; }
    public bool Descending { get; set; } = false;
    public int? Limit { get; set; }
    public Dictionary<string, object> Filters { get; set; } = new();
}