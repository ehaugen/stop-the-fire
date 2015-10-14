using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StopTheFire
{
    public class SpriteSheet
    {
        int currentFrame = 0;
        int counter = 0;

        int frameWidth;
        int frameHeight;
        int frameSpeed;
        int endFrame;

        Texture2D spriteSheet;

        public SpriteSheet(Texture2D spriteSheet, int frameWidth, int frameHeight, int frameSpeed, int endFrame)
        {
            this.spriteSheet = spriteSheet;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.frameSpeed = frameSpeed;
            this.endFrame = endFrame;            
        }

        public void Update()
        {
            // update to the next frame if it is time
            if (counter == (frameSpeed - 1))
                currentFrame = (currentFrame + 1) % endFrame;

            // update the counter
            counter = (counter + 1) % frameSpeed;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            var source = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            var origin = new Vector2(frameWidth / 2.0f, frameHeight);           

            spriteBatch.Begin();
            spriteBatch.Draw(spriteSheet, position, source, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.End();
        }
    }
}
