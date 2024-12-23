using Day23;

List<string[]> connections = File.ReadAllLines("Input/Input.txt").Select(l => l.Split('-')).ToList();

Dictionary<string, ComputerNode> computerNodes = new();

foreach (var connection in connections)
{
    var left = connection[0];
    var right = connection[1];
    
    if (!computerNodes.ContainsKey(left))
    {
        computerNodes[left] = new ComputerNode { Name = left, ConnectedNodes = new() };
    }

    if (!computerNodes.ContainsKey(right))
    {
        computerNodes[right] = new ComputerNode { Name = right, ConnectedNodes = new() };
    }
    
    computerNodes[left].ConnectedNodes.Add(computerNodes[right]);
    computerNodes[right].ConnectedNodes.Add(computerNodes[left]);
}

Dictionary<string, HashSet<ComputerNode>> groups = new();
foreach (var computerNodeA in computerNodes.Values)
{
    foreach (var computerNodeB in computerNodeA.ConnectedNodes)
    {
        foreach (var computerNodeC in computerNodeB.ConnectedNodes)
        {
            if (computerNodeC.ConnectedNodes.Contains(computerNodeA))
            {
                HashSet<ComputerNode> group = new();
                group.Add(computerNodeA);
                group.Add(computerNodeB);
                group.Add(computerNodeC);
                string key = string.Join(",", group.OrderBy(c => c.Name).Select(g => g.Name));
                if (groups.ContainsKey(key))
                    continue;
                groups.Add(key, group);
            }
        }
    }
}

int count = 0;
foreach (var group in groups)
{
    if (!group.Value.Any(c => c.Name.StartsWith('t')))
        continue;
    
    //Console.WriteLine(string.Join(",", group.Value.Select(g => g.Name)));
    count++;
}

Console.WriteLine("Part 1: " + count);

//Expand the groups
foreach (var group in groups)
{
    bool groupExpanded = true;
    while (groupExpanded)
    {
        groupExpanded = false;
        var firstComputerNode = group.Value.First();
        
        foreach (var computerNode in firstComputerNode.ConnectedNodes)
        {
            if (group.Value.Contains(computerNode))
                continue;
            
            //Check if all other nodes in group as also connected to this node
            bool allConnected = true;
            foreach (var node in group.Value)
            {
                if (!computerNode.ConnectedNodes.Contains(node))
                {
                    allConnected = false;
                    break;
                }
            }
            
            if (allConnected)
            {
                group.Value.Add(computerNode);
                groupExpanded = true;
            }
        }
    }
}

HashSet<ComputerNode> lastGroup = new();
foreach (var group in groups)
{
    if (group.Value.Count > lastGroup.Count)
    {
        lastGroup = group.Value;
    }
}

//Sort group alphabetically
var sortedGroup = lastGroup.OrderBy(g => g.Name).ToList();
var sortedGroupNames = string.Join(",", sortedGroup.Select(g => g.Name));
Console.WriteLine("Part 2: " + sortedGroupNames);