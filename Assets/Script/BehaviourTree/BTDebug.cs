using System.Collections.Generic;
using UnityEngine;

// Simple global runtime logger for behaviour trees.
// Stores recent messages in a ring buffer for quick inspection.
// Use BTDebug.Add(...) to append messages and BTDebug.GetRecent(n) to read.
public static class BTDebug
{
    private static readonly int MaxEntries = 256;
    private static readonly List<string> entries = new List<string>(MaxEntries);
    private static int index = 0;
    private static readonly object locker = new object();

    public static void Add(string message)
    {
        lock (locker)
        {
            if (entries.Count < MaxEntries)
            {
                entries.Add(message);
            }
            else
            {
                entries[index] = message;
                index = (index + 1) % MaxEntries;
            }
        }
    }

    // Return up to 'count' most recent messages (newest first).
    public static string[] GetRecent(int count = 20)
    {
        lock (locker)
        {
            int take = Mathf.Clamp(count, 0, entries.Count);
            string[] outArr = new string[take];
            int ri = 0;
            for (int i = 0; i < take; i++)
            {
                int pos = (entries.Count - 1 - i);
                if (pos < 0) break;
                outArr[ri++] = entries[pos];
            }
            return outArr;
        }
    }

    // Clear the debug buffer
    public static void Clear()
    {
        lock (locker)
        {
            entries.Clear();
            index = 0;
        }
    }
}
