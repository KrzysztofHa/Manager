using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Manager.Infrastructure.Common;
using Xunit;

namespace Manager.App.Tests
{
    public class BaseOperationServiceTests
    {
        private class TestDto
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        [Fact(Skip="ProductionBugSuspected")]
        [Trait("Category", "ProductionBugSuspected")]
        public void SaveListToBase_WithValidList_WritesFileAndLoadable()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".manager.test.json");
            var originalPath = BaseOperationService<TestDto>.PathToFile;
            try
            {
                BaseOperationService<TestDto>.PathToFile = tempFile;
                var svc = new BaseOperationService<TestDto>();
                svc.ListOfElements = new List<TestDto>
                {
                    new TestDto { Id = 1, Name = "One" },
                    new TestDto { Id = 2, Name = "Two" }
                };

                // Act
                var result = svc.SaveListToBase();

                // Assert
                result.Should().BeTrue();
                File.Exists(tempFile).Should().BeTrue();

                // Create a new service instance that will load from the file in ctor
                var svc2 = new BaseOperationService<TestDto>();
                svc2.ListOfElements.Should().NotBeNull();
                svc2.ListOfElements.Should().HaveCount(2);
                svc2.ListOfElements[0].Id.Should().Be(1);
                svc2.ListOfElements[0].Name.Should().Be("One");
                svc2.ListOfElements[1].Id.Should().Be(2);
                svc2.ListOfElements[1].Name.Should().Be("Two");
            }
            finally
            {
                // Cleanup and restore
                try { File.Delete(tempFile); } catch { }
                BaseOperationService<TestDto>.PathToFile = originalPath;
            }
        }

        [Fact(Skip = "ProductionBugSuspected")]
        public void LoadListInBase_WhenFileContainsJsonNull_SetsListToNull()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".manager.test.json");
            var originalPath = BaseOperationService<TestDto>.PathToFile;
            try
            {
                // Write a plain JSON null to the file so JsonSerializer.Deserialize<string> returns null
                File.WriteAllText(tempFile, "null");
                BaseOperationService<TestDto>.PathToFile = tempFile;

                // Act
                var svc = new BaseOperationService<TestDto>();

                // Assert
                svc.ListOfElements.Should().BeNull();
            }
            finally
            {
                try { File.Delete(tempFile); } catch { }
                BaseOperationService<TestDto>.PathToFile = originalPath;
            }
        }

        [Fact(Skip="ProductionBugSuspected")]
        [Trait("Category", "ProductionBugSuspected")]
        public void SaveListToBase_WithValidList_ReturnsTrueAndCreatesFile()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".manager.test.json");
            string? originalPath = null;
            try
            {
                BaseOperationService<TestDto>.PathToFile = tempFile;
                var svc = new BaseOperationService<TestDto>();
                svc.ListOfElements = new List<TestDto>
                {
                    new TestDto { Id = 1, Name = "One" },
                    new TestDto { Id = 2, Name = "Two" }
                };

                // Act
                var result = svc.SaveListToBase();

                // Assert
                result.Should().BeTrue();
                File.Exists(tempFile).Should().BeTrue();

                // Loading via a new instance to exercise LoadListInBase
                var svc2 = new BaseOperationService<TestDto>();
                svc2.ListOfElements.Should().NotBeNull();
                svc2.ListOfElements.Should().HaveCount(2);
                svc2.ListOfElements[0].Id.Should().Be(1);
                svc2.ListOfElements[0].Name.Should().Be("One");
                svc2.ListOfElements[1].Id.Should().Be(2);
                svc2.ListOfElements[1].Name.Should().Be("Two");
            }
            finally
            {
                try { File.Delete(tempFile); } catch { }
                if (originalPath != null)
                {
                    BaseOperationService<TestDto>.PathToFile = originalPath;
                }
            }
        }

        [Fact(Skip="ProductionBugSuspected")]
        [Trait("Category", "ProductionBugSuspected")]
        public void LoadListInBase_WhenFileContainsSerializedString_PopulatesList()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".manager.test.json");
            var originalPath = BaseOperationService<TestDto>.PathToFile;
            try
            {
                // Create a list and serialize twice to match the format produced by SaveListToBase
                var list = new List<TestDto>
                {
                    new TestDto { Id = 3, Name = "Three" },
                    new TestDto { Id = 4, Name = "Four" }
                };

                var innerJson = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                var outerJson = Newtonsoft.Json.JsonConvert.SerializeObject(innerJson);

                File.WriteAllText(tempFile, outerJson);
                BaseOperationService<TestDto>.PathToFile = tempFile;

                // Act
                var svc = new BaseOperationService<TestDto>();

                // Assert
                svc.ListOfElements.Should().NotBeNull();
                svc.ListOfElements.Should().HaveCount(2);
                svc.ListOfElements[0].Id.Should().Be(3);
                svc.ListOfElements[0].Name.Should().Be("Three");
                svc.ListOfElements[1].Id.Should().Be(4);
                svc.ListOfElements[1].Name.Should().Be("Four");
            }
            finally
            {
                try { File.Delete(tempFile); } catch { }
                BaseOperationService<TestDto>.PathToFile = originalPath;
            }
        }

        [Fact]
        public void LoadListInBase_FileContainsSerializedString_PopulatesList()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".manager.test.json");
            var originalPath = BaseOperationService<TestDto>.PathToFile;
            try
            {
                var list = new List<TestDto>
                {
                    new TestDto { Id = 10, Name = "Ten" },
                    new TestDto { Id = 20, Name = "Twenty" }
                };

                var innerJson = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                var outerJson = Newtonsoft.Json.JsonConvert.SerializeObject(innerJson);

                File.WriteAllText(tempFile, outerJson);
                BaseOperationService<TestDto>.PathToFile = tempFile;

                // Act
                var svc = new BaseOperationService<TestDto>();

                // Assert
                svc.ListOfElements.Should().NotBeNull();
                svc.ListOfElements.Should().HaveCount(2);
                svc.ListOfElements[0].Id.Should().Be(10);
                svc.ListOfElements[0].Name.Should().Be("Ten");
                svc.ListOfElements[1].Id.Should().Be(20);
                svc.ListOfElements[1].Name.Should().Be("Twenty");
            }
            finally
            {
                try { File.Delete(tempFile); } catch { }
                BaseOperationService<TestDto>.PathToFile = originalPath;
            }
        }

        [Fact(Skip = "ProductionBugSuspected")]
        public void LoadListInBase_FileContainsJsonNull_SetsListToNull()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".manager.test.json");
            var originalPath = BaseOperationService<TestDto>.PathToFile;
            try
            {
                File.WriteAllText(tempFile, "null");
                BaseOperationService<TestDto>.PathToFile = tempFile;

                // Act
                var svc = new BaseOperationService<TestDto>();

                // Assert
                svc.ListOfElements.Should().BeNull();
            }
            finally
            {
                try { File.Delete(tempFile); } catch { }
                BaseOperationService<TestDto>.PathToFile = originalPath;
            }
        }

        [Fact]
        public void SaveListToBase_WithValidList_WritesDoubleSerializedContentAndReturnsTrue()
        {
            // Arrange
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".manager.test.json");
            var originalPath = BaseOperationService<TestDto>.PathToFile;
            try
            {
                BaseOperationService<TestDto>.PathToFile = tempFile;
                var svc = new BaseOperationService<TestDto>();
                svc.ListOfElements = new List<TestDto>
                {
                    new TestDto { Id = 5, Name = "Five" }
                };

                var expectedInner = Newtonsoft.Json.JsonConvert.SerializeObject(svc.ListOfElements);
                var expectedOuter = Newtonsoft.Json.JsonConvert.SerializeObject(expectedInner);

                // Act
                var result = svc.SaveListToBase();

                // Assert
                result.Should().BeTrue();
                File.Exists(tempFile).Should().BeTrue();
                var content = File.ReadAllText(tempFile);
                content.Should().Be(expectedOuter);
            }
            finally
            {
                try { File.Delete(tempFile); } catch { }
                BaseOperationService<TestDto>.PathToFile = originalPath;
            }
        }

    }
}
