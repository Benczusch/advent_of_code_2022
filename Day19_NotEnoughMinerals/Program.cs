using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using Day19_NotEnoughMinerals;

StreamReader streamReader = new StreamReader("input.txt");

bool secondTask = true;

List<Blueprint> blueprints = new List<Blueprint>();
int id = 0;

while (!streamReader.EndOfStream)
{
    string line = streamReader.ReadLine();
    id++;

    var lineSplit = line.Split("costs ");
    Blueprint newBlueprint = new Blueprint() {Id = id};

    string[] subSplit = lineSplit[1].Split(' ');
    newBlueprint.OreRobotPrice = int.Parse(subSplit[0]);

    subSplit = lineSplit[2].Split(' ');
    newBlueprint.ClayRobotPrice = int.Parse(subSplit[0]);

    subSplit = lineSplit[3].Split(' ');
    newBlueprint.ObsidianRobotPrice = (int.Parse(subSplit[0]), int.Parse(subSplit[3]));

    subSplit = lineSplit[4].Split(' ');
    newBlueprint.GeodeRobotPrice = (int.Parse(subSplit[0]), int.Parse(subSplit[3]));

    blueprints.Add(newBlueprint);

    Console.WriteLine($"Blueprint {id} " + (line.Equals(newBlueprint.ToString()) ? "loaded successfully" : "loading ERROR!!!"));
}

int qualitySum = 0;

if (!secondTask)
{
    foreach (Blueprint blueprint in blueprints)
    {
        int yield = RecursiveTargetBasedBestYield(new ResourceManager(), new RobotManager() { OreRobots = 1 }, blueprint, 24, ProductionDecision.Nothing);

        Console.WriteLine($"Blueprint {blueprint.Id} max yield is {yield}");

        qualitySum += yield * blueprint.Id;
    }
    Console.WriteLine();
    Console.WriteLine("-------------------------------------------------------");
    Console.WriteLine($"Sum of qualities is {qualitySum}");
}
else
{
    int qualityProd = 1;
    for (int i = 0; i < 3; i++)
    {
        var blueprint = blueprints[i];
        int yield = RecursiveTargetBasedBestYield(new ResourceManager(), new RobotManager() { OreRobots = 1 }, blueprint, 32, ProductionDecision.Nothing);
        Console.WriteLine($"Blueprint {blueprint.Id} max yield is {yield}");
        qualityProd *= yield;
    }
    Console.WriteLine();
    Console.WriteLine("-------------------------------------------------------");
    Console.WriteLine($"Product of geodes is {qualityProd}");
}


int RecursiveTargetBasedBestYield(ResourceManager resources, RobotManager robots, Blueprint blueprint, int minutesLeft, ProductionDecision nextTarget)
{
    if (minutesLeft <= 0)
    {
        return resources.Geodes;
    }

    var newRobots = new RobotManager(robots);

    switch (nextTarget)
    {
        case ProductionDecision.Nothing:
            break;
        case ProductionDecision.OreRobot:
            if (resources.Ores >= blueprint.OreRobotPrice)
            {
                resources.Ores -= blueprint.OreRobotPrice;
                newRobots.OreRobots++;
                nextTarget = ProductionDecision.Nothing;
            }
            break;
        case ProductionDecision.ClayRobot:
            if (resources.Ores >= blueprint.ClayRobotPrice)
            {
                resources.Ores -= blueprint.ClayRobotPrice;
                newRobots.ClayRobots++;
                nextTarget = ProductionDecision.Nothing;
            }
            break;
        case ProductionDecision.ObsidianRobot:
            if (resources.Ores >= blueprint.ObsidianRobotPrice.Item1 && resources.Clay >= blueprint.ObsidianRobotPrice.Item2)
            {
                resources.Ores -= blueprint.ObsidianRobotPrice.Item1;
                resources.Clay -= blueprint.ObsidianRobotPrice.Item2;
                newRobots.ObsidianRobots++;
                nextTarget = ProductionDecision.Nothing;
            }
            break;
        case ProductionDecision.GeodeRobot:
            if (resources.Ores >= blueprint.GeodeRobotPrice.Item1 && resources.Obsidian >= blueprint.GeodeRobotPrice.Item2)
            {
                resources.Ores -= blueprint.GeodeRobotPrice.Item1;
                resources.Obsidian -= blueprint.GeodeRobotPrice.Item2;
                newRobots.GeodeRobots++;
                nextTarget = ProductionDecision.Nothing;
            }
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(nextTarget), nextTarget, null);
    }

    resources.YieldTurn(robots);
    robots = newRobots;

    if (nextTarget == ProductionDecision.Nothing)
    {
        int maxYield = 0;
        foreach (var decision in new[]{ProductionDecision.OreRobot, ProductionDecision.ClayRobot, ProductionDecision.ObsidianRobot, ProductionDecision.GeodeRobot})
        {
            if (!MakesSense(decision, blueprint, newRobots))
            {
                continue;
            }
            var newResources = new ResourceManager(resources);
            newRobots = new RobotManager(robots);

            maxYield = Math.Max(maxYield,
                RecursiveTargetBasedBestYield(newResources, newRobots, blueprint, minutesLeft - 1, decision));
        }

        return maxYield;
    }

    return RecursiveTargetBasedBestYield(resources, robots, blueprint, minutesLeft - 1, nextTarget);
}

bool MakesSense(ProductionDecision decision, Blueprint blueprint, RobotManager newRobots)
{
    switch (decision)
    {
        case ProductionDecision.OreRobot:
            return Math.Max(blueprint.ObsidianRobotPrice.Item1,
                Math.Max(blueprint.ClayRobotPrice, blueprint.GeodeRobotPrice.Item1)) > newRobots.OreRobots;
            break;
        case ProductionDecision.ClayRobot:
            return blueprint.ObsidianRobotPrice.Item2 > newRobots.ClayRobots;
            break;
        case ProductionDecision.ObsidianRobot:
            return blueprint.GeodeRobotPrice.Item2 > newRobots.ObsidianRobots;
            break;
        default:
            break;
    }
    return true;
}

int RecursiveBestYield(ResourceManager resources, RobotManager robots, Blueprint blueprint, int minutesLeft)
{
    if (minutesLeft <= 0)
    {
        return resources.Geodes;
    }

    List<ProductionDecision> possibleDecisions = new List<ProductionDecision>(5);

    if (resources.Ores >= blueprint.GeodeRobotPrice.Item1 && resources.Obsidian >= blueprint.GeodeRobotPrice.Item2) { possibleDecisions.Add(ProductionDecision.GeodeRobot); }
    else
    {
        possibleDecisions.Add(ProductionDecision.Nothing);
        if (resources.Ores >= blueprint.OreRobotPrice)
        {
            possibleDecisions.Add(ProductionDecision.OreRobot);
        }

        if (resources.Ores >= blueprint.ClayRobotPrice)
        {
            possibleDecisions.Add(ProductionDecision.ClayRobot);
        }

        if (resources.Ores >= blueprint.ObsidianRobotPrice.Item1 &&
            resources.Clay >= blueprint.ObsidianRobotPrice.Item2)
        {
            possibleDecisions.Add(ProductionDecision.ObsidianRobot);
        }
    }

    resources.YieldTurn(robots);

    int bestYield = 0;
    foreach (ProductionDecision decision in possibleDecisions)
    {
        ResourceManager newResources = new ResourceManager(resources);
        RobotManager newRobots = new RobotManager(robots);

        switch (decision)
        {
            case ProductionDecision.Nothing:
                break;
            case ProductionDecision.OreRobot:
                newResources.Ores -= blueprint.OreRobotPrice;
                newRobots.OreRobots++;
                break;
            case ProductionDecision.ClayRobot:
                newResources.Ores -= blueprint.ClayRobotPrice;
                newRobots.ClayRobots++;
                break;
            case ProductionDecision.ObsidianRobot:
                newResources.Ores -= blueprint.ObsidianRobotPrice.Item1;
                newResources.Clay -= blueprint.ObsidianRobotPrice.Item2;
                newRobots.ObsidianRobots++;
                break;
            case ProductionDecision.GeodeRobot:
                newResources.Ores -= blueprint.GeodeRobotPrice.Item1;
                newResources.Obsidian -= blueprint.GeodeRobotPrice.Item2;
                newRobots.GeodeRobots++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        bestYield = Math.Max(bestYield, RecursiveBestYield(newResources, newRobots, blueprint, minutesLeft - 1));
    }
    return bestYield;
}

int UseSimpleStrategy(Blueprint blueprint)
{
    int maxYield = 0;
    int bestOreTarget = 0;
    int bestClayTarget = 0;
    int bestObsidianTarget = 0;

    GeodeYieldCalculator calculator = new GeodeYieldCalculator();
    calculator.Blueprint = blueprint;

    for (int oreTarget = 0; oreTarget < 24; oreTarget++)
    {
        for (int clayTarget = 0; clayTarget < 24; clayTarget++)
        {
            for (int obsidianTarget = 0; obsidianTarget < 24; obsidianTarget++)
            {
                SimpleStrategy strategy = new SimpleStrategy();
                strategy.OreRobotTarget = oreTarget;
                strategy.ClayRobotTarget = clayTarget;
                strategy.ObsidianRobotTarget = obsidianTarget;

                strategy.Blueprint = blueprint;

                calculator.Strategy = strategy;

                int geodes = calculator.CalculateYield();
                if (geodes > maxYield)
                {
                    maxYield = geodes;
                    bestOreTarget = oreTarget;
                    bestClayTarget = clayTarget;
                    bestObsidianTarget = obsidianTarget;
                }
            }
        }
    }

    int quality = blueprint.Id * maxYield;

    Console.WriteLine($"On blueprint {blueprint.Id}, {maxYield} geodes yielded with {bestOreTarget} ore robots, {bestClayTarget} clay robots and {bestObsidianTarget} obsidian robots targeted");
    return quality;
}
enum ProductionDecision
{
    Nothing,
    OreRobot,
    ClayRobot,
    ObsidianRobot,
    GeodeRobot,
}

class Blueprint
{
    public int Id { get; set; }
    public int OreRobotPrice { get; set; }
    public int ClayRobotPrice { get; set; }
    public (int, int) ObsidianRobotPrice { get; set; }
    public (int, int) GeodeRobotPrice { get; set; }

    public override string ToString()
    {
        return $"Blueprint {Id}: Each ore robot costs {OreRobotPrice} ore. Each clay robot costs {ClayRobotPrice} ore. Each obsidian robot costs {ObsidianRobotPrice.Item1} ore and {ObsidianRobotPrice.Item2} clay. Each geode robot costs {GeodeRobotPrice.Item1} ore and {GeodeRobotPrice.Item2} obsidian.";
    }
}

class GeodeYieldCalculator
{
    const int MINUTES = 24;
    public Blueprint Blueprint { get; set; }
    public IStrategy Strategy { get; set; }

    public int CalculateYield()
    {
        ResourceManager resources = new ResourceManager();
        RobotManager robots = new RobotManager() {OreRobots = 1};

        for (int i = 1; i <= MINUTES; i++)
        {
            var robotProduction = Strategy.DecideProduction(i, resources, robots);
            resources.YieldTurn(robots);
            robots.Produce(robotProduction);
        }

        return resources.Geodes;
    }
}

interface IStrategy
{
    abstract RobotManager DecideProduction(int minute, ResourceManager resources, RobotManager robots);
}

class ResourceManager
{
    public int Ores { get; set; } = 0;
    public int Clay { get; set; } = 0;
    public int Obsidian { get; set; } = 0;
    public int Geodes { get; set; } = 0;

    public void YieldTurn(RobotManager robots)
    {
        Ores += robots.OreRobots;
        Clay += robots.ClayRobots;
        Obsidian += robots.ObsidianRobots;
        Geodes += robots.GeodeRobots;
    }

    public ResourceManager()
    {
    }

    public ResourceManager(ResourceManager resourceManager)
    {
        Ores = resourceManager.Ores;
        Clay = resourceManager.Clay;
        Obsidian = resourceManager.Obsidian;
        Geodes = resourceManager.Geodes;
    }
}

class RobotManager
{
    public int OreRobots { get; set; } = 0;
    public int ClayRobots { get; set; } = 0;
    public int ObsidianRobots { get; set; } = 0;
    public int GeodeRobots { get; set; } = 0;

    public void Produce(RobotManager producedRobots)
    {
        OreRobots += producedRobots.OreRobots;
        ClayRobots += producedRobots.ClayRobots;
        ObsidianRobots += producedRobots.ObsidianRobots;
        GeodeRobots += producedRobots.GeodeRobots;
    }

    public RobotManager()
    {

    }

    public RobotManager(RobotManager robotManager)
    {
        OreRobots = robotManager.OreRobots;
        ClayRobots = robotManager.ClayRobots;
        ObsidianRobots = robotManager.ObsidianRobots;
        GeodeRobots = robotManager.GeodeRobots;
    }
}