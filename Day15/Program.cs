
List<List<char>> map = new();
List<List<char>> wideCratesMap = new();
List<char> robotInstructions = new();

(int x, int y) robotPosition = new();
(int x, int y) robotPositionWide = new();

foreach(var line in File.ReadAllLines("Input/Input.txt"))
{
    if (line.Contains('#'))
    {
        map.Add(line.ToList());
        if (line.Contains("@"))
        {
            robotPosition = (line.IndexOf('@'), map.Count - 1);
            map[robotPosition.y][robotPosition.x] = '.';
        }

        List<char> wideCratesLine = new();
        
        foreach (var c in line)
        {
            if (c == '#')
            {
                wideCratesLine.Add('#');
                wideCratesLine.Add('#');
            }
            if (c == 'O')
            {
                wideCratesLine.Add('[');
                wideCratesLine.Add(']');
            }
            if (c == '.')
            {
                wideCratesLine.Add('.');
                wideCratesLine.Add('.');
            }
            if (c == '@')
            {
                robotPositionWide = (wideCratesLine.Count, wideCratesMap.Count);
                wideCratesLine.Add('.');
                wideCratesLine.Add('.');
            }
        }
        
        wideCratesMap.Add(wideCratesLine);
    }
    else
    {
        robotInstructions.AddRange(line.Where(c => c == '<' || c == '>' || c == '^' || c == 'v'));
    }
}

string robotMessage = "";
foreach (var robotInstruction in robotInstructions)
{
    (int x, int y) moveDirection = new();
    PrintMap(map, robotMessage);
    
    switch (robotInstruction)
    {
        case '<':
            moveDirection.x = -1;
            break;
        case '>':
            moveDirection.x = 1;
            break;
        case '^':
            moveDirection.y = -1;
            break;
        case 'v':
            moveDirection.y = 1;
            break;
    }

    (int x, int y) newPosition = (robotPosition.x + moveDirection.x, robotPosition.y + moveDirection.y);
    
    if (map[newPosition.y][newPosition.x] == '#')
    {
        robotMessage = "Robot hit a wall";
        continue;
    }
    
    if (map[newPosition.y][newPosition.x] == 'O')
    {
        int stepsToFreeOrWall = 0;
        
        while (map[newPosition.y][newPosition.x] == 'O')
        {
            newPosition = (newPosition.x + moveDirection.x, newPosition.y + moveDirection.y);
            stepsToFreeOrWall++;
        }
        
        if (map[newPosition.y][newPosition.x] == '#')
        {
            robotMessage = "Crates hit a wall";
            continue;
        }
        
        //Move all crates
        for (int i = 0; i < stepsToFreeOrWall; i++)
        {
            map[newPosition.y][newPosition.x] = 'O';
            newPosition = (newPosition.x - moveDirection.x, newPosition.y - moveDirection.y);
            map[newPosition.y][newPosition.x] = '.';
        }
    }
    robotMessage = "Robot moved";
    robotPosition = newPosition;
}

PrintMap(map, "Robot finished moving");

int gpsScore = 0;

for (int y = 0; y < map.Count; y++)
{
    for (int x = 0; x < map[0].Count(); x++)
    {
        if (map[y][x] == 'O')
            gpsScore += 100 * y + x;
    }
}

Console.WriteLine("Part 1: " + gpsScore);
robotPosition = robotPositionWide;
map = wideCratesMap;

foreach (var robotInstruction in robotInstructions)
{
    (int x, int y) moveDirection = new();
    PrintMap(map, robotMessage);
    
    switch (robotInstruction)
    {
        case '<':
            moveDirection.x = -1;
            break;
        case '>':
            moveDirection.x = 1;
            break;
        case '^':
            moveDirection.y = -1;
            break;
        case 'v':
            moveDirection.y = 1;
            break;
    }

    (int x, int y) newPosition = (robotPosition.x + moveDirection.x, robotPosition.y + moveDirection.y);
    
    if (map[newPosition.y][newPosition.x] == '#')
    {
        robotMessage = "Robot hit a wall normally";
        continue;
    }
    
    if (map[newPosition.y][newPosition.x] == '[' || map[newPosition.y][newPosition.x] == ']')
    {
        int stepsToFreeOrWall = 0;

        if (moveDirection.y == 0) //Same as part 1
        {
            while (map[newPosition.y][newPosition.x] == '[' || map[newPosition.y][newPosition.x] == ']')
            {
                newPosition = (newPosition.x + moveDirection.x, newPosition.y + moveDirection.y);
                stepsToFreeOrWall++;
            }

            if (map[newPosition.y][newPosition.x] == '#')
            {
                robotMessage = "Crates hit a wall";
                continue;
            }

            //Move all crates
            for (int i = 0; i < stepsToFreeOrWall; i++)
            {
                map[newPosition.y][newPosition.x] = map[newPosition.y][newPosition.x- moveDirection.x];
                newPosition = (newPosition.x - moveDirection.x, newPosition.y - moveDirection.y);
                map[newPosition.y][newPosition.x] = '.';
            }
        }
        else
        {
            bool failedMovement = false;
            Dictionary<int, List<int>> cratePositionsOnLine = new();
            cratePositionsOnLine[robotPosition.y] = [robotPosition.x];

            
            int currentY = robotPosition.y;
            
            while (true)
            {
                List<int> cratePositionsOnNextLine = new();
                foreach (var xPositionToCheck in cratePositionsOnLine[currentY])
                {
                    var hit = map[currentY + moveDirection.y][xPositionToCheck];
                    if (hit == '#')
                    {
                        failedMovement = true;
                        break;
                    }

                    if (hit == '.')
                    {
                        continue;
                    }

                    if (hit == '[' || hit == ']')
                    {
                        cratePositionsOnNextLine.Add(xPositionToCheck);
                    }
                }

                if (failedMovement)
                {
                    robotMessage = "Crates hit a wall";
                    break;
                }

                if (cratePositionsOnNextLine.Count == 0)
                {
                    robotMessage = "Robot pushes crates";
                    break;
                }
                
                if (map[currentY + moveDirection.y][cratePositionsOnNextLine.First()] == ']')
                    cratePositionsOnNextLine.Insert(0, cratePositionsOnNextLine.First() - 1);
                
                if (map[currentY + moveDirection.y][cratePositionsOnNextLine.Last()] == '[')
                    cratePositionsOnNextLine.Add(cratePositionsOnNextLine.Last() + 1);
                
                currentY += moveDirection.y;
                cratePositionsOnLine[currentY] = cratePositionsOnNextLine;
            }
            
            if (failedMovement)
            {
                continue;
            }

            
            foreach (var cratesOnLine in cratePositionsOnLine.Reverse())
            {
                var y = cratesOnLine.Key;
                foreach (var x in cratesOnLine.Value)
                {
                    map[y + moveDirection.y][x] = map[y][x];
                    map[y][x] = '.';
                    
                }
            }
            
        }
    }
    
    robotMessage = "Robot moved";
    robotPosition = newPosition;
}

gpsScore = 0;

for (int y = 0; y < map.Count; y++)
{
    for (int x = 0; x < map[0].Count; x++)
    {
        if (map[y][x] == '[')
            gpsScore += 100 * y + x;
    }
}

Console.WriteLine("Part 2: " + gpsScore);

void PrintMap(List<List<char>> map, string message = "")
{
    return;
    Console.Clear();
    Console.WriteLine(message);

    for (int y = 0; y < map.Count; y++)
    {
        for (int x = 0; x < map[0].Count; x++)
        {
            if (robotPosition.x == x && robotPosition.y == y)
                Console.Write("@");
            else
                Console.Write(map[y][x]);
        }
        Console.WriteLine();
    }
    Thread.Sleep(30);
}