using Newtonsoft.Json;
using Oxide.Core.Libraries.Covalence;
using System.Linq;
using UnityEngine;
using System.Reflection;

namespace Oxide.Plugins
{
    [Info("BoomboxURL", "Reheight", "1.0.0")]
    [Description("Allows players to easily set the url of a boombox.")]
    public class BoomboxURL : RustPlugin
    {
        private const string UsePerm = "boomboxurl.use";

        private PropertyInfo _serverIpinfo = typeof(BoomBox).GetProperty("CurrentRadioIp");

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
            AddCovalenceCommand("boombox", nameof(boomboxCMD));
        }

        private void boomboxCMD(IPlayer player, string cmd, string[] args)
        {
            if (!permission.UserHasPermission(player.Id, UsePerm)  && !player.IsAdmin)
                player.Reply("You do not have permission to use this command!");

            if (args.Length <= 0)
                player.Reply("You must provide a URL to a audio stream!");

            BasePlayer plr = player?.Object as BasePlayer;

            Item heldItem = plr.GetActiveItem();

            if (heldItem == null || heldItem.info.shortname != "fun.boomboxportable")
            {
                DeployableBoomBox boombox;
                if (!IsLookingAtBoomBox(plr, out boombox))
                {
                    plr.ChatMessage("You must be holding or looking at a boombox!");
                    return;
                }

                boombox.BoxController.ServerTogglePlay(false);
                SetBoomBoxServerIp(boombox, args[0]);
                boombox.BoxController.ServerTogglePlay(true);
            } else
            {
                BoomBox heldBoombox = heldItem.GetHeldEntity().GetComponent<BoomBox>();

                heldBoombox.ServerTogglePlay(false);
                SetBoomBoxServerIp(heldBoombox, args[0]);
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
