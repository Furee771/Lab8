using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public delegate void IntStepDelegate(double x, double f, double integr);
        public delegate void IntFinishDelegate(double integr);
        // аргументы события интегратора
        public class IntegratorEventArgs
        {
            public double X { get; set; }
            public double F { get; set; }
            public double Integr { get; set; }
        }
        // завершить аргументы события
        public class FinishEventArgs
        {
            public double Sum { get; set; }
        }
        // интегррат
        public class Integrator
        {
            public event EventHandler<IntegratorEventArgs> OnStep;
            public event EventHandler<FinishEventArgs> OnFinish;

            private readonly Equation equation;
            public Integrator(Equation equation)
            {
                this.equation = equation;
            }
            // интегрировать
            public double Integrate(double x1, double x2)
            {
                int N = 100;
                double h = (x2 - x1) / N;
                double sum = 0;
                for (int i = 0; i < N; i++)
                {
                    double x = x1 + i * h;
                    double f = equation.Value(x1 + i * h) * h;
                    sum = sum + equation.Value(x1 + i * h) * h;
                    RaiseStepEvent(x, f, sum);
                }
                RaiseFinishEvent(sum);
                return sum;
            }
            // событие шага повышени
            void RaiseStepEvent(double x, double f, double sum)
            {
                if (OnStep != null)
                {
                    IntegratorEventArgs args = new IntegratorEventArgs()
                    {
                        X = x,
                        F = f,
                        Integr = sum,
                    };
                    OnStep(this, args);
                }
            }
            // поднять финишное событие
            void RaiseFinishEvent(double sum)
            {
                if (OnFinish != null)
                {
                    FinishEventArgs args = new FinishEventArgs()
                    {
                        Sum = sum,
                    };
                    OnFinish(this, args);
                }
            }

        }
        // уравнение
        public class Equation
        {
            private readonly double a;
            private readonly double b;
            private readonly double c;
            public Equation(double a, double b, double c)
            {
                this.a = a;
                this.b = b;
                this.c = c;
            }
            public double Value(double x)
            {
                return a * x * x + b * x + c;
            }
        }
        // начать расчет
        void BeginCalculation()
        {
            var time = DateTime.Now;
            Equation e = new Equation(-10, 20, 0);
            Integrator integr = new Integrator(e);
            FileStream fstream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "output.txt"), FileMode.OpenOrCreate);
            BinaryWriter writer = new BinaryWriter(File.Open(Path.Combine(Directory.GetCurrentDirectory(), "output2.txt"), FileMode.OpenOrCreate));

            integr.OnStep += (sender, args) => {
                byte[] array = Encoding.Default.GetBytes(string.Format("x = {0}, f = {1}, sum = {2}\r\n", args.X, args.F, args.Integr));
                fstream.Write(array, 0, array.Length);
                Thread.Sleep(10);

            };
            integr.OnStep += (sender, args) => {
                writer.Write(args.X);
                writer.Write(args.F);
                writer.Write(args.Integr);
                Thread.Sleep(10);
            };
            integr.OnStep += (sender, args) => {
                chart1.Series[0].Points.AddXY(args.X, args.F);
                chart1.Series[1].Points.AddXY(args.X, args.Integr);
                Thread.Sleep(10);
            };
            integr.OnFinish += (sender, args) =>
            {
                var time1 = DateTime.Now;
                label1.Text = (time1 - time).ToString();
            };

            double integrValue = integr.Integrate(-10, 30);
            fstream.Close();
            writer.Close();
        }

        // интегрировать на финише
        private void Integr_OnFinish(double integr)
        {
            throw new NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BeginCalculation();
        }
        // форма 1 загрузка
     

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}
