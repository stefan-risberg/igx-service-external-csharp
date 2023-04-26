using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


/// <summary>
/// Echo service endpoint
/// </summary>
[ApiController]
[Authorize]
public class Echo : ControllerBase {
    ILogger logger;

    /// <summary>
    /// Base constructor taking a logger
    /// </summary>
    public Echo(ILogger<Echo> _logger) {
        logger = _logger;
    }
    /// <summary>
    /// The post endpoint for echo.
    ///
    /// </summary>
    /// <remarks>
    /// Will only look at the first parameter in <code>inParams</code> and return it.
    /// </remarks>
    /// <param name="req">Body of request</param>
    /// <param name="customerKey" example="keytest">Customer key</param>
    /// <param name="orgKey" example="def">What organization is calling</param>
    [HttpPost("/template/igx-service/{customerKey}/{orgKey}/echo")]
    [ProducesResponseType(typeof(EchoResponse), 200)]
    [ProducesResponseType(500)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public Results<Ok<EchoResponse>, BadRequest> PostEcho(
        [FromBody] EchoRequest req,
        string customerKey,
        string orgKey) {

        try {
        var param1 = req.InParams["1"];
        var resp = new EchoResponse();

        resp.OutParams["1"] = param1;
        resp.Ref = req.Ref;
        return TypedResults.Ok<EchoResponse>(resp);
        } catch (KeyNotFoundException e) {
            logger.LogInformation($"Nothing set in key 1 {e.ToString()}");
            return TypedResults.Ok<EchoResponse>(new EchoResponse());
        }
    }
}

/// <summary>
/// Echo request
/// </summary>
public class EchoRequest {
    /// <summary>
    /// Holds all incoming paramaters from the IGX service.
    /// </summary>
    /// <example> {"1": "test 123"}
    /// </example>
    public Dictionary<string, string> InParams { get ; set ; } = default!;
    /// <summary>
    /// The reference to the request.
    /// </summary>
    /// <example>123123</example>
    public string Ref { get; set ; } = default!;
}

/// <summary>
/// Response to echo request
/// </summary>
public class EchoResponse {
    /// <summary>
    /// Holds all return parameters that are sent back to the IGX Service.
    /// </summary>
    /// <example>{"1": "test 123"}</example>
    public Dictionary<string, string> OutParams { get ; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Reference copied from <c>Request</c>
    /// </summary>
    /// <example>123123</example>
    public string Ref { get; set; } = "";
}
