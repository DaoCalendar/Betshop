using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace premier.basic.Atuomation
{
    public class Manager
    {
        private static Manager instance = new Manager();
        public static Manager Instance { get { return instance; } }

        private Random random = new Random();
        private Timer timer = new Timer();
        private Queue<Task> tasks = new Queue<Task>();

        public WebBrowser Browser { get; set; }

        private Manager()
        {
            timer.Tick += new EventHandler(timer_Tick);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            if (tasks.Count != 0)
            {
                Task nextTask = tasks.Dequeue();

                if (Browser.ReadyState != WebBrowserReadyState.Complete)
                {
                    nextTask.LogData("Waiting for data (global)");

                    timer.Start();
                    return;
                }

                List<Task> newTasks = nextTask.Execute();
                if (newTasks != null)
                {
                    foreach (Task task in newTasks)
                    {
                        tasks.Enqueue(task);
                    }
                }
                timer.Interval = nextTask.Interval * random.Next(70, 100) / 100;
                timer.Start();
            }
        }

        public void AddTask(Task task)
        {
            tasks.Enqueue(task);
        }

        internal void StartTasks()
        {
            timer.Start();
        }
    }
}
