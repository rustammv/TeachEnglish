using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexApi
{
    public class YDictionary
    {
        public string AttrPartOfSpeech;
        public string AttrGender;
        public string AttrNum;
        public string AttrTranscription;

        public string AttrText;
        
        public List<YTranslation> Translations = new List<YTranslation>();
    }

    
    
}
