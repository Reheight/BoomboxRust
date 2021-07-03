using Newtonsoft.Json;
using Oxide.Core.Libraries.Covalence;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System;
using Oxide.Core;
using System.Text.RegularExpressions;
using System.Text;

namespace Oxide.Plugins
{
    [Info("BoomboxURL", "Reheight/RyanJD", "1.0.0")]
    [Description("Allows players to easily set the url of a boombox.")]
    public class BoomboxURL : RustPlugin
    {
        private const string UsePerm = "boomboxurl.customurluse";
        private const string StationUsePerm = "boomboxurl.stationsuse";

        private PropertyInfo _serverIpinfo = typeof(BoomBox).GetProperty("CurrentRadioIp");
        Settings config;

        Regex limitedURLS;
        string presetStationsList;

        Dictionary<int, string> stationsNumbered = new Dictionary<int, string>();

        private void SetBoomBoxServerIp(BoomBox box, string ip)
        {
            _serverIpinfo.SetValue(box, ip);
        }
        private void SetBoomBoxServerIp(DeployableBoomBox box, string ip)
        {
            SetBoomBoxServerIp(box.BoxController, ip);
        }

        private void Init()
        {
            permission.RegisterPermission(UsePerm, this);
            permission.RegisterPermission(StationsUsePerm, this);
            AddCovalenceCommand("boombox", nameof(boomboxCMD));
            AddCovalenceCommand("stations", nameof(stationsCMD));
            AddCovalenceCommand("station", nameof(stationCMD));
        }

        private class Settings
        {
            [JsonProperty(PropertyName = "Whitelist")]
            public bool Whitelist = true;

            [JsonProperty(PropertyName = "Whitelisted Domains")]
            public List<string> WhitelistedDomains = new List<string>()
            {
                "stream.zeno.fm"
            };

            [JsonProperty(PropertyName = "Preset Stations")]
            public Dictionary<string, string> PresetStations = new Dictionary<string, string>()
            {
                { "Country Hits", "http://crystalout.surfernetwork.com:8001/KXBZ_MP3" },
                { "Todays Hits", "https://rfcmedia.streamguys1.com/MusicPulsePremium.mp3"},
                { "Pop Hits", "https://rfcmedia.streamguys1.com/newpophitspremium.mp3" }
            };
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();

            try
            {
                config = Config.ReadObject<Settings>();
                if (config == null) throw new Exception();
            }
            catch
            {
                Config.WriteObject(config, false, $"{Interface.Oxide.ConfigDirectory}/{Name}.jsonError");
                PrintError("The configuration for Discord Authentication seems to contain and error and was replaced with the default configuration, your previous configuration has been renamed with a .jsonError extension!");
                LoadDefaultConfig();
            }

            limitedURLS = new Regex($@"^(http||https):\/\/({ String.Join("|", config.WhitelistedDomains) }).*", RegexOptions.Compiled);
            StringBuilder stationsBuilder = new StringBuilder("The following stations we have are below!\n\n");

            int index = 1;
            foreach (var item in config.PresetStations)
            {
                stationsBuilder.Append(index.ToString()).Append(". ").AppendLine(item.Key);

                stationsNumbered[index] = item.Value;
                index++;
            }

            stationsBuilder.AppendLine().Append("Type /station <number> to play a station while holding or looking at a boombox!");

            presetStationsList = stationsBuilder.ToString();
        }
        protected override void LoadDefaultConfig() => config = new Settings();

        protected override void SaveConfig() => Config.WriteObject(config);

        private void stationsCMD(IPlayer player, string cmd, string[] args)
        {
            if (!permission.UserHasPermission(player.Id, StationUsePerm) && !player.IsAdmin)
            {
                player.Reply("You do not have permission to use this command!");
                return;
            }

            player.Reply(presetStationsList);
        }

        private void stationCMD(IPlayer player, string cmd, string[] args)
        {
            if (!permission.UserHasPermission(player.Id, StationUsePerm) && !player.IsAdmin)
            {
                player.Reply("You do not have permission to use this command!");
                return;
            }

            int index;
            string stationURL;

            if (!int.TryParse(args[0], out index) || !stationsNumbered.TryGetValue(index, out stationURL))
            {
                player.Reply("You must input a number that correlated to a station!");
                return;
            }

            switchStation(player.Object as BasePlayer, stationURL);
        }

        private void boomboxCMD(IPlayer player, string cmd, string[] args)
        {
            if (!permission.UserHasPermission(player.Id, UsePerm)  && !player.IsAdmin)
            {
                player.Reply("You do not have permission to use this command!");
                return;
            }

            if (args.Length <= 0)
            {
                player.Reply("You must provide a URL to a audio stream!");
                return;
            }

            if (!args[0].StartsWith("http"))
                args[0] = $"https://{args[0]}";

            if (config.Whitelist && !limitedURLS.IsMatch(args[0]))
            {
                player.Reply("You must use an accepted URL/Domain");
                return;
            }

            switchStation(player.Object as BasePlayer, args[0]);
        }

        private void switchStation(BasePlayer player, string station)
        {

            Item heldItem = player.GetActiveItem();

            if (heldItem == null || heldItem.info.shortname != "fun.boomboxportable")
            {
                DeployableBoomBox boombox;
                if (!IsLookingAtBoomBox(player, out boombox))
                {
                    player.ChatMessage("You must be holding or looking at a boombox!");
                    return;
                }

                boombox.BoxController.ServerTogglePlay(false);
                SetBoomBoxServerIp(boombox, station);
                boombox.BoxController.ServerTogglePlay(true);
            }
            else
            {
                BoomBox heldBoombox = heldItem.GetHeldEntity().GetComponent<BoomBox>();

                heldBoombox.ServerTogglePlay(false);
                SetBoomBoxServerIp(heldBoombox, station);
                heldBoombox.ServerTogglePlay(true);
            }
        }

        private bool IsLookingAtBoomBox(BasePlayer player, out DeployableBoomBox boombox)
        {
            RaycastHit hit;
            boombox = null;

            if (Physics.Raycast(player.eyes.HeadRay(), out hit, 5))
            {
                BaseEntity entity = hit.GetEntity();
                if (entity is DeployableBoomBox)
                    boombox = entity as DeployableBoomBox;
            }

            return boombox != null;
        }
    }
}
