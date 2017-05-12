using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using KinectDemo.Scripts;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyKinectGame
{
    public class MyGame : Game {
        /**********************************************************************************************/
        // Utility Methods:

        public enum ScreenSpace {
            World = 0,
            Screen
        }

        // This is a reference to your game and all of its data:
        public static MyGame Instance;
        public static RenderJoint Render = new RenderJoint();
        // The amount of time that has passed since the last Update() call:
        public static float UpdateDelta;

        // The amount of time that has passed since the last Draw() call:
        public static float DrawDelta;

        // Determines if the skeleton data will be interpolated from the camera to provide smoother results:
        // Default 15
        // 0 = No Smoothing (Faster Performance)
        // 1 = Slow
        // 100 = Jittery
        public static float Smoothing = 15f;

        // These strings are displayed as debug messages in the top-left corner of the screen:
        public static string Debug_Camera = "";
        public static string Debug_Skeleton = "";
        public static string Debug_State = "";

        // This value determines if the Debug Messages and Skeleton outline will be displayed onscreen:
        public static bool ShowDebug = true;

        public static bool SkeletonActive;
        private int i = 0;

        MuscleGroups MuscleGroups = new MuscleGroups();
        List<bool> SpineMuscleUse = new List<bool>();
        List<bool> ShoulderMuscleUse = new List<bool>();
        bool RaiseArm = new bool();
        JointMovement HeadMovement = new JointMovement(JointType.Head, Instance);
        JointMovement ShoulderRightMovement = new JointMovement(JointType.ShoulderRight, Instance);
        JointMovement ElbowRightMovement = new JointMovement(JointType.ElbowRight, Instance);
        JointMovement HandRightMovement = new JointMovement(JointType.HandRight, Instance);
        JointMovement ShoudlerLeftMovement = new JointMovement(JointType.ShoulderLeft, Instance );
        JointMovement ElbowLeftMovement = new JointMovement(JointType.ElbowLeft, Instance);
        JointMovement HandLeftMovement = new JointMovement(JointType.HandLeft, Instance);
        JointMovement HipCenterMovement = new JointMovement(JointType.HipCenter, Instance);
        JointMovement HipRightMovement = new JointMovement(JointType.HipRight, Instance);
        JointMovement KneeRightMovement = new JointMovement(JointType.KneeRight, Instance);
        JointMovement FootRightMovement = new JointMovement(JointType.FootRight, Instance);
        JointMovement HipLeftMovement = new JointMovement(JointType.HipLeft, Instance);
        JointMovement KneeLeftMovement = new JointMovement(JointType.KneeLeft, Instance);
        JointMovement FootLeftMovement = new JointMovement(JointType.FootRight, Instance);
        // GameState Handlers:


        private readonly Color defaultColor = Color.Black;
       

        /**********************************************************************************************/
        // Example GameState System:

        public void SetupGameStates() {
            GameState.Add("Muscle Map", OnUpdate_TitleScreen, null, OnEnter_TitleScreen, OnExit_TitleScreen);
            GameState.Add("level1", OnUpdate_Level1, null, OnEnter_Level1);
            GameState.Add("level2", OnUpdate_Level2);
            GameState.Add("endscreen", OnUpdate_Endscreen);

            // Set the initial GameState:
            GameState.Set("Muscle Map");
        }

        public void OnUpdate_TitleScreen() {
            i += 1;
            //Renderer.DrawString(Resources.Fonts.Load("font_20"), "Hello Title Screen ", new Vector2(20, 200), defaultColor);
            Renderer.DrawString(Resources.Fonts.Load("font_20"), "Welcome to Muscle Map! ", new Vector2(400,50), defaultColor);
            // Renderer.DrawString(Resources.Fonts.Load("font_20"), Convert.ToString(i), new Vector2(20, 300), defaultColor);
//            if (Skeletons != null) {
//                
//            }
               


        }

        public void OnEnter_TitleScreen() {
         
        }

        public void OnExit_TitleScreen() {
        }

        public void OnUpdate_Level1() {
            Renderer.DrawString(Resources.Fonts.Load("font_20"), "Hello LEVEL1 ", new Vector2(20, 200), defaultColor);
        }

        public void OnEnter_Level1() {
        }

        public void OnUpdate_Level2() {
            // Do something;
            Renderer.DrawString(Resources.Fonts.Load("font_20"), "Hello LEVEL 2 ", new Vector2(20, 200), defaultColor);
        }

        public void OnUpdate_Endscreen() {
            // Do something;
        }

        public static Color GetRandomColor() {
            var rand = new Random((int) DateTime.Now.Ticks);
            return new Color((float) rand.NextDouble(), (float) rand.NextDouble(), (float) rand.NextDouble(), 1f);
        }

      

       /* public bool IsAbove(JointType joint1, JointType joint2) {
            return GetJointPosition(joint1, ScreenSpace.World).Y > GetJointPosition(joint2, ScreenSpace.World).Y;
        }*/

        /// <summary>
        ///     Returns the screen position of the target joint.
        /// </summary>
        /// <param name="joint">The joint to return position data for.</param>
        /// <param name="skeleton">If not set then the first available skeleton will be selected.</param>
        /// <returns>The joint position.</returns>
        public static Vector3 GetJointPosition(JointType joint, ScreenSpace type, CustomSkeleton skeleton = null) {
            if (Instance == null)
                return Vector3.Zero;

            // If the skeleton provided is null then grab the first available skeleton and use it:
            if (skeleton == null && Instance.Skeletons != null && Instance.Skeletons.Count > 0)
                skeleton =
                    Instance.Skeletons.FirstOrDefault(o => o.Joints.Count > 0 && o.State == SkeletonTrackingState.Tracked);
            else
                return Vector3.Zero;

            if (type == ScreenSpace.Screen)
                return skeleton.ScaleTo(joint, Screen.Width, Screen.Height);
           // return skeleton.ScaleTo(joint, Screen.Width, Screen.Height);
            if (skeleton != null) return skeleton.ScaleTo(joint, World.Width, World.Height);
            return Vector3.Zero;
        }

        public float Interpolate(float a, float b, float speed) {
            return MathHelper.Lerp(a, b, DrawDelta * speed);
        }

        public void DrawAllSkeletons() {
            if (Skeletons == null)
                return;

            foreach (var skeleton in Skeletons)
                DrawSkeleton(skeleton);
        }

        public void DrawSkeleton(CustomSkeleton skeleton) {
            if (skeleton == null)
                return;
            // Checking for Movement and sending it to MuscleGroups to tell which muscles are being used 
            MuscleGroups.CurrentMovement("");
            if (ElbowRightMovement.IsMoving(GetJointPosition(ElbowRightMovement.Joint1, ScreenSpace.World)))
                MuscleGroups.CurrentMovement("rightarmraise");
            if (ElbowLeftMovement.IsMoving(GetJointPosition(ElbowLeftMovement.Joint1, ScreenSpace.World)))
                MuscleGroups.CurrentMovement("leftarmraise");
            
            if (KneeLeftMovement.IsMoving(GetJointPosition(KneeLeftMovement.Joint1, ScreenSpace.World)) && 
                !HipCenterMovement.IsMoving(GetJointPosition(HipCenterMovement.Joint1, ScreenSpace.World)))
                MuscleGroups.CurrentMovement("leftkneeraise");
            else if (KneeLeftMovement.IsMoving(GetJointPosition(KneeLeftMovement.Joint1, ScreenSpace.World)))
                MuscleGroups.CurrentMovement("leftkneebend");

            if (KneeRightMovement.IsMoving(GetJointPosition(KneeRightMovement.Joint1, ScreenSpace.World)) && 
                !HipCenterMovement.IsMoving(GetJointPosition(HipCenterMovement.Joint1, ScreenSpace.World)))
                MuscleGroups.CurrentMovement("rightkneeraise");
            else if (KneeRightMovement.IsMoving(GetJointPosition(KneeRightMovement.Joint1, ScreenSpace.World)))
                MuscleGroups.CurrentMovement("rightkneebend");



            if (HandRightMovement.IsMoving(GetJointPosition(HandRightMovement.Joint1, ScreenSpace.World)))
                MuscleGroups.CurrentMovement("rightforearm");
            if (HandLeftMovement.IsMoving(GetJointPosition(HandLeftMovement.Joint1, ScreenSpace.World)))
                MuscleGroups.CurrentMovement("leftforearm");

            
            //Render.DrawJointConnection(skeleton, JointType.Head, JointType.ShoulderCenter, Renderer, HeadMovement.IsMoving(GetJointPosition(HeadMovement.Joint1, ScreenSpace.World)));

            Render.DrawJointConnection(skeleton, JointType.ShoulderCenter, JointType.ShoulderRight, Renderer, MuscleGroups.RightShoudlerMovement );
            Render.DrawJointConnection(skeleton, JointType.ShoulderCenter, JointType.ShoulderLeft, Renderer, MuscleGroups.LeftShoulderMovement);
            Render.DrawJointConnection(skeleton, JointType.ShoulderRight, JointType.ElbowRight, Renderer, MuscleGroups.RightArmMovement);
            Render.DrawJointConnection(skeleton, JointType.ElbowRight, JointType.HandRight, Renderer, MuscleGroups.RightForearmMovement);

            Render.DrawJointConnection(skeleton, JointType.ShoulderLeft, JointType.ElbowLeft, Renderer, MuscleGroups.LeftArmMovement);
            Render.DrawJointConnection(skeleton, JointType.ElbowLeft, JointType.HandLeft, Renderer, MuscleGroups.LeftForearmMovement);


            Render.DrawJointConnection(skeleton, JointType.ShoulderCenter, JointType.HipCenter, Renderer, MuscleGroups.SpineMovement);

            Render.DrawJointConnection(skeleton, JointType.HipCenter, JointType.HipRight, Renderer, MuscleGroups.RightHipMovement);
            Render.DrawJointConnection(skeleton, JointType.HipRight, JointType.KneeRight, Renderer, MuscleGroups.RightThighMovement);
            Render.DrawJointConnection(skeleton, JointType.KneeRight, JointType.FootRight, Renderer, MuscleGroups.RightCalfMovement);
            Render.DrawJointConnection(skeleton, JointType.HipCenter, JointType.HipLeft, Renderer, MuscleGroups.LeftHipMovement);
            Render.DrawJointConnection(skeleton, JointType.HipLeft, JointType.KneeLeft, Renderer, MuscleGroups.LeftThighMovement);
            Render.DrawJointConnection(skeleton, JointType.KneeLeft, JointType.FootLeft, Renderer, MuscleGroups.LeftCalfMovement);
          
            

        }

//        public void DrawJointConnection(CustomSkeleton skeleton, JointType joint1, JointType joint2) {
//            DrawLine(Renderer, 4, Color.Blue, jointToVector(skeleton, joint1), jointToVector(skeleton, joint2));
//        }

        public void DrawLine(SpriteBatch spriteBatch, float width, Color color, Vector2 p1, Vector2 p2) {
            spriteBatch.Draw(Resources.Images.Pixel, p1, null, color, (float) Math.Atan2(p2.Y - p1.Y, p2.X - p1.X),
                Vector2.Zero, new Vector2(Vector2.Distance(p1, p2), width), SpriteEffects.None, 0);
        }

        public static class Screen {
            public static int Width {
                get {
                    if (Instance == null)
                        return 0;

                    return Instance.GraphicsManager.PreferredBackBufferWidth;
                }
            }

            public static int Height {
                get {
                    if (Instance == null)
                        return 0;

                    return Instance.GraphicsManager.PreferredBackBufferHeight;
                }
            }
        }

        public static class World {
            public static int Width {
                get { return 480; }
            }

            public static int Height {
                get { return 360; }
            }
        }

        #region Math Internal

        protected Vector2 jointToVector(CustomSkeleton skeleton, JointType type) {
            return jointToVector(skeleton, type, World.Width, World.Height);
        }

        protected Vector2 jointToVector(CustomSkeleton skeleton, JointType type, int width, int height) {
            var position = skeleton.ScaleTo(type, width, height);
            return new Vector2(position.X, position.Y);
        }

        #endregion

        #region System

        private readonly GraphicsDeviceManager GraphicsManager;
        private SpriteBatch Renderer;
        private KinectSensor Camera;
        public List<CustomSkeleton> Skeletons; //this array will hold all skeletons that are found in the video frame

        #endregion

        #region XNA Framework Overrides:

        public MyGame() {
            GraphicsManager = new GraphicsDeviceManager(this);

            GraphicsManager.PreferredBackBufferHeight = 600;
            GraphicsManager.PreferredBackBufferWidth = 1200;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            // Set the instance so that it is available to other classes:
            Instance = this;

            // The Kinect has a built in system that handles 6 skeletons at once:
            Skeletons = new List<CustomSkeleton>();

            // Find the Kinect Camera:
            if (KinectSensor.KinectSensors.Count > 0) {
                Camera = KinectSensor.KinectSensors[0];

                if (Camera != null) {
                    // Initialize the Kinect sensor:
                    Camera.SkeletonStream.Enable();
                    Camera.Start();
                    Camera.SkeletonStream.Enable(new TransformSmoothParameters {
                        Smoothing = 0.5f,
                        Correction = 0.5f,
                        Prediction = 0.5f,
                        JitterRadius = 0.05f,
                        MaxDeviationRadius = 0.04f
                    });

                    // When the camera senses a skeleton "event" in realtime , it pulls in new video frames that we can interpret:
                    Camera.SkeletonFrameReady += OnSkeletonUpdated;
                    Debug_Camera = "Camera Connected!";
                }
                else {
                    Debug_Camera = "No Camera";
                }
            }
            // Set the debug message to update whenever the game state changes:
            GameState.OnStateActivated += name => { Debug_State = name; };


            //Adding Muscles to Muscle Groups 
            //Groups that use spine
            SpineMuscleUse.Add(RaiseArm);

            //Groups that use Shoulders 
            ShoulderMuscleUse.Add(RaiseArm);


       

            // Setup the custom GameStates:
            SetupGameStates();

            base.Initialize();
        }

        protected override void LoadContent() {
            Renderer = new SpriteBatch(GraphicsDevice);

            // KM - this was missing:
            base.LoadContent();
        }

        protected override void UnloadContent() {
            if (Camera != null)
                Camera.Stop();

            // KM - Release resources that were loaded when unloading:
            Resources.Unload();

            // KM - this was missing:
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime) {
            UpdateDelta = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (Keyboard.GetState().IsKeyDown(Keys.X))
                Exit();
          //  if (IsTouching(JointType.HandLeft, JointType.Head))
           //     Renderer.DrawString(Resources.Fonts.Load("font_20"), "Left hand is over head!", new Vector2(20, 300), defaultColor);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            // KM: This is the amount of time (in seconds) that has elapsed since the last frame.
            //     Use this to properly interpolate the skeleton changes or for rendering.
            //     I have added a public static accessor called FrameDelta which should be used to 
            //     access this information from anywhere in the game.
            //     I also added a utility method Interpolate which will smoothly interpolate between
            //     two values using the delta.
            DrawDelta = (float) gameTime.ElapsedGameTime.TotalSeconds;

            GraphicsDevice.Clear(Color.LightGray);

            Renderer.Begin();

            if (ShowDebug) {
                Renderer.DrawString(Resources.Fonts.Load("font_copy_custom"), Debug_State, new Vector2(20, 20),
                    Color.Green);
                Renderer.DrawString(Resources.Fonts.Load("font_copy_default"), Debug_Camera, new Vector2(20, 50),
                    Color.Blue);
                Renderer.DrawString(Resources.Fonts.Load("font_copy_default"), Debug_Skeleton, new Vector2(20, 80),
                    Color.Red);

                // Renders the Skeleton on screen for Debugging:
                DrawAllSkeletons();
            }

            // Execute the appropriate game state handler each frame:
            if (GameState.ActiveState != null)
                GameState.ActiveState.OnUpdate();

            // Complete the rendering of this frame:
            Renderer.End();

            base.Draw(gameTime);
        }

        #endregion

        #region Kinect Event Handlers:

        public static Vector2 screenMod {
            get {
                if (_screenMod == null || _screenMod == Vector2.Zero)
                    _screenMod = new Vector2(Screen.Width / 2, Screen.Height / 2);
                return _screenMod;
            }
        }

        private static Vector2 _screenMod;

        public class CustomSkeleton {
            public Dictionary<JointType, Vector3> Joints = new Dictionary<JointType, Vector3>();

            public SkeletonTrackingState State = SkeletonTrackingState.NotTracked;

            public void Set(Skeleton skeleton) {
                State = skeleton.TrackingState;

                foreach (Joint joint in skeleton.Joints) {
                    var pos = new Vector3(joint.Position.X, -joint.Position.Y, joint.Position.Z);
                    if (Joints.ContainsKey(joint.JointType))
                        Joints[joint.JointType] = pos;
                    else
                        Joints.Add(joint.JointType, pos);
                }
            }

            public Vector3 ScaleTo(JointType type, float width, float height) {
                if (!Joints.ContainsKey(type))
                    return Vector3.Zero;
                return new Vector3(Joints[type].X * width + screenMod.X, Joints[type].Y * height + screenMod.Y,
                    Joints[type].Z);
            }
        }

        private void OnSkeletonUpdated(object sender, SkeletonFrameReadyEventArgs e) {
            // The live feed returns updates from the camera:
            var skeletonFrame = e.OpenSkeletonFrame();
            if (skeletonFrame == null)
                return;

            // Add smoothing to prevent glitching in the skeletons' movements:
            var raw = new Skeleton[6];
            skeletonFrame.CopySkeletonDataTo(raw);
            for (var i = 0; i < raw.Length; i++) {
                if (raw[i] == null)
                    continue;

                if (Skeletons.Count <= i || Skeletons[i] == null) {
                    // If this skeleton was just added then set it:
                    var newSkeleton = new CustomSkeleton();
                    newSkeleton.Set(raw[i]);
                    Skeletons.Add(newSkeleton);
                    continue;
                }

                if (Smoothing > 0f) {
                    // Set the state of the skeleton:
                    Skeletons[i].State = raw[i].TrackingState;

                    // Add smoothing interpolation to each point:
                    foreach (Joint joint in raw[i].Joints) {
                        var pos = new Vector3();
                        pos.X = Interpolate(Skeletons[i].Joints[joint.JointType].X, joint.Position.X, Smoothing);
                        pos.Y = Interpolate(Skeletons[i].Joints[joint.JointType].Y, -joint.Position.Y, Smoothing);
                        pos.Z = Interpolate(Skeletons[i].Joints[joint.JointType].Z, joint.Position.Z, Smoothing);

                        Skeletons[i].Joints[joint.JointType] = pos;
                    }
                }
                else {
                    // Store the raw data for the skeleton:
                    Skeletons[i].Set(raw[i]);
                }
            }

            skeletonFrame.Dispose();

            if (raw != null && raw.Any(o => o.TrackingState == SkeletonTrackingState.Tracked)) {
                SkeletonActive = true;
                Debug_Skeleton = "Skeleton Found";
            }
            else {
                SkeletonActive = false;
                Debug_Skeleton = "No Skeleton Found";
            }
        }

        #endregion
    }
}

//challenges
//1. Add an actual rectangle for the head
//2. only draw the head and hands if the values for the coordinates are not zero
//3. get the x,y,z values of another joint and display some type of interaction with it