using AssettoServer.Server;
using Microsoft.AspNetCore.Mvc;

namespace AssettoBallPlugin;

[ApiController]
[Route("RallyPlugin")]
public class RallyController : ControllerBase
{
    private readonly ACServer _server;
    private readonly AssettoBallConfiguration _configuration;

    public RallyController(ACServer server, AssettoBallConfiguration configuration)
    {
        _server = server;
        _configuration = configuration;
    }

    [HttpGet("config")]
    [Produces("text/x-lua", new string[] { })]
    public AssettoBallConfiguration Config() => _configuration;
}
