using Day17;

var computer = ComputerFromInput(File.ReadAllLines("Input/Input.txt"));
computer.Run();

string part1 = "Part 1: " + string.Join(',', computer.GetOutput());
Console.WriteLine(part1);
Console.WriteLine();
Console.WriteLine("Press any key to start part 2");
Console.Read();

computer = ComputerFromInput(File.ReadAllLines("Input/Input.txt"));

int digitToGuess = 0;
Dictionary<int, List<string>> base8Matches = new();
var desiredOutput = string.Join(',', computer.Instructions.Select(i => i.Base8));
while (true)
{
    if (digitToGuess == 16)
        break;
    
    if (!base8Matches.ContainsKey(digitToGuess))
        base8Matches[digitToGuess] = new();

    List<string> base8BaseInputs = new();
    if (digitToGuess > 0)
    {
        base8BaseInputs = base8Matches[digitToGuess - 1];
    }
    else
    {
        base8BaseInputs.Add("");
    }

    if (base8BaseInputs.Count == 0)
    {
        Console.WriteLine("Ran out!");
        break;
    }

    foreach (var baseInput in base8BaseInputs)
    {
        for (int i = 0; i < 8; i++)
        {
            var number = baseInput + i;
            computer.registerA = Base8Number.CreateFromBase8(number);
            computer.registerB = new();
            computer.registerC = new();
            computer.instructionPointer = 0;
            computer.output.Clear();

            var startAsBase8 = computer.registerA.Base8.ToString();
            var startAsBase10 = computer.registerA.Base10.ToString();
            
            Console.Clear();
            computer.Run();
            Console.WriteLine("A input: " + startAsBase10 + ". Base8: " + startAsBase8);
            Console.WriteLine("Desired output:  " + desiredOutput);
            computer.PrintOutput();


            var output = computer.output;
            var partInstructions = computer.Instructions.GetRange(computer.Instructions.Count - output.Count, output.Count);
            bool hit = true;

            for (int j = 0; j < output.Count; j++)
            {
                if (output[j].Base10 != partInstructions[j].Base10)
                {
                    hit = false;
                    break;
                }
            }

            Console.WriteLine("Hit: " + hit);

            if (hit)
            {
                base8Matches[digitToGuess].Add(number);
            }

            Thread.Sleep(5);
        }
    }

    digitToGuess++;
}

Console.WriteLine("Done. Found " + base8Matches[digitToGuess - 1].Count + " matches");

Base8Number fastest = Base8Number.CreateFromBase10(long.MaxValue);

foreach (var match in base8Matches[digitToGuess - 1])
{
    var _ = Base8Number.CreateFromBase8(match);
    if (_.Base10 < fastest.Base10)
    {
        fastest = _;
    }
}

Console.WriteLine("Fastest match: " + fastest.Base8);
Console.WriteLine();
Console.WriteLine(part1);
Console.WriteLine("Part 2: " + fastest.Base10);

MagicUnfoldingComputer ComputerFromInput(string[] input)
{
    MagicUnfoldingComputer computer = new();
    computer.registerA = Base8Number.CreateFromBase10(int.Parse(input[0].Replace("Register A: ", "")));
    computer.registerB = Base8Number.CreateFromBase10(int.Parse(input[1].Replace("Register B: ", "")));
    computer.registerC = Base8Number.CreateFromBase10(int.Parse(input[2].Replace("Register C: ", "")));

    var instructions = input[4].Replace("Program: ", "").Split(',').Select(int.Parse);
    computer.Instructions = instructions.Select(i => Base8Number.CreateFromBase10(i)).ToList();
    
    return computer;
}

class MagicUnfoldingComputer
{
    public List<Base8Number> Instructions;
    
    public Base8Number registerA = new();
    public Base8Number registerB = new();
    public Base8Number registerC = new();

    public int instructionPointer = 0;
    public List<Base8Number> output = new();

    public void PrintRegisters()
    {
        Console.WriteLine($"A: {registerA.Base10}, B: {registerB.Base10}, C: {registerC.Base10}");
    }
    
    public string GetOutput()
    {
        return string.Join(",", output.Select(o => o.Base8));
    }
    
    public void PrintOutput()
    {
        Console.WriteLine("Computed output: " + GetOutput());
    }
    
    public void Run()
    {
        int cycles = 0;
        while (instructionPointer < Instructions.Count)
        {
            var opcode = Instructions[instructionPointer];
            var operand = Instructions[instructionPointer + 1];

            GetInstruction(opcode).Invoke(operand);
            instructionPointer += 2;
            cycles++;
            
            if (cycles > 1000)
                break;
        }
        
        Console.WriteLine("Cycles: " + cycles);
    }
    
    Action<Base8Number> GetInstruction(Base8Number opcode)
    {
        return opcode.Base10 switch
        {
            0 => Adv,
            1 => Bxl,
            2 => Bst,
            3 => Jnz,
            4 => Bxc,
            5 => Out,
            6 => Bdv,
            7 => Cdv,
            _ => throw new Exception("Invalid opcode " + opcode.Base10)
        };
    }

    void Adv(Base8Number operand)
    {
        long numerator = registerA.Base10;
        long denominator = (long)Math.Pow(2, ComboOperandTranslate(operand).Base10);
        registerA = Base8Number.CreateFromBase10(numerator / denominator);
    }

    void Bxl(Base8Number operand)
    {
        //Calculate bitwise XOR of register B and operand
        var result = registerB.Base10 ^ operand.Base10;
        registerB = Base8Number.CreateFromBase10(result);
    }

    void Bst(Base8Number operand)
    {
        var translatedOperand = ComboOperandTranslate(operand);
        registerB = Base8Number.CreateFromBase10(translatedOperand.Base10 % 8);
    }

    void Jnz(Base8Number operand) //Goto
    {
        if (registerA.Base10 != 0)
        {
            instructionPointer = (int)operand.Base10 - 2;
        }
    }

    void Bxc(Base8Number operand)
    {
        //Calculate bitwise XOR of register B and register X, then store in register B
        registerB = Base8Number.CreateFromBase10(registerB.Base10 ^ registerC.Base10);
    }

    void Out(Base8Number operand)
    {
        var translatedOperand = ComboOperandTranslate(operand);
        output.Add(Base8Number.CreateFromBase10(translatedOperand.Base10 % 8));
    }

    void Bdv(Base8Number operand)
    {
        long numerator = registerA.Base10;
        long denominator = (long)Math.Pow(2, ComboOperandTranslate(operand).Base10);
        registerB = Base8Number.CreateFromBase10(numerator / denominator);
    }

    void Cdv(Base8Number operand)
    {
        long numerator = registerA.Base10;
        long denominator = (long)Math.Pow(2, ComboOperandTranslate(operand).Base10);
        registerC = Base8Number.CreateFromBase10(numerator / denominator);
    }

    Base8Number ComboOperandTranslate(Base8Number operand)
    {
        var base8 = operand.Base8;
        
        if (base8 is "0" or "1" or "2" or "3")
            return operand;

        if (base8 == "4")
            return registerA;

        if (base8 == "5")
            return registerB;

        if (base8 == "6")
            return registerC;

        throw new Exception("Invalid operand");
    }
}

