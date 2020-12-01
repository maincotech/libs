using System.IO;

namespace System
{
    public partial class Check
    {
        #region Const Literals

        private const string FilenotexistsFailingMessage = "file {0} not exists";
        private const string BetweenFailingMessage = "{0} {1} {2} {3} {4} requird";
        private const string IsEmailAddressFailingMessage = "{0} is not a valide email address";

        #endregion Const Literals

        public static ICheckStrategy IsEmailAddress = new IsEmailAddressStrategy();

        public static ICheckStrategy FileExists<T>(T compareValue)
        {
            return new FileExistsStrategy<T>(compareValue);
        }

        public static ICheckStrategy Between<T>(T minValue, T maxValue, bool allowEqual)
        {
            return new BetweenStrategy<T>(minValue, maxValue, allowEqual);
        }

        private sealed class IsEmailAddressStrategy : ICheckStrategy
        {
            #region ICheckStrategy Members

            public bool Pass(object obj)
            {
                if (obj is string)
                {
                    return (obj as string).IsEmailAddress();
                }

                return false;
            }

            public string GetFailingMessage(string objName)
            {
                return string.Format(IsEmailAddressFailingMessage, objName);
            }

            #endregion ICheckStrategy Members
        }

        private sealed class FileExistsStrategy<T> : ICheckStrategy
        {
            private readonly T _compareValue;

            public FileExistsStrategy(T compareValue)
            {
                this._compareValue = compareValue;
            }

            #region ICheckStrategy Members

            public bool Pass(object obj)
            {
                if (obj == null || _compareValue == null)
                    return false;
                if (_compareValue is string)
                {
                    if (string.IsNullOrEmpty(_compareValue as string))
                        return false;
                    return File.Exists(_compareValue as string);
                }
                if (_compareValue is FileInfo)
                {
                    return (_compareValue as FileInfo).Exists;
                }

                return false;
            }

            public string GetFailingMessage(string objName)
            {
                return string.Format(FilenotexistsFailingMessage, objName, _compareValue);
            }

            #endregion ICheckStrategy Members
        }

        private sealed class BetweenStrategy<T> : ICheckStrategy
        {
            private readonly T _minValue;
            private readonly T _maxValue;

            private bool _isGreaterThan;
            private bool _isLessThan;
            private readonly bool _allowEqual;

            public BetweenStrategy(T minValue, T maxValue, bool allowEqual)
            {
                this._minValue = minValue;
                this._maxValue = maxValue;
                _allowEqual = allowEqual;
            }

            #region ICheckStrategy Members

            public bool Pass(object obj)
            {
                if (_allowEqual)
                {
                    _isGreaterThan = obj is T && ((IComparable)obj).CompareTo(_minValue) > 0;

                    _isLessThan = obj is T && ((IComparable)obj).CompareTo(_maxValue) < 0;
                }
                else
                {
                    _isGreaterThan = obj is T && ((IComparable)obj).CompareTo(_minValue) >= 0;

                    _isLessThan = obj is T && ((IComparable)obj).CompareTo(_maxValue) <= 0;
                }

                return _isGreaterThan && _isLessThan;
            }

            public string GetFailingMessage(string objName)
            {
                var lessEqual = _allowEqual ? "<=" : "<";
                var greatEqual = _allowEqual ? ">=" : ">";

                return string.Format(BetweenFailingMessage, _minValue, lessEqual, objName, greatEqual, _maxValue);
            }

            #endregion ICheckStrategy Members
        }
    }
}