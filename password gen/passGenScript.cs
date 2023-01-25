using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace password_gen
{
    internal class passGenScript
    {
        public static Random random = new Random();
        public static string GeneratePassword(Boolean include, Boolean caps, Boolean nums, Boolean specials, Boolean scramble, int minLen, int maxLen, string[] wordsIn)
        {
            StringBuilder buildingPassword = new StringBuilder();
            // define chars, nums and specials
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            const string charnums = "0123456789";
            const string charspecials = @"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~";
            Console.WriteLine(String.Join(",", wordsIn));
            foreach (string word in wordsIn)
            {
                wordsIn[Array.IndexOf(wordsIn, word)] = word.Trim();
            }
            Console.WriteLine(String.Join(",", wordsIn));

            // potential errors :
            Boolean error = false;
            string errorText = "";
            // if minlen > maxlen
            if (minLen > maxLen)
            {
                errorText = "MinLenMaxLen";
                error = true;
            }
            // if total length of all options combined > minlen (while include is checked on)
            if (include)
            {
                if (specials != nums)
                {
                    if (((string.Join("", wordsIn)).Length) + 1 > minLen)
                    {
                        errorText = "MinLenLowOptions";
                        error = true;
                    }
                }
                if (specials & nums)
                {
                    if (((string.Join("", wordsIn)).Length) + 2 > minLen)
                    {
                        errorText = "MinLenLowOptions";
                        error = true;
                    }
                }
                else
                {
                    if (((string.Join("", wordsIn)).Length) > minLen)
                    {
                        errorText = "MinLenLowOptions";
                        error = true;
                    }
                }

            }
            if (specials & nums & !include)
            {
                if (minLen < 2)
                {
                    errorText = "MinLenLowOptions";
                    error = true;
                }
            }
            // if no words in wordsIn but include is on
            if (!wordsIn.Any() & include)
            {
                errorText = "NoWords";
                error = true;
            }
            // wordsIn length > 5 and include is on
            if (wordsIn.Length > 5 & include)
            {
                errorText = "Words5";
                error = true;
            }
            // max or min length > 32
            if (minLen > 32 | maxLen > 32)
            {
                errorText = "Len32";
                error = true;
            }
            if (minLen < 7 | maxLen < 7)
            {
                errorText = "Len7";
                error = true;
            }
            // if one or multiple words contain whitespaces
            if (include)
            {
                foreach (string word in wordsIn)
                {
                    if (word.Contains(" "))
                    {
                        errorText = "SpaceWord";
                        error = true;
                        break;
                    }
                }
            }


            // if there is no error, password generates
            if (!error)
            {
                // random length of the password
                int length = random.Next(minLen, maxLen + 1);
                // adding random chars to make a base password of said length
                for (int i = 0; i < length; i++)
                {
                    char ranchar = chars[random.Next(0, chars.Length)];
                    buildingPassword.Append(ranchar);
                }
                // all already used indexes are stored
                List<int> wordsAllocatedIndexes = new List<int>();
                int remainingNumOfIndexes = buildingPassword.Length;

                // include words logic
                if (include)
                {

                    // scramble logic
                    if (scramble)
                    {
                        foreach (string word in wordsIn)
                        {
                            // store index of word in wordsIn list to replace word with scrambled word at the end (WD)
                            int index = Array.IndexOf(wordsIn, word);
                            StringBuilder WD = new StringBuilder(word);
                            int lengthWD = WD.Length;
                            // for each character in WD, have 2 random indexes swap
                            for (int i = 0; i < lengthWD; ++i)
                            {
                                int index1 = (random.Next(0, lengthWD));
                                int index2 = (random.Next(0, lengthWD));

                                Char temp = WD[index1];
                                WD[index1] = WD[index2];
                                WD[index2] = temp;

                            }
                            wordsIn[index] = WD.ToString();
                        }
                    }

                    // Include the words into the password
                    // adding each word to a random spot
                    int totalWordsToAdd = wordsIn.Length;
                    for (int i = 0; i < totalWordsToAdd; i++)
                    {
                        // picking the word to add
                        int wordInIndex = random.Next(0, wordsIn.Length);
                        Console.WriteLine(wordInIndex);
                        string word = wordsIn[wordInIndex];
                        Console.WriteLine(word);
                        // current word's index taken up
                        List<int> indexesTakenUp = new List<int>();
                        Console.WriteLine(String.Join(", ", wordsIn));
                        Console.WriteLine(string.Join(", ", wordsAllocatedIndexes));
                        // get the total length of all the words combined
                        string arrayFull = string.Join("", wordsIn);
                        int totalWordsLength = arrayFull.Length;
                        Console.WriteLine(totalWordsLength);
                        // we pick an index between 0 and the password's length minus the total length of all words currently in wordsIn (so we can fit them all)
                        int startIndex = random.Next(0, (buildingPassword.Length - totalWordsLength) + 1);
                        // we add all the indexes the word will take up into indexes taken up
                        indexesTakenUp.Clear();
                        for (int n = 0; n < word.Length; ++n)
                        {
                            indexesTakenUp.Add(startIndex + n);
                        }
                        Console.WriteLine(startIndex);
                        Console.WriteLine(String.Join("", indexesTakenUp));
                        // if the space the word would occupy is alrdy used by another, we try again
                        while (indexesTakenUp.Intersect(wordsAllocatedIndexes).Any())
                        {
                            startIndex = random.Next(0, (buildingPassword.Length - totalWordsLength) + 1);
                            indexesTakenUp.Clear();
                            for (int n = 0; n < word.Length; ++n)
                            {
                                indexesTakenUp.Add(startIndex + n);
                            }
                        }
                        buildingPassword.Remove(startIndex, word.Length);
                        buildingPassword.Insert(startIndex, word);
                        // add every allocated index from the word to wordsAllocatedindexes
                        foreach (int indexx in indexesTakenUp)
                        {
                            wordsAllocatedIndexes.Add(indexx);
                        }
                        Console.WriteLine(string.Join("e", wordsAllocatedIndexes));
                        // remove the current word from wordsIn (words to add)
                        wordsIn = wordsIn.Where((source, index) => index != wordInIndex).ToArray();
                        // update number of indexes remaining
                        remainingNumOfIndexes -= word.Length;
                    }

                }
                // caps logic
                if (caps)
                {
                    // the num of characters to capitalize is the length of the pass / 2 (rounded)
                    int numCharsToCap = (int)(length / 2);
                    Console.WriteLine("cap" + numCharsToCap);
                    for (int i = 0; i < numCharsToCap; ++i)
                    {
                        int indexCharToCap = random.Next(0, buildingPassword.Length - 1);
                        while (!chars.Contains(buildingPassword[indexCharToCap]))
                        {
                            indexCharToCap = random.Next(0, buildingPassword.Length - 1);
                        }
                        char upperChar = Char.ToUpper(buildingPassword[indexCharToCap]);
                        buildingPassword[indexCharToCap] = upperChar;
                    }

                }
                // nums logic
                if (nums)
                {
                    // chose the number of nums to add 
                    int numOfNums = 1 + ((remainingNumOfIndexes - 1) / 3);
                    Console.WriteLine("nums" + numOfNums);
                    Console.WriteLine(remainingNumOfIndexes);
                    // add a random number at a random free index
                    for (int i = 0; i < numOfNums; ++i)
                    {
                        // random number
                        char rannum = charnums[random.Next(0, charnums.Length)];
                        // random index
                        int indexnum = random.Next(0, length);
                        // another index until its not in the alrdy used indexes
                        while (wordsAllocatedIndexes.Contains(indexnum))
                        {
                            indexnum = random.Next(0, length);
                        }
                        buildingPassword[indexnum] = rannum;
                        wordsAllocatedIndexes.Add(indexnum);
                        remainingNumOfIndexes -= 1;
                    }
                }
                // specials logic
                if (specials)
                {
                    // chose the number of nums to add 
                    int numOfSpecials = 1 + ((remainingNumOfIndexes - 1) / 3);
                    Console.WriteLine("specials" + numOfSpecials);
                    Console.WriteLine(remainingNumOfIndexes);
                    // add a random number at a random free index
                    for (int i = 0; i < numOfSpecials; ++i)
                    {
                        // random number
                        char ranspe = charspecials[random.Next(0, charspecials.Length)];
                        // random index
                        int indexspe = random.Next(0, length);
                        // another index until its not in the alrdy used indexes
                        while (wordsAllocatedIndexes.Contains(indexspe))
                        {
                            indexspe = random.Next(0, length);
                        }
                        buildingPassword[indexspe] = ranspe;
                        wordsAllocatedIndexes.Add(indexspe);
                        remainingNumOfIndexes -= 1;
                    }
                }
                string generatedPassword = string.Join("", buildingPassword);
                return generatedPassword;
            }

            // if there is an error, returns the error text
            else
            {
                return errorText;
            }
        }
            
    }
}
