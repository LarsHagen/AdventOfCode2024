
List<List<char>> map = new();
HashSet<char> uniqueAntennaChars = new();

//int lineNum = 0;

foreach (var line in File.ReadAllLines("Input/Input.txt"))
{
    //Console.SetCursorPosition(0, lineNum);
    //Console.WriteLine(line);
    //lineNum++;  
    map.Add(new());
    foreach (var mapChar in line)
    {
        map.Last().Add(mapChar);
        
        if (mapChar != '.')
            uniqueAntennaChars.Add(mapChar);
    }
}

HashSet<(int x, int y)> uniqueAntinodePositionsPart1 = new();
HashSet<(int x, int y)> uniqueAntinodePositionsPart2 = new();


foreach (var antennaChar in uniqueAntennaChars)
{
    var antennaPositions = new List<(int x, int y)>();
    for (var y = 0; y < map.Count; y++)
    {
        for (var x = 0; x < map[y].Count; x++)
        {
            if (map[y][x] == antennaChar)
                antennaPositions.Add((x, y));
        }
    }

    if (antennaPositions.Count == 1)
        continue;

    foreach (var positionA in antennaPositions)
    {
        foreach (var positionB in antennaPositions)
        {
            if (positionA == positionB)
                continue;
            
            int distanceX = positionA.x - positionB.x;
            int distanceY = positionA.y - positionB.y;

            (int x, int y) antinode = (positionA.x + distanceX, positionA.y + distanceY);

            if (antinode.x >= 0 && antinode.x < map[0].Count && antinode.y >= 0 && antinode.y < map.Count)
            {
                uniqueAntinodePositionsPart1.Add(antinode);
            }

            int i = 0;
            while (true)
            {
                
                antinode = (positionA.x + distanceX * i, positionA.y + distanceY * i);
                if (antinode.x < 0 || antinode.x >= map[0].Count || antinode.y < 0 || antinode.y >= map.Count)
                    break;
                
                uniqueAntinodePositionsPart2.Add(antinode);
                //Console.SetCursorPosition(antinode.x, antinode.y);
                //Console.Write("#");
                
                i++;
            }

            i = 0;
            while (true)
            {
                antinode = (positionA.x - distanceX * i, positionA.y - distanceY * i);
                if (antinode.x < 0 || antinode.x >= map[0].Count || antinode.y < 0 || antinode.y >= map.Count)
                    break;
                
                uniqueAntinodePositionsPart2.Add(antinode);
                //Console.SetCursorPosition(antinode.x, antinode.y);
                //Console.Write("#");
                
                i++;
            }
        }
    }
}

//Console.SetCursorPosition(0, map.Count + 1);
//Console.WriteLine("Done");
Console.WriteLine("Part 1: " + uniqueAntinodePositionsPart1.Count);
Console.WriteLine("Part 2: " + uniqueAntinodePositionsPart2.Count); //901 too low