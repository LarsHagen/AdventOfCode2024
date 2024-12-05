
List<(int left,int right)> pageOrderingRules = new();
List<List<int>> pageOrderUpdates = new();

foreach(var line in File.ReadAllLines("Input/Input.txt"))
{
    if (string.IsNullOrEmpty(line))
        continue;
    
    if (line.Contains("|"))
        pageOrderingRules.Add((int.Parse(line.Split("|")[0]), int.Parse(line.Split("|")[1])));
    else
        pageOrderUpdates.Add(line.Split(",").Select(int.Parse).ToList());
}

Dictionary<int, (List<int> after, List<int> before)> rulesPerPage = new();

foreach (var pageOrderingRule in pageOrderingRules)
{
    if (!rulesPerPage.ContainsKey(pageOrderingRule.left))
        rulesPerPage[pageOrderingRule.left] = (new(), new());
    if (!rulesPerPage.ContainsKey(pageOrderingRule.right))
        rulesPerPage[pageOrderingRule.right] = (new(), new());
    
    rulesPerPage[pageOrderingRule.left].after.Add(pageOrderingRule.right);
    rulesPerPage[pageOrderingRule.right].before.Add(pageOrderingRule.left);
}

int sum = 0;
List<List<int>> invalidUpdates = new();
foreach (var update in pageOrderUpdates)
{
    bool valid = true;
    for (var i = 0; i < update.Count; i++)
    {
        var page = update[i];
        var afterList = rulesPerPage[page].after;
        
        for (int j = 0; j < i; j++)
        {
            var before = update[j];
            if (afterList.Contains(before))
            {
                valid = false;
                break;
            }
        }
        if (!valid)
            break;
    }
    
    if (valid)
    {
        //Get middle number in update
        var middle = update[update.Count / 2];
        sum += middle;
    }
    else
    {
        invalidUpdates.Add(update);
    }
}

Console.WriteLine("Part 1: " + sum);

sum = 0;
foreach (var update in invalidUpdates)
{
    for (var i = 0; i < update.Count; i++)
    {
        var page = update[i];
        var afterList = rulesPerPage[page].after;
        
        for (int j = 0; j < i; j++)
        {
            var before = update[j];
            if (afterList.Contains(before))
            {
                update.RemoveAt(i);
                update.Insert(j, page);
                i = 0;
                break;
            }
        }
    }
    
    var middle = update[update.Count / 2];
    sum += middle;
}

Console.WriteLine("Part 2: " + sum);