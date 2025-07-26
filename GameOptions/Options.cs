namespace GameOptions
{
    public class Options
    {
        public enum Languages
        {
            Ru,
            En
        };

        public static Languages Language { get; private set; }

        public static void SetLanguage(Languages newLanguage)
        {
            Language = newLanguage;
        }
    }
}