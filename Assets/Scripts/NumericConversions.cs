using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberConversions
{
    class NumericConversions
    {

        public static int RomanToArabic(string RomanNumeral)
        {
            string currentRomanNotation = "";
            int stringPlace = 0;
            int returnValue = 0;
            int notationValue = 0;
            int secondLetterValue = 0;
            int consecutiveCount = 0;
            char thisChar, lastChar;

            // Declare dictionary to hold Arabic numbers and Roman equivalents.
            SortedDictionary<char, int> dyRoman = new SortedDictionary<char, int>
            {
                { 'M', 1000 },
                { 'D', 500 },
                { 'C', 100 },
                { 'L', 50 },
                { 'X', 10 },
                { 'V', 5 },
                { 'I', 1 },
                { ' ', 0 }
            };

            try
            {
                // Convert to uppercase as Chars are case-sensitive.
                RomanNumeral = RomanNumeral.ToUpper().TrimEnd(' ').TrimStart(' ');

                if (RomanNumeral.Contains(" "))
                    throw new ArgumentOutOfRangeException("Please enter a single Roman numeral - no spaces.", new Exception());

                // FIRST LOOK FOR INVALID ROMAN NUMERAL CONSTRUCTIONS.
                // THIS COULD BE DONE AS FRONT-END VALIDATION RATHER THAN IN THE CLASS.
                for (int x = 0; x < RomanNumeral.Length; x++)
                {
                    // Parse the Roman numeral and compare each character to the last.
                    thisChar = RomanNumeral[x];
                    lastChar = x > 0 ? RomanNumeral[x - 1] : ' ';
                    consecutiveCount = (thisChar == lastChar) ? consecutiveCount + 1 : 1;

                    if (dyRoman.TryGetValue(thisChar, out notationValue) && dyRoman.TryGetValue(lastChar, out secondLetterValue))
                    {
                        int subNumber = (notationValue.ToString()[0] == '1') ? (notationValue / 10) : (notationValue / 5);

                        // If there's more than one consecutive V, L or D.
                        if (consecutiveCount > 1 && notationValue.ToString()[0] == '5')
                            throw new ArgumentOutOfRangeException("Invalid Roman numeral - invalid letter repetitions.", new Exception());

                        // If there's more than three of any character.
                        if (consecutiveCount > 3)
                            throw new ArgumentOutOfRangeException("Invalid Roman numeral - specific characters cannot appear in groups of more than three.", new Exception());

                        // If there's any other invalid combination of characters.
                        // If there's a character before this one, it must be a valid subtractive value or it must be greater than or equal to the current character.
                        if (subNumber > 0 && secondLetterValue > 0 && secondLetterValue != subNumber && secondLetterValue < notationValue)
                            throw new ArgumentOutOfRangeException("Invalid Roman numeral - possible error in subtractive combinations.", new Exception());
                    }
                    else
                    {
                        throw new Exception("Invalid character found.");
                    }

                }

                // ONCE THE STRING IS KNOWN TO BE VALID, parse the string to evaluate individual letters.
                // Using a While loop here to have more control over the movement through the string.
                while (stringPlace < RomanNumeral.Length)
                {
                    // Get the first letter and increment the place.
                    currentRomanNotation = RomanNumeral.Substring(stringPlace, 1);
                    stringPlace++;

                    // Get the value of the first letter.
                    if (dyRoman.TryGetValue(currentRomanNotation[0], out notationValue))
                    {
                        // If there's another letter to the right, get that one.
                        if ((stringPlace <= (RomanNumeral.Length - 1)) &&
                            dyRoman.TryGetValue(RomanNumeral[stringPlace], out secondLetterValue))
                        {
                            // If the value of the second letter is less than the first, then use
                            // subtractive notation (i.e. CM = 900, IX = 9) as long as the second letter 
                            // is valid in that place. 
                            if (secondLetterValue > notationValue)
                            {
                                currentRomanNotation += RomanNumeral[stringPlace];
                                stringPlace++;
                                notationValue = secondLetterValue - notationValue;
                            }
                        }

                        returnValue += notationValue;
                    }
                }
            }
            catch(Exception ex)
            {
                returnValue = 0;
                throw ex;
            }

            return returnValue;

        }
        
        
        
        public static string ArabicToRoman(int InputNumber)
        {
            string returnValue = "";
            char romanChar;
            int arabicNumber, arabicSubLevel;     // Holds arabic number and "9" place under it i.e. 1000 and 900, 10 and 9

            // Declare dictionary to hold Arabic numbers and Roman equivalents.
            // Sorted dictionary used to ensure order of entries. In this case,
            // the dictionary will be sorted starting with the last entry where the key is 1.
            SortedDictionary<int, char> dyRoman = new SortedDictionary<int, char>
            {
                { 1000, 'M' },
                { 500, 'D' },
                { 100, 'C' },
                { 50, 'L' },
                { 10, 'X' },
                { 5, 'V' },
                { 1, 'I' }
            };

            try
            {
                // The class currently does not process numbers over 3999.
                if (InputNumber > 3999 || InputNumber < 1)
                {
                    throw new ArgumentOutOfRangeException
                        ("Input values must be between 1 and 3999.", new Exception());
                }

                // Start at the end of the dictionary. Sorted dictionary orders by the key so 1000 is at the end.
                // Get Arabic number, Roman character and the subtractive level under it.
                int dictionaryElement = dyRoman.Count - 1;
                arabicNumber = dyRoman.ElementAt(dictionaryElement).Key;
                romanChar = dyRoman.ElementAt(dictionaryElement).Value;
                arabicSubLevel = arabicNumber - ((arabicNumber.ToString()[0] == '1') ? (arabicNumber / 10) : (arabicNumber / 5));

                // InputNumber will be continually reduced as the Roman numeral is built.
                while (InputNumber > 0 && InputNumber < 4000)
                {
                    if (InputNumber >= arabicNumber) // If the number remains above the current test.
                    {
                        // If the current Roman numeral ends with three of the current Roman character,
                        // and the current Arabic number starts with 1, remove the three characters and
                        // add the subtractive notation (i.e. III to IV and XXXVIII to XXXIX)
                        if (returnValue.EndsWith(new string(romanChar, 3)) && arabicNumber.ToString()[0] == '1')
                        {
                            returnValue = returnValue.Substring(0, returnValue.Length - 3);
                            returnValue += romanChar;
                            returnValue += dyRoman.ElementAt(dictionaryElement + 1).Value;
                        }
                        else // Otherwise, just add another character.
                        {
                            returnValue += dyRoman.ElementAt(dictionaryElement).Value;
                        }

                        // Subtract the amount that has been added to the Roman numeral.
                        InputNumber -= arabicNumber;
                    }
                    else if (InputNumber >= arabicSubLevel)
                    {
                        // If the number is less than the current level but greater than the sublevel
                        // (i.e. less than 1000 but 900 or greater), add the appropriate letters.

                        if (arabicNumber.ToString()[0] == '1')
                        {
                            returnValue += dyRoman.ElementAt(dictionaryElement - 2).Value;
                        }
                        else
                        {
                            returnValue += dyRoman.ElementAt(dictionaryElement - 1).Value;
                        }

                        returnValue += dyRoman.ElementAt(dictionaryElement).Value;

                        // Subtract the amount that has been added to the Roman numeral.
                        InputNumber -= arabicSubLevel;
                    }
                    else
                    {
                        // Otherwise, move forward in the dictionary and get the new values.
                        dictionaryElement--;
                        arabicNumber = dyRoman.ElementAt(dictionaryElement).Key;
                        romanChar = dyRoman.ElementAt(dictionaryElement).Value;
                        arabicSubLevel = arabicNumber - ((arabicNumber.ToString()[0] == '1') ? (arabicNumber / 10) : (arabicNumber / 5));
                    }
                }

            }
            catch (Exception ex)
            {
                returnValue = "";
                throw ex;
            }

            return returnValue;
        }
        public static string AddOrdinal(int num)
        {
            if( num <= 0 ) return num.ToString();

            switch(num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }
    
            switch(num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }
    }
}