using Day21;

List<string> doorCodes = File.ReadAllLines("Input/Input.txt").ToList();
CachedCalculatedMoveCost wow = new CachedCalculatedMoveCost();
int result = 0;

foreach (var doorCode in doorCodes)
{
    var userInput = GetUserInput(doorCode);
    Console.WriteLine($"{doorCode}: {string.Join("", userInput)}");
    Console.WriteLine(userInput.Length + " * " + int.Parse(doorCode[..^1]));
    result += userInput.Length * int.Parse(doorCode[..^1]);
}

Console.WriteLine("Part 1: " + result);

long sumPart2 = 0;
foreach (var doorCode in doorCodes)
{
    sumPart2 += GetShortestNewMethod(doorCode) * int.Parse(doorCode[..^1]);
}

Console.WriteLine("Part 2: " + sumPart2);

long GetShortestNewMethod(string code)
{
    var keypadDoor = new Keypad(false);
    List<string> allButtonSequenceDoor = keypadDoor.GetAllButtonPressSequence(code);
    
    List<long> allCostsNew = new();
    
    foreach (var buttonSequence in allButtonSequenceDoor)
    {
        allCostsNew.Add(wow.GetCost(buttonSequence, 3));
    }
    
    return allCostsNew.MinBy(x => x);
}

string GetUserInput(string code)
{
    var keypadDoor = new Keypad(false);
    var keypadRobot1 = new Keypad(true);
    var keypadRobot2 = new Keypad(true);

    List<string> allButtonSequenceDoor = keypadDoor.GetAllButtonPressSequence(code);
    
    List<string> allButtonPressSequenceRobot1 = new();
    foreach (var buttonSequence in allButtonSequenceDoor)
    {
        allButtonPressSequenceRobot1.AddRange(keypadRobot1.GetAllButtonPressSequence(buttonSequence));
    }

    List<string> allButtonPressSequenceRobot2 = new();
    foreach (var buttonSequence in allButtonPressSequenceRobot1)
    {
        allButtonPressSequenceRobot2.AddRange(keypadRobot2.GetAllButtonPressSequence(buttonSequence));
    }


    var shortestSequence = allButtonPressSequenceRobot2.OrderBy(x => x.Length).First();
    return shortestSequence;
}

string OldMethod(string input, int layers)
{
    var keypadRobot = new Keypad(true);
    List<string> allButtonSequenceDoor = keypadRobot.GetAllButtonPressSequence(input);
    for (int i = 1; i < layers; i++)
    {
        List<string> allButtonPressSequenceRobot = new();
        foreach (var buttonSequence in allButtonSequenceDoor)
        {
            allButtonPressSequenceRobot.AddRange(keypadRobot.GetAllButtonPressSequence(buttonSequence));
        }
        allButtonSequenceDoor = allButtonPressSequenceRobot;
    }
    var shortestSequence = allButtonSequenceDoor.OrderBy(x => x.Length).First();
    return shortestSequence;
}