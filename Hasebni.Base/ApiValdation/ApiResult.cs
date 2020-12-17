using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hasebni.Base.ApiValidation
{
    public class ApiResultObject<T>
    {
        public OperationResultTypes Result { get; set; } = OperationResultTypes.Success;
        public T Data;
    }

    public class ApiResultCollection<T>
    {
        public OperationResultTypes Result { get; set; } = OperationResultTypes.Success;
        public IEnumerable<T> Data;
    }
}
