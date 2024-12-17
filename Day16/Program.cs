List<List<char>> map = new();

foreach (var line in File.ReadAllLines("Input/Input.txt"))
{
    map.Add(line.ToList());
}

var mapWidth = map[0].Count;
var mapHeight = map.Count;

(int x, int y) reindeerStartPosition = new();
(int x, int y) endPosition = new();

for (int y = 0; y < mapHeight; y++)
{
    for (int x = 0; x < mapWidth; x++)
    {
        if (map[y][x] == 'S')
        {
            reindeerStartPosition = (x, y);
            map[y][x] = '.';
        }

        if (map[y][x] == 'E')
        {
            endPosition = (x, y);
            map[y][x] = '.';
        }
    }
}

//Remove all dead ends
int freeSpaces = map.SelectMany(x => x).Count(x => x == '.');
while (true)
{
    int removedThisRun = 0;
    for (int y = 0; y < mapHeight; y++)
    {
        for (int x = 0; x < mapWidth; x++)
        {
            if ((x, y) == reindeerStartPosition || (x, y) == endPosition)
                continue;
            
            if (map[y][x] == '.')
            {
                //Count number of walls around
                int walls = 0;
                if (map[y][x - 1] == '#')
                    walls++;
                if (map[y][x + 1] == '#')
                    walls++;
                if (map[y - 1][x] == '#')
                    walls++;
                if (map[y + 1][x] == '#')
                    walls++;

                if (walls >= 3) //Dead end
                {
                    map[y][x] = '#';
                    removedThisRun++;
                }
            }
        }
    }
    
    if (removedThisRun == 0)
        break;
    freeSpaces -= removedThisRun;
    Console.WriteLine(freeSpaces);
}


List<Node> nodeMap = new();
List<Node> openNodes = new();
Node startNode = new Node
{
    Position = reindeerStartPosition,
    Direction = Node.DirectionEnum.East,
};

nodeMap.Add(startNode);
openNodes.Add(startNode);

while (openNodes.Count > 0)
{
    openNodes = openNodes.OrderBy(n => n.Cost).ToList();
    var currentNode = openNodes.First();
    openNodes.Remove(currentNode);
    
    if (currentNode.Position == endPosition)
        continue;

    if (map[currentNode.Position.y][currentNode.Position.x] == '#')
    {
        nodeMap.Remove(currentNode);
        continue;
    }

    var forwardNode = new Node
    {
        Position = (currentNode.Position.x + currentNode.DirectionVector.x, currentNode.Position.y + currentNode.DirectionVector.y),
        Direction = currentNode.Direction,
        PreviousNodes = new List<Node>(){currentNode}
    };
    
    var rotateLeftNode = new Node
    {
        Position = currentNode.Position,
        Direction = (Node.DirectionEnum)(((int)currentNode.Direction + 1) % 4),
        PreviousNodes = new List<Node>(){currentNode}
    };
    
    var rotateRightNode = new Node
    {
        Position = currentNode.Position,
        Direction = (Node.DirectionEnum)(((int)currentNode.Direction + 3) % 4),
        PreviousNodes = new List<Node>(){currentNode}
    };
    
    CheckNode(forwardNode);
    if (map[rotateLeftNode.Position.y + rotateLeftNode.DirectionVector.y][rotateLeftNode.Position.x + rotateLeftNode.DirectionVector.x] != '#')
        CheckNode(rotateLeftNode);
    if (map[rotateRightNode.Position.y + rotateRightNode.DirectionVector.y][rotateRightNode.Position.x + rotateRightNode.DirectionVector.x] != '#')
        CheckNode(rotateRightNode);
}

//Print map
for (int y = 0; y < mapHeight; y++)
{
    for (int x = 0; x < mapWidth; x++)
    {
        var node = nodeMap.Where(n => n.Position == (x,y)).MinBy(n => n.Cost);
        if (node == null)
            continue;
        
        var directionChar = node.Direction switch
        {
            Node.DirectionEnum.East => '>',
            Node.DirectionEnum.South => 'v',
            Node.DirectionEnum.West => '<',
            Node.DirectionEnum.North => '^',
            _ => throw new Exception("Invalid direction")
        };
        map[y][x] = directionChar;
    }
}

var endNodes = nodeMap.Where(n => n.Position == endPosition).ToList();
var endNode = endNodes.OrderBy(n => n.Cost).FirstOrDefault();
var endNodeCost = endNode.Cost;
if (endNode == null)
{
    Console.WriteLine("No path found");
    return;
}

Console.WriteLine("Part 1: " + endNodeCost ); 


//Part 2
HashSet<Node> allNodesThatLeadToEnd = new();
List<Node> queue = new() { endNode };
while (queue.Count > 0)
{
    var currentNode = queue.First();
    queue.Remove(currentNode);
    if (!allNodesThatLeadToEnd.Any(n => currentNode.Position == n.Position))
        allNodesThatLeadToEnd.Add(currentNode);
    if (currentNode.PreviousNodes == null)
        continue;
    queue.AddRange(currentNode.PreviousNodes);
}

for (int y = 0; y < mapHeight; y++)
{
    for (int x = 0; x < mapWidth; x++)
    {
        if (reindeerStartPosition.x == x && reindeerStartPosition.y == y)
            Console.Write("S");
        else if (endPosition.x == x && endPosition.y == y)
            Console.Write("E");
        else
        {
            if (allNodesThatLeadToEnd.Any(n => n.Position == (x,y)))
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.Write(map[y][x]);
        }
    }
    Console.WriteLine();
}


Console.WriteLine("Part 2: " + allNodesThatLeadToEnd.Count);


foreach (var en in nodeMap.Where(n => n.Position == endPosition).ToList())
{
    Console.WriteLine("Path to end: " + en.Cost);
}

void CheckNode(Node newNode)
{
    var existingNode = nodeMap.FirstOrDefault(n => n.Position == newNode.Position && n.Direction == newNode.Direction);
    if (existingNode == null)
    {
        nodeMap.Add(newNode);
        openNodes.Add(newNode);
    }
    else
    {
        if (newNode.Cost == existingNode.Cost)
        {
            existingNode.PreviousNodes.AddRange(newNode.PreviousNodes);
        }
        else if (newNode.Cost < existingNode.Cost)
        {
            existingNode.PreviousNodes = newNode.PreviousNodes;
        }
    }
}

class Node
{
    public enum DirectionEnum
    {
        East,
        South,
        West,
        North
    };
    
    
    public (int x, int y) Position { get; set; }
    public DirectionEnum Direction { get; set; }
    
    public (int x, int y) DirectionVector => Direction switch
    {
        DirectionEnum.East => (1, 0),
        DirectionEnum.South => (0, 1),
        DirectionEnum.West => (-1, 0),
        DirectionEnum.North => (0, -1),
        _ => throw new Exception("Invalid direction")
    };
    
    public int Cost => CalculateCost();
    public List<Node> PreviousNodes { get; set; }

    private int CalculateCost()
    {
        var PreviousNode = PreviousNodes?.FirstOrDefault();
        
        if (PreviousNode == null)
            return 0;

        if (PreviousNode.Position == Position && PreviousNode.Direction != Direction)
        {
            return PreviousNode.Cost + 1000;
        }
        
        if (PreviousNode.Position != Position && PreviousNode.Direction == Direction)
        {
            return PreviousNode.Cost + 1;
        }
        
        throw new Exception("Invalid node connection");
    }
}