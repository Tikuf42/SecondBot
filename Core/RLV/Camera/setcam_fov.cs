﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BSB.RLV.Camera
{
    public class SetCam_Fov : RLV_UUID_flag_arg_yn
    {
        public override string SetFlagName { get { return "camfov"; } }
    }
}
