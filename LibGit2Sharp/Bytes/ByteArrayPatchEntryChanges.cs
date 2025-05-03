using LibGit2Sharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGit2Sharp.Bytes
{
    public class ByteArrayPatchEntryChanges(bool isBinaryComparison, TreeEntryChanges treeEntryChanges) : PatchEntryChanges(isBinaryComparison, treeEntryChanges)
    {
        public IEnumerable<ByteArrayLine> ByteArrayLines => lines;

        internal void Append(GitDiffLine line) => lines.Add(new ByteArrayLine(line));

        private readonly IList<ByteArrayLine> lines = [];
    }
}
