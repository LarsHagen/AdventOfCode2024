
string file = "Input/Input.txt";

int mapWidth = file.Contains("Example.txt") ? 11 : 101;
int mapHeight = file.Contains("Example.txt") ? 7 : 103;

var allRobots = File.ReadAllLines(file).Select(x => new Robot(x)).ToList();

for (int i = 0; i < 100; i++)
{
    foreach (var robot in allRobots)
    {
        robot.Move(mapWidth, mapHeight);
    }
}

int halfWidth = mapWidth / 2;
int halfHeight = mapHeight / 2;

int robotsInTopLeft = allRobots.Count(r => r.Position.x < halfWidth && r.Position.y < halfHeight);
int robotsInTopRight = allRobots.Count(r => r.Position.x > halfWidth && r.Position.y < halfHeight);
int robotsInBottomLeft = allRobots.Count(r => r.Position.x < halfWidth && r.Position.y > halfHeight);
int robotsInBottomRight = allRobots.Count(r => r.Position.x > halfWidth && r.Position.y > halfHeight);


Console.WriteLine("Part 1: " + (robotsInTopLeft * robotsInTopRight * robotsInBottomLeft * robotsInBottomRight));

Console.WriteLine("I don't know what the christmas tree is supposed to look like, but we can assume all robots must be visible");
Console.WriteLine("Run simulation until all robots are visible. Then ask user if it looks like a christmas tree, if not, continue simulation.");
Console.WriteLine("Press any key to start simulation");

Console.ReadKey();

long iteration = 100;
while (true)
{
    iteration++;
    foreach (var robot in allRobots)
    {
        robot.Move(mapWidth, mapHeight);
    }
    
    var uniquePositionCount = allRobots.Select(r => r.Position).Distinct().Count();
    if (uniquePositionCount == allRobots.Count)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (allRobots.Any(r => r.Position.x == x && r.Position.y == y))
                    Console.Write("#");
                else
                    Console.Write(".");
            }
            Console.WriteLine();
        }
        
        Console.WriteLine("Does this look like a christmas tree? (y/n)");
        if (Console.ReadLine().ToLower() == "y")
            break;
    }
}

Console.WriteLine("Part 2: " + iteration); //Remember + 100 from part 1

class Robot
{
    public (int x, int y) Position { get; private set; }
    public (int x, int y) Velocity { get; private set; }

    public Robot(string input)
    {
        var split = input.Replace("p=", "").Replace("v=", "").Split(" ");
        Position = (int.Parse(split[0].Split(",")[0]), int.Parse(split[0].Split(",")[1]));
        Velocity = (int.Parse(split[1].Split(",")[0]), int.Parse(split[1].Split(",")[1]));
    }
    
    public void Move(int mapWidth, int mapHeight)
    {
        Position = (Position.x + Velocity.x, Position.y + Velocity.y);
        if (Position.x < 0)
            Position = (mapWidth + Position.x, Position.y);
        if (Position.y < 0)
            Position = (Position.x, mapHeight + Position.y);
        if (Position.x >= mapWidth)
            Position = (Position.x - mapWidth, Position.y);
        if (Position.y >= mapHeight)
            Position = (Position.x, Position.y - mapHeight);
    }
}