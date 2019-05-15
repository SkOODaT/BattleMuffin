using System.Collections.Generic;
using Newtonsoft.Json;

namespace BattleMuffin.Models.Warcraft.Community
{
    /// <summary>
    ///     The talents for a spec.
    /// </summary>
    public class SpecTalents : IWarcraftModel
    {
        /// <summary>
        ///     Gets or sets a value indicating whether this spec is selected.
        /// </summary>
        [JsonProperty("selected")]
        public bool Selected { get; set; }

        /// <summary>
        ///     Gets or sets the talents.
        /// </summary>
        [JsonProperty("talents")]
        public IEnumerable<Talent>? Talents { get; set; }

        /// <summary>
        ///     Gets or sets the spec.
        /// </summary>
        [JsonProperty("spec")]
        public Spec? Spec { get; set; }

        /// <summary>
        ///     Gets or sets the calculated talent.
        /// </summary>
        [JsonProperty("calcTalent")]
        public string? CalcTalent { get; set; }

        /// <summary>
        ///     Gets or sets the calculated spec.
        /// </summary>
        [JsonProperty("calcSpec")]
        public string? CalcSpec { get; set; }
    }
}
