using Day17;

/*var testComputer = new MagicUnfoldingComputer
{
    Instructions = new List<Base8Number> { Base8Number.CreateFromBase8("2"), Base8Number.CreateFromBase8("6") },
    registerC = Base8Number.CreateFromBase10(9)
};

testComputer.Run();
testComputer.PrintRegisters();
testComputer.PrintOutput();
Console.WriteLine();

testComputer = new()
{
    Instructions = new List<Base8Number>
    {
        Base8Number.CreateFromBase8("5"),
        Base8Number.CreateFromBase8("0"),
        Base8Number.CreateFromBase8("5"),
        Base8Number.CreateFromBase8("1"),
        Base8Number.CreateFromBase8("5"),
        Base8Number.CreateFromBase8("4")
    },
    registerA = Base8Number.CreateFromBase10(10)
};

testComputer.Run();
testComputer.PrintRegisters();
testComputer.PrintOutput();
Console.WriteLine();

testComputer = new()
{
    Instructions = new List<Base8Number>
    {
        Base8Number.CreateFromBase8("0"),
        Base8Number.CreateFromBase8("1"),
        Base8Number.CreateFromBase8("5"),
        Base8Number.CreateFromBase8("4"),
        Base8Number.CreateFromBase8("3"),
        Base8Number.CreateFromBase8("0")
    },
    registerA = Base8Number.CreateFromBase10(2024)
};

testComputer.Run();
testComputer.PrintRegisters();
testComputer.PrintOutput();
Console.WriteLine();

testComputer = new()
{
    Instructions = new List<Base8Number>
    {
        Base8Number.CreateFromBase8("1"),
        Base8Number.CreateFromBase8("7")
    },
    registerB = Base8Number.CreateFromBase10(29)
};

testComputer.Run();
testComputer.PrintRegisters();
testComputer.PrintOutput();
Console.WriteLine();

testComputer = new()
{
    Instructions = new List<Base8Number>
    {
        Base8Number.CreateFromBase8("4"),
        Base8Number.CreateFromBase8("0")
    },
    registerB = Base8Number.CreateFromBase10(2024),
    registerC = Base8Number.CreateFromBase10(43690)
};

testComputer.Run();
testComputer.PrintRegisters();
testComputer.PrintOutput();
Console.WriteLine();

testComputer = ComputerFromInput(File.ReadAllLines("Input/Example.txt"));
testComputer.Run();
testComputer.PrintRegisters();
testComputer.PrintOutput();
Console.WriteLine();*/

var computer = ComputerFromInput(File.ReadAllLines("Input/Input.txt"));
computer.Run();
Console.WriteLine("Part 1:");
computer.PrintOutput();

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
    
    public void PrintOutput()
    {
        Console.WriteLine("Output: " + string.Join(",", output.Select(o => o.Base8)));
    }
    
    public void Run(int maxOutputLength = int.MaxValue)
    {
        while (instructionPointer < Instructions.Count)
        {
            if (output.Count >= maxOutputLength)
                break;
            
            var opcode = Instructions[instructionPointer];
            var operand = Instructions[instructionPointer + 1];

            GetInstruction(opcode).Invoke(operand);
            instructionPointer += 2;
        }
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
        int numerator = registerA.Base10;
        int denominator = (int)Math.Pow(2, ComboOperandTranslate(operand).Base10);
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
            instructionPointer = operand.Base10 - 2;
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
        int numerator = registerA.Base10;
        int denominator = (int)Math.Pow(2, ComboOperandTranslate(operand).Base10);
        registerB = Base8Number.CreateFromBase10(numerator / denominator);
    }

    void Cdv(Base8Number operand)
    {
        int numerator = registerA.Base10;
        int denominator = (int)Math.Pow(2, ComboOperandTranslate(operand).Base10);
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