using System;

namespace GamesKeystoneFramework.KeyMathBit
{
    public readonly struct DUlong : IEquatable<DUlong>
    {
        private readonly ulong _low;  // 下位64ビット
        private readonly ulong _high; // 上位64ビット

        public DUlong(ulong high, ulong low)
        {
            _high = high;
            _low = low;
        }

        public static DUlong operator <<(DUlong a, int bits)
        {
            switch (bits)
            {
                case < 0:
                case > 127:
                    throw new ArgumentOutOfRangeException(nameof(bits), "Shift amount must be between 0 and 127.");
                case 0:
                    return a;
                case < 64:
                {
                    ulong newHigh = (a._high << bits) | (a._low >> (64 - bits));
                    ulong newLow = a._low << bits;
                    return new DUlong(newHigh, newLow);
                }
                default:
                {
                    ulong newHigh = a._low << (bits - 64);
                    return new DUlong(newHigh, 0);
                }
            }
        }

        public static DUlong operator >>(DUlong a, int bits)
        {
            switch (bits)
            {
                case < 0:
                case > 127:
                    throw new ArgumentOutOfRangeException(nameof(bits), "Shift amount must be between 0 and 127.");
                case 0:
                    return a;
                case < 64:
                {
                    ulong newLow = (a._low >> bits) | (a._high << (64 - bits));
                    ulong newHigh = a._high >> bits;
                    return new DUlong(newHigh, newLow);
                }
                default:
                {
                    ulong newLow = a._high >> (bits - 64);
                    return new DUlong(0, newLow);
                }
            }
        }

        public static DUlong operator &(DUlong a, DUlong b)
        {
            return new DUlong(a._high & b._high, a._low & b._low);
        }

        public static DUlong operator &(DUlong a, ulong b)
        {
            return new DUlong(a._high, a._low & b);
        }

        public static DUlong operator |(DUlong a, DUlong b)
        {
            return new DUlong(a._high | b._high, a._low | b._low);
        }

        public static DUlong operator |(DUlong a, ulong b)
        {
            return new DUlong(a._high, a._low | b);
        }

        public static DUlong operator ^(DUlong a, DUlong b)
        {
            return new DUlong(a._high ^ b._high, a._low ^ b._low);
        }

        public static DUlong operator ~(DUlong a)
        {
            return new DUlong(~a._high, ~a._low);
        }

        public bool Equals(DUlong other)
        {
            return this._high == other._high && this._low == other._low;
        }

        public override bool Equals(object obj)
        {
            return obj is DUlong other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_high, _low);
        }

        public override string ToString()
        {
            return $"0x{_high:X16}{_low:X16}";
        }
    }
}
