using System;
using System.Text.Json;

namespace ApiApplication
{
    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        // Implementation taken from
        // https://github.com/xsoheilalizadeh/SnakeCaseConversion/blob/master/SnakeCaseConversionBenchmark/SnakeCaseConventioneerBenchmark.cs#L49
        // with the modification proposed here:
        // https://github.com/dotnet/runtime/issues/782#issuecomment-613805803
        public override string ConvertName(string name)
        {
            int upperCaseLength = 0;

            for (int i = 1; i < name.Length; i++)
            {
                if (name[i] >= 'A' && name[i] <= 'Z')
                {
                    upperCaseLength++;
                }
            }

            int bufferSize = name.Length + upperCaseLength;

            Span<char> buffer = new char[bufferSize];

            int bufferPosition = 0;

            int namePosition = 0;

            while (bufferPosition < buffer.Length)
            {
                if (namePosition > 0 && name[namePosition] >= 'A' && name[namePosition] <= 'Z')
                {
                    buffer[bufferPosition] = '_';
                    buffer[bufferPosition + 1] = char.ToLowerInvariant(name[namePosition]);
                    bufferPosition += 2;
                    namePosition++;
                    continue;
                }

                buffer[bufferPosition] = char.ToLowerInvariant(name[namePosition]);

                bufferPosition++;

                namePosition++;
            }

            return buffer.ToString();
        }
    }
}
