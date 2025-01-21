using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GamesKeystoneFramework.PolarCoordinates
{
    public struct PolarCoordinates : IEquatable<PolarCoordinates>, IFormattable
    {

        public float radius;
        public float angle;

        private static readonly PolarCoordinates zeroCoordinates = new(0f, 0f);
        private static readonly PolarCoordinates oneCoordinates = new((float)Math.Sqrt(2), (float)(Math.PI/4));
        private static readonly PolarCoordinates upCoordinates = new(1, (float)(Math.PI / 2));
        private static readonly PolarCoordinates downCoordinates = new();
        private static readonly PolarCoordinates leftCoordinates = new();
        private static readonly PolarCoordinates rightCoordinates = new();

        public PolarCoordinates(float radius, float angle)
        {
            this.radius = radius;
            this.angle = angle;
        }
        public float this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return index switch
                {
                    0 => radius,
                    1 => angle,
                    _ => throw new IndexOutOfRangeException("Invalid PolarCoordinates index!"),
                };
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                switch (index)
                {
                    case 0:
                        radius = value;
                        break;
                    case 1:
                        angle = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid PolarCoordinates index!");
                }
            }

        }

        public float SprMagnitude
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return radius * radius;
            }
        }

        public static PolarCoordinates Zero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return zeroCoordinates;
            }
        }




        public Vector2 ToVector2(PolarCoordinates polarCoordinates)
        {
            return new Vector2(polarCoordinates.radius * Mathf.Cos(angle), polarCoordinates.radius * Mathf.Sin(angle));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return radius.GetHashCode() ^ (angle.GetHashCode() << 2);
        }


        public override bool Equals(object obj)
        {
            if (obj is Vector2 other2)
                return Equals(obj);
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(PolarCoordinates other)
        {
            return radius == other.radius && angle == other.angle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return ToString(null, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F2";
            formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;

            return $"({radius.ToString(format, formatProvider)}, {angle.ToString(format, formatProvider)})";
        }
    }
}