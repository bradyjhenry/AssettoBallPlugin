using AssettoServer.Server;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace AssettoBallPlugin;

[ApiController]
[Route("AssettoBallPlugin")]
public class AssettoBallController : ControllerBase
{
    private readonly ACServer _server;
    private readonly AssettoBallConfiguration _configuration;

    public AssettoBallController(ACServer server, AssettoBallConfiguration configuration)
    {
        _server = server;
        _configuration = configuration;
    }

    [HttpGet("config")]
    [Produces("text/x-lua", new string[] { })]
    public AssettoBallConfiguration Config() => _configuration;

}
