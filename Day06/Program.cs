
List<List<Char>> map = new();

(int x, int y, char direction) guard = (-1,-1,' ');

const char dirUp = '^';
const char dirDown = 'v';
const char dirLeft = '<';
const char dirRight = '>';

List<(int x, int y, char direction)> guardLines = new(); //Guard will at some point move through x,y with direction

Dictionary<char, (int xDir, int yDir)> directionMapping = new()
{
    {dirUp, (0, -1)},
    {dirDown, (0, 1)},
    {dirLeft, (-1, 0)},
    {dirRight, (1, 0)}
}; 

int linePosition = 0;
foreach (String line in File.ReadLines("Input/Input.txt"))
{
    if (line.Contains('^'))
        guard = (line.IndexOf('^'), linePosition, dirUp);
    
    map.Add(line.ToCharArray().ToList());
    linePosition++;
}

var mapWidth = map[0].Count;
var mapHeight = map.Count;

Console.WriteLine("Part 1: " + CalculateGuardPath(guard, out _, out guardLines));
guardLines.Add(guard);

//Find overlaping guardlines to see where we could get her to loop

HashSet<(int x, int y)> overlaps = new();

foreach (var guardLineA in guardLines)
{
    List<(int x, int y)> lineA = new();
    var directionA = directionMapping[guardLineA.direction];
    if (directionA.xDir == 0)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            lineA.Add((guardLineA.x, y));
        }
    }
    else
    {
        for (int x = 0; x < mapWidth; x++)
        {
            lineA.Add((x, guardLineA.y));
        }
    }
    
    foreach (var guardLineB in guardLines)
    {
        List<(int x, int y)> lineB = new();
        
        var directionB = directionMapping[guardLineB.direction];
        
        if (directionB.xDir == 0)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                lineB.Add((guardLineB.x, y));
            }
        }
        else
        {
            for (int x = 0; x < mapWidth; x++)
            {
                lineB.Add((x, guardLineB.y));
            }
        }
        
        foreach (var pointA in lineA)
        {
            foreach (var pointB in lineB)
            {
                if (pointA.x == pointB.x && pointA.y == pointB.y)
                {
                    overlaps.Add(pointA);
                }
            }
        }
    }
}

HashSet<(int x, int y)> loopCausingPositions = new();

int progress = 0;
foreach (var overlap in overlaps)
{
    bool loopObstacleLeft = false;
    bool loopObstacleRight = false;
    bool loopObstacleUp = false;
    bool loopObstacleDown = false;
    
    if (!loopCausingPositions.Contains((overlap.x - 1, overlap.y)))
        CalculateGuardPath(guard, out loopObstacleLeft, out _, (overlap.x - 1, overlap.y));
    
    if (!loopCausingPositions.Contains((overlap.x + 1, overlap.y)))
        CalculateGuardPath(guard, out loopObstacleRight, out _, (overlap.x + 1, overlap.y));
    
    if (!loopCausingPositions.Contains((overlap.x, overlap.y - 1)))
        CalculateGuardPath(guard, out loopObstacleUp, out _, (overlap.x, overlap.y - 1));
    
    if (!loopCausingPositions.Contains((overlap.x, overlap.y + 1)))
        CalculateGuardPath(guard, out loopObstacleDown, out _, (overlap.x, overlap.y + 1));

    progress++;
    Console.Write(progress + "/" + overlaps.Count);
    Console.SetCursorPosition(0, Console.CursorTop);
    
    if (loopObstacleLeft)
        loopCausingPositions.Add((overlap.x - 1, overlap.y));
    if (loopObstacleRight)
        loopCausingPositions.Add((overlap.x + 1, overlap.y));
    if (loopObstacleUp)
        loopCausingPositions.Add((overlap.x, overlap.y - 1));
    if (loopObstacleDown)
        loopCausingPositions.Add((overlap.x, overlap.y + 1));
}

Console.WriteLine();
Console.WriteLine("Part 2: " + loopCausingPositions.Count);

int CalculateGuardPath((int x, int y, char direction) guardStart, out bool looping, out List<(int x, int y, char direction)> guardLines, (int x, int y)? customObstacle = null)
{
    
    looping = false;
    guardLines = new();
    List<List<string>> mapCopy = new List<List<string>>();

    foreach (var line in map)
    {
        mapCopy.Add(new());
        foreach (var mapChar in line)
        {
            mapCopy.Last().Add(mapChar.ToString());
        }
    }
    
    if (customObstacle.HasValue)
    {
        if (customObstacle.Value.x < 0 || customObstacle.Value.x >= mapWidth || customObstacle.Value.y < 0 ||
            customObstacle.Value.y >= mapHeight)
            return 0;
        
        mapCopy[customObstacle.Value.y][customObstacle.Value.x] = "#";
    }
    
    //Remove initial guard
    mapCopy[guardStart.y][guardStart.x] = ".";
    
    (int x, int y, char direction) guard = new();
    guard.x = guardStart.x;
    guard.y = guardStart.y;
    guard.direction = guardStart.direction;
    
    while (guard.x >= 0 && guard.x < mapWidth && guard.y >= 0 && guard.y < mapHeight)
    {
        if (mapCopy[guard.y][guard.x].Contains(guard.direction))
        {
            looping = true;
            break;
        }
        
        mapCopy[guard.y][guard.x] += guard.direction;
    
        var direction = directionMapping[guard.direction];
        (int x, int y) nextPosition = (guard.y + direction.yDir, guard.x + direction.xDir);
    
        if (nextPosition.x < 0 || nextPosition.x >= mapHeight || nextPosition.y < 0 || nextPosition.y >= mapWidth)
        {
            break;
        }
    
        var nextMapChar = mapCopy[nextPosition.x][nextPosition.y];

        if (nextMapChar == "#")
        {
            //Turn right
            guard.direction = guard.direction switch
            {
                dirUp => dirRight,
                dirRight => dirDown,
                dirDown => dirLeft,
                dirLeft => dirUp,
                _ => throw new Exception("Invalid direction")
            };
        
            guardLines.Add(guard);
        
            continue;
        }
    
        guard.x += direction.xDir;
        guard.y += direction.yDir;
    }
    
    int visitedGuardPositions = 0;
    foreach (var line in mapCopy)
    {
        visitedGuardPositions += line.Count(x => x.Contains(dirUp) || x.Contains(dirDown) || x.Contains(dirLeft) || x.Contains(dirRight));
    }

    return visitedGuardPositions;
}