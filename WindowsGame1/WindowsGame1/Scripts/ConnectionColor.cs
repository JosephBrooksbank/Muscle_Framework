using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace KinectDemo.Scripts
{
    class ConnectionColor : MyGame {
        Vector3 PreviousPosition = Vector3.Zero;

        public Color ConnectColor(JointType joint1, JointType joint2) {
            Vector3 CurrentPosition = GetJointPosition(joint1, ScreenSpace.World);

            if (CurrentPosition == PreviousPosition) {
                return Color.Red;
            }
            PreviousPosition = CurrentPosition;
            return Color.Blue;
        }
    }
}
