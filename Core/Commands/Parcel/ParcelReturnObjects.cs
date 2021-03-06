﻿using OpenMetaverse;
using System;
using System.Collections.Generic;

namespace BSB.Commands.CMD_Parcel
{
    class ParcelReturnObjects : CoreCommand
    {
        public override string Helpfile { get { return "Returns all object from the current parcel to their owners"; } }

        public override void Callback(string[] args, EventArgs e)
        {
            ParcelObjectOwnersReplyEventArgs ObjectOwners = (ParcelObjectOwnersReplyEventArgs)e;
            List<UUID> owner_uuids = new List<UUID>();
            foreach(ParcelManager.ParcelPrimOwners owner in ObjectOwners.PrimOwners)
            {
                owner_uuids.Add(owner.OwnerID);
            }
            if (owner_uuids.Count > 0)
            {
                int localid = bot.GetClient.Parcels.GetParcelLocalID(bot.GetClient.Network.CurrentSim, bot.GetClient.Self.SimPosition);
                Parcel p = bot.GetClient.Network.CurrentSim.Parcels[localid];
                if (bot.GetClient.Network.CurrentSim.Parcels.ContainsKey(localid) == true)
                {
                    if (parcel_static.has_parcel_perm(p, bot) == true)
                    {
                        bot.GetClient.Parcels.ReturnObjects(bot.GetClient.Network.CurrentSim, localid, ObjectReturnType.List, owner_uuids);
                        base.Callback(args, e, true);
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
            else
            {
                base.Callback(args, e, false);
            }
        }

        public override bool CallFunction(string[] args)
        {
            if (base.CallFunction(args) == true)
            {
                int localid = bot.GetClient.Parcels.GetParcelLocalID(bot.GetClient.Network.CurrentSim, bot.GetClient.Self.SimPosition);
                if (bot.GetClient.Network.CurrentSim.Parcels.ContainsKey(localid) == true)
                {
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
                    return Failed("Unable to find parcel in memory, please wait and try again");
                }
            }
            return false;
        }
    }
}
