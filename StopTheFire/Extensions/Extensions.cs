using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace StopTheFire.Extensions
{
    public static class Extensions
    {
        public static Point Origin(this Vector2 v)
        {
            return new Point(Convert.ToInt32(v.X), Convert.ToInt32(v.Y));
        }
    }
}
