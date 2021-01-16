namespace _7Wonders
{
    internal static class TextHelper
    {
        /// <summary>
        /// Turns the given noun into a plural if required.
        /// </summary>
        /// <param name="noun">Word that we want to make plural.</param>
        /// <param name="count">Count to check if we need to make a plural. Won't be made plural if the count is exactly 1.</param>
        /// <returns>The plural form of the noun unless count is 1.</returns>
        /// <remarks>Only works for regular plurals obtained by adding "s" to the end.</remarks>
        public static string Pluralize(string noun, int count)
        {
            return count == 1 ? noun : $"{noun}s";
        }
    }
}
