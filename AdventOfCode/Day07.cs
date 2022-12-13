using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day07 : BaseDay
{
    private readonly DeviceDirectory inputDirectory;
    
    private class DeviceFile
    {
        public string Name { get; }
        public long Size { get; }

        public DeviceFile(string name, long size)
        {
            Name = name;
            Size = size;
        }
    }
    
    private class DeviceDirectory
    {
        public string Name { get; set; }
        public DeviceDirectory ParentDirectory { get; set; }
        public List<DeviceFile> Files { get; set; }
        public List<DeviceDirectory> SubDirectories { get; set; }

        public DeviceDirectory(string name, DeviceDirectory parentDirectory)
        {
            Name = name;
            ParentDirectory = parentDirectory;
            Files = new List<DeviceFile>();
            SubDirectories = new List<DeviceDirectory>();
        }

        public long DirectorySize()
        {
            return Files.Sum(f => f.Size) + SubDirectories.Sum(d => d.DirectorySize());
        }
    }

    public Day07()
    {
        string[] input = File.ReadAllLines(InputFilePath).Skip(1).ToArray();
        DeviceDirectory currentDirectory = new DeviceDirectory("/", null);
        const string LS_DIR = "$ ls";
        const string GO_TO_PARENT_DIR = "$ cd ..";
        Regex goToOtherDir = new Regex("^\\$ cd (?<DirectoryName>[a-zA-Z]+)$");
        Regex dir = new Regex("^dir (?<DirectoryName>[a-zA-Z]+)$");
        Regex file = new Regex("^(?<FileSize>\\d+) (?<FileName>[a-zA-Z]+\\.?[a-zA-Z]*)$");

        bool lsOngoing = false;
        foreach (string inputLine in input)
        {
            Match goToOtherDirMatch = goToOtherDir.Match(inputLine);
            if (inputLine.Equals(LS_DIR, StringComparison.InvariantCultureIgnoreCase))
            {
                lsOngoing = true;
            }
            else if (inputLine.Equals(GO_TO_PARENT_DIR, StringComparison.InvariantCultureIgnoreCase))
            {
                lsOngoing = false;
                if (currentDirectory.ParentDirectory != null)
                {
                    currentDirectory = currentDirectory.ParentDirectory;
                }
            }
            else if (goToOtherDirMatch.Success)
            {
                lsOngoing = false;
                currentDirectory = currentDirectory.SubDirectories.Single(d => d.Name.Equals(goToOtherDirMatch.Groups["DirectoryName"].Value, StringComparison.InvariantCulture));
            }
            else if (lsOngoing)
            {
                Match dirMatch = dir.Match(inputLine);
                if (dirMatch.Success)
                {
                    currentDirectory.SubDirectories.Add(new DeviceDirectory(dirMatch.Groups["DirectoryName"].Value, currentDirectory));
                }
                else
                {
                    Match fileMatch = file.Match(inputLine);
                    if (fileMatch.Success)
                    {
                        currentDirectory.Files.Add(new DeviceFile(fileMatch.Groups["FileName"].Value, long.Parse(fileMatch.Groups["FileSize"].Value)));
                    }
                    else
                    {
                        throw new ApplicationException($"Unknown command during ongoing ls: {inputLine}");
                    }
                }
            }
            else
            {
                throw new ApplicationException($"Unknown command: {inputLine}");
            }
        }

        while (currentDirectory.ParentDirectory != null)
        {
            currentDirectory = currentDirectory.ParentDirectory;
        }

        inputDirectory = currentDirectory;
    }

    public override ValueTask<string> Solve_1()
    {
        const long MaxSize = 100000;

        (long sumOfSizes, long dirSize) CalcDirSize(DeviceDirectory directory)
        {
            long sumSize = 0L;
            long dirSize = directory.Files.Sum(f => f.Size);

            foreach (DeviceDirectory subDir in directory.SubDirectories)
            {
                (long sumOfSizes, long dirSize) subDirSize = CalcDirSize(subDir);
                dirSize += subDirSize.dirSize;
                sumSize += subDirSize.sumOfSizes;
            }

            if (dirSize <= MaxSize)
            {
                sumSize += dirSize;
            }

            return (sumSize, dirSize);
        }
        
        return new ValueTask<string>($"Sum of directory sizes: {CalcDirSize(inputDirectory).sumOfSizes}");
    }

    public override ValueTask<string> Solve_2()
    {
        const long TotalSizeOfDisk = 70000000L;
        const long SpaceNeededForUpdate = 30000000L;

        DeviceDirectory currentDirectory = inputDirectory;
        // make sure we are in the root directory
        while (currentDirectory.ParentDirectory != null)
        {
            currentDirectory = currentDirectory.ParentDirectory;
        }

        long currentSpaceAvailable = TotalSizeOfDisk - currentDirectory.DirectorySize();

        (long? sizeOfDirToDelete, long dirSize) CalcDirSize(DeviceDirectory directory, long spaceToBeFreed)
        {
            long dirSize = directory.Files.Sum(f => f.Size);
            long? sizeOfDirToDelete = null;
            foreach ((long? sizeOfDirToDelete, long dirSize) result in directory.SubDirectories.Select(subDirectory => CalcDirSize(subDirectory, spaceToBeFreed)))
            {
                dirSize += result.dirSize;
                if ((sizeOfDirToDelete == null || sizeOfDirToDelete > result.sizeOfDirToDelete) && result.sizeOfDirToDelete >= spaceToBeFreed)
                {
                    sizeOfDirToDelete = result.sizeOfDirToDelete;
                }
            }
            
            if ((sizeOfDirToDelete == null || sizeOfDirToDelete > dirSize) && dirSize >= spaceToBeFreed)
            {
                sizeOfDirToDelete = dirSize;
            }
            
            return (sizeOfDirToDelete, dirSize);
        }
        
        long spaceToBeFreed = SpaceNeededForUpdate - currentSpaceAvailable;
        (long? sizeOfDirToDelete, long dirSize) calcDirSize = CalcDirSize(currentDirectory, spaceToBeFreed);

        return new ValueTask<string>($"Size of dir to delete: {calcDirSize.sizeOfDirToDelete}");
    }
}