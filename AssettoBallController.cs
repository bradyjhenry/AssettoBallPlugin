using AssettoServer.Server;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace AssettoBallPlugin;

[ApiController]
[Route("AssettoBallPlugin")]
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

    private static readonly string BasePath = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Res");

    [HttpGet("balltexture.png")]
    public IActionResult GetWrongWayImage()
    {
        return new PhysicalFileResult(Path.Join(BasePath, "balltexture.png"), "image/png");
    }
}
