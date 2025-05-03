using LibGit2Sharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace LibGit2Sharp.Bytes
{
    /// <summary>
    /// Represents a line with line number and content.
    /// </summary>
    public struct ByteArrayLine
    {
        /// <summary>
        /// A git_diff_line_t value : " +-\n\0FHB"
        /// </summary>
        /// <remarks><seealso href="https://libgit2.org/docs/reference/v1.8.0/diff/git_diff_line_t.html"/></remarks>
        public char Status { get; }

        /// <summary>
        /// Line number in old file or -1 for added line
        /// </summary>
        public int OldLineNumber { get; }

        /// <summary>
        /// Line number in new file or -1 for deleted line
        /// </summary>
        public int NewLineNumber { get; }

        /// <summary>
        /// Number of newline characters in content
        /// </summary>
        public int NumberOfNewline { get; }

        /// <summary>
        /// The raw content of the line in the original blob.
        /// </summary>
        public byte[] ContentRaw { get; }

        /// <summary>
        /// The content of the line in the original blob.
        /// </summary>
        public string Content => _content ??= Encoding.UTF8.GetString(ContentRaw);

        internal ByteArrayLine(GitDiffLine line)
        {
            byte b = ((byte)line.lineOrigin);

            Status = (char)line.lineOrigin;
            OldLineNumber = line.OldLineNo;
            NewLineNumber = line.NewLineNo;
            NumberOfNewline = line.NumLines;

            ContentRaw = MarshalToByteArray(line.content, (int)line.contentLen);
        }

        private Byte[] MarshalToByteArray(IntPtr content, int length)
        {
            var array = new Byte[length];
            Marshal.Copy(content, array, 0, length);
            return array;
        }

        private string _content;
    }
}
