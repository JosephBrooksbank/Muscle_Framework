using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using MyKinectGame;

namespace KinectDemo.Scripts
{
    public class JointMovement
    {
        private Vector3 PrevPos;
        private Vector3 CurrPos;
        private int CurrentlyMoving = 0;
        private int NotMoving = 0;
        public bool moving = false;
        private double error = 3;
        private DateTime CurrentTime;
        private TimeSpan PassedTime;
        private int updateTime = 60;
        public JointType joint1;
        private MyGame instance;

        public JointMovement(JointType joint1, MyGame instance) {
            this.joint1 = joint1;
            this.instance = instance;

        }

        public bool IsMoving(Vector3 CurrPos) {
            
            if (!(Math.Abs(CurrPos.X - PrevPos.X) < error) || !(Math.Abs(CurrPos.Y - PrevPos.Y) < error))
                CurrentlyMoving += 1;
            else
                NotMoving += 1;

            if (PassedTime.Milliseconds > updateTime)
            {
                moving = CurrentlyMoving > NotMoving;
                PrevPos = CurrPos;
                CurrentTime = DateTime.Now;
                CurrentlyMoving = 0;
                NotMoving = 0;
            }

            PassedTime = DateTime.Now - CurrentTime;
            return moving;
        }

    }
}
