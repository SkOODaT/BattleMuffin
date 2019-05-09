﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BattleMuffin.Models
{
    /// <summary>
    ///     An activity feed item.
    /// </summary>
    public class Feed : IWarcraftModel
    {
        /// <summary>
        ///     Gets or sets the feed type.
        /// </summary>
        [JsonProperty("type")]
        public string? Type { get; set; }

        /// <summary>
        ///     Gets or sets the timestamp.
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        ///     Gets or sets the item ID.
        /// </summary>
        [JsonProperty("itemId")]
        public int ItemId { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        [JsonProperty("context")]
        public string? Context { get; set; }

        /// <summary>
        ///     Gets or sets the bonus lists.
        /// </summary>
        [JsonProperty("bonusLists")]
        public IEnumerable<int>? BonusLists { get; set; }

        /// <summary>
        ///     Gets or sets the achievement.
        /// </summary>
        [JsonProperty("achievement")]
        public Achievement? Achievement { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether it is a feat of strength.
        /// </summary>
        [JsonProperty("featOfStrength")]
        public bool? FeatOfStrength { get; set; }

        /// <summary>
        ///     Gets or sets the criteria.
        /// </summary>
        [JsonProperty("criteria")]
        public Criterion? Criteria { get; set; }

        /// <summary>
        ///     Gets or sets the quantity.
        /// </summary>
        [JsonProperty("quantity")]
        public int? Quantity { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }
    }
}
