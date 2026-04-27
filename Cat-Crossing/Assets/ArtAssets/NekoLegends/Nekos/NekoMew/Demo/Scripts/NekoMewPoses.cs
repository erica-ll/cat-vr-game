using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace NekoLegends
{
    public class NekoMewPoses : NekoMew
    {
        protected override void Start()
        {
            base.Start();
            CycleEyes(6);//reprimanded
        }

        public override void AnimPose()
        {
            base.AnimPose();

            switch (_currentPoseIndex)
            {
                case 0: //sad eyes
                    CycleEyes(6);
                    break;
                case 1:
                    CycleEyes(0);
                    break;
            }

        }
       

    }
}
