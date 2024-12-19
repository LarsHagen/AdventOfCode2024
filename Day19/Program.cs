List<string> towels = new();
List<string> patterns = new();

var input = File.ReadAllText("Input/Input.txt").Split("\n");
towels = input[0].Split(", ").ToList();
patterns = input.ToList().GetRange(2, input.Length - 2);

List<string> validPatterns = new();
HashSet<string> cachedValidPatternsPart1 = new(); //Value is all possible towel combinations
HashSet<string> cachedInvalidPatternsPart1 = new();

foreach (var pattern in patterns)
{
    if (IsPatternValidPart1(pattern))
    {
        validPatterns.Add(pattern);
    }
}
Console.WriteLine("Part 1: " + validPatterns.Count);

Dictionary<string, List<PatternNodeTake2>> nodesPerPattern = new();
Dictionary<string, List<PatternNodeTake2>> patternNodesTake2 = new();
foreach (var validPattern in validPatterns)
{
    patternNodesTake2.Add(validPattern, FindAllTowelCombinationsTake2(validPattern));
}

long numberOfValidPatterns = 0;

foreach (var validPattern in validPatterns)
{
    long combinations = patternNodesTake2[validPattern].Sum(x => x.NumberCombinations());
    numberOfValidPatterns += combinations;
}

Console.WriteLine("Part 2: " + numberOfValidPatterns);

bool IsPatternValidPart1(string pattern)
{
    if (cachedValidPatternsPart1.Contains(pattern))
        return true;
    if (cachedInvalidPatternsPart1.Contains(pattern))
        return false;
    
    bool valid = false;
    foreach (var towel in towels)
    {
        if (pattern.StartsWith(towel) && pattern.Length >= towel.Length)
        {
            //Valid towel
            if (pattern.Length == towel.Length)
                return true;

            if (IsPatternValidPart1(pattern.Substring(towel.Length)))
            {
                valid = true;
                break;
            }
        }
    }

    if (valid)
    {
        cachedValidPatternsPart1.Add(pattern);
    }
    else
    {
        cachedInvalidPatternsPart1.Add(pattern);
    }

    return valid;
}

List<PatternNodeTake2> FindAllTowelCombinationsTake2(string pattern)
{
    if (nodesPerPattern.TryGetValue(pattern, out var hit))
        return hit;
    
    List<PatternNodeTake2> newNodes = new();
    foreach (var towel in towels)
    {
        if (pattern.StartsWith(towel) && pattern.Length >= towel.Length)
        {
            var rightSide = pattern.Substring(towel.Length);
            
            PatternNodeTake2 node = new PatternNodeTake2 { PatternString = pattern, TowelMatch = towel };
            
            if (rightSide.Length > 0)
            {
                node.PossibleRightSides = FindAllTowelCombinationsTake2(rightSide);
            }
            
            newNodes.Add(node);
        }
    }
    
    nodesPerPattern.Add(pattern, newNodes);
    return nodesPerPattern[pattern];
}

class PatternNodeTake2
{
    public string PatternString { get; set; }
    public string TowelMatch { get; set; }
    public List<PatternNodeTake2> PossibleRightSides { get; set; }

    private long _numberCombinations = 0;
    
    public long NumberCombinations()
    {
        if (_numberCombinations > 0)
            return _numberCombinations;

        if (PossibleRightSides == null)
        {
            _numberCombinations = 1;
            return 1;
        }

        long sum = 0;
        foreach (var rightSide in PossibleRightSides)
        {
            sum += rightSide.NumberCombinations();
        }
        
        _numberCombinations = sum;
        return sum;
    }
}