using System;
using System.Text;
using System.Threading;
using FluentAssertions;
using Moq;
using Xunit;
using Manager.Consol.Concrete;

namespace Manager.Consol.UnitTests
{
    public class ConsoleServiceTests
    {
        // Helper to mock static Console methods
        private class ConsoleMock : IDisposable
        {
            private readonly TextWriter _originalOut;
            private readonly TextReader _originalIn;
            private readonly StringWriter _stringWriter;
            private StringReader _stringReader;
            private readonly ConsoleColor _originalColor;
            #if WINDOWS
            private readonly bool _originalCursorVisible;
#endif
            private int _windowHeight;
            private int _windowWidth;
            private int _bufferWidth;
            private int _bufferHeight;
            private int _cursorLeft;
            private int _cursorTop;
            // private (int Left, int Top) _cursorPosition;
            private Queue<ConsoleKeyInfo> _inputKeys = new();
            // private bool _exitCalled;

            public ConsoleMock(
                int windowHeight = 40,
                int windowWidth = 140,
                int bufferWidth = 140,
                int bufferHeight = 40,
                int cursorLeft = 0,
                int cursorTop = 0)
            {
                _windowHeight = windowHeight;
                _windowWidth = windowWidth;
                _bufferWidth = bufferWidth;
                _bufferHeight = bufferHeight;
                _cursorLeft = cursorLeft;
                _cursorTop = cursorTop;
                // _cursorPosition = (cursorLeft, cursorTop);
                _originalOut = Console.Out;
                _originalIn = Console.In;
                _stringWriter = new StringWriter();
                _stringReader = new StringReader("");
                Console.SetOut(_stringWriter);
                Console.SetIn(_stringReader);
                _originalColor = Console.ForegroundColor;
                #if WINDOWS
                #if WINDOWS
                _originalCursorVisible = Console.CursorVisible;
#endif
#endif
            }

            public void SetInput(string input)
            {
                _stringReader.Dispose();
                _stringReader = new StringReader(input);
                Console.SetIn(_stringReader);
            }

            public void SetInputKeys(params ConsoleKeyInfo[] keys)
            {
                _inputKeys = new Queue<ConsoleKeyInfo>(keys);
            }

            public string GetOutput() => _stringWriter.ToString();

            public void Dispose()
            {
                Console.SetOut(_originalOut);
                Console.SetIn(_originalIn);
                Console.ForegroundColor = _originalColor;
                #if WINDOWS
                #if WINDOWS
                Console.CursorVisible = _originalCursorVisible;
#endif
#endif
                _stringWriter.Dispose();
                _stringReader.Dispose();
            }
        }

        [Fact]
        public void WriteMessage_ValidMessage_WritesToConsole()
        {
            // Arrange
            using var consoleMock = new ConsoleMock();
            var testMessage = "Hello, test!";

            // Act
            ConsoleService.WriteMessage(testMessage);

            // Assert
            consoleMock.GetOutput().Should().Be(testMessage);
        }

    }
}
