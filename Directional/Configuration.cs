using System;
using System.Numerics;
using Dalamud.Configuration;

namespace Directional;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public bool DrawCardinals { get; set; }
    public bool DrawInterCardinals { get; set; }
    public bool AlwaysDrawDirections { get; set; }
    public bool TrueNorth { get; set; }
    public bool Enabled { get; set; } = true;
    public int FontSize { get; set; } = 12;
    public Vector4 Colour { get; set; } = Vector4.One;
    public int Version { get; set; } = 0;
    
    public void Save()
    {
        Directional.PluginInterface.SavePluginConfig(this);
    }
}
