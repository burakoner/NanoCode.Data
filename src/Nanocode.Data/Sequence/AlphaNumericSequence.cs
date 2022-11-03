using System;
using System.Text;

namespace Nanocode.Data.Sequence
{
    public class AlphaNumericSequence
    {
        public enum SequenceType
        {
            /// <summary>
            /// 00,01,...,09,0A,...0Z,10,11...,A0,A1,...,ZZ
            /// </summary>
            NumericToAlpha,

            /// <summary>
            /// AA,AB,...,AZ,A0,...A9,BA,BB...ZZ,00,01,...99
            /// </summary>
            AlphaToNumeric,

            /// <summary>
            /// A0,A1,...,A9,AA,...AZ,B0,B1...ZZ,00,01,...99
            /// </summary>
            AlphaNumeric,

            /// <summary>
            /// 00,01,...99
            /// </summary>
            NumericOnly,

            /// <summary>
            /// AA,AB,...,ZZ
            /// </summary>
            AlphaOnly
        }

        // Public Properties
        public string Code { get; set; }
        public int CodeLength { get; set; }
        public SequenceType Sequence { get; set; }

        public AlphaNumericSequence(string currentCode, int codeLength = 5, SequenceType sequenceType = SequenceType.AlphaNumeric)
        {
            this.Code = currentCode;
            this.CodeLength = codeLength;
            this.Sequence = sequenceType;
        }

        public string NextCode()
        {
            if (this.Code.Length != this.CodeLength)
            {
                switch (Sequence)
                {
                    case SequenceType.NumericToAlpha:
                        return this.makeCustomLengthString("0", this.CodeLength);
                    case SequenceType.AlphaToNumeric:
                        return this.makeCustomLengthString("A", this.CodeLength);
                    case SequenceType.AlphaNumeric:
                        return "A" + this.makeCustomLengthString("0", this.CodeLength - 1);
                    case SequenceType.NumericOnly:
                        return this.makeCustomLengthString("0", this.CodeLength);
                    case SequenceType.AlphaOnly:
                        return this.makeCustomLengthString("A", this.CodeLength);
                    default:
                        return "";
                }
            }

            // If reached to max
            switch (Sequence)
            {
                case SequenceType.NumericToAlpha:
                    if (this.Code == this.makeCustomLengthString("Z", this.CodeLength))
                        throw new OverflowException("Maximum number is reached");
                    break;
                case SequenceType.AlphaToNumeric:
                    if (this.Code == this.makeCustomLengthString("9", this.CodeLength))
                        throw new OverflowException("Maximum number is reached");
                    break;
                case SequenceType.AlphaNumeric:
                    if (this.Code == this.makeCustomLengthString("9", this.CodeLength))
                        throw new OverflowException("Maximum number is reached");
                    break;
                case SequenceType.NumericOnly:
                    if (this.Code == this.makeCustomLengthString("9", this.CodeLength))
                        throw new OverflowException("Maximum number is reached");
                    break;
                case SequenceType.AlphaOnly:
                    if (this.Code == this.makeCustomLengthString("Z", this.CodeLength))
                        throw new OverflowException("Maximum number is reached");
                    break;
                default:
                    break;
            }

            byte[] aSCIIValues = ASCIIEncoding.ASCII.GetBytes(this.Code.ToUpper());

            int indexToCheck = aSCIIValues.Length - 1;
            bool keepChecking = true;
            while (keepChecking)
            {
                aSCIIValues[indexToCheck] = next(aSCIIValues[indexToCheck], Sequence);
                if (aSCIIValues[indexToCheck] == singleCharacterMaxValue(Sequence) && indexToCheck != 0)
                    indexToCheck--;
                else
                    keepChecking = false;
            }

            this.Code = ASCIIEncoding.ASCII.GetString(aSCIIValues);
            return this.Code;
        }

        private byte next(int current, SequenceType sequence)
        {
            switch (sequence)
            {
                case SequenceType.NumericToAlpha:
                    if (current == 57)
                        current = 65;
                    else if (current == 90)
                        current = 48;
                    else
                        current++;
                    break;
                case SequenceType.AlphaToNumeric:
                    if (current == 90)
                        current = 48;
                    else if (current == 57)
                        current = 65;
                    else
                        current++;
                    break;
                case SequenceType.AlphaNumeric:
                    if (current == 57)
                        current = 65;
                    else if (current == 90)
                        current = 48;
                    else
                        current++;
                    break;
                case SequenceType.NumericOnly:
                    if (current == 57)
                        current = 48;
                    else
                        current++;
                    break;
                case SequenceType.AlphaOnly:
                    if (current == 90)
                        current = 65;
                    else
                        current++;
                    break;
                default:
                    break;
            }

            return Convert.ToByte(current);
        }

        private string makeCustomLengthString(string data, int length)
        {
            string result = "";
            for (int i = 1; i <= length; i++)
                result += data;

            return result;
        }

        private int singleCharacterMaxValue(SequenceType sequence)
        {
            int result = 0;
            switch (sequence)
            {
                case SequenceType.NumericToAlpha:
                    result = 48;
                    break;
                case SequenceType.AlphaToNumeric:
                    result = 65;
                    break;
                case SequenceType.AlphaNumeric:
                    result = 48;
                    break;
                case SequenceType.NumericOnly:
                    result = 48;
                    break;
                case SequenceType.AlphaOnly:
                    result = 65;
                    break;
                default:
                    break;
            }

            return result;
        }

    }
}
