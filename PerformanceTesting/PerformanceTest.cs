using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using static System.Diagnostics.Stopwatch;
using System.Collections.Generic;
using Trees;

static class AVLTreePerformanceTest {
      
  static double Performance_Insert_Values<T>(int[] values)
    where T : Trees.ISet<int>, new()
  {
    // Display the timer frequency and resolution.
    if (Stopwatch.IsHighResolution)
        Debug.WriteLine("Operations timed using the system's high-resolution performance counter.");
    else 
        Debug.WriteLine("Operations timed using the DateTime class.");
    
    Stopwatch stopWatch = new Stopwatch();
    long frequency = Stopwatch.Frequency;
    Debug.WriteLine("Frequency: " + frequency);
    T t = new T();
    
    stopWatch.Start();
    for (int i = 0; i < values.Length; i++)
      t.Add(values[i]);
    stopWatch.Stop();
    
    //Debug.Assert(inserted == values.Length, $"Incorrect number of values inserted into AVLTree. inserted: {inserted}, num. values: {values.Length}");
    
    return stopWatch.Elapsed.TotalMilliseconds;
  }
  
  static void WriteResultsToFile(Dictionary<int, double> results, string path) {
    File.Delete(path);
    StreamWriter sw = new StreamWriter(path, false);
    foreach (var x in results) {
      double val = (x.Value <= 0) ? 0 : Math.Log10(x.Value);
      sw.WriteLine($"{x.Key}, {val}");
    }
    sw.Close();
  }
  
  static void Performance_Insert_Random_Values<T>(
    int blockSize, int blockCount, string name
  ) where T : Trees.ISet<int>, new()
  {
    var results = new Dictionary<int, double>();
    
    Random rand = new Random();
    
    for (int i = 1; i <= blockCount; i++) {
      int inserted = 0;
      HashSet<int> h = new HashSet<int>();
      int numValues = i*blockSize;
      Console.WriteLine($"Epoch: {i}. Number of values: {numValues}");
      
      int randomAttempts = 0;
      while (inserted < numValues) {
        randomAttempts++;
        int r = rand.Next(int.MaxValue);
        inserted += h.Add(r) ? 1 : 0;
      }
      Console.WriteLine("randomAttempts: " + randomAttempts);
      
      Debug.Assert(inserted == numValues, $"Incorrect number of values inserted int HashTable. {inserted}, {numValues}");
      Debug.Assert(h.Count == numValues, $"h.Count ({h.Count}) does not equal maxValue ({numValues})");
      
      int[] values = new int[numValues];
      int j = 0;
      foreach (int n in h) {
        values[j] = n;
        j++;
      }
      
      var ts = Performance_Insert_Values<T>(values);
      results.Add(numValues, ts);
    }
    WriteResultsToFile(results, $"./{blockSize}x{blockCount}_random_values_{name}_results.csv");
  }
  
  static void Performance_Insert_Ordered_Values<T>(
    int blockSize, int blockCount, string name
  ) where T : Trees.ISet<int>, new()
  {
    var results = new Dictionary<int, double>();
    
    for (int i = 1; i <= blockCount; i++) {
      int numValues = i*blockSize;
      int[] values = new int[numValues];
      for (int j = 1; j <= numValues; j++)
        values[j-1] = j;
      Console.WriteLine($"Epoch: {i}. Max value: {numValues}");
      var ts = Performance_Insert_Values<T>(values);
      results.Add(numValues, ts);
    }
    WriteResultsToFile(results, $"./{blockSize}x{blockCount}_ordered_values_{name}_results.csv");
  }
  
  static void Main() {
    int blockSize = 1000;
    int blockCount = 50;
    Performance_Insert_Random_Values<AVLTree<int>>(blockSize, blockCount, "AVLTree");
    Performance_Insert_Random_Values<BinarySearchTree<int>>(blockSize, blockCount, "BinarySearchTree");
    Performance_Insert_Ordered_Values<AVLTree<int>>(blockSize, blockCount, "AVLTree");
    Performance_Insert_Ordered_Values<BinarySearchTree<int>>(blockSize, blockCount, "BinarySearchTree");
  }
}
