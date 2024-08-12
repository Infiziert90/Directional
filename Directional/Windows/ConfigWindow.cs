using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Directional.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    private Directional plugin;

    public ConfigWindow(Directional plugin) : base("Directional Config Window###DirectionalConfigWindow")
    {
        configuration = plugin.Configuration;
        this.plugin = plugin;

        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(250, 200);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }


    public override void Draw()
    {
        var enabled = configuration.Enabled;
        var cardinals = configuration.DrawCardinals;
        var interCardinals = configuration.DrawInterCardinals;
        var trueNorth = configuration.TrueNorth;
        var fontSize = configuration.FontSize;
        var colour = configuration.Colour;
        var alwaysDraw = configuration.AlwaysDrawDirections;

        if (ImGui.Checkbox("Enabled##0", ref enabled))
        {
            configuration.Enabled = enabled;
            configuration.Save();
        }
        if (ImGui.Checkbox("Show on non-enemies##0", ref alwaysDraw))
        {
            configuration.AlwaysDrawDirections = alwaysDraw;
            configuration.Save();
        }

        ImGui.Spacing();
        ImGui.BeginGroup();
        CollapsingHeader("Compass##0", () =>
        {
            if (ImGui.Checkbox("Cardinals##0", ref cardinals))
            {
                configuration.DrawCardinals = cardinals;
                configuration.Save();
            }
            if (ImGui.Checkbox("Inter-cardinals##0", ref interCardinals))
            {
                configuration.DrawInterCardinals = interCardinals;
                configuration.Save();
            }
            if (ImGui.Checkbox("True North##0", ref trueNorth))
            {
                configuration.TrueNorth = trueNorth;
                configuration.Save();
            }
        });
        ImGui.EndGroup();
        ImGui.Spacing();
        ImGui.BeginGroup();
        CollapsingHeader("UI##0",
                         () =>
                         {
                             if (ImGui.InputInt("Font size (between 12 and 128)##0", ref fontSize))
                             {
                                 configuration.FontSize = Math.Clamp(fontSize, 12, 128);
                                 configuration.Save();
                                 plugin.UpdateFontHandle();
                                 
                             }

                             if (ImGui.ColorEdit4($"Colour ##0", ref colour, ImGuiColorEditFlags.NoInputs))
                             {
                                 configuration.Colour = colour;
                                 configuration.Save();
                             }
                         });
        ImGui.EndGroup();
    }

    private static void CollapsingHeader(string label, Action action)
    {
        if (ImGui.CollapsingHeader(label, ImGuiTreeNodeFlags.DefaultOpen)) action();
    }
}
