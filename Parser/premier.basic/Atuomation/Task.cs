using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using premier.parser;

namespace premier.basic.Atuomation
{
    public delegate void RegisterResults(List<SportEvent> results);
    public delegate void LogData(string description);

    public enum TaskType
    {
        OpanAllGames,
        OpenGame,
        NextPage,
        CheckNextPage,
        ParseData,
        Done,
    }

    public class Task
    {
        private static string nextPage = string.Empty;

        public TaskType TaskType { get; set; }
        public string Description { get; set; }
        public HtmlElement Element { get; set; }
        public HtmlDocument Document { get; set; }
        public int Interval { get; set; }
        public RegisterResults RegisterResults { get; set; }
        public LogData LogData { get; set; }

        public Task(TaskType taskType, string description, int interval, HtmlElement element, HtmlDocument document, RegisterResults registerResults, LogData logData)
        {
            TaskType = taskType;
            Description = description;
            Interval = interval;
            Element = element;
            Document = document;
            RegisterResults = registerResults;
            LogData = logData;
        }

        public List<Task> Execute()
        {
            LogData(Description);
            if (Document != null)
            {
                Document.InvokeScript("handleMove");
            }
            if (TaskType == TaskType.OpanAllGames)
            {
                string currentPage = string.Empty;
                foreach (HtmlElement tr in Document.Body.GetElementsByTagName("b"))
                {
                    currentPage = tr.InnerText;
                    break;
                }

                List<Task> newTasks = new List<Task>();
                List<HtmlElement> gamesToOpen = new List<HtmlElement>();
                foreach (HtmlElement tr in Document.Body.GetElementsByTagName("div"))
                {
                    string outerHTML = string.IsNullOrEmpty(tr.OuterHtml) ? string.Empty : tr.OuterHtml.TrimStart();
                    string innerText = string.IsNullOrEmpty(tr.InnerText) ? string.Empty : tr.InnerText.TrimStart();
                    if (outerHTML.StartsWith("<DIV id=block") && innerText.StartsWith("+"))
                    {
                        gamesToOpen.Add(tr);
                    }
                }
                int counter = 0;
                foreach (HtmlElement element in gamesToOpen)
                {
                    counter++;
                    Task task = new Task(TaskType.OpenGame, "Opening game " + counter + " of " + gamesToOpen.Count + "...", Interval, element, Document, RegisterResults, LogData);
                    newTasks.Add(task);
                }
                newTasks.Add(new Task(TaskType.ParseData, "Parsing data on current page (" + currentPage + ")...", Interval, null, Document, RegisterResults, LogData));

                bool nextPage = false;
                foreach (HtmlElement tr in Document.Body.GetElementsByTagName("a"))
                {
                    string outerHTML = string.IsNullOrEmpty(tr.OuterHtml) ? string.Empty : tr.OuterHtml.TrimStart();
                    if (outerHTML.Contains("continue"))
                    {
                        newTasks.Add(new Task(TaskType.NextPage, "Going to next page...", Interval, tr, Document, RegisterResults, LogData));
                        nextPage = true;
                        break;
                    }
                    //previousPage = tr.InnerHtml;
                }

                if (!nextPage)
                {
                    newTasks.Add(new Task(TaskType.Done, "Finished!", Interval, null, null, RegisterResults, LogData));
                }

                return newTasks;
            }
            else if (TaskType == TaskType.OpenGame)
            {
                Element.InvokeMember("click");
                return null;
            }
            else if (TaskType == TaskType.NextPage)
            {
                List<Task> newTasks = new List<Task>();

                Element.InvokeMember("click");

                newTasks.Add(new Task(TaskType.CheckNextPage, "Waiting for data...", Interval, Element, Document, RegisterResults, LogData));
                return newTasks;
            }
            else if (TaskType == TaskType.CheckNextPage)
            {
                List<Task> newTasks = new List<Task>();

                HtmlElement element = null;
                foreach (HtmlElement tr in Document.Body.GetElementsByTagName("a"))
                {
                    string outerHTML = string.IsNullOrEmpty(tr.OuterHtml) ? string.Empty : tr.OuterHtml.TrimStart();
                    if (outerHTML.Contains("continue"))
                    {
                        element = tr;
                        break;
                    }
                }

                if (element != null && element.OuterHtml == Element.OuterHtml)
                {
                    // newTasks.Add(new Task(TaskType.CheckNextPage, Description, Interval, Element, Document, RegisterResults, LogData));
                    newTasks.Add(this);
                }
                else 
                {
                    newTasks.Add(new Task(TaskType.OpanAllGames, "Opening all games on current page...", Interval, null, Document, RegisterResults, LogData));
                }
                return newTasks;
            }
            else if (TaskType == TaskType.ParseData)
            {
                DataParser parser = new DataParser();
                parser.ParseHtml(Document);
                RegisterResults(parser.Events);
            }

            return null;
        }
    }
}
