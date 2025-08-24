using System.ComponentModel;
using document.lib.data.context;

namespace document.lib.bl.tests;

public static class UnitTestContext
{
    private static readonly Lock Lock = new();
    private static readonly int IncrementOffsetBy = 10;
    private static readonly Dictionary<string, int> FolderIdOffsets = [];
    
    private static int _lastOffset = 0;


    public static int GetFolderOffset(string name)
    {
        int offset;
        lock (Lock)
        {
            offset = FolderIdOffsets[name];
        }

        return offset;
    }
    
    public static void RegisterTest(string name)
    {
        lock (Lock)
        {
            FolderIdOffsets[name] = _lastOffset;    
        }
        _lastOffset += IncrementOffsetBy;
    }
}