using Newtonsoft.Json;

namespace BattleMuffin.Models.Warcraft.Community
{
    /// <summary>
    ///     A pet ability.
    /// </summary>
    public class PetAbility : IWarcraftModel
    {
        /// <summary>
        ///     Gets or sets the pet ability ID.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the pet ability name.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        ///     Gets or sets the icon.
        /// </summary>
        [JsonProperty("icon")]
        public string? Icon { get; set; }

        /// <summary>
        ///     Gets or sets the cooldown.
        /// </summary>
        [JsonProperty("cooldown")]
        public int Cooldown { get; set; }

        /// <summary>
        ///     Gets or sets the number of rounds.
        /// </summary>
        [JsonProperty("rounds")]
        public int Rounds { get; set; }

        /// <summary>
        ///     Gets or sets the pet type ID.
        /// </summary>
        [JsonProperty("petTypeId")]
        public int PetTypeId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the ability is passive.
        /// </summary>
        [JsonProperty("isPassive")]
        public bool IsPassive { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether hints are hidden.
        /// </summary>
        [JsonProperty("hideHints")]
        public bool HideHints { get; set; }
    }
}
