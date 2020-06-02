using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    delegate void CalculatorUpdateOutput(Calculator sender, double value, int precision);
    delegate void CalculatorInternalError(Calculator sender, string message);
    enum CalculatorOperation { Add, Sub, Mul, Div }
    enum CalculatorTransformation { Negate, Percent, Sqr, Sqrt, Inverse }
    class Calculator
    {
        double? input = null;
        double? result = null;

        CalculatorOperation? op = null;

        bool hasPoint;
        int fractionDigits = 0;

        public event CalculatorUpdateOutput DidUpdateValue;
        public event CalculatorInternalError InputError;
        public event CalculatorInternalError ComputationError;

        public void AddDigit(int digit)
        {
            if (!hasPoint)
            {
                if (input.HasValue && Math.Log10(input.Value) > 9)
                {
                    InputError?.Invoke(this, "Value too long");
                    return;
                }

                input = (input ?? 0) * 10 + digit;
            }
            else
            {
                if (fractionDigits > 8)
                {
                    InputError?.Invoke(this, "Value too long");
                    return;
                }
                fractionDigits++;
                input = (input ?? 0) + digit * Math.Pow(10, -fractionDigits);
            }
            DidUpdateValue?.Invoke(this, input.Value, fractionDigits);
        }

        public void AddPoint()
        {
            hasPoint = true;
        }

        public void TransformInput(CalculatorTransformation t)
        {
            input = input ?? 0;

            switch (t)
            {
                case CalculatorTransformation.Negate:
                    input = -input;
                    break;
                case CalculatorTransformation.Percent:
                    input /= 100;
                    break;
                case CalculatorTransformation.Sqr:
                    input *= input;
                    break;
                case CalculatorTransformation.Sqrt:
                    if (input.HasValue && input.Value >= 0)
                        input = Math.Sqrt(input ?? 0);
                    else
                        ComputationError.Invoke(this, "Negative Square");
                    break;
                case CalculatorTransformation.Inverse:
                    if (input.HasValue && input.Value != 0)
                        input = 1 / input;
                    else
                        ComputationError.Invoke(this, "Division by Zero");
                    break;
            }

            DidUpdateValue?.Invoke(this, input.Value, fractionDigits);
        }

        public void AddOperation(CalculatorOperation op)
        {
            if (this.op.HasValue)
                Compute();
            this.op = op;
            if (input.HasValue)
            {
                result = input;
                this.Clear();
            }
        }

        public void Compute()
        {
            switch (this.op)
            {
                case CalculatorOperation.Add:
                    result += input;
                    DidUpdateValue?.Invoke(this, result.Value, 0);
                    ResetInput();
                    break;
                case CalculatorOperation.Sub:
                    result -= input;
                    DidUpdateValue?.Invoke(this, result.Value, 0);
                    ResetInput();
                    break;
                case CalculatorOperation.Mul:
                    result *= input;
                    DidUpdateValue?.Invoke(this, result.Value, 0);
                    ResetInput();
                    break;
                case CalculatorOperation.Div:
                    if (input.HasValue && input.Value != 0)
                    {
                        result /= input;
                        DidUpdateValue?.Invoke(this, result.Value, 0);
                        ResetInput();
                    }
                    else
                    {
                        ComputationError.Invoke(this, "Division by Zero");
                    }
                    break;
            }
        }

        public void Delete()
        {
            if (!hasPoint)
            {
                input = (int)(input ?? 0) / 10;
            }
            else
            {
                input = ((int)((input ?? 0) / Math.Pow(10, -fractionDigits)) / 10) * Math.Pow(10, -(--fractionDigits));
                if (fractionDigits == 0)
                {
                    hasPoint = false;
                }
            }
            DidUpdateValue?.Invoke(this, input.Value, fractionDigits);
        }

        public void ResetInput()
        {
            input = null;
            fractionDigits = 0;
            hasPoint = false;
        }

        public void Clear()
        {
            ResetInput();
            DidUpdateValue?.Invoke(this, 0, 0);
        }
    }
}
