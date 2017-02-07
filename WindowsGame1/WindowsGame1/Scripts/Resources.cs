﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectDemo.Scripts;

namespace MyKinectGame
{
    public static class Resources
    {
        public static class Images
        {
            private static Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();

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
                if(!_cache.ContainsKey(name))
                {
                    Texture2D resource = null;
                    if (MyGame.Instance != null)
                    {
                        try
                        {
                            resource = MyGame.Instance.Content.Load<Texture2D>(name);
                        }
                        catch
                        {
                            resource = Pixel;
                        }
                    }
                    _cache.Add(name, resource);
                }

                return _cache[name];
            }

            public static void Unload()
            {
                // Dispose of the solid color texture if it was used:
                if (_pixel != null)
                    _pixel.Dispose();

                foreach(Texture2D resource in _cache.Values.ToList())
                {
                    if (resource == null || resource.IsDisposed)
                        continue;

                    resource.Dispose();
                }

                _cache.Clear();
            }
        }

        public static class Fonts
        {
            private static Dictionary<string, SpriteFont> _cache = new Dictionary<string, SpriteFont>();
            
            /**********************************************************************************************/
            // Utilities:

            public static SpriteFont Load(string name)
            {
                if (!_cache.ContainsKey(name))
                {
                    SpriteFont resource = null;
                    if (MyGame.Instance != null)
                    {
                        try
                        {
                            resource = MyGame.Instance.Content.Load<SpriteFont>(name);
                        }
                        catch
                        {
                            resource = null;
                        }
                    }
                    _cache.Add(name, resource);
                }

                return _cache[name];
            }

            public static void Unload()
            {
                _cache.Clear();
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
