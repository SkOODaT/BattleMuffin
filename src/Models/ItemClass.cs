﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace BattleMuffin.Models
{
    /// <summary>
    ///     An item class.
    /// </summary>
    public class ItemClass
    {
        /// <summary>
        ///     Gets or sets the item class ID.
        /// </summary>
        [JsonProperty("class")]
        public int Class { get; set; }

        /// <summary>
        ///     Gets or sets the item class name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the item subclasses.
        /// </summary>
        [JsonProperty("subclasses")]
        public IEnumerable<ItemSubclass> Subclasses { get; set; }
    }
}
