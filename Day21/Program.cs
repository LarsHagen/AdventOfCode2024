using Day21;

List<string> doorCodes = File.ReadAllLines("Input/Input.txt").ToList();

int result = 0;

foreach (var doorCode in doorCodes)
{
    var userInput = GetUserInput(doorCode);
    Console.WriteLine($"{doorCode}: {string.Join("", userInput)}");
    Console.WriteLine(userInput.Length + " * " + int.Parse(doorCode[..^1]));
    result += userInput.Length * int.Parse(doorCode[..^1]);
}

Console.WriteLine("Part 1: " + result);


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

    //Console.WriteLine("Done");
    var shortestSequence = allButtonPressSequenceRobot2.OrderBy(x => x.Length).First();
    return shortestSequence;
}