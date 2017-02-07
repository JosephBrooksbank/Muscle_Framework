using System.Linq;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace KinectDemo.Scripts {
    internal class JointComparatives : MyGame {
        private JointType altJoint;
        private JointType mainJoint;
        private JointType[] otherJoints;

        public void GetJointInfo(JointType joint1, params JointType[] otherjoints) {
            mainJoint = joint1;
            otherJoints = otherjoints;
        }

        public void GetJointInfo(JointType joint1, JointType joint2) {
            mainJoint = joint1;
            altJoint = joint2;
        }

        public bool IsHighest() {
            var highest = true;
            foreach (var t in otherJoints)
                if (GetJointPosition(mainJoint, ScreenSpace.Screen).Y < GetJointPosition(t, ScreenSpace.Screen).Y)
                    highest = false;
            return highest;
        }


        public static Vector3 GetJointPosition(JointType joint, ScreenSpace type, CustomSkeleton skeleton = null) {
            // if (Instance == null)
            //     return Vector3.Zero;

            // If the skeleton provided is null then grab the first available skeleton and use it:
            if (skeleton == null && Instance.Skeletons != null && Instance.Skeletons.Count > 0)
                skeleton =
                    Instance.Skeletons.FirstOrDefault(
                        o => o.Joints.Count > 0 && o.State == SkeletonTrackingState.Tracked);
            else
                return Vector3.Zero;

            if (type == ScreenSpace.Screen)
                return skeleton?.ScaleTo(joint, Screen.Width, Screen.Height) ?? Vector3.Zero;
            return skeleton?.ScaleTo(joint, World.Width, World.Height) ?? Vector3.Zero;
        }
    }
}