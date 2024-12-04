

List<List<char>> grid = new();

foreach (var line in File.ReadAllLines("Input/Input.txt"))
{
    grid.Add(line.ToCharArray().ToList());
}

int width = grid[0].Count;
int height = grid.Count;
int xmasCount = 0;

for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        if (grid[y][x] != 'X')
        {
            continue;
        }
        
        var up = GetGridString(x, y, 0, -1, 4);
        var down = GetGridString(x, y, 0, 1, 4);
        var left = GetGridString(x, y, -1, 0, 4);
        var right = GetGridString(x, y, 1, 0, 4);
        var upLeft = GetGridString(x, y, -1, -1, 4);
        var upRight = GetGridString(x, y, 1, -1, 4);
        var downLeft = GetGridString(x, y, -1, 1, 4);
        var downRight = GetGridString(x, y, 1, 1, 4);

        if (up == "XMAS")
            xmasCount++;
        if (down == "XMAS")
            xmasCount++;
        if (left == "XMAS")
            xmasCount++;
        if (right == "XMAS")
            xmasCount++;
        if (upLeft == "XMAS")
            xmasCount++;
        if (upRight == "XMAS")
            xmasCount++;
        if (downLeft == "XMAS")
            xmasCount++;
        if (downRight == "XMAS")
            xmasCount++;
    }
}

Console.WriteLine("Part 1: " + xmasCount);

var masCount = 0;

for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        if (grid[y][x] != 'A')
        {
            continue;
        }

        //This direction: \
        var rightDown = GetGridString(x - 1, y - 1, 1, 1, 3);
        var leftUp = GetGridString(x + 1, y + 1, -1, -1, 3);
        
        //This direction: /
        var rightUp = GetGridString(x - 1, y + 1, 1, -1, 3);
        var leftDown = GetGridString(x + 1, y - 1, -1, 1, 3);

        if ((rightDown == "MAS" || leftUp == "MAS") &&
            (rightUp == "MAS" || leftDown == "MAS"))
            masCount++;
    }
}

Console.WriteLine("Part 2: " + masCount);



string GetGridString(int startX, int startY, int dirX, int dirY, int length)
{
    int x = startX;
    int y = startY;
    string result = "";

    for (int i = 0; i < length; i++)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return result;
        }

        result += grid[y][x];
        
        x += dirX;
        y += dirY;
    }

    return result;
}