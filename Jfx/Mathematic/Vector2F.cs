using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.Mathematic
{
    public interface IVector2F : IVector
    {
        public float X { get; }
        public float Y { get; }
    }

    public readonly struct Vector2F : IVector2F
    {
        private static readonly Vector2F zero = new Vector2F(0, 0);
        public static ref readonly Vector2F Zero => ref zero;

        public float X { get; }
        public float Y { get; }

        public Vector2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2F operator +(in Vector2F left, in Vector2F right)
        {
            return new Vector2F(
                left.X + right.X,
                left.Y + right.Y
            );
        }

        public static Vector2F operator -(in Vector2F left, in Vector2F right)
        {
            return new Vector2F(
                left.X - right.X,
                left.Y - right.Y
            );
        }

        public static Vector2F operator *(in Vector2F left, float right)
        {
            return new Vector2F(
                left.X * right,
                left.Y * right
            );
        }
    }
}
