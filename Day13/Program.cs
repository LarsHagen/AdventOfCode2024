
var allText = File.ReadAllLines("Input/Input.txt");

long winAllPrizesCost = 0;
long winAllBigPrizeCost = 0;

for (int i = 0; i < allText.Length; i += 4)
{
    var machine = new Machine(allText[i], allText[i + 1], allText[i + 2]);
    
    var combinations = machine.GetCombinations();
    if (combinations.Count != 0)
    {
        long cheapestCombination = long.MaxValue;
        foreach (var combination in combinations)
        {
            var cost = CombinationCost(combination.buttonPressesA, combination.buttonPressesB);
            if (cost < cheapestCombination)
            {
                cheapestCombination = cost;
            }
        }
    
        winAllPrizesCost += cheapestCombination;
    }

    var combinationsBigPrize = machine.Solve();
    if (combinationsBigPrize != null)
    {
        var cost = CombinationCost(combinationsBigPrize.Value.buttonPressesA, combinationsBigPrize.Value.buttonPressesB);
        winAllBigPrizeCost += cost;
    }
}

Console.WriteLine($"Part 1: {winAllPrizesCost}");
Console.WriteLine($"Part 2: {winAllBigPrizeCost}");

long CombinationCost(long buttonPressesA, long buttonPressesB)
{
    return buttonPressesA * 3 + buttonPressesB * 1; 
}

class Machine
{
    public (int x, int y) ButtonA { get; set; }
    public (int x, int y) ButtonB { get; set; }
    public (int x, int y) Prize { get; set; }
    
    public (long x, long y) PrizeBigNumber { get; set; }
    public Machine(string buttonA, string buttonB, string prize)
    {
        buttonA = buttonA.Replace("Button A:", "").Replace("X+", "").Replace("Y+", "");
        buttonB = buttonB.Replace("Button B:", "").Replace("X+", "").Replace("Y+", "");
        prize = prize.Replace("Prize:", "").Replace("X=", "").Replace("Y=", "");
        
        ButtonA = (int.Parse(buttonA.Split(",")[0]), int.Parse(buttonA.Split(",")[1]));
        ButtonB = (int.Parse(buttonB.Split(",")[0]), int.Parse(buttonB.Split(",")[1]));
        Prize = (int.Parse(prize.Split(",")[0]), int.Parse(prize.Split(",")[1]));

        PrizeBigNumber = (Prize.x + 10000000000000, Prize.y + 10000000000000);
    }
    
    public List<(int buttonPressesA, int buttonPressesB)> GetCombinations()
    {
        var combinations = new List<(int buttonPressesA, int buttonPressesB)>();
        
        for (int a = 0; a < 100; a++)
        {
            for (int b = 0; b < 100; b++)
            {
                if (ButtonA.x * a + ButtonB.x * b == Prize.x && ButtonA.y * a + ButtonB.y * b == Prize.y)
                {
                    combinations.Add((a, b));
                }
            }
        }

        return combinations;
    }

    public (long buttonPressesA, long buttonPressesB)? Solve()
    {
        double X = PrizeBigNumber.x;
        double Y = PrizeBigNumber.y;
        double aDistX = ButtonA.x;
        double aDistY = ButtonA.y;
        double bDistX = ButtonB.x;
        double bDistY = ButtonB.y;
        
        //X = aDistX * aPresses + bDistX * bPresses
        //aDistX * aPresses = X - bDistX * bPresses
        //aPresses = (X - bDistX * bPresses) / aDistX
        
        //Y = aDistY * aPresses + bDistY * bPresses
        //aDistY * aPresses = Y - bDistY * bPresses
        //aPresses = (Y - bDistY * bPresses) / aDistY
        
        //(X - bDistX * bPresses) / aDistX = (Y - bDistY * bPresses) / aDistY
        //(X - bDistX * bPresses) * aDistY = (Y - bDistY * bPresses) * aDistX
        //X * aDistY - bDistX * bPresses * aDistY = Y * aDistX - bDistY * bPresses * aDistX
        //X * aDistY = Y * aDistX - bDistY * bPresses * aDistX + bDistX * bPresses * aDistY
        //X * aDistY = Y * aDistX + bPresses * (bDistX * aDistY - bDistY * aDistX)
        //X * aDistY - Y * aDistX = bPresses * (bDistX * aDistY - bDistY * aDistX)
        //bPresses = (X * aDistY - Y * aDistX) / (bDistX * aDistY - bDistY * aDistX)

        double bPresses = (X * aDistY - Y * aDistX) / (bDistX * aDistY - bDistY * aDistX);
        double aPresses = (X - bDistX * bPresses) / aDistX;

        if (aPresses - (long)aPresses != 0)
        {
            return null;
        }
        if (bPresses - (long)bPresses != 0)
        {
            return null;
        }

        return ((long)aPresses, (long)bPresses);
    }
    
}