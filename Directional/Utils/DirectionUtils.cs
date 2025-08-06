using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Bindings.ImGui;

namespace Directional.Utils;

// Credit to Chase Dallmann for the original label code
public static class DirectionUtils
{
    private const string N = "N";
    private const string S = "S";
    private const string E = "E";
    private const string W = "W";
    private const string NE = "NE";
    private const string NW = "NW";
    private const string SE = "SE";
    private const string SW = "SW";
    private static readonly Vector3 TrueNorth = new(0, 0, -1);
    private static readonly Vector3 TrueSouth = new(0, 0, 1);
    private static readonly Vector3 TrueEast = new(1, 0, 0);
    private static readonly Vector3 TrueWest = new(-1, 0, 0);

    public static void DrawDirectionLabels(
        IGameObject target, bool drawCardinals, bool drawIntercardinals, Vector4 colour, Vector4 colourBorder)
    {
        var position = target.Position;
        var radius = target.HitboxRadius;

        var forward = GetForwardDirection(target.Rotation);
        var right = GetRightDirection(target.Rotation);

        if (drawCardinals)
        {
            DrawLabel(N, position + (forward * radius), colour, colourBorder);
            DrawLabel(S, position - (forward * radius), colour, colourBorder);
            DrawLabel(E, position - (right * radius), colour, colourBorder);
            DrawLabel(W, position + (right * radius), colour, colourBorder);
        }

        if (drawIntercardinals)
        {
            var diagonalFactor = radius * MathF.Sqrt(2) / 2;
            DrawLabel(NE, position + (forward * diagonalFactor) - (right * diagonalFactor), colour, colourBorder);
            DrawLabel(SE, position - (forward * diagonalFactor) - (right * diagonalFactor), colour, colourBorder);
            DrawLabel(SW, position - (forward * diagonalFactor) + (right * diagonalFactor), colour, colourBorder);
            DrawLabel(NW, position + (forward * diagonalFactor) + (right * diagonalFactor), colour, colourBorder);
        }
    }

    public static void DrawDirectionLabelsRelativeToTrueNorth(
        IGameObject target, bool drawCardinals, bool drawIntercardinals, Vector4 colour, Vector4 colourBorder)
    {
        var position = target.Position;
        var radius = target.HitboxRadius;

        if (drawCardinals)
        {
            DrawLabel(N, position + (TrueNorth * radius), colour, colourBorder);
            DrawLabel(S, position + (TrueSouth * radius), colour, colourBorder);
            DrawLabel(E, position + (TrueEast * radius), colour, colourBorder);
            DrawLabel(W, position + (TrueWest * radius), colour, colourBorder);
        }

        if (drawIntercardinals)
        {
            var diagonalFactor = radius * MathF.Sqrt(2) / 2;
            DrawLabel(NE, position + ((TrueNorth + TrueEast) * diagonalFactor), colour, colourBorder);
            DrawLabel(SE, position + ((TrueSouth + TrueEast) * diagonalFactor), colour, colourBorder);
            DrawLabel(SW, position + ((TrueSouth + TrueWest) * diagonalFactor), colour, colourBorder);
            DrawLabel(NW, position + ((TrueNorth + TrueWest) * diagonalFactor), colour, colourBorder);
        }
    }

    private static Vector3 GetForwardDirection(float rotation)
    {
        // Calculate the forward direction based on the rotation
        var x = (float)Math.Sin(rotation);
        var z = (float)Math.Cos(rotation); // Cheating to get the forward direction
        return new Vector3(x, 0, z);
    }


    private static Vector3 GetRightDirection(float rotation)
    {
        const float r = MathF.PI / 2;
        // Calculate the right direction vector components
        var x = (float)Math.Sin(rotation + r);
        var z = (float)Math.Cos(rotation + r);
        return new Vector3(x, 0, z);
    }

    private static void DrawLabel(string text, Vector3 worldPosition, Vector4 colour, Vector4 colourBorder)
    {
        if (!Plugin.GameGui.WorldToScreen(worldPosition, out var screenPosition))
            return;

        var drawList = ImGui.GetBackgroundDrawList();
        var textSize = ImGui.CalcTextSize(text);
        var pos = screenPosition - (textSize / 2);
        var borderSize = (int)MathF.Ceiling(textSize.Y / 40);

        drawList.AddText(pos + new Vector2(-borderSize, -borderSize), ImGui.GetColorU32(colourBorder), text);
        drawList.AddText(pos + new Vector2(borderSize, -borderSize), ImGui.GetColorU32(colourBorder), text);
        drawList.AddText(pos + new Vector2(-borderSize, borderSize), ImGui.GetColorU32(colourBorder), text);
        drawList.AddText(pos + new Vector2(borderSize, borderSize), ImGui.GetColorU32(colourBorder), text);
        drawList.AddText(pos, ImGui.GetColorU32(colour), text);
    }
}
