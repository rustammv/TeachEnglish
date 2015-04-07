using System.Collections.Generic;

namespace YandexApi
{
    public class YTranslation
    {
        public string AttrPartOfSpeech;
        public string AttrGender;
        public string AttrNum;
        public string AttrTranslation;

        public string AttrText;
        
        public List<YSynonym> Synonyms = new List<YSynonym>();
        public List<YMean> Means = new List<YMean>();
        public List<YExample> Examples = new List<YExample>();

    }
}