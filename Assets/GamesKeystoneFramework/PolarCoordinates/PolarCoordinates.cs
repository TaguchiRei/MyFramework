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


        //以下は近似値を保存してある
        private static readonly PolarCoordinates upCoordinates = new(1, 1.5707964f);
        private static readonly PolarCoordinates downCoordinates = new(1, -1.5707964f);
        private static readonly PolarCoordinates leftCoordinates = new(1, 3.1415927f);
        private static readonly PolarCoordinates rightCoordinates = new(1, 0);

        /// <summary>
        /// 初期化等を可能にする
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="angle"></param>
        public PolarCoordinates(float radius, float angle)
        {
            this.radius = radius;
            this.angle = angle;
        }

        /// <summary>
        /// 個別の初期化を作る
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
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



        /// <summary>
        /// Vector2に変換を行う
        /// </summary>
        /// <param name="polarCoordinates"></param>
        /// <returns></returns>
        public Vector2 ToVector2(PolarCoordinates polarCoordinates)
        {
            return new Vector2(polarCoordinates.radius * Mathf.Cos(angle), polarCoordinates.radius * Mathf.Sin(angle));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return radius.GetHashCode() ^ (angle.GetHashCode() << 2);
        }

        public static PolarCoordinates Up
        {
            get { return upCoordinates; }
        }
        public static PolarCoordinates Down
        {
            get { return downCoordinates; }
        }
        public static PolarCoordinates Left
        {
            get { return leftCoordinates; }
        }
        public static PolarCoordinates Right
        {
            get { return rightCoordinates; }
        }

        //汎用メソッドなどを書く
        public PolarCoordinates AngleNormalizeZeroBase(PolarCoordinates polarCoordinates)
        {
            angle = polarCoordinates.angle % (2 * Mathf.PI); // まずは0から2πに収める
            if (angle > Mathf.PI)
                angle -= 2 * Mathf.PI; // 2πを引いて-πからπに収める
            else if (angle <= -Mathf.PI)
                angle += 2 * Mathf.PI;
            return new PolarCoordinates(polarCoordinates.radius, angle);
        }

        public PolarCoordinates AngleNormalizePIMax(PolarCoordinates polarCoordinates)
        {
            angle = polarCoordinates.angle % (2 * Mathf.PI); // まずは0から2πに収める
            if (angle < 0)
                angle += 2 * Mathf.PI; // 負の角度を2πを加えて正の範囲に
            return new PolarCoordinates(polarCoordinates.radius,angle);
        }


        /// <summary>
        /// 要素が等しいかを確認する
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector2 other2)
                return Equals(obj);
            return false;
        }

        /// <summary>
        /// 要素が等しいかを確認する
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(PolarCoordinates other)
        {
            return radius == other.radius && angle == other.angle;
        }

        /// <summary>
        /// string形式で出力する
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// String形式で出力する
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F2";
            formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;

            return $"({radius.ToString(format, formatProvider)}, {angle.ToString(format, formatProvider)})";
        }

        //
        //以下は算術演算子のオーバーロード。
        //通常のVector2等と同じように計算しても意味がなく効率が悪いので、計算のルールを特殊なものに
        //
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator +(PolarCoordinates a, PolarCoordinates b)
        {
            return new PolarCoordinates(a.radius + b.radius, a.angle + b.angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator -(PolarCoordinates a, PolarCoordinates b)
        {
            return new PolarCoordinates(a.radius - b.radius, a.angle - b.angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator *(PolarCoordinates a, PolarCoordinates b)
        {
            return new PolarCoordinates(a.radius * b.radius, a.angle * b.angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator /(PolarCoordinates a, PolarCoordinates b)
        {
            return new PolarCoordinates(a.radius / b.radius, a.angle / b.angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator -(PolarCoordinates a)
        {
            return new PolarCoordinates(0f - a.radius, 0f - a.angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator *(PolarCoordinates a, float b)
        {
            return new PolarCoordinates(a.radius * b, a.angle * b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator *(float a, PolarCoordinates b)
        {
            return new PolarCoordinates(b.radius * a, b.angle * a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator /(PolarCoordinates a, float b)
        {
            return new PolarCoordinates(a.radius * b, a.angle / b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinates operator /(float a, PolarCoordinates b)
        {
            return new PolarCoordinates(b.radius * a, b.angle * a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PolarCoordinates a, PolarCoordinates b)
        {
            float num = a.radius - b.radius;
            float num2 = a.angle - b.angle;
            return num * num + num2 * num2 < 9.99999944E-11f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PolarCoordinates a, PolarCoordinates b)
        {
            return !(a == b);
        }
        //暗黙的な型変換を後で作る(三次元極座標)
    }
    public class PolarCoordinatesSupport
    {
        public PolarCoordinates ToPolarCoordinates(float x, float y)
        {
            return new PolarCoordinates((float)Math.Sqrt(x * x + y * y), (float)Math.Atan(y / x));
        }
    }
}