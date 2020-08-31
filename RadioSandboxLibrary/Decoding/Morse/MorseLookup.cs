using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RadioSandboxLibrary.Decoding.Morse
{
    public static class MorseLookup
    {
        public static char LookupMorse(string morseCode)
        {
            return AlphaCharacters[morseCode];
        }

        private static Dictionary<string, char> AlphaCharacters = new Dictionary<string, char>()
        {
            // Characters
            { ".-"    , 'A'},
            { "-..."  , 'B'},
            { "-.-."  , 'C'},
            { "-.."   , 'D'},
            { "."     , 'E'},
            { "..-."  , 'F'},
            { "--."    , 'G'},
            { "...."  , 'H'},
            { ".."    , 'I'},
            { ".---"  , 'J'},
            { "-.-"   , 'K'},
            { ".-.."  , 'L'},
            { "--"    , 'M'},
            { "-."    , 'N'},
            { "---"   , 'O'},
            { ".--."  , 'P'},
            { "--.-"  , 'Q'},
            { ".-."   , 'R'},
            { "..."   , 'S'},
            { "-"     , 'T'},
            { "..-"   , 'U'},
            { "...-"  , 'V'},
            { ".--"   , 'W'},
            { "-..-"  , 'X'},
            { "-.--"  , 'Y'},
            { "--.."  , 'Z'},

            // Numbers
            
            { ".----" ,'1'},
            { "..---" ,'2'},
            { "...--" ,'3'},
            { "....4" ,'4'},
            { "....." ,'5'},
            { "-...." ,'6'},
            { "--..." ,'7'},
            { "---.." ,'8'},
            { "----." ,'9'},
            { "-----" ,'0'},
                    
            // Special Characters
            //{ ".-.-.-" , '.'}, // Fullstop
            //{ "--..--" , ','}, // Comma
            //{ "---..." , ':'}, // Colon
            //{ "..--.." , '?'}, // Question Mark
            //{ ".----.", '\\'}, // Apostrophe
            //{ "-....-" , '-'}, // Hyphen, dash, minus
            //{ ".-..-." , '"'}, // Quotaion mark
            //{ "-...-"  , '='}, // Equal sign
            //{ ".-.-."  , '+'}, // Plus
            //{ "-..-"   , '*'}, // multiplication
            //{ ".--.-." , '@'}, // At the rate of

            //// Brackets
            //{ "-.--." ,'(' }, // Left bracket
            //{ "-.--." ,'{' }, // Left bracket
            //{ "-.--." ,'[' }, // Left bracket
            //{ "-.--.-",')' }, // right bracket
            //{ "-.--.-",'}' }, // right bracket
            //{ "-.--.-",']' }, // right bracket     

        };
    }
}
