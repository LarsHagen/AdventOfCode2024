List<List<int>> map = new();

foreach (var line in File.ReadAllLines("Input/Input.txt"))
{
    map.Add(line.Select(c => int.Parse(c.ToString())).ToList());
}

var mapHeight = map.Count;
var mapWidth = map[0].Count;

int sumScores = 0;
int sumRatings = 0;

for (int y = 0; y < mapHeight; y++)
{
    for (int x = 0; x < mapWidth; x++)
    {
        if (map[y][x] == 0)
        {
            var pathCalculation = RecursiveTrailheadScore(x, y, 0);
            var score = pathCalculation.Distinct().Count();
            var rating = pathCalculation.Count;
            
            sumScores += score;
            sumRatings += rating;
        }
    }
}

Console.WriteLine("Part 1: " + sumScores);
Console.WriteLine("Part 2: " + sumRatings);

List<(int x, int y)> RecursiveTrailheadScore(int x, int y, int desiredHeight)
{
    var returnSet = new List<(int x, int y)>();
    
    if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
        return returnSet;

    var currentHeight = map[y][x];
    if (currentHeight != desiredHeight)
        return returnSet;

    if (currentHeight == 9)
    {
        returnSet.Add((x,y));
        return returnSet;
    }

    returnSet.AddRange(RecursiveTrailheadScore(x + 1, y, currentHeight + 1));
    returnSet.AddRange(RecursiveTrailheadScore(x - 1, y, currentHeight + 1));
    returnSet.AddRange(RecursiveTrailheadScore(x, y + 1, currentHeight + 1));
    returnSet.AddRange(RecursiveTrailheadScore(x, y - 1, currentHeight + 1));

    return returnSet;
}