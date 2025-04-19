using System;
using Unity.Netcode;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public struct MultiPlayData : INetworkSerializable
    {
        public DataType dataType;
        public string serializedValue;

        public object Value
        {
            get
            {
                object obj = dataType switch
                {
                    DataType.Integer => int.TryParse(serializedValue, out var intValue) ? intValue : 0,
                    DataType.Float => float.TryParse(serializedValue, out var floatValue) ? floatValue : 0,
                    DataType.String => serializedValue,
                    DataType.Bool => bool.TryParse(serializedValue, out var boolValue) ? boolValue : false,
                    DataType.Char => char.TryParse(serializedValue, out var charValue) ? charValue : '\0',
                    _ => null
                };
                return obj;
            }
            set
            {
                serializedValue = value.ToString();
                dataType = value switch
                {
                    int => DataType.Integer,
                    float => DataType.Float,
                    string => DataType.String,
                    bool => DataType.Bool,
                    char => DataType.Char,
                    _ => throw new InvalidOperationException($"未対応の型　: {value.GetType()}")
                };
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref dataType);
            serializer.SerializeValue(ref serializedValue);
        }
    }

    public enum DataType
    {
        Integer,
        Float,
        Bool,
        String,
        Char,
    }
}