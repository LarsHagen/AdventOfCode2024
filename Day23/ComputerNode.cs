namespace Day23;

public class ComputerNode
{
    public string Name { get; set; }
    public HashSet<ComputerNode> ConnectedNodes { get; set; }
}