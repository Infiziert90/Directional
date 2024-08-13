using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Directional.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    private readonly Directional plugin;

    public ConfigWindow(Directional plugin) : base("Directional Config Window###DirectionalConfigWindow")
    {
        configuration = plugin.Configuration;
        this.plugin = plugin;

        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(300, 200);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }


    public override void Draw()
    {
        ImGui.BeginGroup();
        CollapsingHeader("General##0", () =>
        {
            if (ImGui.Checkbox("Enabled##0", ref configuration.Enabled))
                configuration.Save();

            if (ImGui.Checkbox("Show only during combat##0", ref configuration.CombatOnly))
                configuration.Save();

            if (ImGui.Checkbox("Show on non-enemies##0", ref configuration.AlwaysDrawDirections))
                configuration.Save();
        });
        ImGui.EndGroup();

        ImGui.Spacing();
        ImGui.BeginGroup();
        CollapsingHeader("Compass##0", () =>
        {
            if (ImGui.Checkbox("Cardinals##0", ref configuration.DrawCardinals))
                configuration.Save();

            if (ImGui.Checkbox("Inter-cardinals##0", ref configuration.DrawInterCardinals))
                configuration.Save();

            if (ImGui.Checkbox("True North##0", ref configuration.TrueNorth))
                configuration.Save();
        });
        ImGui.EndGroup();

        ImGui.Spacing();
        ImGui.BeginGroup();
        CollapsingHeader("UI##0", () =>
        {
            if (ImGui.InputInt("Font size (between 12 and 128)##0", ref configuration.FontSize))
            {
                configuration.FontSize = Math.Clamp(configuration.FontSize, 12, 128);
                configuration.Save();
                plugin.UpdateFontHandle();
            }

            if (ImGui.ColorEdit4("Colour ##0", ref configuration.Colour, ImGuiColorEditFlags.NoInputs))
            {
                configuration.Colour.W = 1.0f;
                configuration.Save();
            }

            if (ImGui.ColorEdit4("Border Colour ##0", ref configuration.ColourBorder, ImGuiColorEditFlags.NoInputs))
            {
                configuration.ColourBorder.W = 1.0f;
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
