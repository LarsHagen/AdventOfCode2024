List<IOperation> operationsPart1 = new();
operationsPart1.Add(new Add());
operationsPart1.Add(new Multiply());

List<IOperation> operationsPart2 = new();
operationsPart2.Add(new Add());
operationsPart2.Add(new Multiply());
operationsPart2.Add(new Concatenation());

long sumOfSolvedEquationsPart1 = 0;
long sumOfSolvedEquationsPart2 = 0;

foreach (var equation in File.ReadAllLines("Input/Input.txt"))
{
    var split = equation.Split(":");
    var result = long.Parse(split[0]);
    var inputs = split[1].Trim().Split(" ").Select(long.Parse).ToList();

    if (RecursiveBruteForce(result, inputs, operationsPart1))
    {
        sumOfSolvedEquationsPart1 += result;
    }
    
    if (RecursiveBruteForce(result, inputs, operationsPart2))
    {
        sumOfSolvedEquationsPart2 += result;
    }
}

Console.WriteLine("Part 1: " + sumOfSolvedEquationsPart1);
Console.WriteLine("Part 2: " + sumOfSolvedEquationsPart2);

bool RecursiveBruteForce(long desiredResult, List<long> inputs, List<IOperation> operations)
{
    if (inputs.Count == 1)
        return inputs[0] == desiredResult;
    
    var a = inputs[0];
    var b = inputs[1];
    
    foreach (var operation in operations)
    {    
        var calculated = operation.Calculate(a, b);
        var newInputs = new List<long>(inputs.Skip(2));
        newInputs.Insert(0, calculated);
        
        if (RecursiveBruteForce(desiredResult, newInputs, operations))
            return true;
    }

    return false;
}

interface IOperation
{
    long Calculate(long a, long b);
}

class Add : IOperation
{
    public long Calculate(long a, long b)
    {
        return a + b;
    }
}

class Multiply : IOperation
{
    public long Calculate(long a, long b)
    {
        return a * b;
    }
}

class Concatenation : IOperation
{
    public long Calculate(long a, long b)
    {
        return long.Parse(a.ToString() + b.ToString());
    }
}