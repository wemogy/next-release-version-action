using System;
using System.Collections.Generic;
using System.Linq;
using Semver;
using Wemogy.ReleaseVersionAction.Models;

namespace Wemogy.ReleaseVersionAction.Helpers
{
    public static class VersionHelpers
    {
        public static SemVersion? GetCurrentVersionFromTags(List<Tag> tags, SemVersion currentMajorMinorVersion, string folderName, string prefix)
        {
            // Filter relevant tags only
            if (!string.IsNullOrEmpty(folderName))
            {
                tags = tags
                    .Where(x => x.TagName.Substring(0, x.TagName.LastIndexOf("-")).Equals(folderName))
                    .ToList();
            }

            // Extract semantic version number only
            var filtered = tags
                .Select(x => SemVersion.Parse(TagHelpers.ExtractVersion(x, folderName, prefix)))
                .Where(x => x.Major == currentMajorMinorVersion.Major && x.Minor == currentMajorMinorVersion.Minor)
                .ToList();

            if (filtered.Any())
            {
                // Sort Tags lowest first
                var sorted = filtered
                    .OrderBy(x => x.Major)
                    .ThenBy(x => x.Minor)
                    .ThenBy(x => x.Patch)
                    .ThenBy(x => x.Prerelease);

                // Get latest Tag
                return sorted.Last();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if the given version is the has the highest minor version number amongst other tags with the same
        /// major version as the given version.
        /// </summary>
        /// <param name="tags">Tags to be checked agains</param>
        /// <param name="version">Version to check</param>
        /// <param name="folderName">Folder Name for Multi Project repos</param>
        /// <returns>True, if the version is the highest one for its major.</returns>
        public static bool IsHighestMinorVersion(List<Tag> tags, SemVersion version, string folderName, string prefix)
        {
            if (!tags.Any())
            {
                return true;
            }

            // If folders are used, filter relevant tags to those of the current folder only
            if (!string.IsNullOrEmpty(folderName))
            {
                tags = tags
                    .Where(x => x.TagName.Substring(0, x.TagName.LastIndexOf("-")).Equals(folderName))
                    .ToList();
            }

            // Extract semantic version number only
            var filtered = tags
                .Select(x => SemVersion.Parse(TagHelpers.ExtractVersion(x, folderName, prefix)))
                .Where(x => x.Major == version.Major);

            // If there are no other tags for this major version, the current version is the highest.
            if (!filtered.Any())
            {
                return true;
            }

            var highestVersion = filtered.OrderBy(x => x.Minor).Last();
            return highestVersion == version;
        }
    }
}
