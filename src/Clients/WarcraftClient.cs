using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BattleMuffin.Auth;
using BattleMuffin.Enums;
using BattleMuffin.Extensions;
using BattleMuffin.Models;
using BattleMuffin.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BattleMuffin.Clients
{
    /// <summary>
    ///     A client for the World of Warcraft Community APIs.
    /// </summary>
    public class WarcraftClient : IWarcraftClient
    {
        private readonly HttpClient _client;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly Region _region;
        private readonly Locale _locale;
        private readonly string _host;

        private OAuthAccessToken _token;
        private DateTime _tokenExpiration;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WarcraftClient"/> class.
        /// </summary>
        /// <param name="clientId">The Blizzard OAuth client ID.</param>
        /// <param name="clientSecret">The Blizzard OAuth client secret.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">
        ///     Specifies the language that the result will be in. Visit
        ///     https://dev.battle.net/docs/read/community_apis to see a list of available locales.
        /// </param>
        public WarcraftClient(string clientId, string clientSecret, Region region = Region.US, Locale locale = Locale.en_US) : this(clientId, clientSecret, region, locale, InternalHttpClient.Instance)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WarcraftClient"/> class.
        /// </summary>
        /// <param name="clientId">The Blizzard OAuth client ID.</param>
        /// <param name="clientSecret">The Blizzard OAuth client secret.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">
        ///     Specifies the language that the result will be in. Visit
        ///     https://dev.battle.net/docs/read/community_apis to see a list of available locales.
        /// </param>
        /// <param name="client">The <see cref="HttpClient"/> that communicates with Blizzard.</param>
        public WarcraftClient(string clientId, string clientSecret, Region region, Locale locale, HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            _clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));

            if (!ValidateRegionLocale(locale, region))
            {
                throw new ArgumentException("The locale selected is not supported by the selected region.");
            }

            _region = region;
            _locale = locale;
            _host = GetHost(region);
        }

        /// <summary>
        ///     Get the specified achievement.
        /// </summary>
        /// <param name="id">The achievement ID.</param>
        /// <returns>
        ///     The specified achievement.
        /// </returns>
        public async Task<RequestResult<Achievement>> GetAchievementAsync(int id)
        {
            return await GetAchievementAsync(id, _region, _locale);
        }

        /// <summary>
        ///     Get the specified achievement.
        /// </summary>
        /// <param name="id">The achievement ID.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The specified achievement.
        /// </returns>
        public async Task<RequestResult<Achievement>> GetAchievementAsync(int id, Region region, Locale locale)
        {
            var host = GetHost(region);
            return await Get<Achievement>(region, $"{host}/wow/achievement/{id}?locale={locale}");
        }

        /// <summary>
        ///     Get the specified auction.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <returns>
        ///     The specified auction.
        /// </returns>
        public async Task<RequestResult<AuctionFiles>> GetAuctionAsync(string realm)
        {
            return await GetAuctionAsync(realm, _region, _locale);
        }

        /// <summary>
        ///     Get the specified auction.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The specified auction.
        /// </returns>
        public async Task<RequestResult<AuctionFiles>> GetAuctionAsync(string realm, Region region, Locale locale)
        {
            var host = GetHost(region);
            return await Get<AuctionFiles>(region, $"{host}/wow/auction/data/{realm}?locale={locale}");
        }

        /// <summary>
        ///     Get the auction house snapshot from the specified file.
        /// </summary>
        /// <param name="url">The URL for the auction house file.</param>
        /// <returns>
        ///     The auction house snapshot from the specified file.
        /// </returns>
        public async Task<RequestResult<AuctionHouseSnapshot>> GetAuctionHouseSnapshotAsync(string url)
        {
            // TODO: Need to extract the region from the URL or add it to the method signature.
            return await Get<AuctionHouseSnapshot>(Region.US, url);
        }

        /// <summary>
        ///     Get a list of all supported battlegroups.
        /// </summary>
        /// <returns>
        ///     A list of all supported battlegroups.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Battlegroup>>> GetBattlegroupsAsync()
        {
            return await GetBattlegroupsAsync(_region, _locale);
        }

        /// <summary>
        ///     Get a list of all supported battlegroups.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     A list of all supported battlegroups.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Battlegroup>>> GetBattlegroupsAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var battlegroupList = await Get<IEnumerable<Battlegroup>>(region, $"{host}/wow/data/battlegroups/?locale={locale}", "battlegroups");

            return battlegroupList;
        }

        /// <summary>
        ///     Get the specified boss.
        /// </summary>
        /// <remarks>
        ///     A "boss" in this context should be considered a boss encounter, which may include more than one NPC.
        /// </remarks>
        /// <param name="id">The boss ID.</param>
        /// <returns>
        ///     The specified boss.
        /// </returns>
        public async Task<RequestResult<Boss>> GetBossAsync(int id)
        {
            return await GetBossAsync(id, _region, _locale);
        }

        /// <summary>
        ///     Get the specified boss.
        /// </summary>
        /// <remarks>
        ///     A "boss" in this context should be considered a boss encounter, which may include more than one NPC.
        /// </remarks>
        /// <param name="id">The boss ID.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The specified boss.
        /// </returns>
        public async Task<RequestResult<Boss>> GetBossAsync(int id, Region region, Locale locale)
        {
            var host = GetHost(region);
            return await Get<Boss>(region, $"{host}/wow/boss/{id}?locale={locale}");
        }

        /// <summary>
        ///     Get a list of all supported bosses.
        /// </summary>
        /// <remarks>
        ///     A "boss" in this context should be considered a boss encounter, which may include more than one NPC.
        /// </remarks>
        /// <returns>
        ///     A list of all supported bosses.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Boss>>> GetBossesAsync()
        {
            return await GetBossesAsync(_region, _locale);
        }

        /// <summary>
        ///     Get a list of all supported bosses.
        /// </summary>
        /// <remarks>
        ///     A "boss" in this context should be considered a boss encounter, which may include more than one NPC.
        /// </remarks>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     A list of all supported bosses.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Boss>>> GetBossesAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var bossList = await Get<IEnumerable<Boss>>(region, $"{host}/wow/boss/?locale={locale}", "bosses");
            return bossList;
        }

        /// <summary>
        ///     Get the challenge mode data for the entire region.
        /// </summary>
        /// <returns>
        ///     The challenge mode data for the entire region.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Challenge>>> GetChallengesAsync()
        {
            return await GetChallengesAsync(_region, _locale);
        }

        /// <summary>
        ///     Get the challenge mode data for the entire region.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The challenge mode data for the entire region.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Challenge>>> GetChallengesAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var challengeList = await Get<IEnumerable<Challenge>>(region, $"{host}/wow/challenge/region?locale={locale}", "challenge");
            return challengeList;
        }

        /// <summary>
        ///     Get the challenge mode data for the specified realm.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <returns>
        ///     The challenge mode data for the specified realm.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Challenge>>> GetChallengesAsync(string realm)
        {
            return await GetChallengesAsync(realm, _region, _locale);
        }

        /// <summary>
        ///     Get the challenge mode data for the specified realm.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The challenge mode data for the specified realm.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Challenge>>> GetChallengesAsync(string realm, Region region, Locale locale)
        {
            var host = GetHost(region);
            var challengeList = await Get<IEnumerable<Challenge>>(region, $"{host}/wow/challenge/{realm}?locale={locale}", "challenge");
            return challengeList;
        }

        /// <summary>
        ///     Get the specified character.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <param name="characterName">The character name.</param>
        /// <param name="fields">The character fields to include.</param>
        /// <returns>
        ///     The specified character.
        /// </returns>>
        public async Task<RequestResult<Character>> GetCharacterAsync(string realm, string characterName, CharacterFields fields = CharacterFields.None)
        {
            return await GetCharacterAsync(realm, characterName, _region, _locale, fields);
        }

        /// <summary>
        ///     Get the specified character.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <param name="characterName">The character name.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <param name="fields">The character fields to include.</param>
        /// <returns>
        ///     The specified character.
        /// </returns>
        public async Task<RequestResult<Character>> GetCharacterAsync(string realm, string characterName, Region region, Locale locale, CharacterFields fields = CharacterFields.None)
        {
            var host = GetHost(region);
            var queryStringFields = fields.BuildQueryString();
            return await Get<Character>(region, $"{host}/wow/character/{realm}/{characterName}?&locale={locale}{queryStringFields}");
        }

        /// <summary>
        ///     Get a list of all of the achievements that characters can earn as well as the category structure and hierarchy.
        /// </summary>
        /// <returns>
        ///     A list of all of the achievements that characters can earn as well as the category structure and hierarchy.
        /// </returns>
        public async Task<RequestResult<IEnumerable<AchievementCategory>>> GetCharacterAchievementsAsync()
        {
            return await GetCharacterAchievementsAsync(_region, _locale);
        }

        /// <summary>
        ///     Get a list of all of the achievements that characters can earn as well as the category structure and hierarchy.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     A list of all of the achievements that characters can earn as well as the category structure and hierarchy.
        /// </returns>
        public async Task<RequestResult<IEnumerable<AchievementCategory>>> GetCharacterAchievementsAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var achievementList = await Get<IEnumerable<AchievementCategory>>(region, $"{host}/wow/data/character/achievements?locale={locale}", "achievements");
            return achievementList;
        }

        /// <summary>
        ///     Get a list of all supported character classes.
        /// </summary>
        /// <returns>
        ///     A list of all supported character classes.
        /// </returns>
        public async Task<RequestResult<IEnumerable<CharacterClassData>>> GetCharacterClassesAsync()
        {
            return await GetCharacterClassesAsync(_region, _locale);
        }

        /// <summary>
        /// Get a list of all supported character classes.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        /// A list of all supported character classes.
        /// </returns>
        public async Task<RequestResult<IEnumerable<CharacterClassData>>> GetCharacterClassesAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var characterClassList = await Get<IEnumerable<CharacterClassData>>(region, $"{host}/wow/data/character/classes?locale={locale}", "classes");
            return characterClassList;
        }

        /// <summary>
        ///     Get a list of all supported character races.
        /// </summary>
        /// <returns>
        ///     A list of all supported character races.
        /// </returns>
        public async Task<RequestResult<IEnumerable<CharacterRace>>> GetCharacterRacesAsync()
        {
            return await GetCharacterRacesAsync(_region, _locale);
        }

        /// <summary>
        ///     Get a list of all supported character races.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     A list of all supported character races.
        /// </returns>
        public async Task<RequestResult<IEnumerable<CharacterRace>>> GetCharacterRacesAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var characterRaceList = await Get<IEnumerable<CharacterRace>>(region, $"{host}/wow/data/character/races?locale={locale}", "races");
            return characterRaceList;
        }

        /// <summary>
        ///     Get the characters for a user account.
        /// </summary>
        /// <param name="accessToken">An OAuth access token for the user.</param>
        /// <returns>
        ///     The characters for a user account.
        /// </returns>
        public async Task<RequestResult<IEnumerable<GuildCharacter>>> GetCharactersAsync(string accessToken)
        {
            return await GetCharactersAsync(accessToken, _region);
        }

        /// <summary>
        ///     Get the characters for a user account.
        /// </summary>
        /// <param name="accessToken">An OAuth access token for the user.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <returns>
        ///     The characters for a user account.
        /// </returns>
        public async Task<RequestResult<IEnumerable<GuildCharacter>>> GetCharactersAsync(string accessToken, Region region)
        {
            var host = GetHost(region);
            var characters = await Get<IEnumerable<GuildCharacter>>(region, $"{host}/wow/user/characters?access_token={accessToken}", "characters");
            return characters;
        }

        /// <summary>
        ///     Get the specified guild.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <param name="guildName">The guild name.</param>
        /// <param name="fields">The guild fields to include.</param>
        /// <returns>
        ///     The specified guild.
        /// </returns>
        public async Task<RequestResult<Guild>> GetGuildAsync(string realm, string guildName, GuildFields fields = GuildFields.None)
        {
            return await GetGuildAsync(realm, guildName, _region, _locale, fields);
        }

        /// <summary>
        ///     Get the specified guild.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <param name="guildName">The guild name.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <param name="fields">The guild fields to include.</param>
        /// <returns>
        ///     The specified guild.
        /// </returns>
        public async Task<RequestResult<Guild>> GetGuildAsync(string realm, string guildName, Region region, Locale locale, GuildFields fields = GuildFields.None)
        {
            var host = GetHost(region);
            var queryStringFields = fields.BuildQueryString();
            return await Get<Guild>(region, $"{host}/wow/guild/{realm}/{Uri.EscapeUriString(guildName)}?locale={locale}{queryStringFields}");
        }

        /// <summary>
        ///     Get a list of all guild achievements.
        /// </summary>
        /// <returns>
        ///     A list of all guild achievements.
        /// </returns>
        public async Task<RequestResult<IEnumerable<AchievementCategory>>> GetGuildAchievementsAsync()
        {
            return await GetGuildAchievementsAsync(_region, _locale);
        }

        /// <summary>
        ///     Get a list of all guild achievements.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     A list of all guild achievements.
        /// </returns>
        public async Task<RequestResult<IEnumerable<AchievementCategory>>> GetGuildAchievementsAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var guildAchievementsList = await Get<IEnumerable<AchievementCategory>>(region, $"{host}/wow/data/guild/achievements?locale={locale}", "achievements");
            return guildAchievementsList;
        }

        /// <summary>
        ///     Get a list of all guild perks.
        /// </summary>
        /// <returns>
        ///     A list of all guild perks.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Perk>>> GetGuildPerksAsync()
        {
            return await GetGuildPerksAsync(_region, _locale);
        }

        /// <summary>
        ///     Get a list of all guild perks.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     A list of all guild perks.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Perk>>> GetGuildPerksAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var guildPerksList = await Get<IEnumerable<Perk>>(region, $"{host}/wow/data/guild/perks?locale={locale}", "perks");
            return guildPerksList;
        }

        /// <summary>
        ///     Get a list of all guild rewards.
        /// </summary>
        /// <returns>
        ///     A list of all guild rewards.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Reward>>> GetGuildRewardsAsync()
        {
            return await GetGuildRewardsAsync(_region, _locale);
        }

        /// <summary>
        ///     Get a list of all guild rewards.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     A list of all guild rewards.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Reward>>> GetGuildRewardsAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var guildRewardsList = await Get<IEnumerable<Reward>>(region, $"{host}/wow/data/guild/rewards?locale={locale}", "rewards");
            return guildRewardsList;
        }

        /// <summary>
        ///     Get the specified item.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <returns>
        ///     The specified item.
        /// </returns>
        public async Task<RequestResult<Item>> GetItemAsync(int itemId)
        {
            return await GetItemAsync(itemId, _region, _locale);
        }

        /// <summary>
        ///     Get the specified item.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The specified item.
        /// </returns>
        public async Task<RequestResult<Item>> GetItemAsync(int itemId, Region region, Locale locale)
        {
            var host = GetHost(region);
            return await Get<Item>(region, $"{host}/wow/item/{itemId}?locale={locale}");
        }

        /// <summary>
        ///     Get a list of all item classes.
        /// </summary>
        /// <returns>
        ///     A list of all item classes.
        /// </returns>
        public async Task<RequestResult<IEnumerable<ItemClass>>> GetItemClassesAsync()
        {
            return await GetItemClassesAsync(_region, _locale);
        }

        /// <summary>
        ///     Get a list of all item classes.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     A list of all item classes.
        /// </returns>
        public async Task<RequestResult<IEnumerable<ItemClass>>> GetItemClassesAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var itemClassesList = await Get<IEnumerable<ItemClass>>(region, $"{host}/wow/data/item/classes?locale={locale}", "classes");
            return itemClassesList;
        }

        /// <summary>
        ///     Get the specified item set.
        /// </summary>
        /// <param name="itemSetId">The item set ID.</param>
        /// <returns>
        ///     The specified item set.
        /// </returns>
        public async Task<RequestResult<ItemSet>> GetItemSetAsync(int itemSetId)
        {
            return await GetItemSetAsync(itemSetId, _region, _locale);
        }

        /// <summary>
        ///     Get the specified item set.
        /// </summary>
        /// <param name="itemSetId">The item set ID.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The specified item set.
        /// </returns>
        public async Task<RequestResult<ItemSet>> GetItemSetAsync(int itemSetId, Region region, Locale locale)
        {
            var host = GetHost(region);
            return await Get<ItemSet>(region, $"{host}/wow/item/set/{itemSetId}?locale={locale}");
        }

        /// <summary>
        ///     Get a list of all supported mounts.
        /// </summary>
        /// <returns>
        ///     A list of all supported mounts.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Mount>>> GetMountsAsync()
        {
            return await GetMountsAsync(_region, _locale);
        }

        /// <summary>
        ///     Get a list of all supported mounts.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     A list of all supported mounts.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Mount>>> GetMountsAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var mountList = await Get<IEnumerable<Mount>>(region, $"{host}/wow/mount/?locale={locale}", "mounts");
            return mountList;
        }

        /// <summary>
        ///     Get a list of all supported pets.
        /// </summary>
        /// <returns>
        ///     A list of all supported pets.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Pet>>> GetPetsAsync()
        {
            return await GetPetsAsync(_region, _locale);
        }

        /// <summary>
        ///     Get a list of all supported pets.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     A list of all supported pets.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Pet>>> GetPetsAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var petList = await Get<IEnumerable<Pet>>(region, $"{host}/wow/pet/?locale={locale}", "pets");
            return petList;
        }

        /// <summary>
        ///     Get the specified pet ability.
        /// </summary>
        /// <param name="abilityId">The pet ability ID.</param>
        /// <returns>
        ///     The specified pet ability.
        /// </returns>
        public async Task<RequestResult<PetAbility>> GetPetAbilityAsync(int abilityId)
        {
            return await GetPetAbilityAsync(abilityId, _region, _locale);
        }

        /// <summary>
        ///     Get the specified pet ability.
        /// </summary>
        /// <param name="abilityId">The pet ability ID.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The specified pet ability.
        /// </returns>
        public async Task<RequestResult<PetAbility>> GetPetAbilityAsync(int abilityId, Region region, Locale locale)
        {
            var host = GetHost(region);
            return await Get<PetAbility>(region, $"{host}/wow/pet/ability/{abilityId}?locale={locale}");
        }

        /// <summary>
        ///     Get the specified pet species.
        /// </summary>
        /// <param name="speciesId">The pet species ID.</param>
        /// <returns>
        ///     The specified pet species.
        /// </returns>
        public async Task<RequestResult<PetSpecies>> GetPetSpeciesAsync(int speciesId)
        {
            return await GetPetSpeciesAsync(speciesId, _region, _locale);
        }

        /// <summary>
        ///     Get the specified pet species.
        /// </summary>
        /// <param name="speciesId">The pet species ID.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The specified pet species.
        /// </returns>
        public async Task<RequestResult<PetSpecies>> GetPetSpeciesAsync(int speciesId, Region region, Locale locale)
        {
            var host = GetHost(region);
            return await Get<PetSpecies>(region, $"{host}/wow/pet/species/{speciesId}?locale={locale}");
        }

        /// <summary>
        ///     Get the pet stats for the specified pet species, level, breed, and quality.
        /// </summary>
        /// <param name="speciesId">The pet species ID.</param>
        /// <param name="level">The pet level.</param>
        /// <param name="breedId">The breed ID.</param>
        /// <param name="quality">The quality.</param>
        /// <returns>
        ///     The pet stats for the specified pet species, level, breed, and quality.
        /// </returns>
        public async Task<RequestResult<PetStats>> GetPetStatsAsync(int speciesId, int level, int breedId, BattlePetQuality quality)
        {
            return await GetPetStatsAsync(speciesId, level, breedId, quality, _region, _locale);
        }

        /// <summary>
        ///     Get the pet stats for the specified pet species, level, breed, and quality.
        /// </summary>
        /// <param name="speciesId">The pet species ID.</param>
        /// <param name="level">The pet level.</param>
        /// <param name="breedId">The breed ID.</param>
        /// <param name="quality">The quality.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The pet stats for the specified pet species, level, breed, and quality.
        /// </returns>
        public async Task<RequestResult<PetStats>> GetPetStatsAsync(int speciesId, int level, int breedId, BattlePetQuality quality, Region region, Locale locale)
        {
            var host = GetHost(region);
            return await Get<PetStats>(region, $"{host}/wow/pet/stats/{speciesId}?level={level}&breedId={breedId}&qualityId={quality:D}&locale={locale}");
        }

        /// <summary>
        ///     Get a list of all pet types.
        /// </summary>
        /// <returns>
        ///     A list of all pet types.
        /// </returns>
        public async Task<RequestResult<IEnumerable<PetType>>> GetPetTypesAsync()
        {
            return await GetPetTypesAsync(_region, _locale);
        }

        /// <summary>
        ///     Get a list of all pet types.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     A list of all pet types.
        /// </returns>
        public async Task<RequestResult<IEnumerable<PetType>>> GetPetTypesAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var petTypeList = await Get<IEnumerable<PetType>>(region, $"{host}/wow/data/pet/types?locale={locale}", "petTypes");
            return petTypeList;
        }

        /// <summary>
        ///     Get the PvP leaderboard for the specified bracket.
        /// </summary>
        /// <param name="bracket">The PvP leaderboard bracket.  Valid entries are 2v2, 3v3, 5v5, and rbg.</param>
        /// <returns>
        ///     The PvP leaderboard for the specified bracket.
        /// </returns>
        public async Task<RequestResult<PvpLeaderboard>> GetPvpLeaderboardAsync(string bracket)
        {
            return await GetPvpLeaderboardAsync(bracket, _region, _locale);
        }

        /// <summary>
        ///     Get the PvP leaderboard for the specified bracket.
        /// </summary>
        /// <param name="bracket">The PvP leaderboard bracket.  Valid entries are 2v2, 3v3, 5v5, and rbg.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The PvP leaderboard for the specified bracket.
        /// </returns>
        public async Task<RequestResult<PvpLeaderboard>> GetPvpLeaderboardAsync(string bracket, Region region, Locale locale)
        {
            var host = GetHost(region);
            return await Get<PvpLeaderboard>(region, $"{host}/wow/leaderboard/{bracket}?locale={locale}");
        }

        /// <summary>
        ///     Get the specified quest.
        /// </summary>
        /// <param name="questId">The quest ID.</param>
        /// <returns>
        ///     The specified quest.
        /// </returns>
        public async Task<RequestResult<Quest>> GetQuestAsync(int questId)
        {
            return await GetQuestAsync(questId, _region, _locale);
        }

        /// <summary>
        ///     Get the specified quest.
        /// </summary>
        /// <param name="questId">The quest ID.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The specified quest.
        /// </returns>
        public async Task<RequestResult<Quest>> GetQuestAsync(int questId, Region region, Locale locale)
        {
            var host = GetHost(region);
            return await Get<Quest>(region, $"{host}/wow/quest/{questId}?locale={locale}");
        }

        /// <summary>
        ///     Get the statuses for all realms.
        /// </summary>
        /// <returns>
        ///     The statuses for all realms.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Realm>>> GetRealmStatusAsync()
        {
            return await GetRealmStatusAsync(_region, _locale);
        }

        /// <summary>
        ///     Get the statuses for all realms.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The statuses for all realms.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Realm>>> GetRealmStatusAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var realmList = await Get<IEnumerable<Realm>>(region, $"{host}/wow/realm/status?locale={locale}", "realms");
            return realmList;
        }

        /// <summary>
        ///     Get the specified recipe.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>
        ///     The specified recipe.
        /// </returns>
        public async Task<RequestResult<Recipe>> GetRecipeAsync(int recipeId)
        {
            return await GetRecipeAsync(recipeId, _region, _locale);
        }

        /// <summary>
        ///     Get the specified recipe.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The specified recipe.
        /// </returns>
        public async Task<RequestResult<Recipe>> GetRecipeAsync(int recipeId, Region region, Locale locale)
        {
            var host = GetHost(region);
            return await Get<Recipe>(region, $"{host}/wow/recipe/{recipeId}?locale={locale}");
        }

        /// <summary>
        ///     Get the specified spell.
        /// </summary>
        /// <param name="spellId">The spell ID.</param>
        /// <returns>
        ///     The specified spell.
        /// </returns>
        public async Task<RequestResult<Spell>> GetSpellAsync(int spellId)
        {
            return await GetSpellAsync(spellId, _region, _locale);
        }

        /// <summary>
        ///     Get the specified spell.
        /// </summary>
        /// <param name="spellId">The spell ID.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The specified spell.
        /// </returns>
        public async Task<RequestResult<Spell>> GetSpellAsync(int spellId, Region region, Locale locale)
        {
            var host = GetHost(region);
            return await Get<Spell>(region, $"{host}/wow/spell/{spellId}?locale={locale}");
        }

        /// <summary>
        ///     Get a dictionary of talents, indexed by character class.
        /// </summary>
        /// <returns>
        ///     A dictionary of talents, indexed by character class.
        /// </returns>
        public async Task<RequestResult<IDictionary<CharacterClass, TalentSet>>> GetTalentsAsync()
        {
            return await GetTalentsAsync(_region, _locale);
        }

        /// <summary>
        ///     Get a dictionary of talents, indexed by character class.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     A dictionary of talents, indexed by character class.
        /// </returns>
        public async Task<RequestResult<IDictionary<CharacterClass, TalentSet>>> GetTalentsAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var talents = await Get<IDictionary<CharacterClass, TalentSet>>(region, $"{host}/wow/data/talents?locale={locale}");
            return talents;
        }

        /// <summary>
        ///     Get user account details.
        /// </summary>
        /// <param name="accessToken">An OAuth access token for the user.</param>
        /// <returns>
        ///     User account details.
        /// </returns>
        public async Task<RequestResult<UserAccount>> GetUserAsync(string accessToken)
        {
            return await GetUserAsync(accessToken, _region);
        }

        /// <summary>
        ///     Get user account details.
        /// </summary>
        /// <param name="accessToken">An OAuth access token for the user.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <returns>
        ///     User account details.
        /// </returns>
        public async Task<RequestResult<UserAccount>> GetUserAsync(string accessToken, Region region)
        {
            var host = GetHost(region);
            var userAccount = await Get<UserAccount>(region, $"{host}/account/user?access_token={accessToken}");
            return userAccount;
        }

        /// <summary>
        ///     Get the specified zone.
        /// </summary>
        /// <param name="zoneId">The zone ID.</param>
        /// <returns>
        ///     The specified zone.
        /// </returns>
        public async Task<RequestResult<Zone>> GetZoneAsync(int zoneId)
        {
            return await GetZoneAsync(zoneId, _region, _locale);
        }

        /// <summary>
        ///     Get the specified zone.
        /// </summary>
        /// <param name="zoneId">The zone ID.</param>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     The specified zone.
        /// </returns>
        public async Task<RequestResult<Zone>> GetZoneAsync(int zoneId, Region region, Locale locale)
        {
            var host = GetHost(region);
            return await Get<Zone>(region, $"{host}/wow/zone/{zoneId}?locale={locale}");
        }

        /// <summary>
        ///     Get a list of all supported zones.
        /// </summary>
        /// <returns>
        ///     A list of all supported zones.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Zone>>> GetZonesAsync()
        {
            return await GetZonesAsync(_region, _locale);
        }

        /// <summary>
        ///     Get a list of all supported zones.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <param name="locale">Specifies the language that the result will be in.</param>
        /// <returns>
        ///     A list of all supported zones.
        /// </returns>
        public async Task<RequestResult<IEnumerable<Zone>>> GetZonesAsync(Region region, Locale locale)
        {
            var host = GetHost(region);
            var zoneList = await Get<IEnumerable<Zone>>(region, $"{host}/wow/zone/?locale={locale}", "zones");
            return zoneList;
        }

        /// <summary>
        ///     Retrieve an item of type <typeparamref name="T"/> from the Blizzard Community API.
        /// </summary>
        /// <typeparam name="T">
        ///     The return type.
        /// </typeparam>
        /// <param name="region">The region from which to request a token.</param>
        /// <param name="requestUri">
        ///     The URI the request is sent to.
        /// </param>
        /// <param name="arrayName">
        ///     The name of the array to deserialize. This is used to avoid using a root object for JSON arrays.
        /// </param>
        /// <returns>
        ///     The JSON response, deserialized to an object of type <typeparamref name="T"/>.
        /// </returns>
        private async Task<RequestResult<T>> Get<T>(Region region, string requestUri, string arrayName = null)
        {
            // Acquire a new OAuth token if we don't have one. Get a new one if it's expired.
            if (_token == null || DateTime.UtcNow >= _tokenExpiration)
            {
                _token = await GetOAuthToken(region).ConfigureAwait(false);
                _tokenExpiration = DateTime.UtcNow.AddSeconds(_token.ExpiresIn).AddSeconds(-30);
            }

            // Add an authentication header with the token.
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);

            // Retrieve the response.
            var response = await _client.GetAsync(requestUri).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                // Check if the request was successful and made it to the Blizzard API.
                // The API will always send back content if successful.
                if (response.Content != null)
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!string.IsNullOrEmpty(content))
                    {
                        RequestResult<T> requestError = JsonConvert.DeserializeObject<RequestError>(content);
                        return requestError;
                    }
                }

                // If not then it is most likely a problem on our end due to an HTTP error.
                var message = $"Response code {(int)response.StatusCode} ({response.ReasonPhrase}) does not indicate success. Request: {requestUri}";

                throw new HttpRequestException(message);
            }

            // Deserialize an object of type T from the JSON string.
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            try
            {
                if (arrayName != null)
                {
                    json = JObject.Parse(json).SelectToken(arrayName).ToString();
                }

                RequestResult<T> requestResult = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
                {
                    ContractResolver = new WarcraftClientContractResolver(),
#if DEBUG
                    MissingMemberHandling = MissingMemberHandling.Error
#else
                    MissingMemberHandling = MissingMemberHandling.Ignore
#endif
                });

                return requestResult;
            }
            catch (JsonReaderException ex)
            {
                var requestError = new RequestError
                {
                    Code = string.Empty,
                    Detail = ex.Message,
                    Type = typeof(JsonReaderException).ToString()
                };
                return new RequestResult<T>(requestError);
            }
        }

        /// <summary>
        ///     Get an OAuth token.
        /// </summary>
        /// <param name="region">The region from which to request a token.</param>
        /// <returns>
        ///     An OAuth token.
        /// </returns>
        private async Task<OAuthAccessToken> GetOAuthToken(Region region)
        {
            var credentials = $"{_clientId}:{_clientSecret}";
            var host = GetOAuthHost(region);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));

            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var request = await _client.PostAsync($"{host}/oauth/token", requestBody);
            var response = await request.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<OAuthAccessToken>(response);
        }

        /// <summary>
        ///     Get the host for the specified region.
        /// </summary>
        /// <param name="region">Specifies the region that the API will retrieve its data from.</param>
        /// <returns>
        ///     The host for the specified region.
        /// </returns>
        private static string GetHost(Region region)
        {
            switch (region)
            {
                case Region.China:
                    return "https://cn.api.blizzard.com";
                case Region.Europe:
                    return "https://eu.api.blizzard.com";
                case Region.Korea:
                    return "https://kr.api.blizzard.com";
                case Region.Taiwan:
                    return "https://tw.api.blizzard.com";
                default:
                    return "https://us.api.blizzard.com";
            }
        }

        /// <summary>
        ///     Get the OAuth host for the specified region.
        /// </summary>
        /// <param name="region">Specifies the region for which an OAuth token will be acquired.</param>
        /// <returns>
        ///     The OAuth host for the specified region.
        /// </returns>
        private static string GetOAuthHost(Region region)
        {
            switch (region)
            {
                case Region.China:
                    return "https://cn.battle.net";
                case Region.Europe:
                    return "https://eu.battle.net";
                case Region.Korea:
                    return "https://kr.battle.net";
                case Region.Taiwan:
                    return "https://tw.battle.net";
                default:
                    return "https://us.battle.net";
            }
        }

        /// <summary>
        ///     Checks if the locale is supported by the selected region.
        /// </summary>
        /// <param name="locale">The selected locale.</param>
        /// <param name="region">The selected region.</param>
        /// <returns>Returns true if the locale is supported by the selected region.</returns>
        private static bool ValidateRegionLocale(Locale locale, Region region)
        {
            var type = locale.GetType().GetRuntimeField(locale.ToString());
            var attribute = type.GetCustomAttribute<LocaleRegion>();

            return attribute.Region == region;
        }
    }
}
