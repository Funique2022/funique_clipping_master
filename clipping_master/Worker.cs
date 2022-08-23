using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ffmpeg_helper
{
    public enum TimeMark
    {
        None, End, Length
    }

    public sealed class Worker
    {
        readonly FileStructure data;
        readonly List<string> encoders = new List<string>(new string[] { "source" });
        string ext = string.Empty;
        int job_index = 0;
        int index = 0;

        public Worker(FileStructure structure)
        {
            data = structure;
            Console.WriteLine($"Detect there are {data.jobs.job.Count} actions to execute");
        }

        public void Run()
        {
            if (!File.Exists(data.header.filename))
            {
                Console.WriteLine($"Cannot find the video file: {data.header.filename}");
                return;
            }
            if (Directory.Exists(data.header.temp) && data.header.cleantemp)
            {
                Console.WriteLine($"Destroy temp directory: {data.header.temp}");
                Directory.Delete(data.header.temp, true);
            }
            Directory.CreateDirectory(data.header.temp);

            ext = Path.GetExtension(data.header.filename);
            foreach (var i in data.jobs.job)
            {
                if(i.type == 0)
                {
                    Single(i);
                }
                else if(i.type == 1)
                {
                    Multiple(i);
                }
                job_index++;
            }
        }

        bool Single(Job job)
        {
            if (!job.action.HasValue)
            {
                Console.WriteLine($"Job {job_index} does not have action registerd");
                return false;
            }
            Action a = job.action.Value;
            if (!VaildAction(a, 0))
            {
                Console.WriteLine($"Action in Job {job_index} is not vaild");
                return false;
            }

            switch (a.type.ToLower())
            {
                case "cut":
                    index++;
                    string output_name = string.IsNullOrEmpty(a.name) ? $"temp_{index}{ext}" : a.name;
                    Cut(a, index, output_name);
                    break;
                case "merge":
                    Merge(a);
                    break;
            }
            return true;
        }

        bool Multiple(Job job)
        {
            if (!job.actions.HasValue)
            {
                Console.WriteLine($"Job {job_index} does not have actions registerd");
                return false;
            }
            Action[] a = job.actions.Value.action.ToArray();

            if (a.Length == 0)
            {
                Console.WriteLine($"Job {job_index} has zero action inside it");
                return false;
            }
            List<Task> tasks = new List<Task>();

            int c = 0;
            foreach(var i in a)
            {
                if (!VaildAction(i, 1))
                {
                    Console.WriteLine($"Subaction {c} in Action in Job {job_index} is not vaild");
                    continue;
                }

                switch (i.type.ToLower())
                {
                    case "cut":
                        index++;
                        string output_name = string.IsNullOrEmpty(i.name) ? $"temp_{index}.{ext}" : i.name;
                        tasks.Add(CutAsync(i, index, output_name));
                        break;
                }
                c++;
            }

            Task[] _tasks = tasks.ToArray();
            Task.WaitAll(_tasks);
            return true;
        }

        bool Cut(Action target, int _index, string output)
        {
            TimeMark time_mark_mode = TimeMark.None;
            TimeSpan start = TimeSpan.Parse(target.start);
            TimeSpan mark;
            if (TimeSpan.TryParse(target.end, out mark))
                time_mark_mode = TimeMark.End;
            else if (TimeSpan.TryParse(target.length, out mark))
                time_mark_mode = TimeMark.Length;
            else
                time_mark_mode = TimeMark.None;

            TimeSpan realEnd = time_mark_mode == TimeMark.End ? mark : start + mark;
            string arg = $"-ss {start} -to {realEnd} -i \"{data.header.filename.Replace('\\', '/')}\" -codec:v copy -acodec copy \"{data.header.temp}/{output}\"";
            Console.WriteLine($"arg: {arg}");
            ProcessStartInfo info = new ProcessStartInfo("ffmpeg.exe", arg);
            info.UseShellExecute = false;
            Process proc = Process.Start(info);
            proc.WaitForExit();
            return true;
        }

        Task<bool> CutAsync(Action target, int _index, string output)
        {
            return Task.FromResult(Cut(target, _index, output));
        }

        void Merge(Action target)
        {
            string[] files = new string[0];
            string d = string.Empty;

            if (target.all.Value)
            {
                files = Directory.GetFiles(data.header.temp);
            }
            else
            {
                FileNames fn = target.files.Value;
                files = fn.file.ToArray();
            }

            foreach (var i in files)
            {
                d += $"file \'{i}\'\n";
            }
            File.WriteAllText("c.txt", d);

            string coder = string.IsNullOrEmpty(target.encoder) ? "target.encoder" : "copy";
            string arg = $"-f concat -safe 0 -i c.txt -c {coder} -y \"{data.header.output.Replace('\\', '/')}\"";
            Console.WriteLine($"arg: {arg}");
            ProcessStartInfo info = new ProcessStartInfo("ffmpeg.exe", arg);
            info.UseShellExecute = false;
            Process proc = Process.Start(info);
            proc.WaitForExit();
        }

        bool VaildAction(Action target, int job_type)
        {
            switch (target.type.ToLower())
            {
                case "cut":
                    return IsTimeCode(target.start) && (IsTimeCode(target.end) || IsTimeCode(target.length));
                case "merge":
                    return job_type == 0 && (target.all.HasValue ? (target.all.Value ? true : target.files.HasValue) : target.files.HasValue);
            }
            return true;
        }

        bool IsTimeCode(string n) => TimeSpan.TryParse(n, out _);
    }
}
