using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Interface.Utility;

namespace Directional.Utils;

// Credits to 3pp that I can't mention but starts with a K
[StructLayout(LayoutKind.Explicit, Size = 0x1FC)]
public partial struct WorldMatrix {
    [FieldOffset(0x1B4)] public Matrix4x4 Matrix;

    [FieldOffset(0x1F4)] public float Width;
    [FieldOffset(0x1F8)] public float Height;

    public bool WorldToScreen(Vector3 v, out Vector2 pos2d) {
        var result = WorldToScreenDepth(v, out var pos);
        pos2d = new Vector2(pos.X, pos.Y);
        return result;
    }

    public bool WorldToScreenDepth(Vector3 v, out Vector3 pos2d) {
        var m = Matrix;

        var windowPos = ImGuiHelpers.MainViewport.Pos;

        var x = (m.M11 * v.X) + (m.M21 * v.Y) + (m.M31 * v.Z) + m.M41;
        var y = (m.M12 * v.X) + (m.M22 * v.Y) + (m.M32 * v.Z) + m.M42;
        var w = (m.M14 * v.X) + (m.M24 * v.Y) + (m.M34 * v.Z) + m.M44;

        var camX = (Width / 2f);
        var camY = (Height / 2f);

        pos2d = new Vector3(
            camX + (camX * x / w) + windowPos.X,
            camY - (camY * y / w) + windowPos.Y,
            w
        );

        return w > 0.001f;
    }
}

internal class Methods
{
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal unsafe delegate WorldMatrix* GetMatrixDelegate();
    internal static GetMatrixDelegate? GetMatrix;
    
    private static TDelegate Retrieve<TDelegate>(string sig)
        => Marshal.GetDelegateForFunctionPointer<TDelegate>(Directional.SigScanner.ScanText(sig));
    
    internal static void Init() {
        GetMatrix = Retrieve<GetMatrixDelegate>("E8 ?? ?? ?? ?? 48 8D 4C 24 ?? 48 89 4c 24 ?? 4C 8D 4D ?? 4C 8D 44 24 ??");
    }
}
