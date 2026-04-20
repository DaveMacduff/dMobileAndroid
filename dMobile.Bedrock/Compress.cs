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
    /// Provides static methods for compressing and decompressing strings using the GZip algorithm.
    /// </summary>
    /// <remarks>The Compress class offers utility methods for working with GZip-compressed data in memory or
    /// via temporary files. All methods are static and thread-safe. Use these methods to efficiently compress text data
    /// for storage or transmission, and to restore compressed data to its original string form.</remarks>
    public static class Compress
    {
        /// <summary>
        /// Compresses the specified string using the GZip algorithm and returns the compressed data as a byte array.
        /// </summary>
        /// <remarks>The caller is responsible for ensuring that the input string is not null. To
        /// decompress the returned byte array, use a corresponding GZip decompression method and decode the result
        /// using UTF-8.</remarks>
        /// <param name="uncompressedString">The string to compress. The string is encoded using UTF-8 before
        /// compression. Cannot be null.</param>
        /// <returns>A byte array containing the GZip-compressed representation of the input string.</returns>
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
        /// Decompresses a GZip-compressed byte array and returns the resulting string using UTF-8 encoding.
        /// </summary>
        /// <remarks>If the input data is not in a valid GZip format or cannot be decoded as UTF-8, an
        /// exception may be thrown. The method assumes the decompressed data represents valid UTF-8 text.</remarks>
        /// <param name="arrBytData">The byte array containing data compressed in the GZip format. Cannot be null.</param>
        /// <returns>A string containing the decompressed data, decoded as UTF-8 text.</returns>
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
        /// Decompresses a GZip-compressed byte array and returns the resulting string using a temporary file for
        /// intermediate storage.
        /// </summary>
        /// <remarks>This method uses a temporary file to store intermediate decompressed data. The
        /// temporary file is deleted after the operation completes. If the input data does not represent valid
        /// GZip-compressed content, an exception may be thrown.</remarks>
        /// <param name="arrBytData">The byte array containing GZip-compressed data to decompress. Cannot be null.</param>
        /// <returns>A string containing the decompressed contents of the input data, interpreted as UTF-8 text.</returns>
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
