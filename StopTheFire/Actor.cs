using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StopTheFire
{
    public class Actor
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public BoundingBox BoundingBox;
        public Color Color;
        Vector3 halfScale;
        Vector2 mapBound;

        public Actor(Vector3 position, Vector3 scale, Color color)
        {
            this.Position = position;
            this.Color = color;
            halfScale = scale * 0.5f;
            BoundingBox.Min = position - halfScale;
            BoundingBox.Max = position + halfScale;

            mapBound.X = Game1.Instance.MapSize.X * 0.5f;
            mapBound.Y = Game1.Instance.MapSize.Z * 0.5f;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Velocity.X != 0 || Velocity.Y != 0 || Velocity.Z != 0)
            {
                if (Position.X < -mapBound.X || Position.X + halfScale.X > mapBound.X) Velocity.X = -Velocity.X;
                if (Position.Z < -mapBound.Y || Position.Z + halfScale.Z > mapBound.Y) Velocity.Z = -Velocity.Z;

                Vector3 prevPos = Position;
                Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                BoundingBox.Min = Position - halfScale;
                BoundingBox.Max = Position + halfScale;

                QuadTree prevNode = Game1.Instance.RootQuadTree.FindLeaf(prevPos);
                Game1.Instance.RootQuadTree.ActorMoved(this, prevNode);
            }
        } 
    }
} 
