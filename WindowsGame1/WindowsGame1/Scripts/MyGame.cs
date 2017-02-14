using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyKinectGame;

namespace KinectDemo.Scripts {
    public class MyGame : Game {
        /**********************************************************************************************/
        // Utility Methods:

        public enum ScreenSpace {
            World = 0,
            Screen
        }

        // This is a reference to your game and all of its data:
        public static MyGame Instance;

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


        // GameState Handlers:

        private float Elapsed;
        private readonly Color DefaultColor = Color.Black;


        /**********************************************************************************************/
        // Example GameState System:

        public void SetupGameStates() {
            GameState.Add("titlescreen", OnUpdate_TitleScreen, null, OnEnter_TitleScreen, OnExit_TitleScreen);
            GameState.Add("level1", OnUpdate_Level1, null, OnEnter_Level1);
            GameState.Add("level2", OnUpdate_Level2);
            GameState.Add("endscreen", OnUpdate_Endscreen);

            // Set the initial GameState:
            GameState.Set("titlescreen");
        }

        public void OnUpdate_TitleScreen() {
            
            //TODO Think of name for simulation
            DrawText("Welcome to [NAME]!", new Vector2(500, 20), DefaultColor);
            DrawText("Place both hands perpendicular to body for 3 seconds to enter simulation", new Vector2(20, 100), DefaultColor);

            // Testing Comparatives 
            if (IsTouching(JointType.HandLeft, JointType.Head))
                DrawText("Left hand is touching head!", new Vector2(20, 300), DefaultColor);
            var LeftHandtoOthers = new JointComparatives();
            LeftHandtoOthers.GetJointInfo(JointType.HandLeft, JointType.AnkleLeft, JointType.AnkleRight,
                JointType.WristRight, JointType.Head);
            if (LeftHandtoOthers.IsHighest())
                DrawText("Left Hand is highest!", new Vector2(40, 300), DefaultColor);
            if (LeftHandtoOthers.IsLowest())
                DrawText("Left Hand is lowest", new Vector2(40, 300), DefaultColor);

//            var RightHandtoBody = new JointComparatives();
//            RightHandtoBody.GetJointInfo(JointType.HandRight, JointType.ShoulderRight, JointType.ElbowRight,
//                JointType.HipRight, JointType.WristRight, JointType.AnkleRight, JointType.KneeRight);
//            var LeftHandtoBody = new JointComparatives();
//            LeftHandtoBody.GetJointInfo(JointType.HandLeft, JointType.ElbowLeft, JointType.HipLeft, 
//                JointType.WristLeft, JointType.ShoulderLeft, JointType.AnkleLeft, JointType.KneeLeft);
//            if (RightHandtoBody.IsFarthestRight() && LeftHandtoBody.IsFarthestLeft()) {
//                DrawText("Hold Arms for 3 seconds...", new Vector2(20,400), DefaultColor);
//                Elapsed += DrawDelta;
//            }
//            if (DrawDelta >= 3) {
//                GameState.Set("level1");
//            }
            
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

        public static bool IsTouching(JointType joint1, JointType joint2) {
            if (JointComparatives.GetJointPosition(joint1, ScreenSpace.World) == Vector3.Zero) return false;
            return
                Vector3.Distance(JointComparatives.GetJointPosition(joint1, ScreenSpace.World),
                    JointComparatives.GetJointPosition(joint2, ScreenSpace.World)) < 60f;
        }

        public void DrawText(string text, Vector2 pos, Color color, string font = "font_20") { // TODO figure out why I can't set a default value for color 
            Renderer.DrawString(Resources.Fonts.Load(font), text, pos, color);
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

        public void DrawSkeleton(CustomSkeleton skeleton) {
            if (skeleton == null)
                return;

            DrawJointConnection(skeleton, JointType.Head, JointType.ShoulderCenter);
            DrawJointConnection(skeleton, JointType.ShoulderCenter, JointType.ShoulderRight);
            DrawJointConnection(skeleton, JointType.ShoulderRight, JointType.ElbowRight);
            DrawJointConnection(skeleton, JointType.ElbowRight, JointType.HandRight);
            DrawJointConnection(skeleton, JointType.ShoulderCenter, JointType.ShoulderLeft);
            DrawJointConnection(skeleton, JointType.ShoulderLeft, JointType.ElbowLeft);
            DrawJointConnection(skeleton, JointType.ElbowLeft, JointType.HandLeft);
            DrawJointConnection(skeleton, JointType.ShoulderCenter, JointType.HipCenter);
            DrawJointConnection(skeleton, JointType.HipCenter, JointType.HipRight);
            DrawJointConnection(skeleton, JointType.HipRight, JointType.KneeRight);
            DrawJointConnection(skeleton, JointType.KneeRight, JointType.FootRight);
            DrawJointConnection(skeleton, JointType.HipCenter, JointType.HipLeft);
            DrawJointConnection(skeleton, JointType.HipLeft, JointType.KneeLeft);
            DrawJointConnection(skeleton, JointType.KneeLeft, JointType.FootLeft);
        }

        public void DrawJointConnection(CustomSkeleton skeleton, JointType joint1, JointType joint2) {
            DrawLine(Renderer, 4, Color.Blue, JointToVector(skeleton, joint1), JointToVector(skeleton, joint2));
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
            var CompareJoints = new JointComparatives();
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

        public static Vector2 ScreenMod {
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
                return new Vector3(Joints[type].X * width + ScreenMod.X, Joints[type].Y * height + ScreenMod.Y,
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