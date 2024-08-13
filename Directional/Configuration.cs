using System;
using System.Numerics;
using Dalamud.Configuration;

namespace Directional;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public bool AlwaysDrawDirections;
    public Vector4 Colour = Vector4.One;
    public Vector4 ColourBorder = new(1.0f, 1.0f, 1.0f, 0.0f);
    public bool CombatOnly = true;
    public bool DrawCardinals;
    public bool DrawInterCardinals;
    public bool Enabled = true;
    public int FontSize = 12;
    public bool TrueNorth;

    public int Version { get; set; } = 0;

    public void Save()
    {
        Directional.PluginInterface.SavePluginConfig(this);
    }
}
