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
        /// <returns><see cref="byte"/> A byte array of the encrypted 'uncompressedString'.</returns>
        public static byte[] Zip(string uncompressedString)
        {
            var arrBytData = Encoding.UTF8.GetBytes(uncompressedString);
            var ms = new MemoryStream();

            // Create a Stream to Compress into
            using (var comp = new GZipStream(ms, CompressionMode.Compress))
            {
                // Compress the ByteArray into the MS
                comp.Write(arrBytData, 0, arrBytData.Length);
            }

            // Note: This has to be outside the 'End Using' Cleanup, otherwise you lose data
            return ms.ToArray();
        }

        /// <summary>
        /// This method decompresses the compressed data passed in using gzip.
        /// </summary>
        /// <param name="arrBytData"><see cref="byte"/> Byte array of compressed data.</param>
        /// <returns><see cref="string"/> Decompressed string from compressed byte data.</returns>
        public static string UnZip(byte[] arrBytData)
        {
            const int readBufferByteSize = 32768;
            var sb = new StringBuilder();

            using (var ms = new MemoryStream(arrBytData))
            {
                ms.Position = 0;

                // Create the Decompress Stream from a Memory Stream of the Compressed Data
                using var decompress = new GZipStream(ms, CompressionMode.Decompress);

                // Copy the compressed file into the decompression stream.
                var tempBuffer = new byte[readBufferByteSize + 1];
                var numRead = decompress.Read(tempBuffer, 0, tempBuffer.Length);
                while (numRead != 0)
                {
                    _ = sb.Append(Encoding.UTF8.GetString(tempBuffer, 0, numRead));
                    numRead = decompress.Read(tempBuffer, 0, tempBuffer.Length);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// This method decompresses the compressed data passed in using gzip and a temporary file.
        /// </summary>
        /// <param name="arrBytData"><see cref="byte"/> Byte array of compressed data.</param>
        /// <returns><see cref="string"/> Decompressed string from compressed byte data.</returns>
        public static string UnZipUsingTempFile(byte[] arrBytData)
        {
            const int readBufferByteSize = 32768;
            byte[] bytesBuffer;
            int bytesRead;
            var tempFileName = Path.GetTempFileName();

            try
            {
                using var ms = new MemoryStream(arrBytData);
                ms.Position = 0;

                // Create the Decompress Stream from a Memory Stream of the Compressed Data
                using var decompress = new GZipStream(ms, CompressionMode.Decompress);
                File.Delete(tempFileName);
                using var fileSt = new FileStream(
                    tempFileName,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite);
                bytesBuffer = new byte[readBufferByteSize + 1];

                // Copy the compressed file into the decompression stream.
                bytesRead = decompress.Read(bytesBuffer, 0, bytesBuffer.Length);
                while (bytesRead != 0)
                {
                    fileSt.Write(bytesBuffer, 0, bytesRead);
                    bytesRead = decompress.Read(bytesBuffer, 0, bytesBuffer.Length);
                }

                bytesBuffer = new byte[Convert.ToInt32(fileSt.Length) + 1];
                fileSt.Position = 0;
                bytesRead = fileSt.Read(bytesBuffer, 0, bytesBuffer.Length);
                fileSt.Close();
            }
            finally
            {
                File.Delete(tempFileName);
            }

            return Encoding.UTF8.GetString(bytesBuffer, 0, bytesRead);
        }
    }
}
