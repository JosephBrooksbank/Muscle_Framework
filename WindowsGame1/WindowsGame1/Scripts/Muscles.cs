using System;
using System.Collections.Generic;
using Microsoft.Kinect;

namespace KinectDemo.Scripts {
    internal class Muscles : MyGame {
        private List<CustomSkeleton> Skeletons;
        //private JointCollection allJoints;
        private List<JointType> allJoints;

        public void getSkeletons(List<CustomSkeleton> skeletons) {
            Skeletons = skeletons;
            
        }

        private void populateJoints() {
            foreach (JointType joints in Enum.GetValues(typeof(JointType)))
            {
                allJoints.Add(joints);
            }
        }

        public void isMoving() {
            foreach (JointType joint in allJoints) {
                
            }
        }



        
    }
}