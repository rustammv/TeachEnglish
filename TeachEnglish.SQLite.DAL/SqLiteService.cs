namespace TeachEnglish.SQLite.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Validation;
    using System.Linq;
    using DatabaseModel;

    public interface IDatabaseService
    {
        void ClearDataBase();
        void UpdateWord(string keyword, string translation, string partOfSpeechEn, string partOfSpeechRu, List<byte[]> listImage, List<byte[]> listAudio, string transcription = "[empty]");

        List<String> GetListWord(string subWord, int amountWord);
    }

    public class SqLiteService : IDatabaseService
    {
        public void ClearDataBase()
        {
            using (var context = new DbModel())
            {
                foreach (var i in context.ImageFiles)
                {
                    context.ImageFiles.Remove(i);
                }
                foreach (var i in context.AudioFiles)
                {
                    context.AudioFiles.Remove(i);
                }
                foreach (var i in context.Translations)
                {
                    context.Translations.Remove(i);
                }
                foreach (var i in context.Words)
                {
                    context.Words.Remove(i);
                }
                foreach (var i in context.PartsOfSpeechEn)
                {
                    context.PartsOfSpeechEn.Remove(i);
                }
                foreach (var i in context.PartsOfSpeechRu)
                {
                    context.PartsOfSpeechRu.Remove(i);
                }
                context.SaveChanges();
            }
        }

        public void UpdateWord(string keyword, string translation, string partOfSpeechEn, string partOfSpeechRu, List<byte[]> listImage,
            List<byte[]> listAudio, string transcription = "[empty]")
        {
            if (keyword == null) return;
            // add a record to a table PartsOfSpeechEn
            long posEn = AddNewPartOfSpeechEnToContext(partOfSpeechEn);
            // add a record to a table PartsOfSpeechRu
            long posRu = AddNewPartOfSpeechRuToContext(partOfSpeechRu);

            using (var context = new DbModel())
            {
                // add a record to a table Words
                Words newWord;
                // check record
                var word = (from w1 in context.Words where w1.Word == keyword select w1).FirstOrDefault();
                long idw = -1;
                if (word != null)
                {
                    newWord = word;
                    idw = word.IDWord;
                    newWord.Transcription = transcription;
                    //// to remove all audio recording attached to the newWord
                    long removeAudio = RemoveAudioByIDWord(word.IDWord, context);
                }
                else
                {
                    newWord = new Words { Word = keyword, Transcription = transcription };
                }

                foreach (byte[] audioFile in listAudio)
                {
                    newWord.AudioFiles.Add(new AudioFiles
                    {
                        AudioFile = audioFile,
                        Words = newWord,
                    });
                }
                context.SaveChanges();

                var query = (from t in context.Translations
                             join c in context.PartsOfSpeechEn on t.IDPosEn equals posEn
                             join w in context.Words on t.IDWord equals idw
                             select t).FirstOrDefault();
                Translations newTranslation;
                if (query == null)
                {
                    // изменяем объект, вносим новые аудио удалив старые, вносим новые картинки удалив старые
                    newTranslation = new Translations { Words = newWord, Translation = translation };
                    try
                    {
                        long removeImage = RemoveImageByIDWord(newTranslation.IDTranslation, context);
                        FillTranslation(newTranslation, listImage, posEn, posRu);
                        newTranslation.Words = newWord;
                        context.Translations.Add(newTranslation);
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            string ss = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                string ss1 = string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                    }                //// add new word
                }
                else
                {
                    newTranslation = query;
                    long removeImage = RemoveImageByIDWord(newTranslation.IDTranslation, context);
                    FillTranslation(newTranslation, listImage, posEn, posRu);
                }

                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        string ss =
                            string.Format(
                                "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            string ss1 = string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                } 
            }

        }

        public List<string> GetListWord(string subWord, int amountWord)
        {
            List<string> result = new List<string>();
            using (var context = new DbModel())
            {
                var query = (from t in context.Translations
                             join w in context.Words on t.IDWord equals w.IDWord
                             where w.Word.Contains(subWord)
                             select new
                             {
                                 word = w.Word.Trim(),
                                 translation = t.Translation.Trim(),
                                 transcritpion = w.Transcription.Trim()
                             }).Take(amountWord).ToList();
                int i = query.Count();
                if (i > 0)
                    result = query.Select(x => x.word).ToList();
            }
            return result;
        }

        #region Helper methods

        private long AddNewPartOfSpeechEnToContext(string partOfSpeechEn)
        {
            long result = -1;
            if (!string.IsNullOrEmpty(partOfSpeechEn))
            {
                using (var context = new DbModel())
                {
                    var query = (from p in context.PartsOfSpeechEn
                        where p.PosEn == partOfSpeechEn
                        select p).FirstOrDefault();
                    if (query == null)
                    {
                        PartsOfSpeechEn posEn = new PartsOfSpeechEn {PosEn = partOfSpeechEn};
                        context.PartsOfSpeechEn.Add(posEn);
                        context.SaveChanges();
                        result = posEn.IDPosEn;
                    }
                    else
                    {
                        result = query.IDPosEn;
                    }
                }
            }
            return result;
        }

        private long AddNewPartOfSpeechRuToContext(string partOfSpeechRu)
        {
            long result = -1;
            if (!string.IsNullOrEmpty(partOfSpeechRu))
            {
                using (var context = new DbModel())
                {
                    var query = (from p in context.PartsOfSpeechRu
                        where p.PosRu == partOfSpeechRu
                        select p).FirstOrDefault();
                    if (query == null)
                    {
                        PartsOfSpeechRu posRu = new PartsOfSpeechRu {PosRu = partOfSpeechRu};
                        context.PartsOfSpeechRu.Add(posRu);
                        context.SaveChanges();
                        result = posRu.IDPosRu;
                    }
                    else
                    {
                        result = query.IDPosRu;
                    }
                }
            }
            return result;
        }

        public long RemoveImageByIDWord(long idTranslation, DbModel context)
        {
            var list = context.ImageFiles.Where(x => x.IDTranslation == idTranslation);
            int result = list.Count();
            foreach (var a in list)
            {
                context.ImageFiles.Remove(a);
            }
            return result;
        }

        public long RemoveAudioByIDWord(long idWord, DbModel context)
        {
            var list = context.AudioFiles.Where(x => x.IDWord == idWord);
            int result = list.Count();
            foreach (var a in list)
            {
                context.AudioFiles.Remove(a);
            }
            return result;
        }

        private void FillTranslation(Translations tr, List<byte[]> listImage, long posEn, long posRu)
        {
            if (posEn != -1)
            {
                tr.IDPosEn = posEn;
            }

            if (posEn != -1)
            {
                tr.IDPosRu = posRu;
            }

            foreach (byte[] imageFile in listImage)
            {
                tr.ImageFiles.Add(new ImageFiles
                {
                    ImageFile = imageFile,
                    Translations = tr
                });
            }
        }
        #endregion


    }
}
