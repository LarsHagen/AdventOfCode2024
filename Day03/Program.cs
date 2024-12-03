
using System.Text.RegularExpressions;

var memoryInput = File.ReadAllText("Input/Input.txt");

//Use regex to find all "mul"
var mulInstructions = Regex.Matches(memoryInput, "mul\\(\\d{1,3},\\d{1,3}\\)");
var doInstructions = Regex.Matches(memoryInput, "do\\(\\)");
var dontInstructions = Regex.Matches(memoryInput, "don't\\(\\)");

var mulSum = 0;

foreach (Match mulInstruction in mulInstructions)
{
    var values = mulInstruction.Value.Replace("mul(","").Replace(")", "").Split(",").Select(int.Parse).ToList();
    mulSum += values[0] * values[1];
}

Console.WriteLine("Part 1: " + mulSum);

mulSum = 0;

//Add mulInstructions and doInstructions and dontInstructions to a list
var allInstructions = new List<Match>();
allInstructions.AddRange(mulInstructions);
allInstructions.AddRange(doInstructions);
allInstructions.AddRange(dontInstructions);

allInstructions = allInstructions.OrderBy(x => x.Index).ToList();
bool mulEnabled = true;
foreach (var instruction in allInstructions)
{
    if (instruction.Value.Contains("do("))
    {
        mulEnabled = true;
        continue;
    }
    
    if (instruction.Value.Contains("don't("))
    {
        mulEnabled = false;
        continue;
    }
    
    if (mulEnabled)
    {
        var values = instruction.Value.Replace("mul(","").Replace(")", "").Split(",").Select(int.Parse).ToList();
        mulSum += values[0] * values[1];
    }
}

Console.WriteLine("Part 2: " + mulSum);