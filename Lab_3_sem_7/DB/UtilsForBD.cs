using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    static public class UtilsForBD 
    {
        static public List<(string, string?, string?, int)> getAllRecordToRestore(TextsAnswers db) 
        {
            List<(string, string?, string?, int)> answer = new();

            //var answer = Array.Empty<(string, string, string)>;
            foreach (var cur_elem in db.textIDQuestions) {
                string? text = UtilsForBD.getTextbyID(db, cur_elem.TextNum);
                for (int i = 0; i < cur_elem.openCount; i++)
                {
                    answer.Add((text, cur_elem.Question, cur_elem.Answer, cur_elem.ID));
                }
            }
            return answer;
        }
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
        static public int getTextID(TextsAnswers bd, string inner_text)
        {
            return bd.IDtoText.Where(x => x.Text == inner_text).First().ID;
        }
        static public string? getTextbyID(TextsAnswers db, int ID)
        {
            foreach (var cur_elem in db.IDtoText) 
            {
                if (cur_elem.ID == ID) {
                    return cur_elem.Text;
                }
            }
            return null;
        }
        static public void decOpenCount(TextsAnswers db, int QueryNum)
        {
            foreach (var cur_elem in db.textIDQuestions) 
            {
                if (cur_elem.ID == QueryNum)
                {
                    if (cur_elem.openCount > 0) {
                        db.textIDQuestions.Remove(cur_elem);
                        db.SaveChanges();
                        cur_elem.openCount--;
                        db.textIDQuestions.Add(cur_elem);
                        db.SaveChanges();
                        break;
                    }
                }
            }
        }
        static public int addQuestionAnswertoBD(TextsAnswers db, string inner_text, string? question, string? answer, int QueryNum = -1) 
        {
            if (!CheckTextInDB(db, inner_text)) 
            {
                addTextToBD(db, inner_text);
            }
            int textID = getTextID(db, inner_text);
            int ID = 1;
            int[] uniqIDs = db.textIDQuestions.Select(x => x.ID).ToArray();
            string? res = getAnswerBD(db, inner_text, question);
            Array.Sort(uniqIDs);
            if (res == null)
            {
                foreach (int curID in uniqIDs)
                {
                    if (curID == ID)
                    {
                        ID++;
                    }
                    else
                    {
                        break;
                    }
                }
                bool isTextWithoutQuestionInDB = false;
                foreach (TextIDQuestion cur_elem in db.textIDQuestions)
                {
                    if (cur_elem.TextNum == textID && cur_elem.Question == null && cur_elem.ID == QueryNum)
                    {
                        // доабвления ответа к уже существующему тексту
                        //record with same text exist in table. but question doesn't exist
                        isTextWithoutQuestionInDB = true;
                        db.textIDQuestions.Remove(cur_elem);
                        db.SaveChanges();
                        db.textIDQuestions.Add(
                            new TextIDQuestion
                            {
                                Answer = answer,
                                ID = cur_elem.ID,
                                TextNum = cur_elem.TextNum,
                                Question = question,
                                openCount = cur_elem.openCount + 1
                            }); ;
                        db.SaveChanges();
                        return cur_elem.ID;
                    }
                }
                //факически, доабвление текста без вопроса и ответа
                if (isTextWithoutQuestionInDB == false) 
                {
                    db.Add(new TextIDQuestion
                    {
                        Answer = answer,
                        ID = ID,
                        TextNum = textID,
                        Question = question,
                        openCount = 1
                    });
                    db.SaveChanges();
                }
                return ID;
            }
            else 
            {
                //record with text, question, answer exist in table
                foreach (TextIDQuestion cur_elem in db.textIDQuestions)
                {
                    //удаляем существующую запись с текстом и пустыми полями ответа и вопроса, если такая была
                    if (cur_elem.TextNum == textID && cur_elem.ID == QueryNum && cur_elem.Question == null)
                    {
                        db.textIDQuestions.Remove(cur_elem);
                        db.SaveChanges();
                        break;
                    }
                }
                foreach (TextIDQuestion cur_elem in db.textIDQuestions)
                {
                    if (cur_elem.TextNum == textID && cur_elem.Answer != null)
                    {
                        //увеличиваем счетчик ссылающихся на эту запись вкладок, если вопрос ранее не был привян к этой вкладки
                        db.textIDQuestions.Remove(cur_elem);
                        db.SaveChanges();
                        cur_elem.openCount++;
                        db.Add(cur_elem);
                        db.SaveChanges();
                        return cur_elem.ID;
                    }
                }
            }
            return -1;
        }
        static public string? getAnswerBD(TextsAnswers db, string inner_text, string? inner_question)
        {
            if (!CheckTextInDB(db, inner_text)) 
            {
                addTextToBD(db, inner_text);
            }
            int textID = getTextID(db, inner_text);
            foreach (TextIDQuestion cur_elem in db.textIDQuestions) 
            {
                if (cur_elem.TextNum == textID && cur_elem.Question == inner_question) 
                { 
                    return cur_elem.Answer;
                }
            }
            return null;
        }
        static public void DropDatabase(TextsAnswers db)
        {
            db.IDtoText.RemoveRange(db.IDtoText);
            db.SaveChanges();
            db.textIDQuestions.RemoveRange(db.textIDQuestions);
            db.SaveChanges();
        }
    }
}
