namespace MayhemCommon
{
    public enum MessageSubCode
    {
        // Login Server Code
        Register,
        Login,
        ListCharacters,
        SelectCharacter,
        CreateCharacter,
        ZoneUnlock,
        ZoneItemUpdate,
        Alert, // Might be general

        // Chat Server Code
        Chat,

        // Game Server Code
        ActionRequest,
        Attack,
        CharacterDeath,
        CharacterLoading,
        CharInfo,
        CoinsTransfer,
        ContainerSetItem,
        DeleteObjects,
        DeathNotification,
        DialogAction,
        DialogPage,
        EventNotification,
        EquipItem,
        FinishLoading,
        GlobalStorageData,
        ItemTransfer,
        LeaveZone,
        LoadZone,
        LootContainer,
        LootContainerItemRemove,
        MoveTo,
        ObjectHint,
        ObjectNotification,
        PlayerInGame,
        PlayerInit,
        PlayerStatus,
        PositionUpdate,
        ShowObjects,
        StatusUpdate,
        StopMove,
        TakeItem,
        Target,
        TargetRequest,
        TargetStatus,
        TeleportToLocation,
        UserInfo,
        QuestAreaTrigger
    }
}