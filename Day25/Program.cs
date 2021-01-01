using System;

namespace Day25
{
    class Program
    {
        static void Main(string[] args)
        {
            bool test = false;

            var cardPublicKey = test ? 5764801 : 9033205;
            var doorPublicKey = test ? 17807724 : 9281649;

            var cardLoopSize = GetLoopSize(cardPublicKey);
            var doorLoopSite = GetLoopSize(doorPublicKey);

            var cardEncryptionKey = TransformNumber(cardPublicKey, doorLoopSite);
            var doorEncryptionKey = TransformNumber(doorPublicKey, cardLoopSize);

            if (cardEncryptionKey != doorEncryptionKey)
            {
                throw new InvalidOperationException();
            }
        }

        static int GetLoopSize(int publicKey, int initialSubjectNumber = 7)
        {
            long number = 1;
            
            for (int loopSize = 1; loopSize < int.MaxValue; loopSize++)
            {
                number = TransformNumberInc(initialSubjectNumber, number);

                if (number == publicKey)
                {
                    return loopSize;
                }
            }

            throw new InvalidOperationException();
        }

        private static long TransformNumber(int initialSubjectNumber, int loopSize)
        {
            long number = 1;

            for (int i = 0; i < loopSize; i++)
            {
                number = TransformNumberInc(initialSubjectNumber, number);
            }

            return number;
        }

        private static long TransformNumberInc(int initialSubjectNumber, long number)
        {
            number = number * initialSubjectNumber;
            number = number % 20201227;
            return number;
        }
    }
}
