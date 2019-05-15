using Newtonsoft.Json;

namespace BattleMuffin.Models.Warcraft.Community
{
    /// <summary>
    ///     A set bonus.
    /// </summary>
    public class SetBonus : IWarcraftModel
    {
        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        [JsonProperty("description")]
        public string? Description { get; set; }

        /// <summary>
        ///     Gets or sets the minimum number of set items that must be equipped to receive the set bonus.
        /// </summary>
        [JsonProperty("threshold")]
        public int Threshold { get; set; }
    }
}
