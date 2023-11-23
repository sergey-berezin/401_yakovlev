using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    static public class UtilsForBD 
    {
        static public void addTextToBD(TextsAnswers bd, string new_text) 
        {
            //check text existing
            TextID? temp_value = bd.IDtoText.FirstOrDefault(x => x.Text == new_text);
            if (temp_value == null) {
                //get unique ID for text
                int textID = 0;
                int[] textIDs = bd.IDtoText.Select(x => x.ID).ToArray();
                Array.Sort(textIDs);
                foreach (int curID in textIDs) {
                    if (curID == textID)
                    {
                        textID++;
                    }
                    else
                    { 
                        break;
                    }
                }
                bd.Add(new TextID { ID = textID, Text = new_text });
                bd.SaveChanges();
            }
        }
        static public bool CheckTextInDB(TextsAnswers bd, string inner_text)
        {
            TextID? founded_pair = bd.IDtoText.FirstOrDefault(x => x.Text == inner_text);
            if (founded_pair == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        static public int getTextIDwithAddText(TextsAnswers bd, string inner_text)
        {
            if (CheckTextInDB(bd, inner_text) == false) 
            { 
                addTextToBD(bd, inner_text);
            }
            return bd.IDtoText.Where(x => x.Text == inner_text).First().ID;
        }
        static public void addQuestionAnswertoBD(TextsAnswers bd, string inner_text, string question, string answer) 
        {
            int textID = getTextIDwithAddText(bd, inner_text);
            TextIDQuestion? res = bd.textIDQuestions.FirstOrDefault(x => x.ID == textID && x.Question == question);
            if (res == null) 
            {
                bd.Add(new TextIDQuestion { Answer = answer, ID = textID, Question = question });
                bd.SaveChanges();
            }
        }
        static public string? getAnswerBD(TextsAnswers bd, string inner_text, string inner_question)
        {
            int textID = getTextIDwithAddText(bd, inner_text);
            TextIDQuestion? res = bd.textIDQuestions.FirstOrDefault(x => x.ID == textID && x.Question == inner_question);
            if (res == null)
            {
                return null;
            }
            return res.Answer;
        }
    }
}
