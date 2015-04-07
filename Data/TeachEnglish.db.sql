--SQLite Maestro 12.1.0.1
------------------------------------------
--Host     : main
--Database : E:\C#\Project\TeachEnglish\Data\TeachEnglish.db


CREATE TABLE AudioFiles (
  IDAudio    integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
  AudioFile  blob NOT NULL,
  IDWord     integer NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (IDWord)
    REFERENCES Words(IDWord)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

CREATE TABLE ImageFiles (
  IDImage        integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
  ImageFile      blob NOT NULL,
  IDTranslation  integer NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (IDTranslation)
    REFERENCES Translations(IDTranslation)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

CREATE TABLE PartsOfSpeechEn (
  IDPosEn  integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
  PosEn    nvarchar(50) NOT NULL
);

CREATE TABLE PartsOfSpeechRu (
  IDPosRu  integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
  PosRu    nvarchar(50) NOT NULL
);

CREATE TABLE Translations (
  IDTranslation  integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
  Translation    nvarchar(50) NOT NULL,
  IDWord         integer NOT NULL,
  IDPosRu        integer,
  IDPosEn        integer,
  /* Foreign keys */
  FOREIGN KEY (IDWord)
    REFERENCES Words(IDWord)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION, 
  FOREIGN KEY (IDPosRu)
    REFERENCES PartsOfSpeechRu(IDPosRu)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION, 
  FOREIGN KEY (IDPosEn)
    REFERENCES PartsOfSpeechEn(IDPosEn)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

CREATE TABLE Words (
  IDWord         integer PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
  Word           nvarchar(50) NOT NULL,
  Transcription  nvarchar(50) NOT NULL DEFAULT [Empty]
);

