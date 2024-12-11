
var initialStones = File.ReadAllText("Input/Input.txt").Split(" ").Select(long.Parse);

const int blinksToCalculatePart1 = 25;
const int blinksToCalculatePart2 = 75;

foreach (var stone in initialStones)
{
    new Stone(stone);
}

for (int i = 0; i < blinksToCalculatePart2; i++)
{
    foreach (var stone in Stone.stoneMap.Values.ToArray())
    {
        stone.Blink();
    }
}

Dictionary<Stone, Dictionary<int,long>> cache = new();

long totalStones = 0;
foreach (var initialStone in initialStones)
{
    totalStones += GetBlinkTranformationCount(Stone.stoneMap[initialStone], blinksToCalculatePart1);
}

Console.WriteLine("Part 1: " + totalStones);

totalStones = 0;
foreach (var initialStone in initialStones)
{
    totalStones += GetBlinkTranformationCount(Stone.stoneMap[initialStone], blinksToCalculatePart2);
}

Console.WriteLine("Part 2: " + totalStones);

long GetBlinkTranformationCount(Stone current, int remaningSteps)
{
    if (remaningSteps == 0)
        return 1;

    if (!cache.ContainsKey(current))
        cache.Add(current, new());

    if (cache[current].ContainsKey(remaningSteps))
        return cache[current][remaningSteps];
    
    long count = 0;
    foreach (var newStone in current.NewStonesOnBlink)
    {
        count += GetBlinkTranformationCount(newStone, remaningSteps - 1);
    }

    cache[current].Add(remaningSteps, count);
    
    return count;
}

class Stone
{
    public static Dictionary<long, Stone> stoneMap = new();
    public long Number { get; set; }
    public List<Stone> NewStonesOnBlink { get; set; }
    
    private bool _hasBlinked;

    public Stone(long number)
    {
        Number = number;
        stoneMap.Add(number, this);
    }
    
    public void Blink()
    {
        if (_hasBlinked)
            return;
        
        var newStones = CalculateBlinkStones();
        NewStonesOnBlink = new List<Stone>();
        foreach (var newStone in newStones)
        {
            if (stoneMap.TryGetValue(newStone, out var value))
            {
                NewStonesOnBlink.Add(value);
            }
            else
            {
                NewStonesOnBlink.Add(new Stone(newStone));
            }
        }
        
        _hasBlinked = true;
    }
    
    List<long> CalculateBlinkStones()
    {
        if (Number == 0)
        {
            return new() { 1 };
        }
    
        var inputStoneAsString = Number.ToString();
    
        if (inputStoneAsString.Length % 2 == 0)
        {
            var halfLength = inputStoneAsString.Length / 2;
            var firstHalf = inputStoneAsString.Substring(0, halfLength);
            var secondHalf = inputStoneAsString.Substring(halfLength);
        
            return new (){long.Parse(firstHalf), long.Parse(secondHalf)};
        }
    
        return new() { Number * 2024 };
    }
}