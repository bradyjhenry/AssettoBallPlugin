﻿using AssettoServer.Server;
using AssettoServer.Server.Configuration;
using AssettoServer.Server.Plugin;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Reflection;
using AssettoServer.Shared.Services;

namespace AssettoBallPlugin;

public class AssettoBall : CriticalBackgroundService, IAssettoServerAutostart
{

    private readonly EntryCarManager _entryCarManager;

    private readonly ACServerConfiguration _serverConfiguration;

    private readonly Func<EntryCar, GameEntryCar> _entryCarFactory;

    private readonly Dictionary<int, GameEntryCar> _instances = new();

    private readonly AssettoBallConfiguration _configuration;

    private Game? _game;

    public AssettoBall(EntryCarManager entryCarManager, Func<EntryCar, GameEntryCar> entryCarFactory, AssettoBallConfiguration configuration, ACServerConfiguration serverConfiguration, CSPServerScriptProvider scriptProvider, IHostApplicationLifetime applicationLifetime) : base(applicationLifetime)
    {
        _entryCarManager = entryCarManager;
        _entryCarFactory = entryCarFactory;
        _serverConfiguration = serverConfiguration;
        _configuration = configuration;

        if (_serverConfiguration.Extra.EnableClientMessages)
        {
            using var streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("AssettoBallPlugin.lua.assettoballplugin.lua")!);
            scriptProvider.AddScript(streamReader.ReadToEnd(), "assettoballplugin.lua");

            using var streamReader2 = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("AssettoBallPlugin.lua.assettoballui.lua")!);
            scriptProvider.AddScript(streamReader2.ReadToEnd(), "assettoballui.lua");
        }
        else
        {
            throw new ConfigurationException("AssettoBall: EnableClientMessages must be set to true in extra_cfg man!");
        }

        Log.Debug("AssettoBall Loaded My Man");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        foreach (var entryCar in _entryCarManager.EntryCars)
        {
            var entryCarAssettoBall = _entryCarFactory(entryCar);
            _instances.Add(entryCar.SessionId, entryCarAssettoBall);
        }

        _game = new Game(_configuration, _instances);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _game.Update();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during game update");
            }
            finally
            {
                await Task.Delay(16, stoppingToken);
            }
        }
    }


}
