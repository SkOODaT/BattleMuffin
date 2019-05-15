using Newtonsoft.Json;

namespace BattleMuffin.Models.Warcraft.Community
{
    /// <summary>
    ///     A title.
    /// </summary>
    public class Title : IWarcraftModel
    {
        /// <summary>
        ///     Gets or sets the title ID.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the title name.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the title is selected.
        /// </summary>
        [JsonProperty("selected")]
        public bool? Selected { get; set; }
    }
}
