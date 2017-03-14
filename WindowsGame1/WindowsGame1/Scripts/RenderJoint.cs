using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyKinectGame;

namespace KinectDemo.Scripts
{
    public class RenderJoint
    {
        Color JointColor;
        public void DrawJointConnection(MyGame.CustomSkeleton skeleton, JointType joint1, JointType joint2, SpriteBatch renderer, bool isMoving) {
            JointColor = isMoving ? Color.Red : Color.Blue;
            DrawLine(renderer, 4, JointColor, jointToVector(skeleton, joint1), jointToVector(skeleton, joint2));
        }


        public void DrawLine(SpriteBatch spriteBatch, float width, Color color, Vector2 p1, Vector2 p2) {
            spriteBatch.Draw(Resources.Images.Pixel, p1, null, color, (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X),
                Vector2.Zero, new Vector2(Vector2.Distance(p1, p2), width), SpriteEffects.None, 0);
        }

        protected Vector2 jointToVector(MyGame.CustomSkeleton skeleton, JointType type) {
            return jointToVector(skeleton, type, MyGame.World.Width, MyGame.World.Height);
        }

        protected Vector2 jointToVector(MyGame.CustomSkeleton skeleton, JointType type, int Width, int Height) {
            var position = skeleton.ScaleTo(type, Width, Height);
            return new Vector2(position.X, position.Y);
        }
    }
}
