namespace YandexApi
{
    using  System.Collections.Generic;
    public static class LangPair
    {
        public static Dictionary<int, string> LangPairs = new Dictionary<int, string>();

        static LangPair()
        {
            LangPairs.Add((int) Lang.EnRu, "en-ru");
            LangPairs.Add((int) Lang.RuEn, "ru-en");  
        }
    }
}