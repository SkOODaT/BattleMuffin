using Newtonsoft.Json;

namespace BattleMuffin.Models
{
    /// <summary>
    ///     A member of a challenge mode group.
    /// </summary>
    public class GroupMember : IWarcraftModel
    {
        /// <summary>
        ///     Gets or sets the character.
        /// </summary>
        [JsonProperty("character")]
        public GroupCharacter? Character { get; set; }

        /// <summary>
        ///     Gets or sets the character spec for the challenge mode run.
        /// </summary>
        [JsonProperty("spec")]
        public Spec? Spec { get; set; }
    }
}
