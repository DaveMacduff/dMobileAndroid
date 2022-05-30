// <copyright file="Compress.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="Compress" />.
    /// </summary>
    public class Compress
    {
        /// <summary>
        /// This method compress an uncompressed string passed in using the gzip.
        /// </summary>
        /// <param name="uncompressedString"><see cref="string"/> Uncompressed string to compress.</param>
        /// <returns><see cref="byte[]"/> A byte array of the encrypted 'uncompressedString'.</returns>
        public static byte[] Zip(string uncompressedString)
        {
            byte[] arrBytData = Encoding.UTF8.GetBytes(uncompressedString);
            MemoryStream ms = new MemoryStream();

            // Create a Stream to Compress into
            using (GZipStream comp = new GZipStream(ms, CompressionMode.Compress))
            {
                // Compress the ByteArray into the MS
                comp.Write(arrBytData, 0, arrBytData.Length);
            }

            // Note: This has to be outside of the 'End Using' Cleanup, otherwise you lose data
            return ms.ToArray();
        }

        /// <summary>
        /// This method decompresses the compressed data passed in using gzip.
        /// </summary>
        /// <param name="arrBytData"><see cref="byte[]"/> Byte array of compressed data.</param>
        /// <returns><see cref="string"/> Decompressed string from compressed byte data.</returns>
        public static string UnZip(byte[] arrBytData)
        {
            const int ReadBufferByteSize = 32768;
            StringBuilder sb = new StringBuilder();

            using (MemoryStream ms = new MemoryStream(arrBytData))
            {
                ms.Position = 0;

                // Create the Decompress Stream from a Memory Stream of the Compressed Data
                using GZipStream decomp = new GZipStream(ms, CompressionMode.Decompress);

                // Copy the compressed file into the decompression stream.
                byte[] tempbuffer = new byte[ReadBufferByteSize + 1];
                int numRead = 0;
                numRead = decomp.Read(tempbuffer, 0, tempbuffer.Length);
                while (numRead != 0)
                {
                    _ = sb.Append(Encoding.UTF8.GetString(tempbuffer, 0, numRead));
                    numRead = decomp.Read(tempbuffer, 0, tempbuffer.Length);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// This method decompresses the compressed data passed in using gzip and a temporary file.
        /// </summary>
        /// <param name="arrBytData"><see cref="byte[]"/> Byte array of compressed data.</param>
        /// <returns><see cref="string"/> Decompressed string from compressed byte data.</returns>
        public static string UnZipUsingTempFile(byte[] arrBytData)
        {
            const int ReadBufferByteSize = 32768;

            byte[] bytBuffer = new byte[1];
            int bytesRead = 0;
            string tempFileName = Path.GetTempFileName();

            try
            {
                using MemoryStream ms = new MemoryStream(arrBytData);
                ms.Position = 0;

                // Create the Decompress Stream from a Memory Stream of the Compressed Data
                using GZipStream decomp = new GZipStream(ms, CompressionMode.Decompress);
                File.Delete(tempFileName);
                using FileStream fileSt = new FileStream(
                    tempFileName,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite);
                bytBuffer = new byte[ReadBufferByteSize + 1];

                // Copy the compressed file into the decompression stream.
                bytesRead = decomp.Read(bytBuffer, 0, bytBuffer.Length);
                while (bytesRead != 0)
                {
                    fileSt.Write(bytBuffer, 0, bytesRead);
                    bytesRead = decomp.Read(bytBuffer, 0, bytBuffer.Length);
                }

                bytBuffer = new byte[Convert.ToInt32(fileSt.Length) + 1];
                fileSt.Position = 0;
                bytesRead = fileSt.Read(bytBuffer, 0, bytBuffer.Length);
                fileSt.Close();
            }
            catch
            {
                throw;
            }
            finally
            {
                File.Delete(tempFileName);
            }

            return Encoding.UTF8.GetString(bytBuffer, 0, bytesRead).ToString();
        }
    }
}
