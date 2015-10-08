using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StopTheFire.Particles;

namespace StopTheFire
{
    public class Building
    {
        public List<Vector2> Windows { get; set; }
        public Window Window;
        public Vector2 Position { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public Texture2D Tile { get; set; }
        public float Columns { get; set; }
        public float Rows { get; set; }
        public ParticleSystem FireParticleSystem { get; set; }
        public Vector2 FireHeightMax { get; set; }
        public Vector2 FireHeight { get; set; }

        public Building(Texture2D tile, Vector2 position, int width, int height, Window window) 
        {
            Tile = tile;
            Position = position;
            Height = height;
            Width = width;            
            AssignWindows(window);
        }

        private void AssignWindows(Window window)
        {
            Window = window;
            Windows = new List<Vector2>();

            float x;
            float y;
            
            float columnSpacing;
            float rowSpacing;

            Columns = (float)Math.Floor(Width / (window.Width * 2));
            Rows = (float)Math.Floor(Height / (window.Height * 2));
            columnSpacing = Width / Columns;
            rowSpacing = Height / Rows;

            for(int i=1; i<=Columns; i++)
            {
                for(int j=1;j<=Rows;j++)
                {
                    x = (columnSpacing * i) - (window.Width/2) + Position.X - columnSpacing/2;
                    y = (rowSpacing * j) - (window.Height / 2) + Position.Y - rowSpacing/2;
                    Windows.Add(new Vector2(x, y));
                }
            }
        }

        public void Clear()
        {
            // TODO: clear windows
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(Tile, Position, new Rectangle(0, 0, Width, Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.End();

            spriteBatch.Begin();
            foreach(Vector2 position in Windows)
            {
                spriteBatch.Draw(Window.Sprite, position, Color.White);
            }
            spriteBatch.End();
        }
    }
}
