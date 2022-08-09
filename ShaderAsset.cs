using System.Reflection;

namespace cs2hlsl;


public abstract class ShaderAsset<TProperties> where TProperties : Properties
{
    // https://docs.unity3d.com/ja/2021.3/Manual/SL-Shader.html
    public abstract string? Name { get; }
    public abstract Properties Properties { get; }
    public abstract SubShader<TProperties>[] SubShaders { get; }
    public abstract string? FallbackShaderName { get; }
    public abstract string? CustomEditor { get; }
}

public abstract class Properties
{
    // https://docs.unity3d.com/ja/2021.3/Manual/SL-Properties.html
    // example
    // [PerRendererData] [Label(MainTex)] public Texture2D _MainTex = Texture2D.white;
    // [HDR] [Label(TintColor)] public Color _Color = Color.black;
    // [Range(0, 1)] [Label(TintColor)] public float _Power = 1;
}

public abstract class SubShader<TProperties> where TProperties : Properties
{
    // https://docs.unity3d.com/ja/2021.3/Manual/SL-SubShader.html
    public abstract Dictionary<string, string> Tags { get; }
    public abstract int? LOD { get; }
    public abstract Pass<TProperties>[] Passes { get; }
}

public abstract class Pass
{
    public enum Stage
    {
        Geometry, Vertex, Fragment, Compute
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class EntryPointAttribute : Attribute
    {
        public Stage Stage { get; }
        public EntryPointAttribute(Stage stage) => Stage = stage;
    }
}

public abstract class Pass<TProperties> : Pass where TProperties : Properties
{
    // https://docs.unity3d.com/ja/2021.3/Manual/SL-Pass.html
    public abstract string? Name { get; }
    public abstract Dictionary<string, string> Tags { get; }
    public abstract Dictionary<string, string> Commands { get; } // https://docs.unity3d.com/ja/2021.3/Manual/shader-shaderlab-commands.html


    #region HLSLPROGRAM

    public TProperties PerMaterial { get; } = Activator.CreateInstance<TProperties>();

    const BindingFlags EntryPointBindingFlags = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic;
    public MethodInfo VertexEP => GetType().GetMethods(EntryPointBindingFlags).SingleOrDefault(m => m.GetCustomAttribute<EntryPointAttribute>()?.Stage == Stage.Vertex) ?? throw new Exception($"not found method VS");
    public MethodInfo FragmentEP => GetType().GetMethods(EntryPointBindingFlags).SingleOrDefault(m => m.GetCustomAttribute<EntryPointAttribute>()?.Stage == Stage.Fragment) ?? throw new Exception($"not found method VS");
    

    public Dictionary<Semantic, object> Vertex(IReadOnlyDictionary<Semantic, object> input) => Call(VertexEP, input);
    public Dictionary<Semantic, object> Fragment(IReadOnlyDictionary<Semantic, object> input) => Call(FragmentEP, input);

    Dictionary<Semantic, object> Call(MethodInfo function, IReadOnlyDictionary<Semantic, object> input)
    {
        // prepare input semantics
        var arguments = new object[function.GetParameters().Length];
        foreach (var parameter in function.GetParameters())
        {
            if(parameter.IsOut) continue;

            if(parameter.GetCustomAttribute<SemanticAttribute>() is not {} parameterSemanticAttribute) continue;
            arguments[parameter.Position] = input[parameterSemanticAttribute.Value];
        }
        
        var returnValue = function.Invoke(this, arguments);

        // colect output semantics
        var output = new Dictionary<Semantic, object>();
        if(function.ReturnParameter.GetCustomAttribute<SemanticAttribute>() is {} returnSemanticAttribute)
        {
            // TODO: struct return (nest)
            output[returnSemanticAttribute.Value] = returnValue as ValueType ?? throw new Exception($"function returned null");
        }
        foreach (var parameter in function.GetParameters())
        {
            if(!parameter.IsOut) continue;

            // TODO: struct return (nest)
            if(parameter.GetCustomAttribute<SemanticAttribute>() is not {} parameterSemanticAttribute) continue;
            output[parameterSemanticAttribute.Value] = arguments[parameter.Position] as ValueType ?? throw new Exception($"function not set out parameter {parameter.Name}");
        }
        return output;
    }

    #endregion // HLSLPROGRAM
}
