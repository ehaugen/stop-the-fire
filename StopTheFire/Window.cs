using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StopTheFire
{
    public class Window
    {
        public Texture2D Sprite { get; set; }
        public float Height 
        {
            get { return Sprite.Height * Scale; } 
        }
        public float Width 
        {
            get { return Sprite.Width * Scale; } 
        }
        public float Scale { get; set; }

        public Window(Texture2D sprite, float scale)
        {
            Sprite = sprite;
            Scale = scale;
        }
    }
}
