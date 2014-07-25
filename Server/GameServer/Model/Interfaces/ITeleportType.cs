namespace GameServer.Model.Interfaces
{
    public interface ITeleportType
    {
        Position GetNearestTeleportLocation(ICharacter character);
    }
}