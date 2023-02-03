namespace Day19_NotEnoughMinerals
{
    class SimpleStrategy : IStrategy
    {
        public int OreRobotTarget { get; set; }
        public int ClayRobotTarget { get; set; }
        public int ObsidianRobotTarget { get; set; }
        public Blueprint Blueprint { get; set; }

        public RobotManager DecideProduction(int minute, ResourceManager resources, RobotManager robots)
        {
            RobotManager robotOrder = new RobotManager();
            //bool stop = false;

            //while (!stop)
            //{
            if (Blueprint.GeodeRobotPrice.Item1 <= resources.Ores && Blueprint.GeodeRobotPrice.Item2 <= resources.Obsidian)
            {
                robotOrder.GeodeRobots++;
                resources.Ores -= Blueprint.GeodeRobotPrice.Item1;
                resources.Obsidian -= Blueprint.GeodeRobotPrice.Item2;
            }
            else if (OreRobotTarget > robots.OreRobots && Blueprint.OreRobotPrice <= resources.Ores)
            {
                robotOrder.OreRobots++;
                resources.Ores -= Blueprint.OreRobotPrice;
            }
            else if (ClayRobotTarget > robots.ClayRobots && Blueprint.ClayRobotPrice <= resources.Ores)
            {
                robotOrder.ClayRobots++;
                resources.Ores -= Blueprint.ClayRobotPrice;
            }
            else if (ObsidianRobotTarget > robots.ObsidianRobots && Blueprint.ObsidianRobotPrice.Item1 <= resources.Ores && Blueprint.ObsidianRobotPrice.Item2 <= resources.Clay)
            {
                robotOrder.ObsidianRobots++;
                resources.Ores -= Blueprint.ObsidianRobotPrice.Item1;
                resources.Clay -= Blueprint.ObsidianRobotPrice.Item2;
            }
            //else
            //{
            //    stop = true;
            //}
            //}

            return robotOrder;
        }
    }
}
