using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace KinectDemo.Scripts
{
    public class MuscleGroups {
        public bool SpineMovement;
        public bool RightShoudlerMovement;
        public bool LeftShoulderMovement;
        public bool LeftArmMovement;

        
        public MuscleGroups(){
        }

        public void CurrentMovement(string MovementName) {
                   
            switch (MovementName) {
                case "rightarmraise":
                    SpineMovement = true;
                    RightShoudlerMovement = true;
                    break;
                case "leftarmraise":
                    SpineMovement = true;
                    LeftShoulderMovement = true;
                    break;
                default:
                    LeftArmMovement = false;
                    SpineMovement = false;
                    RightShoudlerMovement = false;
                    LeftShoulderMovement = false;
                    break;

            }

        }




    }
}
