using System;
using System.Threading;
using TracerLib;
using TracerLib.Serialization;
using App.Output;
using System.IO;

namespace MyTracerApp
{
    class MainApp
    {
        static void Main(string[] args)
        {
            MainApp app = new MainApp();
            Thread thread = new Thread(new ParameterizedThreadStart(app.Method));
            Tracer tracer = new Tracer();
            Foo foo = new Foo(tracer);

            foo.MyMethod();
            thread.Start(tracer);
            foo.MyMethod();
            thread.Join();

            Thread thread1 = new Thread(new ParameterizedThreadStart(app.Method));
            foo.MyMethod();
            thread1.Start(tracer);
            thread1.Join();


            var xmlSerializer = new LXmlSerializer();
            var jsonSerializer = new JsonSerializer();
            TraceResult traceResult = tracer.GetTraceResult();
            string json = jsonSerializer.Serialize(traceResult);
            string xml = xmlSerializer.Serialize(traceResult);

            var projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            var dataPath = Path.Combine(projectDirectory, "Files");

            var filePrinter = new FilePrinter(Path.GetFullPath(Path.Combine(dataPath, "trace.xml")));
            var consolePrinter = new ConsolePrinter();

            filePrinter.PrintResult(xml);
            consolePrinter.PrintResult(xml);

            filePrinter = new FilePrinter(Path.GetFullPath(Path.Combine(dataPath, "trace.json")));

            filePrinter.PrintResult(json);
            consolePrinter.PrintResult(json);
        }

        public void Method(object o)
        {
            Tracer tracer = (Tracer)o;
            tracer.StartTrace();
            Thread.Sleep(50);
            tracer.StopTrace();
        }

        public class Foo
        {
            private Bar _bar;
            private ITracer _tracer;

            internal Foo(ITracer tracer)
            {
                _tracer = tracer;
                _bar = new Bar(_tracer);
            }

            public void MyMethod()
            {
                _tracer.StartTrace();
                _bar.InnerMethod();
                _bar.InnerMethod();
                _tracer.StopTrace();
            }
        }

        public class Bar
        {
            private ITracer _tracer;

            internal Bar(ITracer tracer)
            {
                _tracer = tracer;
            }

            public void InnerMethod()
            {
                _tracer.StartTrace();
                Thread.Sleep(100);
                _tracer.StopTrace();
            }
        }
    }
}
