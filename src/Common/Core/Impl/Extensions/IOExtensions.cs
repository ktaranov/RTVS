﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Common.Core.IO;

namespace Microsoft.Common.Core {
    public static class IOExtensions {
        public static string MakeRelativePath(this string path, string basePath) {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(basePath)) {
                return path;
            }
            var bp = basePath.EndsWithOrdinal("\\") ? basePath : basePath + "\\";
            if (path.EqualsIgnoreCase(bp) || bp.EqualsIgnoreCase(path + "\\")) {
                return string.Empty;
            }
            return path.StartsWithIgnoreCase(bp) ? path.Substring(bp.Length) : path;
        }

        public static bool ExistsOnPath(string fileName) {
            return GetFullPath(fileName) != null;
        }

        public static string GetFullPath(string fileName) {
            if (File.Exists(fileName)) {
                return Path.GetFullPath(fileName);
            }

            var values = Environment.GetEnvironmentVariable("PATH");
            var paths = values.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var path in paths) {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath)) {
                    return fullPath;
                }
            }
            return null;
        }

        /// <summary>
        /// Recursively enumerate sub-directories and gets all files for the given <paramref name="basedir"/>
        /// </summary>
        public static IEnumerable<IFileSystemInfo> GetAllFiles(this IDirectoryInfo basedir) {
            List<IFileSystemInfo> files = new List<IFileSystemInfo>();
            Queue<IDirectoryInfo> dirs = new Queue<IDirectoryInfo>();
            dirs.Enqueue(basedir);
            while (dirs.Count > 0) {
                var dir = dirs.Dequeue();
                foreach (var info in dir.EnumerateFileSystemInfos()) {
                    var subdir = info as IDirectoryInfo;
                    if (subdir != null) {
                        dirs.Enqueue(subdir);
                    } else {
                        files.Add(info);
                    }
                }
            }
            return files;
        }

    }
}
