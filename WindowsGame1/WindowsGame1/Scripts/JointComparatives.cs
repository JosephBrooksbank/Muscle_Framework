using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace KinectDemo.Scripts
{
    class JointComparatives : MyGame {
        private JointType mainJoint;
        private JointType[] otherJoints;
        private JointType altJoint;
        public void GetJointInfo(JointType joint1, params JointType[] otherjoints) {
            mainJoint = joint1;
            otherJoints = otherjoints;
        }

        public void GetJointInfo(JointType joint1, JointType joint2) {
            mainJoint = joint1;
            altJoint = joint2;

        }

        public bool IsHighest() {
            bool highest = true;
            foreach (JointType t in otherJoints) {
                if (GetJointPosition(mainJoint, ScreenSpace.Screen).Y < GetJointPosition(t, ScreenSpace.Screen).Y)
                   highest = false;
            }
            return highest;
        }


        public static Vector3 GetJointPosition(JointType joint, MyGame.ScreenSpace type, MyGame.CustomSkeleton skeleton = null)
        {
           // if (Instance == null)
           //     return Vector3.Zero;

            // If the skeleton provided is null then grab the first available skeleton and use it:
            if (skeleton == null && Instance.Skeletons != null && Instance.Skeletons.Count > 0)
                skeleton =
                    Instance.Skeletons.FirstOrDefault(
                        o => o.Joints.Count > 0 && o.State == SkeletonTrackingState.Tracked);
            else
                return Vector3.Zero;

            if (type == MyGame.ScreenSpace.Screen)
                return skeleton?.ScaleTo(joint, MyGame.Screen.Width, MyGame.Screen.Height) ?? Vector3.Zero;
            return skeleton?.ScaleTo(joint, MyGame.World.Width, MyGame.World.Height) ?? Vector3.Zero;
        }
    }
}
