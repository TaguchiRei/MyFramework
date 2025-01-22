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

        //�ȉ��͋ߎ��l��ۑ����Ă���
        private static readonly PolarCoordinates upCoordinates = new(1, 1.5707964f);
        private static readonly PolarCoordinates downCoordinates = new(1, -1.5707964f);
        private static readonly PolarCoordinates leftCoordinates = new(1, 3.1415927f);
        private static readonly PolarCoordinates rightCoordinates = new(1, 0);

        /// <summary>
        /// �����������\�ɂ���
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="angle"></param>
        public PolarCoordinates(float radius, float angle)
        {
            this.radius = radius;
            this.angle = angle;
        }

        /// <summary>
        /// �ʂ̏����������
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
        /// Vector2�ɕϊ����s��
        /// </summary>
        /// <param name="polarCoordinates"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 ToVector2()
        {
            return new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
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

        //�ėp���\�b�h�Ȃǂ�����
        /// <summary>
        /// 0����2�΂̊Ԃɂ܂Ƃ߂�
        /// </summary>
        /// <param name="polarCoordinates"></param>
        /// <returns></returns>
        public PolarCoordinates AngleNormalizeZeroBase(PolarCoordinates polarCoordinates)
        {
            angle = polarCoordinates.angle % (2 * Mathf.PI); // �܂���0����2�΂Ɏ��߂�
            if (angle > Mathf.PI)
                angle -= 2 * Mathf.PI; // 2�΂�������-�΂���΂Ɏ��߂�
            else if (angle <= -Mathf.PI)
                angle += 2 * Mathf.PI;
            return new PolarCoordinates(polarCoordinates.radius, angle);
        }

        /// <summary>
        /// -�΂���΂̊ԂɊۂ߂�
        /// </summary>
        /// <param name="polarCoordinates"></param>
        /// <returns></returns>
        public PolarCoordinates AngleNormalizePIMax(PolarCoordinates polarCoordinates)
        {
            angle = polarCoordinates.angle % (2 * Mathf.PI); // �܂���0����2�΂Ɏ��߂�
            if (angle < 0)
                angle += 2 * Mathf.PI; // ���̊p�x��2�΂������Đ��͈̔͂�
            return new PolarCoordinates(polarCoordinates.radius, angle);
        }


        /// <summary>
        /// �v�f�������������m�F����
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is Vector2 other2)
                return Equals(obj);
            return false;
        }

        /// <summary>
        /// �v�f�������������m�F����
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(PolarCoordinates other)
        {
            return radius == other.radius && angle == other.angle;
        }

        /// <summary>
        /// string�`���ŏo�͂���
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// String�`���ŏo�͂���
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
        //�ȉ��͎Z�p���Z�q�̃I�[�o�[���[�h�B
        //�ʏ��Vector2���Ɠ����悤�Ɍv�Z���Ă��Ӗ����Ȃ������������̂ŁA�v�Z�̃��[�������Ȃ��̂�
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
        //�ÖٓI�Ȍ^�ϊ�����ō��(�O�����ɍ��W)
    }
    public class PolarCoordinatesSupport
    {
        public PolarCoordinates ToPolarCoordinates(float x, float y)
        {
            return new PolarCoordinates((float)Math.Sqrt(x * x + y * y), (float)Math.Atan(y / x));
        }
        public PolarCoordinates ToPolarCoordinates(Vector2 vector2)
        {
            return new PolarCoordinates((float)Math.Sqrt(vector2.x * vector2.x + vector2.y * vector2.y), (float)Math.Atan(vector2.y / vector2.x));
        }
        public float GetAngle(float x, float y)
        {
            return (float)Math.Sqrt(x * x + y * y);
        }
        public float GetAngle(Vector2 vector2)
        {
            return (float)Math.Sqrt(vector2.x * vector2.x + vector2.y * vector2.y);
        }
        public float GetRadius(float x, float y)
        {
            return (float)(Math.Atan(y / x));
        }
        public float GetRadius(Vector2 vector2)
        {
            return (float)(Math.Atan(vector2.y / vector2.x));
        }
    }
}