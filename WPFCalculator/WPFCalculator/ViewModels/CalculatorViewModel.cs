using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFCalculator.ViewModels
{
    public class CalculatorViewModel : ObservableObject
    {
        private enum Operation
        {
            None,
            Add,
            Subtract,
            Multiply,
            Divide,
            Percent,
            Sin,
            Cos,
            Tan,
            Square,
            SquareRoot,
            Equal,
            Memory,
            MemoryClear
        }

        private Dictionary<string, Operation> BinaryOperations = new Dictionary<string, Operation>()
        {
            { "+", Operation.Add },
            { "-", Operation.Subtract },
            { "*", Operation.Multiply },
            { "/", Operation.Divide },
            { "%", Operation.Percent },
            { "=", Operation.Equal }
        };

        private Dictionary<string, Operation> UnaryOperations = new Dictionary<string, Operation>()
        {
            { "Sin", Operation.Sin },
            { "Cos", Operation.Cos },
            { "Tan", Operation.Tan },
            { "Sqr", Operation.Square },
            { "SqRt", Operation.SquareRoot },
            { "M", Operation.Memory },
            { "MC", Operation.MemoryClear }
        };

        private static string _operandString;
        private static double _operand1;
        private static double _operand2;
        private static Operation _binaryOperator;
        private static bool empty = true;
        private static double memoryReturn;

        public ICommand ButtonNumberCommand { get; set; }
        public ICommand ButtonOperationCommand { get; set; }

        private Operation CurrentOperation { get; set; }

        private string _displayContent;

        public string DisplayContent
        {
            get { return _displayContent; }
            set
            {
                _displayContent = value;
                OnPropertyChanged("DisplayContent");
            }
        }

        public CalculatorViewModel()
        {
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            _displayContent = "Enter Number";
            ButtonNumberCommand = new RelayCommand(new Action<object>(UpdateOperandString));
            ButtonOperationCommand = new RelayCommand(new Action<object>(SetOperation));
        }

        private void UpdateOperandString(object obj)
        {
            if (obj.ToString() != "CE")
            {
                _operandString += obj.ToString();

            }
            else
            {
                _operandString = "";
            }
            DisplayContent = _operandString;
        }

        private Operation CurrentOperator(string operationString)
        {
            if (BinaryOperations.ContainsKey(operationString))
            {
                return BinaryOperations[operationString];
            }
            else if (UnaryOperations.ContainsKey(operationString))
            {
                return UnaryOperations[operationString];
            }

            return Operation.None;
        }

        private void SetOperation(object obj)
        {
            Operation operation = CurrentOperator(obj.ToString());

            if (double.TryParse(_operandString, out double result))
            {
                if (BinaryOperations.ContainsValue(operation))
                {
                    if (operation == Operation.Equal)
                    {
                        _operand2 = result;
                        DisplayContent = ProcessBinaryOperation(_binaryOperator).ToString();
                        _binaryOperator = Operation.None;
                    }
                    else if (operation == Operation.Percent)
                    {
                        _operand2 = result;
                        _binaryOperator = Operation.Percent;
                        DisplayContent = ProcessBinaryOperation(_binaryOperator).ToString();
                    }
                    else
                    {
                        _operand1 = result;
                        _binaryOperator = operation;
                        _operandString = "";
                        DisplayContent = "";
                    }
                }
                else if (UnaryOperations.ContainsValue(operation))
                {
                    _operand1 = result;
                    DisplayContent = ProcessUnaryOperation(operation).ToString();
                }
                else
                {
                    DisplayContent = "Unknown Operation Encountered";
                }

                _operandString = DisplayContent;
            }
            else
            {
                DisplayContent = "Please enter a valid number.";
            }
        }

        private double ProcessUnaryOperation(Operation operation)
        {
            switch (operation)
            {
                case Operation.Sin:
                    return Math.Sin(_operand1);
                case Operation.Cos:
                    return Math.Cos(_operand1);
                case Operation.Tan:
                    return Math.Tan(_operand1);
                case Operation.Square:
                    return Math.Pow(_operand1, 2);
                case Operation.SquareRoot:
                    return Math.Sqrt(_operand1);
                case Operation.Memory:
                    if (empty == true)
                    {
                        memoryReturn = _operand1;
                        empty = false;
                        return 0;
                    }
                    else
                    {
                        return _operand1 = memoryReturn;
                    }
                case Operation.MemoryClear:
                    empty = true;
                    return _operand1;
                default:
                    DisplayContent = "Unknown Operation Encountered";
                    return 0;
            }
        }

        private double ProcessBinaryOperation(Operation operation)
        {
            switch (operation)
            {
                case Operation.Add:
                    return _operand1 + _operand2;
                case Operation.Subtract:
                    return _operand1 - _operand2;
                case Operation.Multiply:
                    return _operand1 * _operand2;
                case Operation.Divide:
                    return _operand1 / _operand2;
                case Operation.Percent:
                    return _operand1 * (_operand2 / 100);
                default:
                    DisplayContent = "Unknown Operation Encountered";
                    return 0;
            }
        }
    }
}
