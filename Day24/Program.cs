using System.Collections;
using Day24;

List<Wire> initialWires = new();
List<ILogicGate> gatesToUpdate = new();
List<ILogicGate> gatesUpdated = new();

foreach (var line in File.ReadAllLines("Input/InputSolved.txt"))
{
    if (string.IsNullOrEmpty(line))
        continue;
    
    if (line.Contains(':'))
    {
        initialWires.Add(new Wire(line.Split(':')[0], int.Parse(line.Split(':')[1]) == 1));
    }
    else
    {
        gatesToUpdate.Add(ILogicGate.Create(line));
    }
}

int updated = 0;

do
{
    updated = 0;
    foreach (var gate in gatesToUpdate.ToArray())
    {
        if (gate.Update())
        {
            gatesToUpdate.Remove(gate);
            gatesUpdated.Add(gate);
            updated++;
        }
    }
    Console.WriteLine("Updated: " + updated);
}
while (updated > 0);

List<Wire> zWires = Wire.Wires.Values.Where(w => w.Name.StartsWith("z")).OrderByDescending(w => w.Name).ToList();
List<byte> binary = zWires.Select(zWire => zWire.On.Value ? (byte)1 : (byte)0).ToList();
long zValue = BinaryToInteger(binary);
Console.WriteLine("Part 1: " + zValue);


Console.WriteLine("Expected output");
List<Wire> xWires = Wire.Wires.Values.Where(w => w.Name.StartsWith("x")).OrderByDescending(w => w.Name).ToList();
List<Wire> yWires = Wire.Wires.Values.Where(w => w.Name.StartsWith("y")).OrderByDescending(w => w.Name).ToList();
long xValue = BinaryToInteger(xWires.Select(wire => wire.On.Value ? (byte)1 : (byte)0).ToList());
long yValue = BinaryToInteger(yWires.Select(wire => wire.On.Value ? (byte)1 : (byte)0).ToList());
Console.WriteLine("Expected output: " + xValue + " + " + yValue + " = " + (xValue + yValue));
Console.WriteLine(string.Join("", IntegerToBinary(xValue + yValue)));
Console.WriteLine(string.Join("", IntegerToBinary(zValue)));

var expectedBinaryArray = IntegerToBinary(xValue + yValue);
var actualBinaryArray = IntegerToBinary(zValue);

for (int i = expectedBinaryArray.Count -1; i >= 0; i--)
{
    int wireId = expectedBinaryArray.Count - 1 - i;
    if (expectedBinaryArray[i] != actualBinaryArray[i])
    {
        Console.WriteLine("Error: Binary does not match at point " + wireId);
        Console.WriteLine("Make sure output for z wire " + wireId + " is an XOR gate and that one if the inputs for this gate is another XOR gate with x and y wires with id " + wireId);
        Console.WriteLine("Swap in file then run again");
        return;
    }
}
Console.WriteLine("All is correct");
List<string> swapedLines = new();
swapedLines.Add("z10");
swapedLines.Add("vcf");
swapedLines.Add("z17");
swapedLines.Add("fhg");
swapedLines.Add("fsq");
swapedLines.Add("dvb");
swapedLines.Add("z39");
swapedLines.Add("tnc");
swapedLines = swapedLines.OrderBy(x => x).ToList();
Console.WriteLine("Part 2: " + string.Join(",", swapedLines));
return;







List<Wire> expectedInputs = new();
List<Wire> zWiresWithUnexpectedInputs = new();

for (int z = 0; z <= 45; z++)
{
    try
    {
        expectedInputs.Add(Wire.Wires["x" + z.ToString("00")]);
        expectedInputs.Add(Wire.Wires["y" + z.ToString("00")]);
    }
    catch{}

    var zName = "z" + z.ToString("00");
    var outWiresToSolves = new Queue<Wire>();
    outWiresToSolves.Enqueue(Wire.Wires[zName]);
    HashSet<Wire> foundInputWires = new();
    while (outWiresToSolves.Count > 0)
    {
        var wire = outWiresToSolves.Dequeue();
        var gate = gatesUpdated.First(g => g.Output == wire);
        
        if (initialWires.Contains(gate.Inputs[0]))
        {
            foundInputWires.Add(gate.Inputs[0]);
        }
        else
        {
            outWiresToSolves.Enqueue(gate.Inputs[0]);
        }
        
        if (initialWires.Contains(gate.Inputs[1]))
        {
            foundInputWires.Add(gate.Inputs[1]);
        }
        else
        {
            outWiresToSolves.Enqueue(gate.Inputs[1]);
        }
    }
    
    
    //Check that found input wires match expected input wires
    if (foundInputWires.Count != expectedInputs.Count)
    {
        Console.WriteLine("Error: Found input wires count does not match expected input wires count");
        zWiresWithUnexpectedInputs.Add(Wire.Wires[zName]);
    }
    else if (foundInputWires.Any(w => !expectedInputs.Contains(w)))
    {
        Console.WriteLine("Error: Found input wires does not match expected input wires");
        zWiresWithUnexpectedInputs.Add(Wire.Wires[zName]);
    }
    else
    {
        Console.WriteLine("zName inputs seem to be correct: " + zName);
    }

    //Console.Read();
}

//Find dead ends
foreach (var wire in Wire.Wires)
{
    if (wire.Key.StartsWith('z'))
        continue;
    
    bool used = gatesUpdated.Any(g => g.Inputs.Contains(wire.Value));
    
    if (used)
        continue;
    
    Console.WriteLine("Dead end: " + wire.Key);
    
}


long BinaryToInteger(List<byte> binary)
{
    long result = 0;
    foreach (var digit in binary)
    {
        result = 2 * result + digit;
    }
    return result;
}

List<byte> IntegerToBinary(long integer)
{
    List<byte> binary = new();
    while (integer > 0)
    {
        binary.Add((byte)(integer % 2));
        integer /= 2;
    }
    binary.Reverse();
    return binary;
}