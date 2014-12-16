using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StopTheFire
{

    public interface IActor
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        Rectangle BoundingBox { get; set; }
    }
}
