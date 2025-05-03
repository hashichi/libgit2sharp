using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Handles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGit2Sharp.Bytes
{
    /// <summary>
    /// 
    /// </summary>
    public class ByteArrayPatch : Patch
    {
        internal unsafe ByteArrayPatch(DiffHandle diff)
        {
            using (diff)
            {
                int count = Proxy.git_diff_num_deltas(diff);
                for (int i = 0; i < count; i++)
                {
                    using (var patch = Proxy.git_patch_from_diff(diff, i))
                    {
                        var delta = Proxy.git_diff_get_delta(diff, i);
                        AddFileChange(delta);
                        Proxy.git_patch_print(patch, PrintCallBack);
                    }
                }
            }
        }

        private unsafe void AddFileChange(git_diff_delta* delta)
        {
            var treeEntryChanges = new TreeEntryChanges(delta);

            changes.Add(treeEntryChanges.Path, new ByteArrayPatchEntryChanges(delta->flags.HasFlag(GitDiffFlags.GIT_DIFF_FLAG_BINARY), treeEntryChanges));
        }

        private unsafe int PrintCallBack(git_diff_delta* delta, GitDiffHunk hunk, GitDiffLine line, IntPtr payload)
        {
            string patchPart = LaxUtf8Marshaler.FromNative(line.content, (int)line.contentLen);

            // Deleted files mean no "new file" path

            var pathPtr = delta->new_file.Path != null
                ? delta->new_file.Path
                : delta->old_file.Path;
            var filePath = LaxFilePathMarshaler.FromNative(pathPtr);

            ByteArrayPatchEntryChanges currentChange = (ByteArrayPatchEntryChanges)this[filePath.Native];
            string prefix = string.Empty;

            switch (line.lineOrigin)
            {
                case GitDiffLineOrigin.GIT_DIFF_LINE_CONTEXT:
                    prefix = " ";
                    break;

                case GitDiffLineOrigin.GIT_DIFF_LINE_ADDITION:
                    linesAdded++;
                    currentChange.LinesAdded++;
                    currentChange.AddedLines.Add(new Line(line.NewLineNo, patchPart));
                    prefix = "+";
                    break;

                case GitDiffLineOrigin.GIT_DIFF_LINE_DELETION:
                    linesDeleted++;
                    currentChange.LinesDeleted++;
                    currentChange.DeletedLines.Add(new Line(line.OldLineNo, patchPart));
                    prefix = "-";
                    break;
            }

            string formattedOutput = string.Concat(prefix, patchPart);

            fullPatchBuilder.Append(formattedOutput);
            currentChange.AppendToPatch(formattedOutput);
            currentChange.Append(line);

            return 0;
        }

    }
}
