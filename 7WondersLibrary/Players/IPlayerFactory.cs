namespace _7Wonders
{
    public interface IPlayerFactory
    {
        Player CreatePlayer(int index, Tableau tableau);
    }
}
