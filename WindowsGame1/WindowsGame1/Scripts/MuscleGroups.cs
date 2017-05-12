using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace KinectDemo.Scripts
{
    public class MuscleGroups {
        public int SpineMovement;
        public int RightShoudlerMovement;
        public int RightArmMovement;
        public int LeftShoulderMovement;
        public int LeftArmMovement;
        public int RightThighMovement;
        public int LeftThighMovement;
        public int RightHipMovement;
        public int LeftHipMovement;
        public int RightCalfMovement;
        public int LeftCalfMovement;
        public int RightForearmMovement;
        public int LeftForearmMovement;
        


        
        public MuscleGroups(){
        }

        public void CurrentMovement(string MovementName) {
                   
            switch (MovementName) {
                case "rightarmraise":
                    SpineMovement += 1;
                    RightShoudlerMovement += 1;
                    break;
                case "leftarmraise":
                    SpineMovement += 1;
                    LeftShoulderMovement += 1;
                    break;
                case "rightkneeraise":
                    RightThighMovement += 1;
                    RightHipMovement += 1;
                    break;
                case "leftkneeraise":
                    LeftThighMovement += 1;
                    LeftHipMovement += 1;
                    break;
                case "leftkneebend":
                    LeftThighMovement += 1;
                    LeftCalfMovement += 1;
                    SpineMovement += 1;
                    break;
                case "rightkneebend":
                    RightThighMovement += 1;
                    RightCalfMovement += 1;
                    SpineMovement += 1;
                    break;
                case "rightforearm":
                    RightForearmMovement += 1;
                    RightArmMovement += 1;
                    break;
                case "leftforearm":
                    LeftForearmMovement += 1;
                    LeftArmMovement += 1;
                    break;
                default:
                    LeftArmMovement = 0;
                    RightArmMovement = 0;
                    LeftForearmMovement = 0;
                    RightForearmMovement = 0;
                    SpineMovement = 0;                  
                    RightShoudlerMovement = 0;
                    LeftShoulderMovement = 0;
                    RightThighMovement = 0;
                    LeftThighMovement = 0;
                    RightHipMovement = 0;
                    LeftHipMovement = 0;
                    RightCalfMovement = 0;
                    LeftCalfMovement = 0;
                    break;

            }

        }




    }
}
