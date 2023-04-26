using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


#pragma warning disable 1591
[ApiController]
[Authorize]
public class TestConnection : ControllerBase {
    ILogger logger;

    /// <summary>
    /// Base constructor taking a logger
    /// </summary>
    public TestConnection(ILogger<Echo> _logger) {
        logger = _logger;
    }

    [HttpPost("/template/igx-service/{customerKey}/{orgKey}/test-connection")]
    [ProducesResponseType(typeof(EchoResponse), 200)]
    [ProducesResponseType(500)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public Results<Ok<EchoResponse>, BadRequest> PostTestConnection(
        [FromBody] EchoRequest req,
        string customerKey,
        string orgKey) {

        var resp = new EchoResponse();

        resp.OutParams = req.InParams;
        resp.OutParams["1"] = "OK";
        resp.Ref = req.Ref;
        return TypedResults.Ok<EchoResponse>(resp);
    }
}

public class KeepAliveRequest {
    public Dictionary<string, string> InParams { get; set; } = default!;
    public string Ref { get; set; } = default!;
}

public class KeepAliveResponse {
    public Dictionary<string, string> OutParams { get; set; } = new Dictionary<string, string>();
    public string Ref { get; set; } = "";
}
