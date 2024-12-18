
bool visualize = true;

List<(int x, int y)> fallList = File.ReadAllLines("Input/Input.txt")
    .Select(s => s.Split(','))
    .Select(s => (int.Parse(s[0]), int.Parse(s[1]))).ToList();

int mapWidth = fallList.Max(x => x.x) + 1;
int mapHeight = fallList.Max(x => x.y) + 1;

HashSet<(int x, int y)> corruptedPoints = new();
Dictionary<int, HashSet<(int x, int y)>> corruptedPointsPerStep = new();

for (int i = 0; i < fallList.Count; i++)
{
    corruptedPoints.Add(fallList[i]);
    corruptedPointsPerStep.Add(i, corruptedPoints);
    corruptedPoints = new(corruptedPoints);
}

//int stepsToFillPart1 = 12; //Example: 12. Input: 1024
int stepsToFillPart1 = 1024;
HashSet<(int x, int y)> mapAfterSteps = corruptedPointsPerStep[stepsToFillPart1 - 1];


var path = PathFinding(mapAfterSteps);

if (visualize)
{
    Console.Clear();
    PrintMap(mapAfterSteps, path);
}
Console.WriteLine("Part 1: " + (path.Count - 1));
if (visualize)
{
    Console.WriteLine("Press any key for part 2");
    Console.Read();
}
else
{
    Console.WriteLine("Calculating part 2");
}

int step = stepsToFillPart1;
List<(int x, int y)> lastValidPath = path;

while (true)
{
    List<(int x, int y)> validPath = PathFinding(corruptedPointsPerStep[step]);
    
    if (validPath.Count > 0)
    {
        lastValidPath = validPath;
        if (visualize)
        {
            Console.Clear();
            PrintMap(corruptedPointsPerStep[step], validPath);
            Thread.Sleep(100);
        }
        
        //Find next corruption that blocks the path
        for (int i = step; i < fallList.Count; i++)
        {
            if (validPath.Contains(fallList[i]))
            {
                step = i;
                break;
            }
        }
    }
    else
    {
        var blockingPoint = fallList[step];
        
        if (visualize)
        {
            Console.Clear();
            lastValidPath.RemoveRange(0, lastValidPath.IndexOf(blockingPoint));
            PrintMap(corruptedPointsPerStep[step], lastValidPath, blockingPoint);
        }

        Console.WriteLine("Part 2: " + blockingPoint.x + "," + blockingPoint.y); 
        break;
    }
}

List<(int x, int y)> PathFinding(HashSet<(int x, int y)> corruptedPoints)
{
    Dictionary<(int x, int y), Node> nodes = new();
    var currentNode = new Node { Position = (0, 0) };
    nodes.Add(currentNode.Position, currentNode);
    List<Node> nodesToCheck = new();
    nodesToCheck.Add(currentNode);
    
    (int x, int y) endPosition = (mapWidth - 1, mapHeight - 1);
    
    Node? endNode = null;
    
    while (nodesToCheck.Count > 0)
    {
        //Sort nodesToCheck by distance to end
        nodesToCheck = nodesToCheck.OrderBy(n => n.MinStepsToEnd(endPosition)).ToList();
        
        currentNode = nodesToCheck[0];
        nodesToCheck.Remove(currentNode);
        
        if (currentNode.Position == endPosition)
        {
            endNode = currentNode;
            continue;
        }
        
        if (endNode != null && currentNode.MinStepsToEnd(endPosition) >= endNode.Steps)
            continue; //We can not get to end any faster
        
        var possibleMoves = new List<(int x, int y)>
        {
            (currentNode.Position.x - 1, currentNode.Position.y),
            (currentNode.Position.x + 1, currentNode.Position.y),
            (currentNode.Position.x, currentNode.Position.y - 1),
            (currentNode.Position.x, currentNode.Position.y + 1)
        };
        
        foreach (var move in possibleMoves)
        {
            if (corruptedPoints.Contains(move) || move.x < 0 || move.x >= mapWidth || move.y < 0 || move.y >= mapHeight)
                continue; //Blocked
            
            if (!nodes.ContainsKey(move))
            {
                var newNode = new Node { Position = move, Previous = currentNode };
                nodes.Add(move, newNode);
                nodesToCheck.Add(newNode);
            }
            else
            {
                if (nodes[move].Steps > currentNode.Steps + 1)
                {
                    nodes[move].Previous = currentNode;
                    nodesToCheck.Add(nodes[move]);
                }
            }
        }
    }
    
    if (nodes.TryGetValue(endPosition, out endNode))
        return endNode.GetPath();
    
    return new();
}

void PrintMap(HashSet<(int x, int y)> corruptedPoints, List<(int x, int y)> path = null, (int x, int y)? highlightPoint = null)
{
    for (int y = 0; y < mapHeight; y++)
    {
        for (int x = 0; x < mapWidth; x++)
        {
            if (highlightPoint != null && highlightPoint.Value == (x, y))
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Write("X");
                continue;
            }
            
            if (path != null && path.Contains((x, y)))
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write(" ");
                continue;
            }

            if (corruptedPoints.Contains((x, y)))
                Console.BackgroundColor = ConsoleColor.Red;
            else
                Console.ResetColor();
            
            Console.Write(" ");
        }
        Console.WriteLine();
    }
    Console.ResetColor();
}

class Node
{
    public Node Previous { get; set; }
    public (int x, int y) Position { get; set; }
    public int Steps => Previous == null ? 0 : Previous.Steps + 1;

    public int MinStepsToEnd((int x, int y) end)
    {
        return Steps + Math.Abs(Position.x - end.x) + Math.Abs(Position.y - end.y);
    }
    
    public List<(int x, int y)> GetPath()
    {
        var path = new List<(int x, int y)>();
        var currentNode = this;
        while (currentNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Previous;
        }
        return path;
    }
} 