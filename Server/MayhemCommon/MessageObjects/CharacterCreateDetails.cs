using System;

namespace MayhemCommon.MessageObjects
{
    [Serializable]
    public class CharacterCreateDetails
    {
        public string CharacterName { get; set; }
        public string Sex { get; set; }
        public string CharacterClass { get; set; }
    }
}