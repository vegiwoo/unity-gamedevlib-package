using System;

// ReSharper disable once CheckNamespace
namespace GameDevLib.Helpers
{
    public static class GameDevLibExtensions 
    {
        /// <summary>
        /// Converts a string value to an enum case.
        /// </summary>
        /// <param name="value">String value to convert.</param>
        /// <param name="defaultValue">Default case value (used if an empty string is passed or conversion fails).</param>
        /// <typeparam name="T">Enum type to whose case string value is converted.</typeparam>
        /// <returns>Enum case.</returns>
        /// <remarks>
        /// Example: StatusEnum MyStatus = "Active".ToEnum(StatusEnum.None);
        /// </remarks>
        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return Enum.TryParse<T>(value, true, out T result) ? result : defaultValue;
        }

    }
}