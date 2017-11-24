using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace RubikCubeSolver.Tests
{
    /// <summary>
    /// Provides a data source for a data theory, with the data coming from a text file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestFileDataAttribute : DataAttribute
    {
        private readonly string _filePath;

        public TestFileDataAttribute(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var path = Path.IsPathRooted(_filePath)
                ? _filePath
                : Path.GetRelativePath(Directory.GetCurrentDirectory(), _filePath);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"Could not find file at path: {path}");
            }

            var fileData = File.ReadAllText(_filePath);

            return new[]
            {
                new object[] { fileData }
            };

        }
    }
}
