// <copyright file="SysSound.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Android.Content.Res;
    using Android.Media;
    using Java.IO;
    using Stream = System.IO.Stream;
    using Android.Webkit;
    using System.Threading.Tasks;
    using Java.Nio.FileNio.Attributes;
    using Xamarin.Forms;
    using Plugin.SimpleAudioPlayer;

    /// <summary>
    /// Defines the <see cref="SysSound" />.
    /// </summary>
    public class SysSound
    {
        /// <summary>
        /// Defines the fileName.
        /// </summary>
        private string fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SysSound"/> class.
        /// </summary>
        /// <param name="fileName">The fileName<see cref="string"/>.</param>
        public SysSound(string fileName)
        {
            this.FileName = fileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SysSound"/> class.
        /// </summary>
        /// <param name="stream">The stream<see cref="System.IO.Stream"/>.</param>
        public SysSound(Stream stream)
        {
            // read the data from the stream
            this.SoundBytes = new byte[stream.Length];
            stream.Read(this.SoundBytes, 0, Convert.ToInt32(stream.Length));
        }

        /// <summary>
        /// Defines the Flags.
        /// </summary>
        private enum Flags
        {
            /// <summary>
            /// Defines the SND_SYNC.
            /// </summary>
            SND_SYNC = 0x0000,  /* play synchronously (default) */

            /// <summary>
            /// Defines the SND_ASYNC.
            /// </summary>
            SND_ASYNC = 0x0001,  /* play asynchronously */

            /// <summary>
            /// Defines the SND_NODEFAULT.
            /// </summary>
            SND_NODEFAULT = 0x0002,  /* silence (!default) if sound not found */

            /// <summary>
            /// Defines the SND_MEMORY.
            /// </summary>
            SND_MEMORY = 0x0004,  /* pszSound points to a memory file */

            /// <summary>
            /// Defines the SND_LOOP.
            /// </summary>
            SND_LOOP = 0x0008,  /* loop the sound until next sndPlaySound */

            /// <summary>
            /// Defines the SND_NOSTOP.
            /// </summary>
            SND_NOSTOP = 0x0010,  /* don't stop any currently playing sound */

            /// <summary>
            /// Defines the SND_NOWAIT.
            /// </summary>
            SND_NOWAIT = 0x00002000, /* don't wait if the driver is busy */

            /// <summary>
            /// Defines the SND_ALIAS.
            /// </summary>
            SND_ALIAS = 0x00010000, /* name is a registry alias */

            /// <summary>
            /// Defines the SND_ALIAS_ID.
            /// </summary>
            SND_ALIAS_ID = 0x00110000, /* alias is a predefined ID */

            /// <summary>
            /// Defines the SND_FILENAME.
            /// </summary>
            SND_FILENAME = 0x00020000, /* name is file name */

            /// <summary>
            /// Defines the SND_RESOURCE.
            /// </summary>
            SND_RESOURCE = 0x00040004,  /* name is resource name or atom */
        }

        /// <summary>
        /// Gets or sets the SoundBytes.
        /// </summary>
        private byte[] SoundBytes { get; set; }

        /// <summary>
        /// Gets or sets the FileName.
        /// </summary>
        private string FileName
        {
            get => this.fileName;

            set => this.fileName = value;
        }

        /// <summary>
        /// Play the sound.
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

        /// <summary>
        /// The WCE_PlaySound.
        /// </summary>
        /// <param name="szSound">The szSound<see cref="string"/>.</param>
        /// <param name="hMod">The hMod<see cref="IntPtr"/>.</param>
        /// <param name="flags">The flags<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        [DllImport("CoreDll.DLL", EntryPoint = "PlaySound", SetLastError = true)]
        private static extern int WCE_PlaySound(string szSound, IntPtr hMod, int flags);

        /// <summary>
        /// The WCE_PlaySoundBytes.
        /// </summary>
        /// <param name="szSound">The szSound<see cref="byte[]"/>.</param>
        /// <param name="hMod">The hMod<see cref="IntPtr"/>.</param>
        /// <param name="flags">The flags<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        [DllImport("CoreDll.DLL", EntryPoint = "PlaySound", SetLastError = true)]
        private static extern int WCE_PlaySoundBytes(byte[] szSound, IntPtr hMod, int flags);
    }
}
