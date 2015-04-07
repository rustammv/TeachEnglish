using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Xml;

namespace YandexApi
{
    public class ApiDictionary
    {
        public const string XmlInterfaceUrl = "https://dictionary.yandex.net/api/v1/dicservice/lookup";

        private readonly string apiKey;

        private string xmlInterface;
        public string ApiKey
        {
            get { return apiKey; }
        }

        public ApiDictionary(string apiKey)
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                this.apiKey = apiKey;
            }
        }

        protected void CreateStringRequest(Lang directionOfTranslation, string textTranslator)
        {
            string lang = LangPair.LangPairs[(int)directionOfTranslation];
            this.xmlInterface = string.Format(@"{0}?key={1}&lang={2}&text={3}", XmlInterfaceUrl, apiKey, lang, textTranslator);
        }

        protected YExample GetYExample(XmlElement node)
        {
            YExample example = new YExample
            {
                AttrGender = node.GetAttribute("gen"),
                AttrNum = node.GetAttribute("num"),
                AttrPartOfSpeech = node.GetAttribute("pos"),
                AttrText = node.GetAttribute("text")
            };

            foreach (XmlElement nodeChild in node.ChildNodes)
            {
                if (nodeChild.LocalName == "text")
                {
                    example.Example = nodeChild.InnerText;
                }
                if (nodeChild.LocalName == "tr")
                {
                    foreach (XmlElement nodeExTr in nodeChild.ChildNodes)
                    {
                        if (nodeExTr.LocalName == "text")
                        {
                            example.ExampleTranslation = nodeExTr.InnerText;
                        }
                    }
                }
            }
            return example;
        }

        protected YMean GetYMean(XmlElement node)
        {
            YMean mean = new YMean
            {
                AttrGender = node.GetAttribute("gen"),
                AttrNum = node.GetAttribute("num"),
                AttrPartOfSpeech = node.GetAttribute("pos"),
                AttrText = node.GetAttribute("text")
            };
            foreach (XmlElement nodeChild in node.ChildNodes)
            {
                if (nodeChild.LocalName == "text")
                {
                    mean.Mean = nodeChild.InnerText;
                }
                if (nodeChild.LocalName == "tr")
                {
                    foreach (XmlElement nodeMeanTr in nodeChild.ChildNodes)
                    {
                        if (nodeMeanTr.LocalName == "text")
                        {
                            mean.MeanTranslation = nodeMeanTr.InnerText;
                        }
                    }
                }
            }
            return mean;
        }

        protected YSynonym GetYSynonym(XmlElement node)
        {
            YSynonym synonym = new YSynonym
            {
                AttrGender = node.GetAttribute("gen"),
                AttrNum = node.GetAttribute("num"),
                AttrPartOfSpeech = node.GetAttribute("pos"),
                AttrText = node.GetAttribute("text")
            };

            foreach (XmlElement nodeChild in node.ChildNodes)
            {
                if (nodeChild.LocalName == "text")
                {
                    synonym.Synonym = nodeChild.InnerText;
                }
                if (nodeChild.LocalName == "tr")
                {
                    foreach (XmlElement nodeSynTr in nodeChild.ChildNodes)
                    {
                        if (nodeSynTr.LocalName == "text")
                        {
                            synonym.SynonymTranslation = nodeSynTr.InnerText;
                        }
                    }
                }
            }
            return synonym;
        }

        protected YTranslation GetYTranslation(XmlElement node)
        {
            YTranslation tr = new YTranslation
            {
                AttrGender = node.GetAttribute("gen"),
                AttrNum = node.GetAttribute("num"),
                AttrPartOfSpeech = node.GetAttribute("pos"),
                AttrText = node.GetAttribute("text")
            };
            foreach (XmlElement nodeChild in node.ChildNodes)
            {
                if (nodeChild.LocalName == "text")
                {
                    tr.AttrTranslation = nodeChild.InnerText;
                }
                if (nodeChild.LocalName == "mean")
                {
                    YMean mean = GetYMean(nodeChild);
                    tr.Means.Add(mean);
                }
                if (nodeChild.LocalName == "ex")
                {
                    YExample example = GetYExample(nodeChild);
                    tr.Examples.Add(example);
                }
                if (nodeChild.LocalName == "syn")
                {
                    YSynonym synonym = GetYSynonym(nodeChild);
                    tr.Synonyms.Add(synonym);
                }
            }
            return tr;
        }

        protected YDictionary GetYDictionary(XmlElement node)
        {
            YDictionary dict = new YDictionary
            {
                AttrGender = node.GetAttribute("gen"),
                AttrNum = node.GetAttribute("num"),
                AttrPartOfSpeech = node.GetAttribute("pos"),
                AttrTranscription = node.GetAttribute("ts")
            };

            // getting all childnodes of node "def"
            foreach (XmlElement nodeChild in node.ChildNodes)
            {
                if (nodeChild.LocalName == "text")
                {
                    dict.AttrText = nodeChild.InnerText;
                }
                if (nodeChild.LocalName == "tr")
                {
                    YTranslation tr = GetYTranslation(nodeChild);
                    if (tr != null)
                    {
                        dict.Translations.Add(tr);
                    }
                }

            }
            return dict;
        }

        public List<YDictionary> GetTranslation(Lang directionOfTranslation, string textTranslator)
        {
            CreateStringRequest(directionOfTranslation, textTranslator);
            XmlDocument doc = GetXmlResponse(directionOfTranslation, textTranslator);
            if (doc == null) return null;

            List<YDictionary> resultTranslator = new List<YDictionary>();

            foreach (XmlElement nodaChild in doc.DocumentElement)
            {
                if (nodaChild.LocalName == "def")
                {
                    YDictionary defTranslator = GetYDictionary(nodaChild);
                    resultTranslator.Add(defTranslator);
                }
            }
            return resultTranslator;
        }

        protected XmlDocument GetXmlResponse(Lang directionOfTranslation, string textTranslator)
        {
            CreateStringRequest(directionOfTranslation, textTranslator);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(xmlInterface);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                XmlReader xmlReader = XmlReader.Create(response.GetResponseStream());
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlReader);
                return doc;
            }
            catch (WebException e)
            {
                return null;
            }
        }
    }
}