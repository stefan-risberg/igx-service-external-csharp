public class Request
{
    public Dictionary<string, string>? InParams { get; set; }
    public string? Ref { get; set; }
}

public class Response
{
    public Dictionary<string, string>? OutParams { get; set; }
    public string? Ref { get; set; }
}
