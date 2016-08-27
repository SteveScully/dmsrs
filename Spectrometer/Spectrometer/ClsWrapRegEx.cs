using System;
using System.Text.RegularExpressions;

namespace Raman
{
    class RegExWrapper
    {

            // Create a regular expression.
        public Regex RegularExpression = new Regex("regularexpression", RegexOptions.Singleline); //01-08-2014 treat all as single line




            ///'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ///'''''''''''''''''''''''''''REG_EX''''''''''''''''''''''''''''''''''''
            ///'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            public bool TstRegEx( string patrn,  string strng)
            {
                bool functionReturnValue = false;
                RegularExpression = new Regex(patrn, RegexOptions.IgnoreCase );
                Match m = RegularExpression.Match(strng);
                // Execute search.
                if (m.Success)
                {
                    functionReturnValue = true;
                }
                else
                {
                    functionReturnValue = false;
                }
                return functionReturnValue;

            }

            public string RepRegex( string patrn,  string StrIn,  string strReplacement)
            {
                string Rstr;

                Rstr = Regex.Replace(StrIn, patrn, strReplacement, RegexOptions.IgnoreCase);
                // Set pattern.


                //   RepRegEx = _RegEx.Replace(StrIn, strReplacement) ' Execute search.
                return Rstr;


            }

            public MatchCollection ExeRegEx( string patrn,  string strng)
            {
                RegularExpression = new Regex(patrn, RegexOptions.IgnoreCase );
                //  RegularExpression = New Regex(patrn, RegexOptions.Multiline)


                return RegularExpression.Matches(strng);
                // Execute search.

            }

            public string Str0ExeRegEx( string patrn,  string strng)
            {
                string functionReturnValue = null;
                RegularExpression = new Regex(patrn, RegexOptions.IgnoreCase ); // RegexOptions.IgnoreCase );


                MatchCollection Matches = default(MatchCollection);
                // Create variable.

                Matches = RegularExpression.Matches(strng);
                // Execute search.
                if (Matches.Count > 0)
                {
                    //UPGRADE_WARNING: Couldn't resolve default property of object Matches.Item().Value. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    functionReturnValue = Matches[0].Groups[0].Value;
                }
                else
                {
                    functionReturnValue = "";
                }
                return functionReturnValue;
            }


            public string StrNExeRegEx( string patrn,  string strng, int MatchNum)
            {
                string functionReturnValue = null;
                int countReq = 0;
                RegularExpression = new Regex(patrn, RegexOptions.IgnoreCase );
                MatchCollection Matches = default(MatchCollection);
                // Create variable.
                //UPGRADE_WARNING: Couldn't resolve default property of object MatchNum. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                if ((MatchNum > 0))
                    countReq = MatchNum;
                else
                    countReq = 0;
                //sws 010703 changed countreq for 0 item to mirror str0 func
                Matches = RegularExpression.Matches(strng);
                // Execute search.
                if ((Matches.Count - 1) >= countReq)
                {
                    functionReturnValue = Matches[MatchNum].Groups[0].Value;
                }
                else
                {
                    functionReturnValue = "";
                }
                return functionReturnValue;
            }

















        }
    }
