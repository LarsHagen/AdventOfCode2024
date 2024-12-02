Console.WriteLine("Day 02");

int validReports = 0;
int validReportsPart2 = 0;

foreach (var report in File.ReadAllLines("Input/Input.txt"))
{
    var parsedReport = report.Split(" ").Select(int.Parse).ToList();
    
    if (ValidReport(parsedReport))
    {
        validReports++;
        validReportsPart2++;
    }
    else
    {
        //For part two, if a report failed try same report but with one value skipped each time
        for (int i = 0; i < parsedReport.Count; i++)
        {
            if (ValidReport(parsedReport, i))
            {
                validReportsPart2++;
                break;
            }
        }
        
    }

}

Console.WriteLine("Part 1: " + validReports);
Console.WriteLine("Part 2: " + validReportsPart2);

bool ValidReport(IList<int> report, int skip = -1)
{
    int? previousLevel = null;
    bool? increase = null;

    for (int i = 0; i < report.Count; i++)
    {
        if (i == skip)
        {
            continue;
        }
        
        var level = report[i];
        
        //We have no previous level to compare to, so it's fine
        if (previousLevel == null)
        {
            previousLevel = level;
            continue;
        }
        
        
        //Make sure difference is between 1 and 3
        if (level == previousLevel.Value)
            return false;
        if (Math.Abs(level - previousLevel.Value) > 3)
            return false;

        
        if (increase == null) //We don't yet know if we are increasing or decreasing
            increase = level > previousLevel;
        else if (increase.Value != level > previousLevel) //Check that we are always increasing or decreasing within the same report
            return false;
        
        
        previousLevel = level;
    }
    
    //Valid report
    return true;
}