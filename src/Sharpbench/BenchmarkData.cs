using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sharpbench
{
    public class BenchmarkData
    {
        private BenchmarkDataDto _data = new BenchmarkDataDto();        
        
        public int Rows => _data.SourceCodes.GetLength(0);
        public int Columns => _data.SourceCodes.GetLength(1);

        public string GetSourceCode(int row, int column)
        {
            return _data.SourceCodes[row, column];            
        }

        public BuildConfiguration GetBuildConfiguration(int row)
        {
            return _data.RowConfigurations[row] ?? _data.DefaultConfiguration;
        }

        public void SetSourceCode(int row, int column, string sourceCode)
        {
            AdujstSize(row, column);
            _data.SourceCodes[row, column] = sourceCode;
        }

        private void AdujstSize(int row, int column)
        {
            _data.SourceCodes = _data.SourceCodes.AdjustArray(row, column);
            _data.RowConfigurations = _data.RowConfigurations.AdjustArray(row);
        }

        public void AddRow()
        {
            var row = Rows;
            for (var column = 0; column < Columns; column++)
            {
                SetSourceCode(row, column, "// TODO");
            }
        }

        public void AddColumn()
        {
            var column = Columns;
            for (var row = 0; row < Rows; row++)
            {
                SetSourceCode(row, column, "// TODO");
            }
        }

        private class BenchmarkDataDto
        {
            public BuildConfiguration DefaultConfiguration { get; set; } = new BuildConfiguration();
            public BuildConfiguration[] RowConfigurations { get; set; } = new BuildConfiguration[0];
            public string[,] SourceCodes { get; set; } = new string[0,0];
        }        
    }

    public enum Optimization
    {
        Debug,
        Release
    }

    public enum Platform
    {
        X86,
        X64
    }

    public class BuildConfiguration
    {
        public Optimization Optimization { get; set; }
        public Platform Platform { get; set; }
        public string TargetFramework { get; set; } = "v4.6.2";
    }
}
