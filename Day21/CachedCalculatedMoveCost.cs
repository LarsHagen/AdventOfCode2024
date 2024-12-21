namespace Day21;

public class CachedCalculatedMoveCost
{
    private Dictionary<string, long> cached = new();
    private Keypad robotKeypad = new (true);
    public long GetCost(string sequence, int robotLayers)
    {
        var key = sequence + robotLayers;
        if (cached.ContainsKey(key))
        {
            return cached[key];
        }
        
        if (robotLayers == 0)
        {
            return sequence.Length;
        }

        (int x, int y) currentPosition = robotKeypad.ButtonCoordinates['A'];
        long length = 0;
        foreach (var c in sequence)
        {
            var coordinate = robotKeypad.ButtonCoordinates[c];
            var possiblePaths = robotKeypad.GetSequencesFromTo(currentPosition, coordinate);

            long minCost = long.MaxValue;
            foreach (var possiblePath in possiblePaths)
            {
                var cost = GetCost(possiblePath + "A", robotLayers-1);
                if (cost < minCost)
                {
                    minCost = cost;
                }
            }
            
            length += minCost;
            
            currentPosition = coordinate;
        }
        
        cached[key] = length;
        return length;
    }
}