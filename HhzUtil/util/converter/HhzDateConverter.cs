using System;
using System.Globalization;


namespace HhzUtil.util.converter
{
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Utilities;

    public class HhzDateConverter : DateTimeConverterBase
    {

        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd";

        private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
        private string _dateTimeFormat;
        private CultureInfo _culture;

        /// <summary>
        /// Gets or sets the date time styles used when converting a date to and from JSON.
        /// </summary>
        /// <value>The date time styles used when converting a date to and from JSON.</value>
        public DateTimeStyles DateTimeStyles
        {
            get { return _dateTimeStyles; }
            set { _dateTimeStyles = value; }
        }

        /// <summary>
        /// Gets or sets the date time format used when converting a date to and from JSON.
        /// </summary>
        /// <value>The date time format used when converting a date to and from JSON.</value>
        public string DateTimeFormat
        {
            get { return _dateTimeFormat ?? string.Empty; }
            set { _dateTimeFormat = (string.IsNullOrEmpty(value)) ? null : value; }
        }

        /// <summary>
        /// Gets or sets the culture used when converting a date to and from JSON.
        /// </summary>
        /// <value>The culture used when converting a date to and from JSON.</value>
        public CultureInfo Culture
        {
            get { return _culture ?? CultureInfo.CurrentCulture; }
            set { _culture = value; }
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string text;

            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;

                if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                    || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                    dateTime = dateTime.ToUniversalTime();

                text = dateTime.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);
            }
            #if !NET20
            else if (value is DateTimeOffset)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
                if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                    || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                    dateTimeOffset = dateTimeOffset.ToUniversalTime();

                text = dateTimeOffset.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);
            }
            #endif
            else
            {
                throw new JsonSerializationException("Unexpected value when converting date. Expected DateTime or DateTimeOffset");
            }

            writer.WriteValue(text);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool nullable = (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>));
            #if !NET20
            Type t = (nullable)
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;
            #endif

            if (reader.TokenType == JsonToken.Null)
            {
                if (!(objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    throw new JsonSerializationException("Cannot convert null value.");

                return null;
            }

            if (reader.TokenType == JsonToken.Date)
            {
            #if !NET20
                if (t == typeof(DateTimeOffset))
                    return reader.Value is DateTimeOffset ? reader.Value : new DateTimeOffset((DateTime)reader.Value);
            #endif

                return reader.Value;
            }

            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException("Unexpected token parsing date. Expected String, got");

            string dateText = reader.Value.ToString();

            if (string.IsNullOrEmpty(dateText) && nullable)
                return null;

            #if !NET20
            if (t == typeof(DateTimeOffset))
            {
                if (!string.IsNullOrEmpty(_dateTimeFormat))
                    return DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
                else
                    return DateTimeOffset.Parse(dateText, Culture, _dateTimeStyles);
            }
            #endif

            if (!string.IsNullOrEmpty(_dateTimeFormat))
                return DateTime.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
            else
                return DateTime.Parse(dateText, Culture, _dateTimeStyles);
        }
    }
}
