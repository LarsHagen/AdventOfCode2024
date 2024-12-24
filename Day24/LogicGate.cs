namespace Day24;

public interface ILogicGate
{
    public Wire Output { get; set; }
    public List<Wire> Inputs { get; set; } 
    
    public bool Update();

    public static ILogicGate Create(string input)
    {
        var components = input.Split(' ');
        ILogicGate gate = components[1] switch
        {
            "AND" => new AndGate(),
            "OR" => new OrGate(),
            "XOR" => new XorGate(),
            _ => throw new Exception("Invalid gate type")
        };
        
        if (Wire.Wires.ContainsKey(components[4]))
            gate.Output = Wire.Wires[components[4]];
        else
            gate.Output = new Wire(components[4]);
        
        gate.Inputs = new();
        
        if (Wire.Wires.ContainsKey(components[0]))
            gate.Inputs.Add(Wire.Wires[components[0]]);
        else
            gate.Inputs.Add(new Wire(components[0]));
        
        if (Wire.Wires.ContainsKey(components[2]))
            gate.Inputs.Add(Wire.Wires[components[2]]);
        else
            gate.Inputs.Add(new Wire(components[2]));
        
        return gate;
    }
        
}

public class AndGate : ILogicGate
{
    public Wire Output { get; set; }
    public List<Wire> Inputs { get; set; }
    
    public bool Update()
    {
        if (Inputs.Any(i => i.On == null))
            return false;
        
        Output.On = Inputs.All(wire => wire.On.Value);
        
        return true;
    }
}

public class OrGate : ILogicGate
{
    public Wire Output { get; set; }
    public List<Wire> Inputs { get; set; }
    
    public bool Update()
    {
        if (Inputs.Any(i => i.On == null))
            return false;
        
        Output.On = Inputs.Any(wire => wire.On.Value);
        
        return true;
    }
}

public class XorGate : ILogicGate
{
    public Wire Output { get; set; }
    public List<Wire> Inputs { get; set; }
    
    public bool Update()
    {
        if (Inputs.Any(i => i.On == null))
            return false;
        
        Output.On = Inputs.Count(wire => wire.On.Value) == 1;
        
        return true;
    }
}