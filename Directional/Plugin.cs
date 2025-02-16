﻿using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Command;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Directional.Utils;
using Directional.Windows.Config;

namespace Directional;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IGameGui GameGui { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

    private const string CommandName = "/dtarget"; // :D
    public readonly WindowSystem WindowSystem = new("Directional");

    public Configuration Configuration { get; init; }
    private ConfigWindow ConfigWindow { get; init; }

    private IFontHandle Font { get; set; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        ConfigWindow = new ConfigWindow(this);
        WindowSystem.AddWindow(ConfigWindow);

        PluginInterface.UiBuilder.Draw += DrawUi;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi += ToggleConfigUi;

        Font = PluginInterface.UiBuilder.FontAtlas.NewGameFontHandle(new GameFontStyle(GameFontFamily.Axis, Configuration.FontSize));

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Draw the compass directionals around the target for easier callouts"
        });
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();
        PluginInterface.UiBuilder.Draw -= DrawUi;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleConfigUi;
        ConfigWindow.Dispose();
        CommandManager.RemoveHandler(CommandName);
    }

    private void DrawUi()
    {
        WindowSystem.Draw();

        if (!Configuration.Enabled)
            return;

        var player = ClientState.LocalPlayer;
        if (player == null)
            return;

        var target = player.TargetObject;
        var isInCombat = player.StatusFlags.HasFlag(StatusFlags.InCombat);
        var isInCombatConfig = !(!isInCombat && Configuration.CombatOnly);

        if (target is IBattleNpc battleNpc && isInCombatConfig)
        {
            var shouldDrawCardinals = Configuration.DrawCardinals;
            var shouldDrawInterCardinals = Configuration.DrawInterCardinals;

            if (battleNpc.StatusFlags.HasFlag(StatusFlags.Hostile) || Configuration.AlwaysDrawDirections)
            {
                if (Configuration.TrueNorth)
                {
                    using (Font.Push())
                    {
                        DirectionUtils.DrawDirectionLabelsRelativeToTrueNorth(
                            target,
                            shouldDrawCardinals,
                            shouldDrawInterCardinals,
                            Configuration.Colour,
                            Configuration.ColourBorder);
                    }
                }
                else
                {
                    using (Font.Push())
                    {
                        DirectionUtils.DrawDirectionLabels(
                            target,
                            shouldDrawCardinals,
                            shouldDrawInterCardinals,
                            Configuration.Colour,
                            Configuration.ColourBorder);
                    }
                }
            }
        }
    }

    internal void UpdateFontHandle()
    {
        Font = PluginInterface.UiBuilder.FontAtlas.NewGameFontHandle(new GameFontStyle(GameFontFamily.Axis, Configuration.FontSize));
    }

    private void OnCommand(string command, string args)
    {
        Configuration.Enabled = !Configuration.Enabled;
    }

    public void ToggleConfigUi()
    {
        ConfigWindow.Toggle();
    }
}
