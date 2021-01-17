namespace _7Wonders
{
    public static class PlayerFactory
    {
        public static Player CreatePlayer(PlayerType playerType, string name, Tableau tableau)
        {
            switch (playerType)
            {
                case PlayerType.Robot:
                    return new RobotPlayer("Robot " + name, tableau);

                case PlayerType.Console:
                    return new ConsolePlayer("Human " +name, tableau);

                default:
                    return new DefaultPlayer(name, tableau);
            }
        }
    }
}