List<List<char>> map = new();

foreach(var line in File.ReadAllLines("Input/Input.txt"))
{
    map.Add(line.ToList());
}

var mapHeight = map.Count;
var mapWidth = map[0].Count;
Dictionary<(int x, int y), Region> regionLookup = new();

for (int y = 0; y < mapHeight; y++)
{
    for (int x = 0; x < mapWidth; x++)
    {
        regionLookup.Add((x, y), null);
    }
}

for (int y = 0; y < mapHeight; y++)
{
    for (int x = 0; x < mapWidth; x++)
    {
        //Find neighbor up and back
        var neighborUp = map.ElementAtOrDefault(y - 1)?.ElementAtOrDefault(x);
        var neighborBack = map.ElementAtOrDefault(y)?.ElementAtOrDefault(x - 1);

        Region currentRegion = null;
        
        if (neighborUp != null && neighborUp == map[y][x])
        {
            var neighborUpRegion = regionLookup[(x, y - 1)];
            neighborUpRegion.GardenPlots.Add((x,y));
            currentRegion = neighborUpRegion;
            regionLookup[(x, y)] = currentRegion;
        }
        
        if (neighborBack != null && neighborBack == map[y][x])
        {
            var neighborBackRegion = regionLookup[(x - 1, y)];
            if (currentRegion == null)
            {
                currentRegion = neighborBackRegion;
                currentRegion.GardenPlots.Add((x,y));
                regionLookup[(x, y)] = currentRegion;
            }
            else if (neighborBackRegion != currentRegion)
            {
                //Merge regions
                currentRegion.GardenPlots.AddRange(neighborBackRegion.GardenPlots);
                foreach (var gardenPlot in neighborBackRegion.GardenPlots)
                {
                    regionLookup[gardenPlot] = currentRegion;
                }
            }
        }
        
        if (currentRegion == null)
        {
            currentRegion = new Region();
            currentRegion.GardenPlots.Add((x, y));
            regionLookup[(x, y)] = currentRegion;
        }
    }
}

var uniqueRegions = regionLookup.Values.Distinct();

int totalCost = 0;

foreach (var uniqueRegion in uniqueRegions)
{
    var firstPlotInRegion = uniqueRegion.GardenPlots.First();
    var regionChar = map[firstPlotInRegion.y][firstPlotInRegion.x];

    int exposedSides = 0;
    
    foreach (var gardenPlotInRegion in uniqueRegion.GardenPlots)
    {
        if (map.ElementAtOrDefault(gardenPlotInRegion.y - 1)?.ElementAtOrDefault(gardenPlotInRegion.x) != regionChar)
            exposedSides++;

        if (map.ElementAtOrDefault(gardenPlotInRegion.y + 1)?.ElementAtOrDefault(gardenPlotInRegion.x) != regionChar)
            exposedSides++;

        if (map.ElementAtOrDefault(gardenPlotInRegion.y)?.ElementAtOrDefault(gardenPlotInRegion.x - 1) != regionChar)
            exposedSides++;

        if (map.ElementAtOrDefault(gardenPlotInRegion.y)?.ElementAtOrDefault(gardenPlotInRegion.x + 1) != regionChar)
            exposedSides++;
        
    }
    
    totalCost += uniqueRegion.GardenPlots.Count * exposedSides;
}

Console.WriteLine("Part 1: " + totalCost);

int discountTotalCost = 0;

foreach (var uniqueRegion in uniqueRegions)
{
    var firstPlotInRegion = uniqueRegion.GardenPlots.First();
    var regionChar = map[firstPlotInRegion.y][firstPlotInRegion.x];

    int countMergedSidesTop = 0;
    bool previousTopHadFence = false;
    
    int countMergedSidesBottom = 0;
    bool previousBottomHadFence = false;
    
    //Scan left to right for top and bottom fences
    for (int y = 0; y < mapHeight; y++)
    {
        for (int x = 0; x < mapWidth; x++)
        {
            var scannedRegion = regionLookup[(x, y)];
            if (scannedRegion != uniqueRegion)
            {
                previousTopHadFence = false;
                previousBottomHadFence = false;
                continue;
            }
            
            //Check if top is exposed
            if (map.ElementAtOrDefault(y - 1)?.ElementAtOrDefault(x) != regionChar)
            {
                if (!previousTopHadFence) //New fence
                    countMergedSidesTop++;
                
                previousTopHadFence = true;
            }
            else
            {
                previousTopHadFence = false;
            }

            //Check if bottom is exposed
            if (map.ElementAtOrDefault(y + 1)?.ElementAtOrDefault(x) != regionChar)
            {
                if (!previousBottomHadFence) //New fence
                    countMergedSidesBottom++;
                
                previousBottomHadFence = true;
            }
            else
            {
                previousBottomHadFence = false;
            }
        }
    }
    
    int countMergedSidesLeft = 0;
    bool previousLeftHadFence = false;
    
    int countMergedSidesRight = 0;
    bool previousRightHadFence = false;
    
    //Scan top to bottom for left and right fences
    for (int x = 0; x < mapWidth; x++)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            var scannedRegion = regionLookup[(x, y)];
            if (scannedRegion != uniqueRegion)
            {
                previousRightHadFence = false;
                previousLeftHadFence = false;
                continue;
            }

            //Check if left is exposed
            if (map.ElementAtOrDefault(y)?.ElementAtOrDefault(x - 1) != regionChar)
            {
                if (!previousLeftHadFence) //New fence
                    countMergedSidesLeft++;
                
                previousLeftHadFence = true;
            }
            else
            {
                previousLeftHadFence = false;
            }

            //Check if right is exposed
            if (map.ElementAtOrDefault(y)?.ElementAtOrDefault(x + 1) != regionChar)
            {
                if (!previousRightHadFence) //New fence
                    countMergedSidesRight++;
                
                previousRightHadFence = true;
            }
            else
            {
                previousRightHadFence = false;
            }
        }
    }
    
    var totalFenceCount = countMergedSidesTop + countMergedSidesBottom + countMergedSidesLeft + countMergedSidesRight;
    discountTotalCost += uniqueRegion.GardenPlots.Count * totalFenceCount;
}

Console.WriteLine("Part 2: " + discountTotalCost);


class Region
{
    public List<(int x, int y)> GardenPlots = new();
}
