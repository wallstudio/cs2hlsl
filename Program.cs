using cs2hlsl;

var shader = new Shader();
var input = new Dictionary<Semantic, object>(){ [Semantic.SV_Position] = new Vec4<float>(10, 20, 0, 1)};
Console.WriteLine(string.Join(", ", input.Select(kv => $"{kv.Key.Value} = {kv.Value}")));
var output = shader.SubShaders[0].Passes[0].Fragment(input);
Console.WriteLine(string.Join(", ", output.Select(kv => $"{kv.Key.Value} = {kv.Value}")));


class Shader : ShaderAsset<Shader.MyProperties>
{
    public override string? Name => nameof(Shader);

    public override Properties Properties => new MyProperties();
    public class MyProperties : Properties
    {
        public Texture2D _MainTex = Texture2D.White;
    }

    public override SubShader<MyProperties>[] SubShaders => new SubShader<MyProperties>[]{ new MySubShader() };
    class MySubShader : SubShader<MyProperties>
    {
        public override Dictionary<string, string> Tags => throw new NotImplementedException();
        public override int? LOD => throw new NotImplementedException();

        public override Pass<MyProperties>[] Passes => new Pass<MyProperties>[] { new MyPass() };
        class MyPass : Pass<MyProperties>
        {
            public override string? Name => null;
            public override Dictionary<string, string> Tags => throw new NotImplementedException();
            public override Dictionary<string, string> Commands => throw new NotImplementedException();

            Vec2<float> _Offset;

            [EntryPoint(Stage.Fragment)]
            [return: Semantic(Semantic.SV_Target0)] Vec4<float> frag([Semantic(Semantic.SV_Position)] Vec4<float> position)
            {
                return PerMaterial._MainTex.Sample(new Vec2<float>(position.x + _Offset.x, position.y + _Offset.x));
            }
        }
    }

    public override string? FallbackShaderName => null;
    public override string? CustomEditor => null;

}

