using System.ComponentModel;
using document.lib.data.context;

namespace document.lib.bl.tests;

public static class UnitTestContext
{
    private static readonly Lock Lock = new();
    private static readonly int IncrementOffsetBy = 100;
    private static readonly Dictionary<string, int> FolderIdOffsets = [];
    private static readonly Dictionary<string, int> RegisterIdOffsets = [];
    
    private static int _lastFolderOffset = 0;
    private static int _lastRegisterOffset = 0;


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
            FolderIdOffsets[name] = _lastFolderOffset;    
            RegisterIdOffsets[name] = _lastRegisterOffset;    
        }
        _lastFolderOffset += IncrementOffsetBy;
        _lastRegisterOffset += IncrementOffsetBy;
    }    
    
    public static int GetRegisterOffset(string name)
    {
        int offset;
        lock (Lock)
        {
            offset = RegisterIdOffsets[name];
        }

        return offset;
    }
}