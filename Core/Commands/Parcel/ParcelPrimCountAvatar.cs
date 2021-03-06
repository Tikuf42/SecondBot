﻿using OpenMetaverse;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSB.Commands.CMD_Parcel
{
    class ParcelPrimCountAvatar : CoreCommand_SmartReply_2arg
    {
        public override string[] ArgTypes { get { return new[] { "Smart","Avatar" }; } }
        public override string[] ArgHints { get { return new[] { "Smart reply target: Channel,Avatar,HTTP url","the avatar (UUID or Firstname Lastname) we want the count of back" }; } }
        public override string Helpfile { get { return "Returns the number of prims owned by the selected avatar on the current parcel as follows: DATA=SIMNAME,PARCELID,AVATAR UUID,COUNT"; } }

        public override void Callback(string[] args, EventArgs e)
        {
            ParcelObjectOwnersReplyEventArgs ObjectOwners = (ParcelObjectOwnersReplyEventArgs)e;
            List<UUID> owner_uuids = new List<UUID>();
            List<int> owner_counts = new List<int>();
            foreach (ParcelManager.ParcelPrimOwners owner in ObjectOwners.PrimOwners)
            {
                owner_uuids.Add(owner.OwnerID);
                owner_counts.Add(owner.Count);
            }
            if (owner_uuids.Count > 0)
            {
                if (UUID.TryParse(args[1], out UUID avatar) == true)
                {
                    int localid = bot.GetClient.Parcels.GetParcelLocalID(bot.GetClient.Network.CurrentSim, bot.GetClient.Self.SimPosition);
                    collection.Add("sim", bot.GetClient.Network.CurrentSim.Name);
                    if (owner_uuids.Contains(avatar) == true)
                    {
                        int owner_count = owner_counts[owner_uuids.IndexOf(avatar)];
                        collection.Add("count", owner_count.ToString());
                    }
                    else
                    {
                        collection.Add("count", "0");
                    }
                    base.Callback(args, e, bot.GetCommandsInterface.SmartCommandReply(true,args[0], "avatar="+ avatar.ToString(), CommandName,collection));
                }
                else
                {
                    base.Callback(args, e, false);
                }
            }
            else
            {
                base.Callback(args, e, false);
            }
        }

        public override bool CallFunction(string[] args)
        {
            if (base.CallFunction(args) == true)
            {
                if (UUID.TryParse(args[1], out _) == true)
                {
                    int localid = bot.GetClient.Parcels.GetParcelLocalID(bot.GetClient.Network.CurrentSim, bot.GetClient.Self.SimPosition);
                    if (bot.CreateAwaitEventReply("parcelobjectowners", this, args) == true)
                    {
                        bot.GetClient.Parcels.RequestObjectOwners(bot.GetClient.Network.CurrentSim, localid);
                        return true;
                    }
                    else
                    {
                        return Failed("Unable to await reply");
                    }
                }
                else
                {
                    return Failed("Arg 2 is not a vaild avatar");
                }
            }
            return false;
        }
    }
}
