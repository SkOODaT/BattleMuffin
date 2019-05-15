using Newtonsoft.Json;

namespace BattleMuffin.Models.Warcraft.Community
{
    /// <summary>
    ///     Reputation.
    /// </summary>
    public class Reputation : IWarcraftModel
    {
        /// <summary>
        ///     Gets or sets the reputation ID.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the reputation name.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        ///     Gets or sets the standing.
        /// </summary>
        [JsonProperty("standing")]
        public int Standing { get; set; }

        /// <summary>
        ///     Gets or sets the reputation value.
        /// </summary>
        [JsonProperty("value")]
        public int Value { get; set; }

        /// <summary>
        ///     Gets or sets the maximum value.
        /// </summary>
        [JsonProperty("max")]
        public int Max { get; set; }
    }
}
