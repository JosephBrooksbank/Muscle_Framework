using System;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace KinectDemo.Scripts {
    internal class JointComparatives : MyGame {
        private JointType MainJoint;
        private JointType[] OtherJoints;
        private Vector3 OldPosition = Vector3.Zero;
        private char[] axis;
        /// <summary>
        ///     Sets joints to be used for comparisons
        /// </summary>
        /// <param name="joint1"> The joint being focused on for comparisons</param>
        /// <param name="otherjoints"> A list of all joints that the main joint is being compared against </param>
        public void GetJointInfo(JointType joint1, params JointType[] otherjoints) {
            MainJoint = joint1;
            OtherJoints = otherjoints;
        }

        /// <summary>
        ///     Checks to see if the main joint is highest
        /// </summary>
        /// <returns> Whether or not main joint is highest </returns>
        public bool IsHighest() {
            var Highest = true;
            foreach (var T in OtherJoints)
                if (GetJointPosition(MainJoint, ScreenSpace.Screen).Y > GetJointPosition(T, ScreenSpace.Screen).Y)
                    Highest = false;
            return Highest;
        }

        /// <summary>
        ///     Checks to see if main joint is lowest
        /// </summary>
        /// <returns> Whether or not main joint is lowest </returns>
        public bool IsLowest() {
            var Lowest = true;
            foreach (var T in OtherJoints)
                if (GetJointPosition(MainJoint, ScreenSpace.Screen).Y < GetJointPosition(T, ScreenSpace.Screen).Y)
                    Lowest = false;
            return Lowest;
        }

        /// <summary>
        ///     Checks to see if the main joint is farthest left
        /// </summary>
        /// <returns> Whether or not main joint is farthest left </returns>
        public bool IsFarthestLeft() {
            var Farthest = true;
            foreach (var T in OtherJoints)
                if (GetJointPosition(MainJoint, ScreenSpace.Screen).X > GetJointPosition(T, ScreenSpace.Screen).X)
                    Farthest = false;
            return Farthest;
        }

        /// <summary>
        ///     Checks to see if main joint is farthest right
        /// </summary>
        /// <returns> Whether or not main joint is farthest right </returns>
        public bool IsFarthestRight() {
            var Farthest = true;
            foreach (var T in OtherJoints)
                if (GetJointPosition(MainJoint, ScreenSpace.Screen).X < GetJointPosition(T, ScreenSpace.Screen).X)
                    Farthest = false;
            return Farthest;
        }



        /// <summary>
        /// Checks to see if all the joints are level 
        /// </summary>
        /// <returns> Whether or not the joints are level with the main joint </returns>
        public bool IsLevel() {
            var Level = true;
            foreach (var T in OtherJoints)
                if (
                    !(Math.Abs(GetJointPosition(MainJoint, ScreenSpace.Screen).Y -
                               GetJointPosition(T, ScreenSpace.Screen).Y) < 10))
                    Level = false;
            return Level;
        }


        public bool IsMoving() {
            if ((Math.Abs(GetJointPosition(MainJoint, ScreenSpace.World).X - OldPosition.X) > 10) ||
                (Math.Abs(GetJointPosition(MainJoint, ScreenSpace.World).Y - OldPosition.Y) > 10)
                || (Math.Abs(GetJointPosition(MainJoint, ScreenSpace.World).Z - OldPosition.Z) > 10)) {

                OldPosition = GetJointPosition(MainJoint, ScreenSpace.World);
                return true;
            }

            return false;

        }



        /// <summary>
        ///     Gets the position in a vector3 of a current joint
        /// </summary>
        /// <param name="joint"> The joint who's position is being checked </param>
        /// <param name="type"> The space in which the value is calculated </param>
        /// <param name="skeleton"> The skeleton which the joint is a part of</param>
        /// <returns> The position of a joint </returns>
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