using System;

namespace Sharpbench
{
    public static class Utils
    {
        public static T[,] AdjustArray<T>(this T[,] sourceArray, int row, int column)
        {
            if (row < 0) throw new ArgumentOutOfRangeException(nameof(row));
            if (column < 0) throw new ArgumentOutOfRangeException(nameof(column));
            var sourceRowCount = sourceArray.GetLength(0);
            var sourceColumnCount = sourceArray.GetLength(1);
            if (sourceRowCount > row && sourceColumnCount > column) return sourceArray;            
            var requiredRowsCount = Math.Max(row + 1, sourceRowCount);
            var requiredColumnsCount = Math.Max(column + 1, sourceColumnCount);
            var newArray = new T[requiredRowsCount, requiredColumnsCount];
            for (var rowIndex = 0; rowIndex < sourceRowCount; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < sourceColumnCount; columnIndex++)
                {
                    newArray[rowIndex, columnIndex] = sourceArray[rowIndex, columnIndex];
                }
            }
            return newArray;
        }

        public static T[] AdjustArray<T>(this T[] sourceArray, int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (sourceArray.Length > index) return sourceArray;
            var newArray = new T[index+1];
            Array.Copy(sourceArray, newArray, sourceArray.Length);
            return newArray;
        }
    }
}
