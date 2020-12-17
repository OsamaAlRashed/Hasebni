using Hasebni.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hasebni.Base
{
    public abstract class HasebniRepository
    {
        protected readonly HasebniDbContext Context;
        public HasebniRepository(HasebniDbContext context)
        {
            Context = context;
        }
    }
}
