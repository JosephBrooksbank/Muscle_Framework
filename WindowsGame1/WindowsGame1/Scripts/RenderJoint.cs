using System;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyKinectGame;

namespace KinectDemo.Scripts
{
    /// <summary>
    /// The Class to render the muscles, with color based on intensity of usage 
    /// </summary>
    public class RenderJoint
    {
        private Color JointColor;    

        /// <summary>
        /// The function used to draw connections between two joints 
        /// </summary>
        /// <param name="skeleton"> The skeleton of the player</param>
        /// <param name="joint1"> The first joint in the connection </param>
        /// <param name="joint2"> The second joint in the connection </param>
        /// <param name="renderer"> The object used to draw lines on the screen </param>
        /// <param name="color"> The level of intensity, used to choose color of line </param>
        public void DrawJointConnection(MyGame.CustomSkeleton skeleton, JointType joint1, JointType joint2, SpriteBatch renderer, int color) {
           
            
                if (color == 0)
                    JointColor = Color.Blue;
                else if (color > 0 && color < 2)
                    JointColor = Color.Green;
                else if (color >= 2 && color < 4)
                    JointColor = Color.Yellow;
                else if (color >= 4 && color < 5)
                    JointColor = Color.Orange;
                else if (color >= 5)
                    JointColor = Color.Red;
               

            DrawLine(renderer, 4, JointColor, JointToVector(skeleton, joint1), JointToVector(skeleton, joint2));
        }
        /// <summary>
        /// A function to draw a line between two points 
        /// </summary>
        /// <param name="spriteBatch"> The spritebatch object used for visuals </param>
        /// <param name="width"> The width of the line </param>
        /// <param name="color"> The color of the line </param>
        /// <param name="p1"> The first point of the line </param>
        /// <param name="p2"> The second point of the line </param>
        public void DrawLine(SpriteBatch spriteBatch, float width, Color color, Vector2 p1, Vector2 p2) {
            spriteBatch.Draw(Resources.Images.Pixel, p1, null, color, (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X),
                Vector2.Zero, new Vector2(Vector2.Distance(p1, p2), width), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Converts a joint position into a vector object 
        /// </summary>
        /// <param name="skeleton"> The skeleton to be used </param>
        /// <param name="type"> The joint to convert </param>
        /// <returns> The joint's position in 2D space as a Vector2 </returns>
        protected Vector2 JointToVector(MyGame.CustomSkeleton skeleton, JointType type) {
            return JointToVector(skeleton, type, MyGame.World.Width, MyGame.World.Height);
        }
        /// <summary>
        /// Converts a joint position into a vector object 
        /// </summary>
        /// <param name="skeleton"> The skeleton to be used </param>
        /// <param name="type"> The joint to convert </param>
        /// <param name="width"> The width of the screen </param>
        /// <param name="height"> The height of the screen </param>
        /// <returns></returns>
        protected Vector2 JointToVector(MyGame.CustomSkeleton skeleton, JointType type, int width, int height) {
            var Position = skeleton.ScaleTo(type, width, height);
            return new Vector2(Position.X, Position.Y);
        }
    }
}
