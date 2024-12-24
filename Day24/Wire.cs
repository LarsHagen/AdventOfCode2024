namespace Day24;

public class Wire
{
    public static Dictionary<string, Wire> Wires = new();
    
    public string Name { get; set; }
    public bool? On { get; set; }
    
    public Wire(string name, bool? on = null)
    {
        Name = name;
        On = on;
        
        Wires.Add(Name, this);
    }
}