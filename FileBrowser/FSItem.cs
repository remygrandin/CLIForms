using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using CLIForms.Components.Globals;

namespace FileBrowser
{
    public enum FSType
    {
        File,
        Folder
    }
    public class FSItem : MenuItem
    {
        public FSType FSType;

        public string Path;
        

        public static bool ShowChildrenFiles = true;

        public FSItem(string path) : base("")
        {
            InitItem(path);
        }

        public FSItem(FSItem parent, string path) : base(parent, "")
        {
            InitItem(path);
            
        }

        public FSItem(string path, char? hotChar, bool inheritStyle = true, params MenuItem[] children) : base("", hotChar, inheritStyle, children)
        {
            InitItem(path);
        }

        public FSItem(string path, params MenuItem[] children) : base("", children)
        {
            InitItem(path);
        }

        private void InitItem(string path)
        {
            Path = path;
            if (File.Exists(path))
            {
                FSType = FSType.File;
                Text = new FileInfo(path).Name;

            }
            else if (Directory.Exists(path))
            {
                FSType = FSType.Folder;
                Text = new DirectoryInfo(path).Name;
            }
            else
            {
                throw new Exception("object don't exist");
            }
        }

        private List<MenuItem> _children;

        public override List<MenuItem> Children
        {
            get
            {
                if (_children != null)
                    return _children;

                if (FSType == FSType.File)
                    return null;
                else if (FSType == FSType.Folder)
                {
                    List<MenuItem> childrenItems = new List<MenuItem>();
                    if (ShowChildrenFiles)
                    {
                        try
                        {
                            foreach (FileSystemInfo fsInfo in new DirectoryInfo(Path).EnumerateFileSystemInfos())
                            {
                                childrenItems.Add(new FSItem(this, fsInfo.FullName));
                            }
                        }
                        catch (UnauthorizedAccessException e)
                        {

                        }
                    }
                    else
                    {
                        try
                        {
                            foreach (DirectoryInfo fsInfo in new DirectoryInfo(Path).EnumerateDirectories())
                            {
                                childrenItems.Add(new FSItem(fsInfo.FullName, this));
                            }
                        }
                        catch (UnauthorizedAccessException e)
                        {

                        }
                    }

                    _children = childrenItems;
                    return childrenItems;
                }
                return null;
            }
            set => _children = value;
        }
        private bool? _hasChildren;

        public override bool HasChildren
        {
            get
            {
                if (FSType == FSType.File)
                    return false;

                if (_hasChildren == null)
                {
                    _hasChildren = Children.Any();
                }

                return _hasChildren.Value;
            }
        }

        public override string ToString()
        {
            return Path;
        }
    }
}
