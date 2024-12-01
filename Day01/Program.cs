List<int> left = new();
List<int> right = new();

Dictionary<int, int> uniqueCountInRight = new();

foreach (var line in File.ReadLines("Input/Input.txt"))
{
    var split = line.Split("   ");
    var l = int.Parse(split[0]);
    var r = int.Parse(split[1]);
    left.Add(l);
    right.Add(r);
    
    //Add one to the count of the right value
    if (!uniqueCountInRight.TryAdd(r, 1))
        uniqueCountInRight[r]++;
}

left.Sort();
right.Sort();

int sumPart1 = 0;
int similarityScorePart2 = 0;

for (int i = 0; i < left.Count; i++)
{
    //Absolute difference between the two values
    int diff = Math.Abs(left[i] - right[i]);
    sumPart1 += diff;

    //Get number of times left[i] appears in the right list
    int numInRight = 0;
    uniqueCountInRight.TryGetValue(left[i], out numInRight);

    similarityScorePart2 += left[i] * numInRight;
}

Console.WriteLine("Part 1: " + sumPart1);
Console.WriteLine("Part 2: " + similarityScorePart2);