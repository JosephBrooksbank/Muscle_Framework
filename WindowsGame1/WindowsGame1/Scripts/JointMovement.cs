using System;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace KinectDemo.Scripts {
    /// <summary>
    /// The Class to handle i
    /// </summary>
    public class JointMovement {
        private readonly double Error = 3;
        private readonly int UpdateTime = 60;
        private int CurrentlyMoving;
        private DateTime CurrentTime;
        public JointType Joint1;
        public bool Moving;
        private int NotMoving;
        private TimeSpan PassedTime;
        private Vector3 PrevPos;


        public JointMovement(JointType joint1) {
            Joint1 = joint1;
        }
        /// <summary>
        /// Function to test whether a joint is moving, based on its previous position 
        /// </summary>
        /// <param name="currPos"> The current position of the joint </param>
        /// <returns> Whether the joint is moving or not </returns>
        public bool IsMoving(Vector3 currPos) {
            if (!(Math.Abs(currPos.X - PrevPos.X) < Error) || !(Math.Abs(currPos.Y - PrevPos.Y) < Error))
                CurrentlyMoving += 1;
            else
                NotMoving += 1;

            if (PassedTime.Milliseconds > UpdateTime) {
                Moving = CurrentlyMoving > NotMoving;
                PrevPos = currPos;
                CurrentTime = DateTime.Now;
                CurrentlyMoving = 0;
                NotMoving = 0;
            }

            PassedTime = DateTime.Now - CurrentTime;
            return Moving;
        }
    }
}