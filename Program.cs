using cs2hlsl;

var shader = new MyShader();
var input = new Dictionary<Semantic, object>(){ [Semantic.SV_Position] = new Vec4<float>(10, 20, 0, 1)};
Console.WriteLine(string.Join(", ", input.Select(kv => $"{kv.Key.Value} = {kv.Value}")));
var output = shader.Fragment(input);
Console.WriteLine(string.Join(", ", output.Select(kv => $"{kv.Key.Value} = {kv.Value}")));


class MyShader : Pass<Properties>
{
    public override string Name => throw new NotImplementedException();
    public override Dictionary<string, string> Tags => throw new NotImplementedException();
    public override Dictionary<string, string> Commands => throw new NotImplementedException();


    [EntryPoint(Stage.Fragment)]
    [return: Semantic(Semantic.SV_Target0)] Vec4<float> Frag([Semantic(Semantic.SV_Position)] Vec4<float> position)
    {
        // this.PerMaterial._MainTex.Sample
        return new (position.x + 1, position.y + 1, position.z + 1, 1);
    }
}
