
List<long> monkeyBuyers = File.ReadAllLines("Input/Input.txt").Select(long.Parse).ToList();
Dictionary<long, long> _cache = new();
List<List<(long price, List<long>)>> _monkeyShops = new();

long sum = 0;
foreach (var monkeyBuyer in monkeyBuyers)
{
    List<(long price, List<long>)> _monkeyShop = new();
    
    long bestPrice = long.MinValue;
    var current = Evolve(monkeyBuyer);
    Queue<long> last4Changes = new();
    
    for (int i = 1; i < 2000; i++)
    {
        var previousPrice = current % 10;
        current = Evolve(current);
        var currentPrice = current % 10;
        
        if (last4Changes.Count == 4)
        {
            last4Changes.Dequeue();
        }
        last4Changes.Enqueue(currentPrice - previousPrice);
        
        _monkeyShop.Add((currentPrice, last4Changes.ToList()));
        
        if (currentPrice > bestPrice)
            bestPrice = currentPrice;

        
    }
    sum += current;
    
    _monkeyShops.Add(_monkeyShop);
}

Console.WriteLine("Part 1: " + sum);
Console.WriteLine();
Console.WriteLine("Press any key to start part 2 (slow!)");
Console.ReadKey();
Console.WriteLine("Starting part 2... Really slow takes >1 hour. Progress will go faster as we start caching results.");

List<long> bestCombo = new();
long bestNumberBananas = long.MinValue;

HashSet<string> _checkedCombos = new();
var consoleTop = Console.GetCursorPosition().Top;
int totalWork = _monkeyShops.Count;
int workDone = 1;
DateTime start = DateTime.Now;
for (int initialMonkeyShop = 0; initialMonkeyShop < _monkeyShops.Count; initialMonkeyShop++)
{
    DateTime now = DateTime.Now;

    //Calculate estimated time left
    var elapsed = now - start;
    var ticksPerWork = elapsed.Ticks / workDone;
    var estimated = TimeSpan.FromTicks(ticksPerWork * (totalWork - workDone));

    Console.SetCursorPosition(0, consoleTop);
    
    int progressPercentage = (int)((double)workDone / totalWork * 100);
    workDone++;
    for (int p = 0; p < 100; p++)
    {
        Console.BackgroundColor = p < progressPercentage ? ConsoleColor.Green : ConsoleColor.White;
        Console.Write(" ");
    }

    Console.ResetColor();
    Console.Write(" " + workDone + "/" + totalWork + ". Unique combos so far: " + _checkedCombos.Count +
                  ". Estimated remaning time: " + estimated.ToString(@"hh\:mm\:ss"));
    Console.WriteLine("");

    var monkeyShopOffersForI = _monkeyShops[initialMonkeyShop];

    //Go over each combo
    foreach (var (_, combos) in monkeyShopOffersForI)
    {
        Console.SetCursorPosition(0, consoleTop + 1);
        var key = string.Join("", combos);
        if (_checkedCombos.Contains(key))
        {
            continue;
        }

        if (combos.Count < 4) //Skip if not enough data
        {
            continue;
        }

        long numberBananas = 0; //How many bananas we can buy with this combo?

        foreach (var monkeyShop in _monkeyShops)
        {
            //Get first time we see this combo
            var monkeyShopOffer = monkeyShop.FirstOrDefault(x => x.Item2.SequenceEqual(combos));
            if (monkeyShop != null)
            {
                numberBananas += monkeyShopOffer.price;
            }
        }

        _checkedCombos.Add(key);

        if (numberBananas > bestNumberBananas)
        {
            Console.WriteLine("New best: " + numberBananas + " " + string.Join(", ", combos));
            bestNumberBananas = numberBananas;
            bestCombo = combos;
        }
    }
}

Console.WriteLine("Best combo: " + string.Join(", ", bestCombo));
Console.WriteLine("Part 2: " + bestNumberBananas);

long Evolve(long secret)
{
    long key = secret;
    if (_cache.TryGetValue(key, out var evolve))
    {
        return evolve;
    }
    
    //Multiply by 64
    var step1 = secret * 64;
    //Mix into secret (bitwise XOR of multiplied and secret)
    secret = step1 ^ secret;
    //Prune secret number (secret % 16777216
    secret %= 16777216;
    
    //Devide by 32 to nearest whole number
    var step2 = secret / 32;
    //Mix
    secret = step2 ^ secret;
    //Prune
    secret %= 16777216;
    
    //Multiply by 2048
    var step3 = secret * 2048;
    //Mix
    secret = step3 ^ secret;
    //Prune
    secret %= 16777216;
    
    _cache[key] = secret;
    return secret;
}