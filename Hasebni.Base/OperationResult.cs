using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Base
{
    public class OperationResult<T> 
    {
        public OperationResultTypes OperationResultType { get; set; }
        public bool IsSuccess => OperationResultType == OperationResultTypes.Success;
        public string OperationResultMessage { get; set; }
        public Exception Exception { get; set; }
        public T Result { get; set; }
        public IEnumerable<T> IEnumerableResult { get; set; }
        public ResultTypes ResultType { get; set; }
    }
}
