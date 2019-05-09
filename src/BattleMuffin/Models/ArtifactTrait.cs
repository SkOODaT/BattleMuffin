﻿using Newtonsoft.Json;

namespace BattleMuffin.Models
{
    /// <summary>
    ///     An artifact trait.
    /// </summary>
    public class ArtifactTrait : IWarcraftModel
    {
        /// <summary>
        ///     Gets or sets the artifact trait ID.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the rank.
        /// </summary>
        [JsonProperty("rank")]
        public int Rank { get; set; }
    }
}
