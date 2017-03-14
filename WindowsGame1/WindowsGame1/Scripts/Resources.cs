using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyKinectGame
{
    public static class Resources
    {
        public static class Images
        {
            private static Dictionary<string, Texture2D> Cache = new Dictionary<string, Texture2D>();

            public static Texture2D Pixel
            {
                get
                {
                    if (_pixel == null && MyGame.Instance != null)
                    {
                        _pixel = new Texture2D(MyGame.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                        _pixel.SetData(new[] { Color.White });
                    }

                    return _pixel;
                }
            }
            private static Texture2D _pixel;

            /**********************************************************************************************/
            // Utilities:

            public static Texture2D Load(string name)
            {
                if(!Cache.ContainsKey(name))
                {
                    Texture2D Resource = null;
                    if (MyGame.Instance != null)
                    {
                        try
                        {
                            Resource = MyGame.Instance.Content.Load<Texture2D>(name);
                        }
                        catch
                        {
                            Resource = Pixel;
                        }
                    }
                    Cache.Add(name, Resource);
                }

                return Cache[name];
            }

            public static void Unload()
            {
                // Dispose of the solid color texture if it was used:
                if (_pixel != null)
                    _pixel.Dispose();

                foreach(Texture2D Resource in Cache.Values.ToList())
                {
                    if (Resource == null || Resource.IsDisposed)
                        continue;

                    Resource.Dispose();
                }

                Cache.Clear();
            }
        }

        public static class Fonts
        {
            private static Dictionary<string, SpriteFont> Cache = new Dictionary<string, SpriteFont>();
            
            /**********************************************************************************************/
            // Utilities:

            public static SpriteFont Load(string name)
            {
                if (!Cache.ContainsKey(name))
                {
                    SpriteFont Resource = null;
                    if (MyGame.Instance != null)
                    {
                        try
                        {
                            Resource = MyGame.Instance.Content.Load<SpriteFont>(name);
                        }
                        catch
                        {
                            Resource = null;
                        }
                    }
                    Cache.Add(name, Resource);
                }

                return Cache[name];
            }

            public static void Unload()
            {
                Cache.Clear();
            }
        }

        public static void Unload()
        {
            Images.Unload();
            Fonts.Unload();

            // Unload all other assets that were loaded:
            MyGame.Instance.Content.Unload();
        }
    }
}
