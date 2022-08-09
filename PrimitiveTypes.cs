namespace cs2hlsl;


public record struct Vec2<T>(T x, T y) where T : unmanaged { public static implicit operator Vec2<T>(T x) => new Vec2<T>(x, x); }
public record struct Vec3<T>(T x, T y, T z) where T : unmanaged { public static implicit operator Vec3<T>(T x) => new Vec3<T>(x, x, x); }
public record struct Vec4<T>(T x, T y, T z, T w) where T : unmanaged { public static implicit operator Vec4<T>(T x) => new Vec4<T>(x, x, x, x); }
