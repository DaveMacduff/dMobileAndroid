namespace dMobile.Bedrock
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class SysSound
    {
        #region Fields

        private byte[] soundBytes;
        private string fileName;

        #endregion

        #region Constructors

        /// <summary> 
        /// Construct the Sound object to play sound data from the specified file. 
        /// </summary> 
        public SysSound(string fileName)
        {
            this.FileName = fileName;
        }

        /// <summary> 
        /// Construct the Sound object to play sound data from the specified stream. 
        /// </summary> 
        public SysSound(Stream stream)
        {
            // read the data from the stream
            this.SoundBytes = new byte[stream.Length];
            stream.Read(this.SoundBytes, 0, (int)stream.Length);
        }

        #endregion

        #region Enumerations

        private enum Flags
        {
            SND_SYNC = 0x0000,  /* play synchronously (default) */
            SND_ASYNC = 0x0001,  /* play asynchronously */
            SND_NODEFAULT = 0x0002,  /* silence (!default) if sound not found */
            SND_MEMORY = 0x0004,  /* pszSound points to a memory file */
            SND_LOOP = 0x0008,  /* loop the sound until next sndPlaySound */
            SND_NOSTOP = 0x0010,  /* don't stop any currently playing sound */
            SND_NOWAIT = 0x00002000, /* don't wait if the driver is busy */
            SND_ALIAS = 0x00010000, /* name is a registry alias */
            SND_ALIAS_ID = 0x00110000, /* alias is a predefined ID */
            SND_FILENAME = 0x00020000, /* name is file name */
            SND_RESOURCE = 0x00040004  /* name is resource name or atom */
        }

        #endregion

        #region Properties

        private byte[] SoundBytes
        {
            get
            {
                return this.soundBytes;
            }

            set
            {
                this.soundBytes = value;
            }
        }

        private string FileName
        {
            get
            {
                return this.fileName;
            }

            set
            {
                this.fileName = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary> 
        /// Play the sound 
        /// </summary> 
        public void Play()
        {
            // if a file name has been registered, call WCE_PlaySound, 
            //  otherwise call WCE_PlaySoundBytes 
            if (this.FileName != null)
            {
                WCE_PlaySound(this.FileName, IntPtr.Zero, (int)(Flags.SND_ASYNC | Flags.SND_FILENAME));
            }
            else
            {
                WCE_PlaySoundBytes(this.SoundBytes, IntPtr.Zero, (int)(Flags.SND_ASYNC | Flags.SND_MEMORY));
            }
        }

        #endregion

        #region Private Members

        [DllImport("CoreDll.DLL", EntryPoint = "PlaySound", SetLastError = true)]
        private static extern int WCE_PlaySound(string szSound, IntPtr hMod, int flags);

        [DllImport("CoreDll.DLL", EntryPoint = "PlaySound", SetLastError = true)]
        private static extern int WCE_PlaySoundBytes(byte[] szSound, IntPtr hMod, int flags);

        #endregion
    }
}
