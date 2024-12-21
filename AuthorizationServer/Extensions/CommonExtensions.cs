using System.Text;

namespace AuthorizationServer.Extensions
{
    public static class CommonExtensions
    {
        public enum TrimType
        {
            Comma,
            Pipe,
            Colon
        }

        public static string EmptyIfNull(this string value)
        {
            return value == null ? string.Empty : value.ToString();
        }

        public static string UCleanupPipeConcat(this string value)
        {
            string a = EmptyIfNull(value);
            if (a.EndsWith("|")) { a = a.Remove(a.Length - 1, 1); }
            if (a == "|") { a = string.Empty; }

            return a;
        }

        public static string TrimDuplicates(this string input, TrimType trimType)
        {
            string result = string.Empty;

            switch (trimType)
            {
                case TrimType.Comma:
                    result = input.TrimCharacter(',');
                    break;
                case TrimType.Pipe:
                    result = input.TrimCharacter('|');
                    break;
                case TrimType.Colon:
                    result = input.TrimCharacter(':');
                    break;
                default:
                    break;
            }

            return result;
        }

        private static string TrimCharacter(this string input, char character)
        {
            string result = string.Empty;

            while (input.EndsWith(character.ToString()))
            {
                input = input.Remove(input.Length - 1, 1);
            }

            if (input == character.ToString()) { input = string.Empty; }

            result = string.Join(character.ToString(), input.Split(character)
                .Where(str => str != string.Empty)
                .ToArray());

            return result;
        }

        public static string Left(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str)) return str;
            maxLength = Math.Abs(maxLength);
            return str.Length <= maxLength ? str : str[..maxLength];
        }

        public static string Right(this string value, int length)
        {
            return value[^length..];
        }

        public static bool ArrayIsNullOrEmpty(string[] strArray)
        {
            return strArray == null || strArray.Length < 1;
        }

        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
            //if (enumerable == null)
            //{
            //    return true;
            //}
            ///* If this is a list, use the Count property. 
            // * The Count property is O(1) while IEnumerable.Count() is O(N). */
            //if (enumerable is ICollection<T> collection)
            //{
            //    return collection.Count < 1;
            //}
            //return enumerable.Any();
        }

        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="collection">The collection, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count < 1;
        }

        /// <summary>Enumerates get flags in this collection.</summary>
        ///
        /// <param name="value">The value.
        /// </param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process get flags in this collection.</returns>
        public static IEnumerable<T> GetFlags<T>(this T value) where T : struct
        {
            return GetFlags(value, Enum.GetValues(value.GetType()).Cast<T>().ToArray());
        }

        /// <summary>Enumerates get flags in this collection.</summary>
        ///
        /// <param name="value"> The value.
        /// </param>
        /// <param name="values">The values.
        /// </param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process get flags in this collection.</returns>
        private static IEnumerable<T> GetFlags<T>(T value, T[] values) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Type must be an enum.");
            }
            ulong bits = Convert.ToUInt64(value);
            var results = new List<T>();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                ulong mask = Convert.ToUInt64(values[i]);
                if (i == 0 && mask == 0L)
                    break;
                if ((bits & mask) == mask)
                {
                    results.Add(values[i]);
                    bits -= mask;
                }
            }
            return bits != 0L
                ? Enumerable.Empty<T>()
                : Convert.ToUInt64(value) != 0L
                ? results.Reverse<T>()
                : bits == Convert.ToUInt64(value) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L
                ? values.Take(1)
                : Enumerable.Empty<T>();
        }

        public static FileStorageLocations GetMaxFileStorageLocationValue(FileStorageLocations id)
        {
            //FileStorageLocations fid = int64(id);
            return id.GetFlags().Max();
        }

        // var maxStatus = Item.Status.GetFlags().Max();
        //                FileStorageLocations storageLocationId = FileStorageLocations.NotDefined;
        //                        storageLocationId |= FileStorageLocations.WebShare; // Webshare file already existed
        //storageLocationId |= FileStorageLocations.AzureStorage; // AzureStorage file NOW exists
        public static string StripPunctuationAndSpace(this string s)
        {
            // Note: the following are not punctuation: $^+|<>= –
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                if (!char.IsPunctuation(c) && !char.IsWhiteSpace(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }

}
