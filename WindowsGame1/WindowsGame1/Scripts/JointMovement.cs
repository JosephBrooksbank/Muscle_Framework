using System;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using MyKinectGame;

namespace KinectDemo.Scripts {
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


        public JointMovement(JointType joint1, MyGame instance) {
            this.Joint1 = joint1;
        }

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