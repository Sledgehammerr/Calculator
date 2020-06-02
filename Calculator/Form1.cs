using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Form1 : Form
    {
        private Calculator calc;
        public Form1()
        {
            InitializeComponent();
            calc = new Calculator();
            calc.DidUpdateValue += CalculatorDidUpdateValue;
            calc.InputError += CalculatorError;
            calc.ComputationError += CalculatorError;
            calc.Clear();
            label1.Text = "0";
        }

        private void CalculatorError(Calculator sender, string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void CalculatorDidUpdateValue(Calculator sender, double value, int precision)
        {
            if (precision > 0)
                label1.Text = value.ToString("F" + precision.ToString());
            else
                label1.Text = value.ToString();
        }

        private void button_Click(object sender, EventArgs e)
        {
            int digit = -1;
            Button b = sender as Button;
            if (int.TryParse(b.Text, out digit))
            {
                calc.AddDigit(digit);

            } else
            {
                switch (b.Tag)
                {
                    case "plus":
                        calc.AddOperation(CalculatorOperation.Add);
                        break;
                    case "minus":
                        calc.AddOperation(CalculatorOperation.Sub);
                        break;
                    case "multiplication":
                        calc.AddOperation(CalculatorOperation.Mul);
                        break;
                    case "division":
                        calc.AddOperation(CalculatorOperation.Div);
                        break;
                    case "negate":
                        calc.TransformInput(CalculatorTransformation.Negate);
                        break;
                    case "percent":
                        calc.TransformInput(CalculatorTransformation.Percent);
                        break;
                    case "sqr":
                        calc.TransformInput(CalculatorTransformation.Sqr);
                        break;
                    case "sqrt":
                        calc.TransformInput(CalculatorTransformation.Sqrt);
                        break;
                    case "inverse":
                        calc.TransformInput(CalculatorTransformation.Inverse);
                        break;
                    case "point":
                        calc.AddPoint();
                        break;
                    case "compute":
                        calc.Compute();
                        break;
                    case "delete":
                        calc.Delete();
                        break;
                    case "clear":
                        calc.Clear();
                        break;
                    default:
                        MessageBox.Show(b.Tag?.ToString() ?? "");
                        break;
                }
            }
        }
    }
}
