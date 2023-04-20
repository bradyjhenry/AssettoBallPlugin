using AssettoServer.Server.Plugin;
using Autofac;

namespace AssettoBallPlugin;

public class AssettoBallModule : AssettoServerModule<AssettoBallConfiguration>
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AssettoBall>().AsSelf().As<IAssettoServerAutostart>().SingleInstance();
        builder.RegisterType<EntryCarAssettoBall>().AsSelf();
    }
}
