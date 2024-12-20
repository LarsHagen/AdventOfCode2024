List<List<char>> map = File.ReadAllLines("Input/Input.txt").Select(x => x.ToList()).ToList();
var mapHeight = map.Count;
var mapWidth = map[0].Count;
(int x, int y) start = (0, 0);
(int x, int y) end = (0, 0);
for (int y = 0; y < mapHeight; y++)
{
    for (int x = 0; x < mapWidth; x++)
    {
        if (map[y][x] == 'S')
        {
            start = (x, y);
            continue;
        }
        
        if (map[y][x] == 'E')
        {
            end = (x, y);
        }
    }
}

Dictionary<(int x, int y), PathNode> pathNodes = new();
List<CollapsableWall> collapsableWalls = new();

PathNode startNode = new PathNode()
{
    Steps = 0,
    Position = start
};

pathNodes.Add(start, startNode);

PathNode currentNode = startNode;
while (currentNode.Position != end)
{
    if (map[currentNode.Position.y][currentNode.Position.x + 1] != '#')
    {
        var newPosition = (currentNode.Position.x + 1, currentNode.Position.y);
        if (!pathNodes.ContainsKey(newPosition))
        {
            currentNode = CreateNode(currentNode, newPosition);
            continue;
        }
    }
    
    if (map[currentNode.Position.y][currentNode.Position.x - 1] != '#')
    {
        var newPosition = (currentNode.Position.x - 1, currentNode.Position.y);
        if (!pathNodes.ContainsKey(newPosition))
        {
            currentNode = CreateNode(currentNode, newPosition);
            continue;
        }
    }
    
    if (map[currentNode.Position.y + 1][currentNode.Position.x] != '#')
    {
        var newPosition = (currentNode.Position.x, currentNode.Position.y + 1);
        if (!pathNodes.ContainsKey(newPosition))
        {
            currentNode = CreateNode(currentNode, newPosition);
            continue;
        }
    }
    
    if (map[currentNode.Position.y - 1][currentNode.Position.x] != '#')
    {
        var newPosition = (currentNode.Position.x, currentNode.Position.y - 1);
        if (!pathNodes.ContainsKey(newPosition))
        {
            currentNode = CreateNode(currentNode, newPosition);
            continue;
        }
    }
}

//PrintMap();

for (int y = 1; y < mapHeight-1; y++)
{
    for (int x = 1; x < mapWidth-1; x++)
    {
        if (map[y][x] != '#')
            continue;
        
        CalculatePart1Cheat(x, y);
    }
}

int numberUsefulCheats = 0;
int saveAtLeast = 100;
foreach (var collapsableWall in collapsableWalls)
{
    if (GetSavedDistance(collapsableWall) >= saveAtLeast)
    {
        numberUsefulCheats++;
    }
}
Console.WriteLine("Part 1: " + numberUsefulCheats);

Dictionary<string, CollapsableWall> collapsableWallsPart2 = new();
for (int y = 0; y < mapHeight; y++)
{
    for (int x = 0; x < mapWidth; x++)
    {
        if (map[y][x] != '#')
            continue;
        
        CalculatePart2Cheat(x, y);
    }
}

Console.WriteLine("Found part 2 cheats " + collapsableWallsPart2.Count);

numberUsefulCheats = 0;
foreach (var collapsableWall in collapsableWallsPart2.Values)
{
    if (GetSavedDistance(collapsableWall) >= saveAtLeast)
    {
        numberUsefulCheats++;
    }
}
Console.WriteLine("Part 2: " + numberUsefulCheats); 

int GetSavedDistance(CollapsableWall cheat)
{
    return cheat.LaterNode.Steps - cheat.EarlierNode.Steps - cheat.CheatingDistance;
}

PathNode CreateNode(PathNode previous, (int x, int y) position)
{
    PathNode newNode = new PathNode { Steps = previous.Steps + 1, Position = position, Previous = previous };
    previous.Next = newNode;
    pathNodes.Add(position, newNode);
    return newNode;
}

void CalculatePart1Cheat(int x, int y)
{
    List<(int x, int y)> openNeighbours = new();
    if (map[y][x + 1] != '#')
        openNeighbours.Add((x + 1, y));
    if (map[y][x - 1] != '#')
        openNeighbours.Add((x - 1, y));
    if (map[y + 1][x] != '#')
        openNeighbours.Add((x, y + 1));
    if (map[y - 1][x] != '#')
        openNeighbours.Add((x, y - 1));
        
    if (openNeighbours.Count != 2)
        return;
        
    var node1 = pathNodes[openNeighbours[0]];
    var node2 = pathNodes[openNeighbours[1]];
        
    var earlierNode = node1.Steps < node2.Steps ? node1 : node2;
    var laterNode = node1.Steps < node2.Steps ? node2 : node1;
        
    collapsableWalls.Add(new()
    {
        CheatingDistance = 2,
        StartPosition = (x, y),
        EarlierNode = earlierNode,
        LaterNode = laterNode
    });
}

void CalculatePart2Cheat(int x, int y)
{
    List<(int x, int y)> openNeighbours = new();
    if (x + 1 < mapWidth && map[y][x + 1] != '#')
        openNeighbours.Add((x + 1, y));
    if (x - 1 >= 0 && map[y][x - 1] != '#')
        openNeighbours.Add((x - 1, y));
    if (y + 1 < mapHeight && map[y + 1][x] != '#')
        openNeighbours.Add((x, y + 1));
    if (y - 1 >= 0 && map[y - 1][x] != '#')
        openNeighbours.Add((x, y - 1));
        
    if (openNeighbours.Count == 0)
        return;


    foreach (var openNeighbour in openNeighbours)
    {
        var cheatStartNode = pathNodes[openNeighbour];
        
        for (int offsetX = -20; offsetX <= 20; offsetX++)
        {
            for (int offsetY = -20; offsetY <= 20; offsetY++)
            {
                //int cheatDistance = Math.Abs((cheatStartNode.Position.x + offsetX) + (cheatStartNode.Position.y + offsetY)) + 3;
                int cheatDistanceX = Math.Abs(cheatStartNode.Position.x - (x + offsetX));
                int cheatDistanceY = Math.Abs(cheatStartNode.Position.y - (y + offsetY));
                int cheatDistance = cheatDistanceX + cheatDistanceY;
                
                if (cheatDistance > 20)
                    continue;
                
                var potentialNode = pathNodes.GetValueOrDefault((x + offsetX, y + offsetY));
                if (potentialNode == null)
                    continue;
                
                if (potentialNode.Steps <= cheatStartNode.Steps)
                    continue;
                
                CollapsableWall cheat = new()
                {
                    CheatingDistance = cheatDistance,
                    StartPosition = (x + offsetX, y + offsetY),
                    EarlierNode = cheatStartNode,
                    LaterNode = potentialNode
                };
                
                if (!collapsableWallsPart2.ContainsKey(cheat.GetKey()))
                {
                    collapsableWallsPart2.Add(cheat.GetKey(), cheat);
                }
            }
        }
    }
}

void PrintMap(CollapsableWall? cheat = null)
{
    for (int y = 0; y < mapHeight; y++)
    {
        for (int x = 0; x < mapWidth; x++)
        {
            //if (cheat != null && cheat.StartPosition == (x, y))
            if (cheat != null && cheat.EarlierNode.Position == (x, y))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("O");
                continue;
            }
            
            if (cheat != null && cheat.LaterNode.Position == (x, y))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("X");
                continue;
            }
            
            if (pathNodes.ContainsKey((x,y)))
                Console.ForegroundColor = ConsoleColor.Green;
            else if (map[y][x] == '#')
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ResetColor();
            
            Console.Write(map[y][x]);
        }
        Console.WriteLine();
    }
    Console.ResetColor();
}

class PathNode
{
    public int Steps { get; set; }
    public (int x, int y) Position { get; set; }
    public PathNode Previous { get; set; }
    public PathNode Next { get; set; }
}

class CollapsableWall
{
    public int CheatingDistance { get; set; }
    public (int x, int y) StartPosition { get; set; }
    public PathNode EarlierNode { get; set; }
    public PathNode LaterNode { get; set; }
    
    public string GetKey() => $"{EarlierNode.Position.x},{EarlierNode.Position.y},{LaterNode.Position.x},{LaterNode.Position.y}";
}