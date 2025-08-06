using System;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;

namespace Directional.Windows.Config;

public partial class ConfigWindow
{
    private void Settings()
    {
        using var tabItem = ImRaii.TabItem("Settings");
        if (!tabItem.Success)
            return;

        var changed = false;

        if (ImGui.CollapsingHeader("General", ImGuiTreeNodeFlags.DefaultOpen))
        {
            changed |= ImGui.Checkbox("Enabled##0", ref Plugin.Configuration.Enabled);
            changed |= ImGui.Checkbox("Show only during combat", ref Plugin.Configuration.CombatOnly);
            changed |= ImGui.Checkbox("Show on non-enemies", ref Plugin.Configuration.AlwaysDrawDirections);
        }

        ImGui.Spacing();

        if (ImGui.CollapsingHeader("Compass", ImGuiTreeNodeFlags.DefaultOpen))
        {
            changed |= ImGui.Checkbox("Cardinals", ref Plugin.Configuration.DrawCardinals);
            changed |= ImGui.Checkbox("Inter-cardinals", ref Plugin.Configuration.DrawInterCardinals);
            changed |= ImGui.Checkbox("True North", ref Plugin.Configuration.TrueNorth);
        }

        ImGui.Spacing();

        if (ImGui.CollapsingHeader("UI", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 3);
            if (ImGui.InputInt("Font size (between 12 and 128)", ref Plugin.Configuration.FontSize))
            {
                Plugin.Configuration.FontSize = Math.Clamp(Plugin.Configuration.FontSize, 12, 128);
                changed = true;

                Plugin.UpdateFontHandle();
            }

            if (ImGui.ColorEdit4("Color", ref Plugin.Configuration.Colour, ImGuiColorEditFlags.NoInputs))
            {
                Plugin.Configuration.Colour.W = 1.0f;
                changed = true;
            }

            if (ImGui.ColorEdit4("Border Color", ref Plugin.Configuration.ColourBorder, ImGuiColorEditFlags.NoInputs))
            {
                Plugin.Configuration.ColourBorder.W = 1.0f;
                changed = true;
            }
        }

        if (changed)
            Plugin.Configuration.Save();
    }
}
