using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyKinectGame;

namespace KinectDemo.Scripts
{
    /// <summary>
    /// The main class for the game 
    /// </summary>
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
        public static string DebugCamera = "";
        public static string DebugSkeleton = "";
        public static string DebugState = "";

        // This value determines if the Debug Messages and Skeleton outline will be displayed onscreen:
        public static bool ShowDebug = true;

        public static bool SkeletonActive;

        private readonly MuscleGroups MuscleGroups = new MuscleGroups();
      
        private readonly JointMovement HeadMovement = new JointMovement(JointType.Head);
        private JointMovement ShoulderRightMovement = new JointMovement(JointType.ShoulderRight);
        private readonly JointMovement ElbowRightMovement = new JointMovement(JointType.ElbowRight);
        private readonly JointMovement HandRightMovement = new JointMovement(JointType.HandRight);
        private JointMovement ShoudlerLeftMovement = new JointMovement(JointType.ShoulderLeft );
        private readonly JointMovement ElbowLeftMovement = new JointMovement(JointType.ElbowLeft);
        private readonly JointMovement HandLeftMovement = new JointMovement(JointType.HandLeft);
        private readonly JointMovement HipCenterMovement = new JointMovement(JointType.HipCenter);
        private JointMovement HipRightMovement = new JointMovement(JointType.HipRight);
        private readonly JointMovement KneeRightMovement = new JointMovement(JointType.KneeRight);
        private JointMovement FootRightMovement = new JointMovement(JointType.FootRight);
        private JointMovement HipLeftMovement = new JointMovement(JointType.HipLeft);
        private readonly JointMovement KneeLeftMovement = new JointMovement(JointType.KneeLeft);
        private JointMovement FootLeftMovement = new JointMovement(JointType.FootRight);
        // GameState Handlers:


        private readonly Color DefaultColor = Color.Black;
       

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
            Renderer.DrawString(Resources.Fonts.Load("font_20"), "Welcome to Muscle Map! ", new Vector2(400,50), DefaultColor);          

               


        }

        public void OnEnter_TitleScreen() {
         
        }

        public void OnExit_TitleScreen() {
        }

        public void OnUpdate_Level1() {
            Renderer.DrawString(Resources.Fonts.Load("font_20"), "Hello LEVEL1 ", new Vector2(20, 200), DefaultColor);
        }

        public void OnEnter_Level1() {
        }

        public void OnUpdate_Level2() {
            // Do something;
            Renderer.DrawString(Resources.Fonts.Load("font_20"), "Hello LEVEL 2 ", new Vector2(20, 200), DefaultColor);
        }

        public void OnUpdate_Endscreen() {
            // Do something;
        }

        public static Color GetRandomColor() {
            var Rand = new Random((int) DateTime.Now.Ticks);
            return new Color((float) Rand.NextDouble(), (float) Rand.NextDouble(), (float) Rand.NextDouble(), 1f);
        }

        /// <summary>
        ///     Returns the screen position of the target joint.
        /// </summary>
        /// <param name="joint">The joint to return position data for.</param>
        /// <param name="type"> Type of screen </param>
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
            if (skeleton != null) return skeleton.ScaleTo(joint, World.Width, World.Height);
            return Vector3.Zero;
        }

        public float Interpolate(float a, float b, float speed) {
            return MathHelper.Lerp(a, b, DrawDelta * speed);
        }

        public void DrawAllSkeletons() {
            if (Skeletons == null)
                return;

            foreach (var Skeleton in Skeletons)
                DrawSkeleton(Skeleton);
        }

        /// <summary>
        /// Draws the player on the screen
        /// </summary>
        /// <param name="skeleton"></param>
        public void DrawSkeleton(CustomSkeleton skeleton) {
            if (skeleton == null)
                return;
            // Checking for Movement and sending it to MuscleGroups to tell which muscles are being used 
            // Resets all muscles
            MuscleGroups.CurrentMovement("");

            // Arms raising, uses back and shoulder muscles 
            if (ElbowRightMovement.IsMoving(GetJointPosition(ElbowRightMovement.Joint1, ScreenSpace.World)))
                MuscleGroups.CurrentMovement("rightarmraise");
            if (ElbowLeftMovement.IsMoving(GetJointPosition(ElbowLeftMovement.Joint1, ScreenSpace.World)))
                MuscleGroups.CurrentMovement("leftarmraise");
            
            // If legs are raised, hip and back muscles used. If knees are bent, leg muscles are used
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


            // If hands are moved (bicep curl style), arm muscles are used 
            if (HandRightMovement.IsMoving(GetJointPosition(HandRightMovement.Joint1, ScreenSpace.World)))
                MuscleGroups.CurrentMovement("rightforearm");
            if (HandLeftMovement.IsMoving(GetJointPosition(HandLeftMovement.Joint1, ScreenSpace.World)))
                MuscleGroups.CurrentMovement("leftforearm");

            if (HeadMovement.IsMoving(GetJointPosition(HeadMovement.Joint1, ScreenSpace.World)))
                MuscleGroups.CurrentMovement("head");
                    
            Render.DrawJointConnection(skeleton, JointType.Head, JointType.ShoulderCenter, Renderer, MuscleGroups.NeckMovement);
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
            public static int Width => 480;

            public static int Height => 360;
        }

        #region Math Internal

        protected Vector2 JointToVector(CustomSkeleton skeleton, JointType type) {
            return JointToVector(skeleton, type, World.Width, World.Height);
        }

        protected Vector2 JointToVector(CustomSkeleton skeleton, JointType type, int width, int height) {
            var Position = skeleton.ScaleTo(type, width, height);
            return new Vector2(Position.X, Position.Y);
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
            GraphicsManager = new GraphicsDeviceManager(this) {
                PreferredBackBufferHeight = 600,
                PreferredBackBufferWidth = 1200
            };


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
                    DebugCamera = "Camera Connected!";
                }
                else {
                    DebugCamera = "No Camera";
                }
            }
            // Set the debug message to update whenever the game state changes:
            GameState.OnStateActivated += name => { DebugState = name; };

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
            Camera?.Stop();

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
                Renderer.DrawString(Resources.Fonts.Load("font_copy_custom"), DebugState, new Vector2(20, 20),
                    Color.Green);
                Renderer.DrawString(Resources.Fonts.Load("font_copy_default"), DebugCamera, new Vector2(20, 50),
                    Color.Blue);
                Renderer.DrawString(Resources.Fonts.Load("font_copy_default"), DebugSkeleton, new Vector2(20, 80),
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

        public static Vector2 ScreenModifier {
            get {
                if (_ScreenModifier == null || _ScreenModifier == Vector2.Zero)
                    _ScreenModifier = new Vector2(Screen.Width / 2, Screen.Height / 2);
                return _ScreenModifier;
            }
        }

        private static Vector2 _ScreenModifier;

        public class CustomSkeleton {
            public Dictionary<JointType, Vector3> Joints = new Dictionary<JointType, Vector3>();

            public SkeletonTrackingState State = SkeletonTrackingState.NotTracked;

            public void Set(Skeleton skeleton) {
                State = skeleton.TrackingState;

                foreach (Joint Joint in skeleton.Joints) {
                    var Pos = new Vector3(Joint.Position.X, -Joint.Position.Y, Joint.Position.Z);
                    if (Joints.ContainsKey(Joint.JointType))
                        Joints[Joint.JointType] = Pos;
                    else
                        Joints.Add(Joint.JointType, Pos);
                }
            }

            public Vector3 ScaleTo(JointType type, float width, float height) {
                if (!Joints.ContainsKey(type))
                    return Vector3.Zero;
                return new Vector3(Joints[type].X * width + ScreenModifier.X, Joints[type].Y * height + ScreenModifier.Y,
                    Joints[type].Z);
            }
        }

        private void OnSkeletonUpdated(object sender, SkeletonFrameReadyEventArgs e) {
            // The live feed returns updates from the camera:
            var SkeletonFrame = e.OpenSkeletonFrame();
            if (SkeletonFrame == null)
                return;

            // Add smoothing to prevent glitching in the skeletons' movements:
            var Raw = new Skeleton[6];
            SkeletonFrame.CopySkeletonDataTo(Raw);
            for (var I = 0; I < Raw.Length; I++) {
                if (Raw[I] == null)
                    continue;

                if (Skeletons.Count <= I || Skeletons[I] == null) {
                    // If this skeleton was just added then set it:
                    var NewSkeleton = new CustomSkeleton();
                    NewSkeleton.Set(Raw[I]);
                    Skeletons.Add(NewSkeleton);
                    continue;
                }

                if (Smoothing > 0f) {
                    // Set the state of the skeleton:
                    Skeletons[I].State = Raw[I].TrackingState;

                    // Add smoothing interpolation to each point:
                    foreach (Joint Joint in Raw[I].Joints) {
                        var Pos = new Vector3 {
                            X = Interpolate(Skeletons[I].Joints[Joint.JointType].X, Joint.Position.X, Smoothing),
                            Y = Interpolate(Skeletons[I].Joints[Joint.JointType].Y, -Joint.Position.Y, Smoothing),
                            Z = Interpolate(Skeletons[I].Joints[Joint.JointType].Z, Joint.Position.Z, Smoothing)
                        };

                        Skeletons[I].Joints[Joint.JointType] = Pos;
                    }
                }
                else {
                    // Store the raw data for the skeleton:
                    Skeletons[I].Set(Raw[I]);
                }
            }

            SkeletonFrame.Dispose();

            if (Raw != null && Raw.Any(o => o.TrackingState == SkeletonTrackingState.Tracked)) {
                SkeletonActive = true;
                DebugSkeleton = "Skeleton Found";
            }
            else {
                SkeletonActive = false;
                DebugSkeleton = "No Skeleton Found";
            }
        }

        #endregion
    }
}

//challenges
//1. Add an actual rectangle for the head
//2. only draw the head and hands if the values for the coordinates are not zero
//3. get the x,y,z values of another joint and display some type of interaction with it