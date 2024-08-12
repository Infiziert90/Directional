using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;

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

    public static void DrawDirectionLabels(IGameObject target, bool drawCardinals, bool drawIntercardinals, Vector4 colour)
    {
        var position = target.Position;
        var radius = target.HitboxRadius;

        var forward = GetForwardDirection(target.Rotation);
        var right = GetRightDirection(target.Rotation);

        if (drawCardinals)
        {
            DrawLabel(N, position + (forward * radius), colour);
            DrawLabel(S, position - (forward * radius), colour);
            DrawLabel(E, position - (right * radius), colour);
            DrawLabel(W, position + (right * radius), colour);
        }

        if (drawIntercardinals)
        {
            var diagonalFactor = radius * MathF.Sqrt(2) / 2;
            DrawLabel(NE, position + (forward * diagonalFactor) - (right * diagonalFactor), colour);
            DrawLabel(SE, position - (forward * diagonalFactor) - (right * diagonalFactor), colour);
            DrawLabel(SW, position - (forward * diagonalFactor) + (right * diagonalFactor), colour);
            DrawLabel(NW, position + (forward * diagonalFactor) + (right * diagonalFactor), colour);
        }
    }

    public static void DrawDirectionLabelsRelativeToTrueNorth(
        IGameObject target, bool drawCardinals, bool drawIntercardinals, Vector4 colour)
    {
        var position = target.Position;
        var radius = target.HitboxRadius;

        if (drawCardinals)
        {
            DrawLabel(N, position + (TrueNorth * radius), colour);
            DrawLabel(S, position + (TrueSouth * radius), colour);
            DrawLabel(E, position + (TrueEast * radius), colour);
            DrawLabel(W, position + (TrueWest * radius), colour);
        }

        if (drawIntercardinals)
        {
            var diagonalFactor = radius * MathF.Sqrt(2) / 2;
            DrawLabel(NE, position + ((TrueNorth + TrueEast) * diagonalFactor), colour);
            DrawLabel(SE, position + ((TrueSouth + TrueEast) * diagonalFactor), colour);
            DrawLabel(SW, position + ((TrueSouth + TrueWest) * diagonalFactor), colour);
            DrawLabel(NW, position + ((TrueNorth + TrueWest) * diagonalFactor), colour);
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

    private static void DrawLabel(string text, Vector3 worldPosition, Vector4 colour)
    {
        var screenPosition = WorldToScreen(worldPosition);
        var drawList = ImGui.GetForegroundDrawList();

        var x = (float)Math.Round(screenPosition.X, 2);
        var y = (float)Math.Round(screenPosition.Y, 2);
        drawList.AddText(new Vector2(x, y), ImGui.GetColorU32(colour), text);
    }

    private static Vector2 WorldToScreen(Vector3 worldPos)
    {
        Directional.GameGui.WorldToScreen(worldPos, out var screenPos);
        return screenPos;
    }
}
