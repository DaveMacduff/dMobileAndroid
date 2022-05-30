// <copyright file="Translator.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

#if MULTILANGUAGE
namespace dMobile.Bedrock
{   
    using System;
    using System.IO;
    using System.Globalization;
    using System.Resources;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Reflection;

    /// <summary>
    /// Manages loading translations and displaying them to the user.
    /// Text in Translator.cs itself, should not be translated.
    /// Class Flow: Construct, AddLanguage, Load
    /// </summary>
    public partial class Translator : IDisposable
    {
        static readonly string ValidLanguageCodes = "ENC (English Canada), ENU (English US), FRC (French Canada)";
        /// <summary>
        /// Modes used for storage and access.
        /// Affect memory used and speed of access.
        /// </summary>
        public enum StorageMode
        {
            /// <summary>
            /// Array of pointers to locations in file.
            /// Lookup position in file, create pointer to position seek, read.
            /// </summary>
            IndexedTextFiles,

            /// <summary>
            /// Locally store the array of strings.
            /// Access these strings by index.
            /// </summary>
            Memory,

            /// <summary>
            /// Access the translations from a database.
            /// </summary>
            Database,
        }

        /// <summary>
        /// Singleton
        /// </summary>
        static Translator instance = new Translator();

        /// <summary>
        /// Dictionary of resource paths indexed by Languages enum.
        /// </summary>
        Dictionary<Languages, string> textPaths;


        /// <summary>
        /// Language which will be used.
        /// </summary>
        Languages selectedLanguage;

        /// <summary>
        /// Selected language is loaded and ready to be used.
        /// </summary>
        bool languageLoaded;

        /// <summary>
        /// Language that was last used to get an ID but failed.
        /// </summary>
        string invalidLanguageName;

        /// <summary>
        /// Array of words that can be translated.
        /// </summary>
        private string[] storedVocabulary;

        /// <summary>
        /// Locations within a textfile to start reading from.
        /// Read from fileIndices[i] to fileIndices[i+1] to read a line.
        /// Stores one extra index to prevent special case of last line lookup.
        /// </summary>
        private long[] fileIndices;

        /// <summary>
        /// Buffer used for filestream reading.
        /// </summary>
        private byte[] fileReadBuffer;

        /// <summary>
        /// Filestream
        /// </summary>
        FileStream fs;
        Encoding encoding = null;

        /// <summary>
        /// Mode used to access lookups.
        /// </summary>
        StorageMode storageMode;

        /// <summary>
        /// Number of bytes stored in memory storage array.
        /// </summary>
        private long bytesInMemory;
        /// <summary>
        /// Number of chars stored in memory storage array.
        /// </summary>
        private long charsInMemory;

        private Translator()
        {
            this.storageMode = StorageMode.IndexedTextFiles;
            languageLoaded = false;
            storedVocabulary = null;
            invalidLanguageName = string.Empty;
            textPaths = new Dictionary<Languages, string>();
        }

        /// <summary>
        /// Gets an instance of the Translator.
        /// </summary>
        public static Translator GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// Adds all the language definitions from system settings.
        /// </summary>
        /// <param name="environment">Instance of environment.</param>
        public void AddLanguageDefinitions(dMobile.Bedrock.SysEnvironment environment)
        {
            // Instead, look for the language folder and iterate over those files.
            DirectoryInfo langDir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\Language");
            if (!langDir.Exists)
            {
                throw new Exception("Missing Language directory. Cannot start multilanguage without translations.");
            }

            FileInfo[] files = langDir.GetFiles();
            if (files.Length == 0)
            {
                throw new Exception("No translation files found. Add translation files to Language directory.");
            }

            // iterate over each file
            for (int i = 0; i < files.Length; ++i)
            {
                AddLanguageDefinition(files[i].FullName);
            }


            //int count = environment.SysSettings.LanguagePaths.Count;
            //if (count == 0)
            //{
            //    throw new Exception("Language files could not be found." +
            //        "Change the SystemSettings.xml LanguagePaths value to be of the format:" +
            //        "<DefaultLanguage>ENU</DefaultLanguage>" +
            //        "<LanguagePaths><path>\\dMobileTranslations.<3 letter language code>.txt</path><path>...</path></LanguagePaths>" +
            //        "where the 3 letter language code is " +
            //        ValidLanguageCodes + ".");
            //}

            //// iterate over each file
            //for (int i = 0; i < count; ++i)
            //{
            //    string path = environment.SysSettings.LanguagePaths[i];
            //    AddLanguageDefinition(path);
            //}
        }

        /// <summary>
        /// Adds a language to the translator.
        /// Parses the language from the file name.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        public void AddLanguageDefinition(string path)
        {
            // do not add empty paths. The user may have removed a path and thought empty was ok.
            // it will fail later in a more understandable way.
            if (string.IsNullOrEmpty(path))
                return;

            // TODO check for language license information here.

            Languages language;
            if (ParseLanguageFromFilename(path, out language))
            {
                AddLanguageDefinition(path, language);
            }
            else
                throw new Exception(
                    string.Format("Language could not be parsed from filename \"{0}\"." +
                    "Change the SystemSettings.xml LanguagePaths value to be of the format:" +
                    "<DefaultLanguage>ENU</DefaultLanguage>" +
                    "<LanguagePaths><path>\\dMobileTranslations.<3 letter language code>.txt</path><path>...</path></LanguagePaths>" +
                    "where the 3 letter language code is " +
                    ValidLanguageCodes + ".", path));
        }

        /// <summary>
        /// Adds a language to the translator.
        /// This is done by storing the key:language and value:path to the language file.
        /// </summary>
        /// <param name="language">Language to add.</param>
        /// <param name="path">Path to the text file.</param>
        public void AddLanguageDefinition(string path, Languages language)
        {
            if (string.IsNullOrEmpty(path))
            {
                return; // no path, no add. This lets it fail later in a more understandable way.
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    string.Format(
                    "The \"{0}\" language file does not exist at path \"{1}\".\n\n" +
                    "To fix this, update your SystemSettings.xml <LanguagePaths> to point to the correct location.",
                    language,
                    path));
            }

            textPaths.Add(language, path);
        }


        /// <summary>
        /// Loads a language for display.
        /// Only one language can be loaded at a time.
        /// </summary>
        /// <param name="language">Language to read.</param>
        public void Load(string language)
        {
            Languages lang = GetLanguageID(language);
            if (lang == Languages.Invalid)
            {
                throw new IndexOutOfRangeException(
                    string.Format(
                    "Attempted to load language \"{0}\" which does not have a path reference.\n\n" +
                    "To use the language, add the file to your Language directory and edit SystemSettings.xml " +
                    "so that the values of <LanguagePaths> contains the path to the file.",
                    language)
                );
            }
            else
                Load(lang);
        }

        /// <summary>
        /// Loads a language for display.
        /// Only one language can be loaded at a time.
        /// </summary>
        /// <param name="language">Language to read.</param>
        public void Load(Languages language)
        {
            if (language == Languages.Invalid)
            {
                throw new IndexOutOfRangeException(
                    string.Format(
                    "Attempted to load an invalid language \"{0}\".\n\n" +
                    "This is caused by trying to access a language not found in SystemSettings.xml <LanguagePaths>." +
                    "To fix this error, make sure that you have all the languages required listed in SystemSettings.xml",
                    invalidLanguageName)
                );
            }

            // Has the language been added?
            if (textPaths.Count == 0 || !textPaths.ContainsKey(language))
            {
                throw new Exception(
                    string.Format(
                        "No languages have been added.\n\n"+
                        "Edit SystemSettings.xml and add all required languages to the <LanguagePaths> node. ie.\n"+
                        "<LanguagePaths>" +
                        "<Path>path\\filename1.ENU.txt</Path>" +
                        "<Path>path\\filename2.FRC.txt</Path>" +
                        "</LanguagePaths>",
                        GetLanguageName(language)
                    )
                );
            }
                

            // Are we trying to load the already loaded language?
            if (languageLoaded && language == selectedLanguage)
                return;

            languageLoaded = false;
            selectedLanguage = language;

            switch (storageMode)
            {
                case StorageMode.Memory:
                    LoadInMemory();
                    break;
                case StorageMode.IndexedTextFiles:
                    LoadIndexedTextFile();
                    break;
                case StorageMode.Database:
                    throw new NotImplementedException("Translator - Database mode is not available.");
                    //DBConnection.GetInstance();
            }
            

            languageLoaded = true;
        }

        private void LoadInMemory()
        {
            // First time setup
            if (storedVocabulary == null)
                storedVocabulary = new string[keyCount];

            string filename = ConvertResourceToTextPath(selectedLanguage);

            bytesInMemory = 0;
            charsInMemory = 0;

            // Text lookup in sequential order
            using (StreamReader sr = new StreamReader(filename))
            {
                int index = 0;
                int lineIndex = 0;
                while (!sr.EndOfStream)
                {
                    lineIndex++;
                    string line = sr.ReadLine();
                    int valIndex = line.IndexOf('=') + 1;
                    if (valIndex != -1)
                    {
                        int key = index++;
                        storedVocabulary[key] = line.Substring(valIndex);
                        //charsInMemory += storedVocabulary[key].Length;
                        //bytesInMemory += sr.CurrentEncoding.GetByteCount(storedVocabulary[key]);
                    }
                    //if (index > storedVocabulary.Length)
                    //    throw new Exception(string.Format("Line [{0}], failed: {1}", lineIndex, line));
                }
            }
        }

        /// <summary>
        /// Loads a text file, create indices for it and store those in an array.
        /// </summary>
        private void LoadIndexedTextFile()
        {
            string fileName = ConvertResourceToTextPath(selectedLanguage);
            encoding = GetEncoding(fileName);
            int keyIndex = 0;
            fileIndices = new long[keyCount + 1];//+1 for the indexing the last byte as well.
            int bufferSize = 1024;
            int unspentBytes = 0; // bytes to count for a wrap.
            using (StreamReader sr = new StreamReader(fileName, Translator.GetEncoding(fileName)))
            {
                // First we read until the first expected character [a-zA-Z] which it must be because it is an enum.
                char[] buffer = new char[bufferSize];
                byte[] byteBuffer = new byte[bufferSize * 2];
                int len = 0;
                int ki = 0;
                int vi = 0;
                int ni = 0;
                int p = 0; // add 3 for the 3 byte order marking at the beginning
                int stage = 0;
                
                while ((len = sr.Read(buffer, 0, bufferSize)) != 0)
                {
                    int index = 0;
                    while (index < len)
                    {
                        switch (stage)
                        {
                            case 0:
                                // Start of key
                                // find first alpha char
                                for (; index < len; ++index)
                                {
                                    if (char.IsLetter(buffer[index]))
                                    {
                                        ki = index;
                                        stage = 1;
                                        ++index;
                                        break;
                                    }
                                }
                                break;
                            case 1:
                                // Start of value
                                // find '=' char
                                for (; index < len; ++index)
                                {
                                    if (buffer[index] == '=')
                                    {
                                        vi = index + 1;
                                        stage = 2;
                                        ++index;
                                        break;
                                    }
                                }
                                break;
                            case 2:
                                // End of value
                                // find \n
                                for (; index < len; ++index)
                                {
                                    if (buffer[index] == '\n')
                                    {
                                        ni = index;
                                        stage = 0;
                                        int bytes;

                                        int bufCount = ni + 1 - ki;
                                        if (bufCount >= len)
                                            bufCount--;

                                        try
                                        {
                                            bytes = sr.CurrentEncoding.GetByteCount(buffer, ki, bufCount) + unspentBytes;
                                        }
                                        catch (Exception e)
                                        {
                                            throw e;
                                        }
                                        fileIndices[keyIndex + 1] = fileIndices[keyIndex] + bytes;
                                        unspentBytes = 0;
                                        keyIndex++;
                                        // we need to count the bytes in the char buffer. This can be more due to utf encoding sometimes needing 2-4 bytes to make a char.
                                        //int bCount = sr.CurrentEncoding.GetByteCount(buffer, ki, (ni + 1) - ki);
                                        // now add the entry to the index list
                                        //fileIndices[keyIndex + 1] = bCount;
                                        //fileIndices[keyIndex++] = ki;
                                        ++index;
                                        break;
                                    }
                                }

                                break;
                        }
                        if (index == len)
                        {
                            if (ni != len - 1) // if ni was just found do not buffer again
                            {
                                // we could not find ending character before a stream wrap.
                                // therefore add the bytes up.
                                unspentBytes += sr.CurrentEncoding.GetByteCount(buffer, ki, len - ki);
                                ki = 0;
                            }
                        }
                    }
                    p += len;
                } // end while
            }
            if (unspentBytes > 0)
                fileIndices[keyIndex + 1] = fileIndices[keyIndex] + unspentBytes;
            fileReadBuffer = new byte[bufferSize];
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }
            fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, false);
        }

        /// <summary>
        /// Find the translated word for the currently selected language.
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public string Translate(TranslatorVocabulary phrase)
        {
           
            switch (storageMode)
            {
                case StorageMode.IndexedTextFiles:
                    return TranslateIndexedFile(phrase);
                case StorageMode.Memory:
                    return storedVocabulary[(int)phrase];
                case StorageMode.Database:
                    return TranslateDatabase(phrase);
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns a string from reading a filestream.
        /// </summary>
        /// <param name="phrase">Phrase to translate.</param>
        /// <returns>Text translation.</returns>
        private string TranslateIndexedFile(TranslatorVocabulary phrase)
        {
            int phraseIndex = (int)phrase;
            int totalBytes = (int)(fileIndices[phraseIndex + 1] - fileIndices[phraseIndex]);
            fs.Seek(3 + fileIndices[phraseIndex], SeekOrigin.Begin); // add 3 for skipping BOM
            int bytesRead = 0;
            while (bytesRead < totalBytes)
                bytesRead += fs.Read(fileReadBuffer, bytesRead, totalBytes - bytesRead);

            //TODO make an index of string lengths as well so this is a lookup instead of expensive string manipulation
            string output = System.Text.Encoding.UTF8.GetString(fileReadBuffer, 0, bytesRead);
            int eq = output.IndexOf('=');
            return output.Substring(eq + 1).Trim();
        }

        /// <summary>
        /// Gets a translation from the database.
        /// </summary>
        /// <param name="phrase">Phrase to translate.</param>
        /// <returns>Text translation.</returns>
        private string TranslateDatabase(TranslatorVocabulary phrase)
        {
            throw new NotImplementedException("Translator - Database translation not implemented.");
            //return DBConnection.GetInstance().GetTranslation((int)phrase, DBConnection.LoadedLanguages.French);
        }
        

        /// <summary>
        /// Gets the resource path for a language.
        /// </summary>
        /// <param name="language">Specified language.</param>
        /// <returns>Path to the resource file if it exists; otherwise null.</returns>
        private string GetResourcePath(Languages language)
        {
            if ((textPaths != null) && (textPaths.ContainsKey(language)))
                return textPaths[language];

            return null;
        }

        /// <summary>
        /// Gets a text path assuming it is right next to a resource of the same name.
        /// </summary>
        /// <param name="language">Language dictates the path to get.</param>
        /// <returns>Path to the text file.</returns>
        private string ConvertResourceToTextPath(Languages language)
        {
            if ((textPaths == null) || (!textPaths.ContainsKey(language)))
                return null;

            string filename = textPaths[selectedLanguage];
            int extIndex = filename.LastIndexOf('.');
            return filename.Substring(0, extIndex) + ".txt";
        }

        private bool ParseLanguageFromFilename(string fileName, out Languages language)
        {
            bool success = false;
            Regex r = new Regex(@"\.([a-zA-Z]{3}).txt$");
            Match m = r.Match(fileName);
            if (m.Success)
            {
                language = GetLanguageID(m.Groups[1].Value);
                if (language != Languages.Invalid)
                    success = true;
            }
            else
            {
                language = Languages.Invalid;
            }
            return success;
        }

        /// <summary>
        /// Gets a language enum ID from a string.
        /// </summary>
        /// <param name="name">Name of the three letter language code. ie. ENU.
        /// https://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx</param>
        /// <returns>Language ID if found; otherwise Invalid.</returns>
        public Languages GetLanguageID(string name)
        {
            switch (name.ToUpper())
            {
                case "ENU":
                    return Languages.ENU;
                case "ENC":
                    return Languages.ENC;
                case "FRC":
                    return Languages.FRC;
                default:
                    invalidLanguageName = name;
                    return Languages.Invalid;
            }
        }

        /// <summary>
        /// Gets a language name by enum ID.
        /// </summary>
        /// <param name="language">Language ID</param>
        /// <returns>Returns three letter code representing culture and language.
        /// Returns "Invalid" if a name cannot be found.
        /// https://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx</returns>
        private string GetLanguageName(Languages language)
        {
            switch (language)
            {
                case Languages.ENC:
                    return "ENC";
                case Languages.ENU:
                    return "ENU";
                case Languages.FRC:
                    return "FRC";
                default:
                    return "Invalid";
            }
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to UTF8 when detection of the text file's endianness fails.
        /// http://stackoverflow.com/a/19283954
        /// Changed to default to UTF8 since that is what it will likely be.
        /// </summary>
        /// <param name="fileName">The text file to analyze.</param>
        /// <returns>The detected encoding. UTF8 if undetermined.</returns>
        public static Encoding GetEncoding(string fileName)
        {
            // Read the BOM
            byte[] bom = new byte[4];
            using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode;          //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            //if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }

        /// <summary>
        /// Aligns the string within padded text.
        /// Useful for multilanguage because the strings vary in length between language.
        /// </summary>
        /// <param name="text">Text to pad.</param>
        /// <param name="totalLength">Length to pad to.</param>
        /// <param name="textBiasLeft">If the string cannot be centered it will
        /// make the text bias left (add the extra pad character to the right).
        /// If this value is false, the text will bias to the right
        /// (add the extra pad character to the left).</param>
        /// <param name="padCharacter">Character used to pad.</param>
        /// <returns>Padded string.</returns>
        public static string PadCenter(string text, int totalLength, bool textBiasLeft, char padCharacter)
        {
            if (text == null)
                text = "";

            int len = text.Length;
            if (len >= totalLength)
                return text.Substring(0, totalLength);

            StringBuilder sb = new StringBuilder(totalLength);

            // we need to know:
            // - how many pad chars there are
            // - if odd do the bias
            // - if even, center
            int padCharCount = totalLength - len;
            int half = padCharCount / 2;
            if (padCharCount % 2 == 0)
            {
                // even amount of whitespace remaining. Pad evenly on both sides.
                sb.Append(padCharacter, half).Append(text).Append(padCharacter, half);
            }
            else
            {
                // odd amount of whitespace remaining. Pad biased on one side.
                if (textBiasLeft)
                    sb.Append(padCharacter, half).Append(text).Append(padCharacter, half + 1);
                else
                    sb.Append(padCharacter, half + 1).Append(text).Append(padCharacter, half);
            }

            return sb.ToString();
        }

        
        /// <summary>
        /// Disposes the translator and all resources.
        /// </summary>
        public void Dispose()
        {
            // Clear filestream
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }

            storedVocabulary = null;
            languageLoaded = false;
        }

        
        /// <summary>
        /// Gets the number of words.
        /// </summary>
        public int WordCount { get { return keyCount; } }

        /// <summary>
        /// Gets the mode used to store the words.
        /// </summary>
        public StorageMode Storage { get { return storageMode; } }

        /// <summary>
        /// Gets the number of bytes in memory stored by the memory storage method values.
        /// </summary>
        public long BytesInMemory { get { return bytesInMemory; } }

        /// <summary>
        /// Gets the number of chars in memory stored by the memory storage method values.
        /// </summary>
        public long CharsInMemory { get { return charsInMemory; } }

        /// <summary>
        /// Gets the currently loaded language.
        /// </summary>
        public Languages Language { get { return selectedLanguage; } }

        /// <summary>
        /// Gets if at least one language path has been added.
        /// </summary>
        public bool LanguagesExist { get { return (textPaths != null) && (textPaths.Count > 0); } }
    }
}
#endif