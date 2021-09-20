using System;
using System.Threading;
using TracerLib;
using TracerLib.Serialization;

namespace MyTracerApp
{
    class MainApp
    {
        static void Main(string[] args)
        {
            Tracer tracer = new Tracer();
            tracer.StartTrace();
            Thread.Sleep(100);
            tracer.StopTrace();

            MainApp app = new MainApp();
            Thread thread1 = new Thread(new ParameterizedThreadStart(app.Method));
            thread1.Start(tracer);
            thread1.Join();

            var xmlSerializer = new LXmlSerializer();
            var jsonSerializer = new JsonSerializer();
            TraceResult traceResult = tracer.GetTraceResult();
            string json = jsonSerializer.Serialize(traceResult);
            string xml = xmlSerializer.Serialize(traceResult);
            
            Console.WriteLine(xml);
            Console.WriteLine(json);
        }

        public void Method(object o)
        {
            Tracer tracer = (Tracer)o;
            tracer.StartTrace();
            Thread.Sleep(50);
            tracer.StopTrace();
        }
    }
}
