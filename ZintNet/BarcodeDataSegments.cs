using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZintNet
{
    /// <summary>
    /// Class for managing barcode message segments.
    /// </summary>
    public static class BarcodeDataSegments
    {
        /// <summary>
        /// Message segment structure.
        /// </summary>
        public struct BarcodeMessage
        {
            /// <summary>
            /// Data part of the message segment.
            /// </summary>
            public char[] data { get; set; }

            /// <summary>
            /// ECI part of the message segemnt.
            /// </summary>
            public int eci { get; set; }
        }

        /// <summary>
        /// List of barcode message segments.
        /// </summary>
        public static List<BarcodeMessage> barcodeMessageSegment = new List<BarcodeMessage>();
        /// <summary>
        /// Adds a new barcode message segment.
        /// </summary>
        /// <param name="data">message part od the message sectment.</param>
        /// <param name="eci">the ECI part of the message segment.</param>
        public static void AddSegments(string data, int eci)
        {
            BarcodeMessage segment = new BarcodeMessage();
            segment.data = data.ToCharArray();
            segment.eci = eci;
            barcodeMessageSegment.Add(segment);
        }
    }
}
