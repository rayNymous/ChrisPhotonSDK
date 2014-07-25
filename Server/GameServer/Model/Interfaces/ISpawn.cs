namespace GameServer.Model.Interfaces
{
    public interface ISpawn
    {
        void Start();
        void OnCharacterDeath(ICharacter victim, ICharacter killer);
    }
}