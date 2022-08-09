namespace cs2hlsl;


[AttributeUsage(AttributeTargets.Field | AttributeTargets.ReturnValue | AttributeTargets.Parameter, AllowMultiple = false)]
public class SemanticAttribute : Attribute
{
    public Semantic Value { get; }
    public SemanticAttribute(string value) => Value = new(value);
}

public record Semantic(string Value)
{
    // https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-semantics#system-value-semantics
    public const string SV_VertexID = nameof(SV_VertexID);
    public const string SV_Position = nameof(SV_Position);
    public const string SV_Depth = nameof(SV_Depth);
    public const string SV_Target0 = nameof(SV_Target0);
    public const string SV_Target1 = nameof(SV_Target1);
    public const string SV_Target2 = nameof(SV_Target2);
    public const string SV_Target3 = nameof(SV_Target3);
    public const string SV_Target4 = nameof(SV_Target4);
    public const string SV_Target5 = nameof(SV_Target5);
    public const string SV_Target6 = nameof(SV_Target6);
    public const string SV_Target7 = nameof(SV_Target7);

    public static implicit operator Semantic(string name) => new(name);
}
