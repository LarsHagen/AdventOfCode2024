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

    var codeCoordinates = keypadDoor.GetButtonCoordinates(code.ToArray());
    List<string> allButtonSequenceDoor = keypadDoor.GetAllButtonPressSequence(codeCoordinates);
    
    List<string> allButtonPressSequenceRobot1 = new();
    foreach (var buttonSequence in allButtonSequenceDoor)
    {
        codeCoordinates = keypadRobot1.GetButtonCoordinates(buttonSequence.ToArray());
        allButtonPressSequenceRobot1.AddRange(keypadRobot1.GetAllButtonPressSequence(codeCoordinates));
    }

    Console.WriteLine("");
    List<string> allButtonPressSequenceRobot2 = new();

    for (var index = 0; index < allButtonPressSequenceRobot1.Count; index++)
    {
        var buttonSequence = allButtonPressSequenceRobot1[index];
        Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);
        Console.WriteLine("Working: " + index + "/" + allButtonPressSequenceRobot1.Count);
        codeCoordinates = keypadRobot2.GetButtonCoordinates(buttonSequence.ToArray());
        allButtonPressSequenceRobot2.AddRange(keypadRobot2.GetAllButtonPressSequence(codeCoordinates));
    }

    Console.WriteLine("Done");
    var shortestSequence = allButtonPressSequenceRobot2.OrderBy(x => x.Length).First();
    return shortestSequence;
}